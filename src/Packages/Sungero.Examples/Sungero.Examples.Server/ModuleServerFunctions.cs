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
            
      var typeGuid = entity.TypeDiscriminator.ToString();
      var entityExternalLink = ExternalEntityLinks.GetAll()
                                                  .Where(x => string.Equals(x.EntityType, typeGuid, StringComparison.OrdinalIgnoreCase) &&
                                                                            x.EntityId == entity.Id &&
                                                                            x.ExtEntityType == extEntityType)
                                                  .FirstOrDefault();
      

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
          hyperlink = connector1C.GetSyncEntity1CHyperlink(entityExternalLink.ExtEntityType, entityExternalLink.ExtEntityId);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Integration1C. Error while getting sync entity 1C hyperlink. EntityId = {0}, ExtEntityType = {1}, ExtEntityId = {2}.", ex, 
                             entity.Id, entityExternalLink.ExtEntityType, entityExternalLink.ExtEntityId);
          errorMessage =  Examples.Resources.OpenRecord1CError;
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
        hyperlink = connector1C.GetIncomingInvoice1CHyperlink(incommingInvoice.Number.Trim(),
                                                              incommingInvoice.Date.Value,
                                                              incommingInvoice.BusinessUnit?.TIN,
                                                              incommingInvoice.BusinessUnit?.TRRC,
                                                              incommingInvoice.Counterparty?.TIN,
                                                              Sungero.Parties.CompanyBases.As(incommingInvoice.Counterparty)?.TRRC);
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while getting incoming invoice 1C hyperlink. IncomingInvoice Id = {0}.", ex, incommingInvoice.Id);        
        errorMessage =  Examples.Resources.OpenRecord1CError;
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Получить коннектор к 1С.
    /// </summary>
    /// <returns>Коннектор к 1С.</returns>
    public virtual Sungero.Integration1CExtensions.Connector1C GetConnector1C()
    {
      return Integration1CExtensions.Connector1C.Get(Constants.Module.ServiceUrl1C, Constants.Module.UserName1C, Constants.Module.Password1C);
    }    
    
    #endregion
  }
}