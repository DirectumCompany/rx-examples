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
    
    [Public]
    public static string GetSyncEntity1CHyperlink(Sungero.Domain.Shared.IEntity entity)
    {
      var hyperlink = string.Empty;
      
      var typeGuid = entity.TypeDiscriminator.ToString().ToUpper();
      var entityExternalLinks = ExternalEntityLinks.GetAll().Where(x => x.EntityType.ToUpper() == typeGuid &&
                                                                   x.EntityId == entity.Id);
      if (!entityExternalLinks.Any())
      {
        throw new Exception("Нет связанной записи в 1С.");
      }
      else
      {
        var entityExternalLink = entityExternalLinks.First();
        
        if (entityExternalLink.IsDeleted == true)
          throw new Exception("Запись в 1С удалена.");
        
        var getHyperlinkRequestUrl = string.Format("{0}/hs/gethyperlink/GetHyperlink/{1}/{2}",
                                                    Constants.Module.ServiceUrl1C, 
                                                    entityExternalLink.ExtEntityId,
                                                    entityExternalLink.ExtEntityType);
      
        hyperlink = ExecuteGetRequest(getHyperlinkRequestUrl, Constants.Module.UserName1C, Constants.Module.Password1C);         
      }
      
      return hyperlink;
    }
    
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
  }
}