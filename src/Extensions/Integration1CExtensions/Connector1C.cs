using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Sungero.Integration1CExtensions
{
  /// <summary>
  /// Коннектор к веб-сервису 1С.
  /// </summary>
  public class Connector1C
  {

    #region Поля и свойства

    private HttpClient Client;
    
    #endregion

    #region Методы api по работе с http-запросами.

    /// <summary>
    /// Выполнить GET-запрос к веб-сервису 1С.
    /// </summary>
    /// <param name="url">Адрес запроса.</param>
    /// <returns>Результат запроса в виде строки.</returns>    
    public string RunGetRequest(string url)
    {      
      var response = this.Client.GetAsync(url).Result;
      var responseContent = response.Content.ReadAsStringAsync().Result;

      if (!response.IsSuccessStatusCode)
        throw new Exception($"Integration1C. Get request execution error. URL: {url}. Status code: {response.StatusCode}. Response content: {responseContent}.");

      return responseContent;
    }

    /// <summary>
    /// Выполнить POST-запрос к веб-сервису 1С.
    /// </summary>
    /// <param name="url">Адрес запроса.</param>
    /// <param name="content">Данные.</param>
    /// <returns>Результат запроса в виде строки.</returns>    
    public string RunPostRequest(string url, object content)
    {      
      var jsonContent = JsonConvert.SerializeObject(content);
      var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
      var response = this.Client.PostAsync(url, httpContent).Result;
      var responseContent = response.Content.ReadAsStringAsync().Result;

      if (!response.IsSuccessStatusCode)
        throw new Exception($"Integration1C. Post request execution error. URL: {url}. Status code: {response.StatusCode}. Response content: {responseContent}.");

      return responseContent;
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Получить коннектор к 1С с авторизацией по логину и паролю.
    /// </summary>
    /// <param name="login">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Коннектор к 1С.</returns>
    public static Connector1C Get(string login, string password)
    {
      var connector1C = new Connector1C(login, password);

      return connector1C;
    }

    private Connector1C(string login, string password)
    {
      this.Client = this.GetClient(login, password);
    }

    private HttpClient GetClient(string login, string password)
    {
      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.ServerCertificateCustomValidationCallback =
        (message, cert, chain, sslPolicyErrors) => true;

      var client = new HttpClient(httpClientHandler);
      client.Timeout = TimeSpan.FromMinutes(5);
      var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
      return client;
    }

    #endregion
  }
}
