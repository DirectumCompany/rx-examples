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
        var convertedDoc = Sungero.Examples.IncomingInvoices.As(info.Document.ConvertTo(Sungero.Examples.IncomingInvoices.Info));
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
    
    /// <summary>
    /// Отфильтровать легкие сообшения.
    /// </summary>
    /// <param name="businessUnitBox">Абонентский ящик.</param>
    /// <param name="messages">Сообщения.</param>
    /// <returns>Список отфильтрованных сообщений.</returns>
    public override List<NpoComputer.DCX.Common.IMessage> FilterLiteMessages(ExchangeCore.IBusinessUnitBox businessUnitBox, List<NpoComputer.DCX.Common.IMessage> messages)
    {
      var filteredMessages = new List<NpoComputer.DCX.Common.IMessage>();
      
      // Типы документов сервисов: 
      // - СБИС - https://sbis.ru/help/integration/catalog/guide#_tipy_vlozhenij_dokumenta
      // - Диадок(см. AttachmentType) - https://developer.kontur.ru/Docs/diadoc-api/proto/Entity%20message.html
      var sbisUTDTypes = new string[] { "УпдДоп", "УпдСчфДоп" };
      
      // Пример данных по организации и департаменту в СБИС.
      var sbisSenderBoxId = "1121011351/110901001";
      var sbisSenderDepartmentCode = "1";
      // Пример данных по организации и департаменту в Диадок.
      var diadocSenderBoxId = "405b3fb0af6a4215b89bab864ff8aca0@diadoc.ru";
      var diadocSenderDepartmentCode = "6e9ab7bd-576d-4fc2-9a1e-9282ec8bebb3";
      
      foreach (var message in messages)
      {
        // Не обрабатывать входящие документы от организации с Ид 1121011351/110901001.
        if (string.Equals(message.Sender?.BoxId, sbisSenderBoxId))
          continue;
        
        // Не обрабатывать входящие документы от подразделения с Ид 1 организации с Ид 1121011351/110901001.
        if (string.Equals(message.Sender?.BoxId, sbisSenderBoxId) && string.Equals(message.FromDepartment?.Id, sbisSenderDepartmentCode))
          continue;
        
        // Получать из СБИС только сообщения с УПД.
        if (businessUnitBox.ExchangeService.ExchangeProvider == ExchangeCore.ExchangeService.ExchangeProvider.Sbis &&
            message.IsRoot && !message.PrimaryDocuments.Any(pdoc => sbisUTDTypes.Contains(pdoc.ServiceType)))
          continue;
        
        // Ответные сообщения на пропущенные основные сообщения не нужно загружать.
        if (!message.IsRoot &&
            // Родительского сообщения нет среди новых ещё необработанных.
            !filteredMessages.Any(m => m.IsRoot && m.ServiceMessageId == message.ParentServiceMessageId) &&
            // Родительского сообщения нет в очереди обработки.
            !ExchangeCore.MessageQueueItems.GetAll(item => (bool)item.IsRootMessage && item.RootMessageId == message.ParentServiceMessageId).Any() &&
            // Родительское сообщение не обработано и по нему нет инфошки.
            // Для Диадока.
            !ExchangeDocumentInfos.GetAll(info => info.ServiceMessageId == message.ParentServiceMessageId).Any() &&
            // Для СБИСа.
            !ExchangeDocumentInfos.GetAll(info => info.ServiceMessageId.Contains(message.ParentServiceMessageId)).Any())
          continue;
        
        filteredMessages.Add(message);
      }
      
      return filteredMessages;
    }
  }
}
