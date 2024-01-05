using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartProcessing.Structures.Module;
using Sungero.Commons;
using Sungero.Company;
using Sungero.Docflow;

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
    /// <param name="extEntityType">Тип записи 1С.</param>
    /// <returns>Структура: Hyperlink - ссылка на связанную запись 1С, ErrorMessage - текст ошибки.</returns>
    [Remote, Public]
    public static Structures.Module.IGetHyperlink1CResult GetSyncEntity1CHyperlink(Sungero.Domain.Shared.IEntity entity, string extEntityType)
    {
      var result = Examples.Structures.Module.GetHyperlink1CResult.Create();
      var hyperlink = string.Empty;
      var errorMessage = string.Empty;
      
      var typeGuid = entity.TypeDiscriminator.ToString().ToUpper();
      var entityExternalLinks = ExternalEntityLinks.GetAll().Where(x => x.EntityType.ToUpper() == typeGuid &&
                                                                   x.EntityId == entity.Id &&
                                                                   x.ExtEntityType == extEntityType);
      if (!entityExternalLinks.Any())
      {
        errorMessage = Examples.Resources.OpenRecord1CErrorNotExist;
      }
      else
      {
        var entityExternalLink = entityExternalLinks.First();
        
        if (entityExternalLink.IsDeleted == true)
          errorMessage =  Examples.Resources.OpenRecord1CErrorIsDelete;
        
        var getHyperlinkRequestUrl = string.Format("{0}/hs/gethyperlink/GetHyperlink/{1}/{2}",
                                                    Constants.Module.ServiceUrl1C, 
                                                    entityExternalLink.ExtEntityId,
                                                    entityExternalLink.ExtEntityType);
        try
        {
          hyperlink = ExecuteGetRequest(getHyperlinkRequestUrl, Constants.Module.UserName1C, Constants.Module.Password1C);
        }
        catch
        {
          errorMessage =  Examples.Resources.OpenRecord1CError;
        }
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Выполнить GET-запрос.
    /// </summary>
    /// <param name="url">GET-запрос.</param>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <returns></returns>
    public static string ExecuteGetRequest(string url, string userName, string password)
    {
      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.ServerCertificateCustomValidationCallback =
        (message, cert, chain, sslPolicyErrors) => true;

      var client = new HttpClient(httpClientHandler);
      client.Timeout = TimeSpan.FromMinutes(5);
      var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

      var response = client.GetAsync(url).Result;
      var responseContent = response.Content.ReadAsStringAsync().Result;

      if (!response.IsSuccessStatusCode)
        throw new Exception($"Get request execution error. URL: {url}. Status code: {response.StatusCode}. Response content: {responseContent}.");

      return responseContent;
    }
    
    #endregion
  }
}