using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartProcessing.Structures.Module;
using Sungero.Commons;
using Sungero.Company;
using Sungero.Docflow;
using Sungero.Integration1CExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sungero.Examples.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать входящее письмо (пример использования доп. классификатора).
    /// </summary>
    /// <param name="documentInfo">Информация о документе.</param>
    /// <param name="responsible">Ответственный за верификацию.</param>
    /// <returns>Входящее письмо.</returns>
    [Public]
    public virtual IOfficialDocument CreateIncomingLetter(IDocumentInfo documentInfo,
                                                          IEmployee responsible)
    {
      // Входящее письмо.
      var document = RecordManagement.IncomingLetters.Create();
      Sungero.SmartProcessing.PublicFunctions.Module.FillIncomingLetterProperties(document, documentInfo, responsible);
      
      // Доп. классификатор.
      var additionalClassifiers = documentInfo.ArioDocument.RecognitionInfo.AdditionalClassifiers;
      if (additionalClassifiers.Count > 0)
        document.Note = string.Format("Доп. класс = {0}", additionalClassifiers.FirstOrDefault().PredictedClass);
      
      return document;
    }
    
    #region Интеграция с 1С
    
    /// <summary>
    /// Получить ссылку на связанную запись 1С.
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <param name="extEntityType">Тип объекта 1С.</param>
    /// <returns>Структура: Hyperlink - ссылка на связанную запись 1С, ErrorMessage - текст ошибки.</returns>
    [Remote, Public]
    public virtual Structures.Module.IGetHyperlink1CResult GetSyncEntity1CHyperlink(Sungero.Domain.Shared.IEntity entity, string extEntityType)
    {
      var result = Examples.Structures.Module.GetHyperlink1CResult.Create();
      var hyperlink = string.Empty;
      var errorMessage = string.Empty;
      
      var entityExternalLink = this.GetExternalEntityLink(entity, extEntityType);

      if (entityExternalLink == null)
      {
        errorMessage = Examples.Resources.OpenRecord1CErrorNotExist;
      }
      else
      {
        if (entityExternalLink.IsDeleted == true)
          errorMessage =  Examples.Resources.OpenRecord1CErrorIsDelete;
        
        try
        {
          var connector1C = this.GetConnector1C();
          hyperlink = connector1C.RunGetRequest(string.Format("{0}/hs/gethyperlink/GetHyperlink/{1}/{2}",
                                                              Constants.Module.ServiceUrl1C, entityExternalLink.ExtEntityId, entityExternalLink.ExtEntityType));
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Integration1C. Error while getting sync entity 1C hyperlink. EntityId = {0}, ExtEntityType = {1}, ExtEntityId = {2}.", ex,
                             entity.Id, entityExternalLink.ExtEntityType, entityExternalLink.ExtEntityId);
          errorMessage = Examples.Resources.OpenRecord1CError;
        }
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Получить ссылку на входящий счет 1С.
    /// </summary>
    /// <param name="incommingInvoice">Входящий счет.</param>
    /// <returns>Структура: Hyperlink - ссылка на входящий счет 1С, ErrorMessage - текст ошибки.</returns>
    [Remote, Public]
    public virtual Structures.Module.IGetHyperlink1CResult GetIncomingInvoice1CHyperlink(Sungero.Examples.IIncomingInvoice incommingInvoice)
    {
      var result = Examples.Structures.Module.GetHyperlink1CResult.Create();
      var hyperlink = string.Empty;
      var errorMessage = string.Empty;
      
      try
      {
        var connector1C = this.GetConnector1C();
        // Ограничение: работает только, если у нашей организации и контрагента заполнены поля: ИНН и КПП.
        var getHyperlinkRequestUrl = string.Format("{0}/hs/gethyperlink/GetIncomingInvoiceHyperlink/{1}/{2}/{3}/{4}/{5}/{6}",
                                                   Constants.Module.ServiceUrl1C, incommingInvoice.Number.Trim(), incommingInvoice.Date.Value.ToString("yyyy-MM-dd"),
                                                   incommingInvoice.BusinessUnit?.TIN, incommingInvoice.BusinessUnit?.TRRC,
                                                   incommingInvoice.Counterparty?.TIN, Sungero.Parties.CompanyBases.As(incommingInvoice.Counterparty)?.TRRC);
        hyperlink = connector1C.RunGetRequest(getHyperlinkRequestUrl);        
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while getting incoming invoice 1C hyperlink. IncomingInvoice Id = {0}.", ex, incommingInvoice.Id);
        errorMessage = Examples.Resources.OpenRecord1CError;
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Создать входящий счет в 1С.
    /// </summary>
    /// <param name="incommingInvoice">Входящий счет в Directum RX.</param>
    /// <returns>True - входящий счет успешно создан в 1С, иначе - False.</returns>
    [Public]
    public virtual bool CreateIncomingInvoice1C(Sungero.Examples.IIncomingInvoice incommingInvoice)
    {
      var created = false;
      
      // Получить ссылку на контрагента.
      var counterpartyExtEntityLink = this.GetExternalEntityLink(incommingInvoice.Counterparty, Constants.Module.CounterpartyExtEntityType);
      if (counterpartyExtEntityLink == null)
      {
        Logger.DebugFormat("Integration1C. Incoming invoice not created in 1C: counterparty is not sync to 1C. IncomingInvoice Id = {0}.", incommingInvoice.Id);
        return false;
      }
      
      // Получить ИД договора в 1С.
      var contractExtEntityId = string.Empty;
      if (incommingInvoice.Contract != null)
      {
        var contractExtEntityLink = this.GetExternalEntityLink(incommingInvoice.Contract, Constants.Module.ContractsExtEntityType);
        if (contractExtEntityLink != null)
          contractExtEntityId = contractExtEntityLink.ExtEntityId;
      }
      
      try
      {
        var connector1C = this.GetConnector1C();
        
        // Получить ИД организации в 1С.
        var businessUnit1CId = this.GetBusinessUnit1CId(connector1C, incommingInvoice.BusinessUnit?.TIN, incommingInvoice.BusinessUnit?.TRRC);
        if (string.IsNullOrEmpty(businessUnit1CId))
        {
          Logger.DebugFormat("Integration1C. Incoming invoice not created in 1C: not found single business unit in 1C. IncomingInvoice Id = {0}.", incommingInvoice.Id);
          return false;
        }
        
        // Создать входящий счет в 1С.
        var incomingInvoice1C = Structures.Module.IncomingInvoice1C.Create();
        incomingInvoice1C.Организация_Key = businessUnit1CId;
        incomingInvoice1C.Контрагент_Key = counterpartyExtEntityLink.ExtEntityId;
        incomingInvoice1C.НомерВходящегоДокумента = incommingInvoice.Number.Trim();
        incomingInvoice1C.ДатаВходящегоДокумента = incommingInvoice.Date.Value;
        incomingInvoice1C.Комментарий = incommingInvoice.Note;
        if (!string.IsNullOrEmpty(contractExtEntityId))
          incomingInvoice1C.ДоговорКонтрагента_Key = contractExtEntityId;
        
        var response = connector1C.RunPostRequest(string.Format("{0}{1}", Constants.Module.ServiceUrl1C, Constants.Module.CreatingIncInvoiceUrlPart1C), incomingInvoice1C);
        
        var createdIncomingInvoice1C = JsonConvert.DeserializeObject<Sungero.Examples.Structures.Module.IncomingInvoice1C>(response);
        var createdIncomingInvoice1CId = createdIncomingInvoice1C?.Ref_Key;
        
        // Создать запись в регистре сведений "Сроки оплаты документов" в 1С.
        if (incommingInvoice.PaymentDueDate.HasValue)
        {
          var paymentTermContent = new {
            Организация_Key = businessUnit1CId,
            Документ = createdIncomingInvoice1CId,
            Документ_Type = "StandardODATA.Document_СчетНаОплатуПоставщика",
            СрокОплаты = incommingInvoice.PaymentDueDate
          };
          
          var paymentTerm = connector1C.RunPostRequest(string.Format("{0}{1}", Constants.Module.ServiceUrl1C, Constants.Module.CreatingPaymentTermUrlPart1C), paymentTermContent);
        }
        
        created = !string.IsNullOrEmpty(createdIncomingInvoice1CId);
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while getting incoming invoice 1C hyperlink. IncomingInvoice Id = {0}.", ex, incommingInvoice.Id);
        created = false;
      }
      
      return created;
    }
    
    /// <summary>
    /// Получить ссылку на объект внешней системы.
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <param name="extEntityType">Тип объекта 1С.</param>
    /// <returns>Ссылка на объект внешней системы. Если не найдена, то null.</returns>
    public virtual IExternalEntityLink GetExternalEntityLink(Sungero.Domain.Shared.IEntity entity, string extEntityType)
    {
      var typeGuid = entity.TypeDiscriminator.ToString();
      var entityExternalLink = ExternalEntityLinks.GetAll()
        .Where(x => string.Equals(x.EntityType, typeGuid, StringComparison.OrdinalIgnoreCase) &&
               x.EntityId == entity.Id &&
               x.ExtEntityType == extEntityType &&
               x.ExtSystemId == Constants.Module.ExtSystemId1C)
        .FirstOrDefault();
      return entityExternalLink;
    }

    /// <summary>
    /// Получить ИД организации в 1С по ИНН и КПП.
    /// </summary>
    /// <param name="connector1C">Коннектор к 1С.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>ИД организации в 1С. Если организация не найдена - null.</returns>
    public virtual string GetBusinessUnit1CId(Sungero.Integration1CExtensions.Connector1C connector1C, string tin, string trrc)
    {
      var response = connector1C.RunGetRequest(string.Format("{0}{1}?$filter=ИНН eq '{2}' and КПП eq '{3}'&$format=json",
                                                             Constants.Module.ServiceUrl1C, Constants.Module.GetBusinessUnitsUrlPart1C,
                                                             tin, trrc));

      var jsonDataResponse = (JObject)JsonConvert.DeserializeObject(response);
      var businessUnits1C = jsonDataResponse["value"];
      var businessUnits1CCount = businessUnits1C.Count();

      if (businessUnits1CCount == 0)
      {
        Logger.DebugFormat("Integration1C. Business unit by TIN and TRRC not found in 1C. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc);
        return null;
      }
      
      if (businessUnits1CCount > 1)
      {
        Logger.DebugFormat("Integration1C. Found {3} business units in 1C by TIN and TRRC. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc, businessUnits1CCount);
        return null;
      }
      
      var businessUnit1CId = businessUnits1C.FirstOrDefault()?["Ref_Key"].Value<string>();
      return businessUnit1CId;
    }
    
    /// <summary>
    /// Получить коннектор к 1С.
    /// </summary>
    /// <returns>Коннектор к 1С.</returns>
    public virtual Sungero.Integration1CExtensions.Connector1C GetConnector1C()
    {
      return Integration1CExtensions.Connector1C.Get(Constants.Module.UserName1C, Constants.Module.Password1C);
    }
    
    #endregion
  }
}