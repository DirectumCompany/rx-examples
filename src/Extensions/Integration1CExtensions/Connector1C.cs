using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sungero.Integration1CExtensions
{
  /// <summary>
  /// Коннектор к веб-сервису 1С.
  /// </summary>
  public class Connector1C
  {

    #region Поля и свойства

    private HttpClient Client;

    private string ServiceUrl { get; set; }

    #endregion

    #region Методы api для получения ссылок на объекты 1С.

    /// <summary>
    /// Получить ссылку на запись 1С.
    /// </summary>
    /// <param name="extEntityType">Тип объекта 1С.</param>
    /// <param name="extEntityId">ИД записи 1С.</param>
    /// <returns>Ссылка на запись 1С.</returns>
    public string GetSyncEntity1CHyperlink(string extEntityType, string extEntityId)
    {
      return this.RunGetRequest(string.Format($"{this.ServiceUrl}/hs/gethyperlink/GetHyperlink/{extEntityId}/{extEntityType}"));
    }

    /// <summary>
    /// Получить ссылку на входящий счет 1С.
    /// </summary>
    /// <param name="number">Номер счета.</param>
    /// <param name="date">Дата Счета.</param>
    /// <param name="bisinessUnitTin">ИНН нашей организации.</param>
    /// <param name="bisinessUnitTrrc">КПП нашей организации.</param>
    /// <param name="counterpartyTin">ИНН контрагента.</param>
    /// <param name="counterpartyTrrc">КПП контрагента.</param>
    /// <returns>Ссылка на входящий счет 1С.</returns>
    public string GetIncomingInvoice1CHyperlink(string number, DateTime date, string bisinessUnitTin, string bisinessUnitTrrc,
                                                string counterpartyTin, string counterpartyTrrc)
    {
      var getHyperlinkRequestUrl = string.Format("{0}/hs/gethyperlink/GetIncomingInvoiceHyperlink/{1}/{2}/{3}/{4}/{5}/{6}",
                                                  this.ServiceUrl, number, date.ToString("yyyy-MM-dd"),
                                                  bisinessUnitTin, bisinessUnitTrrc, counterpartyTin, counterpartyTrrc);
      return this.RunGetRequest(getHyperlinkRequestUrl);
    }

    #endregion

    #region Методы api для создания входящего счета.

    /// <summary>
    /// Создать входящий счет в 1С.
    /// </summary>
    /// <param name="incomingInvoice1C">Данные для создания входящего счета.</param>
    /// <returns>Созданный входящий счет в 1С. Если счет не создан - null.</returns>
    public IncomingInvoice1C CreateIncomingInvoice1C(IncomingInvoice1C incomingInvoice1C)
    {
      var response = this.RunPostRequest($"{this.ServiceUrl}/odata/standard.odata/Document_СчетНаОплатуПоставщика?$format=json&$expand=*", incomingInvoice1C);

      var jsonSerializerSettings = new JsonSerializerSettings() 
      { 
        DateFormatHandling = DateFormatHandling.IsoDateFormat, 
        DateParseHandling = DateParseHandling.DateTimeOffset, 
        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
      };

      return JsonConvert.DeserializeObject<IncomingInvoice1C>(response, jsonSerializerSettings);
    }

    /// <summary>
    /// Получить список организаций 1С по ИНН и КПП.
    /// </summary>
    /// <param name="Tin">ИНН.</param>
    /// <param name="Trrc">КПП.</param>
    /// <returns>Список организаций 1С.</returns>
    public List<BusinessUnit1C> GetBusinessUnit1CList(string Tin, string Trrc)
    {
      var response = this.RunGetRequest(string.Format($"{this.ServiceUrl}/odata/standard.odata/Catalog_Организации?$filter=ИНН eq '{Tin}' and КПП eq '{Trrc}'&$format=json"));
      
      var jsonSerializerSettings = new JsonSerializerSettings()
      {
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateParseHandling = DateParseHandling.DateTimeOffset,
        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
      };

      var businessUnit1CList = JsonConvert.DeserializeObject<BusinessUnitList1C>(response, jsonSerializerSettings);
      return businessUnit1CList == null ? null : businessUnit1CList.Value;
    }

    #endregion

    #region Методы по работе с http-запросами.

    private string RunGetRequest(string url)
    {      
      var response = this.Client.GetAsync(url).Result;
      var responseContent = response.Content.ReadAsStringAsync().Result;

      if (!response.IsSuccessStatusCode)
        throw new Exception($"Integration1C. Get request execution error. URL: {url}. Status code: {response.StatusCode}. Response content: {responseContent}.");

      return responseContent;
    }

    private string RunPostRequest(string url, object content)
    {      
      var jsonContent = JsonConvert.SerializeObject(content);
      var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
      var response = this.Client.PostAsync(url, httpContent).Result;
      var responseContent = response.Content.ReadAsStringAsync().Result;

      if (!response.IsSuccessStatusCode)
        throw new Exception($"Integration1C. Post request execution error. URL: {url}. Status code: {response.StatusCode}. Response content: {responseContent}.");

      return responseContent;
    }


    private HttpClient GetClient(string userName, string password)
    {
      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.ServerCertificateCustomValidationCallback =
        (message, cert, chain, sslPolicyErrors) => true;

      var client = new HttpClient(httpClientHandler);
      client.Timeout = TimeSpan.FromMinutes(5);
      var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
      return client;
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Получить коннектор к 1С с авторизацией по логину и паролю.
    /// </summary>
    /// <param name="serviceUrl">Адрес сервиса 1С.</param>
    /// <param name="login">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Коннектор к 1С.</returns>
    public static Connector1C Get(string serviceUrl, string login, string password)
    {
      var connector1C = new Connector1C(serviceUrl, login, password);

      return connector1C;
    }

    private Connector1C(string serviceUrl, string login, string password)
    {
      this.ServiceUrl = serviceUrl;
      this.Client = this.GetClient(login, password);
    }

    #endregion
  }
}
