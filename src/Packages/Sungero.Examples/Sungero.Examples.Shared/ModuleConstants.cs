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
    
    /// <summary>
    /// Часть пути запроса для создания входящего счета в 1С.
    /// </summary>
    public const string CreatingIncInvoiceUrlPart1C = "/odata/standard.odata/Document_СчетНаОплатуПоставщика?$format=json&$expand=*";

    /// <summary>
    /// Часть пути запроса для создания записи в регистре сведений "Сроки оплаты документов" в 1С.
    /// </summary>
    public const string CreatingPaymentTermUrlPart1C = "/odata/standard.odata/InformationRegister_СрокиОплатыДокументов?$format=json&$expand=*";

    /// <summary>
    /// Часть пути запроса для обращения к справочнику "Организации" в 1С.
    /// </summary>
    public const string GetBusinessUnitsUrlPart1C = "/odata/standard.odata/Catalog_Организации";
  }
}