using System;
using System.Collections.Generic;
using System.Linq;
using NpoComputer.DCX.Common;
using Sungero.Core;
using Sungero.CoreEntities;
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
    protected override bool ProcessDocumentsFromNewIncomingMessage(IMessage message, IMessageQueueItem queueItem, List<IExchangeDocumentInfo> infos,
                                                                   List<IDocument> processingDocuments, ICounterparty sender, bool isIncoming, IBoxBase box)
    {
      base.ProcessDocumentsFromNewIncomingMessage(message, queueItem, infos, processingDocuments, sender, isIncoming, box);
      
      
      var incomingInvoices = processingDocuments.Where(d => d.NonformalizedKind == Constants.Module.NonformalizedKind.IncomingInvoice);
      
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
        double totalammount;
        double.TryParse(metadata.Where(m => m.Key == Constants.Module.MetadataKeyIncomingInvoice.TotalSum).FirstOrDefault().Value, out totalammount);
        convertedDoc.TotalAmount = totalammount;
        convertedDoc.Counterparty = sender;
        convertedDoc.BusinessUnit = ExchangeCore.PublicFunctions.BoxBase.GetBusinessUnit(box);
        convertedDoc.BusinessUnitBox = ExchangeCore.PublicFunctions.BoxBase.GetRootBox(box);
        convertedDoc.Save();
      }
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
        .All(d => d.NonformalizedKind == Constants.Module.NonformalizedKind.Title && d.DocumentType == DocumentType.Nonformalized && d.IsUnknownDocumentType == true);
      
      return result && !isTitle;
    }
  }
}
