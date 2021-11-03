using System;
using System.Collections.Generic;
using System.Linq;
using NpoComputer.DCX.Common;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Exchange;
using Sungero.ExchangeCore;
using Sungero.Parties;

namespace Sungero.Examples.Module.Exchange.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Обработать документы, созданные из сообщения.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="queueItem">Элемент очереди.</param>
    /// <param name="infos">Информация по обработанным документам.</param>
    /// <param name="processingDocuments">Обрабатываемые документы.</param>
    /// <param name="sender">Отправитель.</param>
    /// <param name="isIncoming">True - от контрагента, false - наше.</param>
    /// <param name="box">Абонентский ящик.</param>
    /// <returns>Признак успешности обработки сообщения.</returns>
    protected override bool ProcessDocumentsFromNewIncomingMessage(IMessage message, IMessageQueueItem queueItem, List<Sungero.Exchange.IExchangeDocumentInfo> infos,
                                                                   List<IDocument> processingDocuments, ICounterparty sender, bool isIncoming, IBoxBase box)
    {
      base.ProcessDocumentsFromNewIncomingMessage(message, queueItem, infos, processingDocuments, sender, isIncoming, box);
      
      // Загрузка метаданных для входящих счетов из Диадока.
      if (processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindDiadoc.IncomingInvoice).Any())
        this.ProcessIncomingInvoicesWithMetadata(processingDocuments, infos, sender, box);
      
      if (processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindSbis.IncomingInvoice).Any())
        this.ProcessIncomingInvoicesFromXml(processingDocuments, infos);
      
      // Загрузка печатной формы для входящих счетов из СБИС.
      if (processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindSbis.IncomingInvoice).Any())
        this.ProcessPrintedFormsIncomingInvoices(processingDocuments, infos);

      return true;
    }
    
    /// <summary>
    /// Проверить, что сообщение содержит документы неподдерживаемого типа.
    /// </summary>
    /// <param name="message">Сообщение из сервиса обмена.</param>
    /// <returns>True, если содержит, иначе False.</returns>
    public override bool IsMessageWithUnsupportedDocuments(NpoComputer.DCX.Common.IMessage message)
    {
      var result = base.IsMessageWithUnsupportedDocuments(message);
      
      var isTitle = message.PrimaryDocuments
        .All(d => d.NonformalizedKind == Constants.Module.NonformalizedKindDiadoc.Title && d.DocumentType == DocumentType.Nonformalized && d.IsUnknownDocumentType == true);
      
      var isIncomingInvoice = message.PrimaryDocuments
        .All(d => d.NonformalizedKind == Constants.Module.NonformalizedKindSbis.IncomingInvoice && d.DocumentType == DocumentType.Nonformalized && d.IsUnknownDocumentType == true);
      
      return result && !isTitle && !isIncomingInvoice;
    }
    
    /// <summary>
    /// Обработать входящий счет с метадатой.
    /// </summary>
    /// <param name="processingDocuments">Обрабатываемые документы.</param>
    /// <param name="infos">Информация по обрабатываемым документам.</param>
    /// <param name="sender">Отправитель.</param>
    /// <param name="box">Абонентский ящик.</param>
    protected void ProcessIncomingInvoicesWithMetadata(List<IDocument> processingDocuments, List<Sungero.Exchange.IExchangeDocumentInfo> infos, ICounterparty sender, IBoxBase box)
    {
      Logger.Debug("Execute ProcessIncomingInvoicesWithMetadata");
      var incomingInvoices = processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindDiadoc.IncomingInvoice);
      var serviceDocumentIds = incomingInvoices.Select(s => s.ServiceEntityId);
      
      var sortedInfos = infos.Where(i => serviceDocumentIds.Contains(i.ServiceDocumentId));
      
      foreach (var info in sortedInfos)
      {
        var convertedDoc = Contracts.IncomingInvoices.As(info.Document.ConvertTo(Contracts.IncomingInvoices.Info));
        var incomingInvoice = incomingInvoices.Where(d => d.ServiceEntityId == info.ServiceDocumentId).FirstOrDefault();
        var metadata = incomingInvoice.Metadata;
        System.DateTime dateDocument;
        Calendar.TryParseDate(metadata.Where(m => m.Key == Constants.Module.MetadataKeyIncomingInvoice.DocumentDate).FirstOrDefault().Value, out dateDocument);
        convertedDoc.Date = dateDocument;
        convertedDoc.Number = metadata.Where(m => m.Key == Constants.Module.MetadataKeyIncomingInvoice.DocumentNumber).FirstOrDefault().Value;
        double totalAmmount;
        double.TryParse(metadata.Where(m => m.Key == Constants.Module.MetadataKeyIncomingInvoice.TotalSum).FirstOrDefault().Value, out totalAmmount);
        convertedDoc.TotalAmount = totalAmmount;
        convertedDoc.Counterparty = sender;
        convertedDoc.BusinessUnit = ExchangeCore.PublicFunctions.BoxBase.GetBusinessUnit(box);
        convertedDoc.BusinessUnitBox = ExchangeCore.PublicFunctions.BoxBase.GetRootBox(box);
        convertedDoc.Save();
      }
    }
    
    /// <summary>
    /// Заполнить входящий счет по содержимому Xml.
    /// </summary>
    /// <param name="processingDocuments">Обрабатываемые документы.</param>
    /// <param name="infos">Информация по обрабатываемым документам.</param>
    protected void ProcessIncomingInvoicesFromXml(List<IDocument> processingDocuments, List<Sungero.Exchange.IExchangeDocumentInfo> infos)
    {
      Logger.Debug("Execute ProcessIncomingInvoicesFromXml");
      var incomingInvoices = processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindSbis.IncomingInvoice);
      var serviceDocumentIds = incomingInvoices.Select(s => s.ServiceEntityId);
      
      var sortedInfos = infos.Where(i => serviceDocumentIds.Contains(i.ServiceDocumentId));
      
      foreach (var info in sortedInfos)
      {
        var incomingInvoice = incomingInvoices.Where(d => d.ServiceEntityId == info.ServiceDocumentId).FirstOrDefault();
        var xdoc = System.Xml.Linq.XDocument.Load(new System.IO.MemoryStream(incomingInvoice.Content));
        RemoveNamespaces(xdoc);
        var convertedDoc = Contracts.IncomingInvoices.As(info.Document.ConvertTo(Contracts.IncomingInvoices.Info));
        
        var xmlDocumentElement = xdoc.Element("Файл").Element("Документ");
        var documentNumber = GetAttributeValueByName(xmlDocumentElement.Element("СвСчет"), "НомерСчет");
        var attributeDocumentDate = GetAttributeValueByName(xmlDocumentElement.Element("СвСчет"), "ДатаСчет");
        var attributeTotalAmount = GetAttributeValueByName(xmlDocumentElement.Element("ТаблСчет").Element("ВсегоОпл"), "СтТовУчНалВсего");        
        
        System.DateTime documentDate;
        Calendar.TryParseDate(attributeDocumentDate, out documentDate);
        double totalAmmount;
        double.TryParse(attributeTotalAmount, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out totalAmmount);
        
        convertedDoc.Date = documentDate;
        convertedDoc.Number = documentNumber;
        convertedDoc.TotalAmount = totalAmmount;
        convertedDoc.Subject = string.Empty;
        convertedDoc.Save();
      }
    }
    
    /// <summary>
    /// Получить печатную форму из сервиса обмена.
    /// </summary>
    /// <param name="processingDocuments">Обрабатываемые документы.</param>
    /// <param name="infos">Информация по обрабатываемым документам.</param>
    protected void ProcessPrintedFormsIncomingInvoices(List<IDocument> processingDocuments, List<Sungero.Exchange.IExchangeDocumentInfo> infos)
    {
      Logger.Debug("Execute ProcessPrintedFormsIncomingInvoices");
      var incomingInvoicesSbis = processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKindSbis.IncomingInvoice);
      var serviceDocumentIds = incomingInvoicesSbis.Select(s => s.ServiceEntityId);
      var sortedInfos = infos.Where(i => serviceDocumentIds.Contains(i.ServiceDocumentId));
      foreach (var info in sortedInfos)
      {
        this.GeneratePublicBodyFromService(info.Document, info.VersionId.Value);
      }
    }
    
    /// <summary>
    /// Получить значение атрибута по имени.
    /// </summary>
    /// <param name="element">Элемент xml-документа.</param>
    /// <param name="attributeName">Наименование атрибута.</param>
    /// <returns>Значение атрибута.</returns>
    private static string GetAttributeValueByName(System.Xml.Linq.XElement element, string attributeName)
    {
      var attribute = element.Attribute(attributeName);
      return attribute == null ? string.Empty : attribute.Value;
    }
    
    /// <summary>
    /// Перекрытие. Генерация печатной формы.
    /// </summary>
    /// <param name="document">Документ.</param>
    protected override void GeneratePublicBodyAsync(IOfficialDocument document)
    {
      if (Contracts.IncomingInvoices.Is(document) && document.HasVersions)
        this.GeneratePublicBodyFromService(document, document.LastVersion.Id);
      else
        base.GeneratePublicBodyAsync(document);
    }
  }
}
