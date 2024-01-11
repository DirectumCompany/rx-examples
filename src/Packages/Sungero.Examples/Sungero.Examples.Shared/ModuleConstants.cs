using System;
using Sungero.Core;

namespace Sungero.Examples.Constants
{
  public static class Module
  {
    /// <summary>
    /// Адрес веб-сервера 1С.
    /// </summary>
    public const string ServiceUrl1C = "https://w1333w10/1C_Melnikov2";
    
    /// <summary>
    /// Имя пользователя 1С.
    /// </summary>
    public const string UserName1C = "ИвановИИ";

    /// <summary>
    /// Пароль пользователя 1С.
    /// </summary>
    public const string Password1C = "";

    /// <summary>
    /// Идентификатор системы 1С.
    /// </summary>
    public const string ExtSystemId1C = "1C_Acc";    
    
    /// <summary>
    /// Тип объекта системы 1C для договоров.
    /// </summary>
    public const string ContractsExtEntityType = "ДоговорыКонтрагентов";
    
    /// <summary>
    /// Тип объекта системы 1C для контрагентов.
    /// </summary>
    public const string CounterpartyExtEntityType = "Контрагенты";    
  }
}