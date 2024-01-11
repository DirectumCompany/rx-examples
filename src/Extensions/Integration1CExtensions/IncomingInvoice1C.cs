using System;
using Newtonsoft.Json;

namespace Sungero.Integration1CExtensions
{
  /// <summary>
  /// Данные для создания входящего счета.
  /// </summary>
  public class IncomingInvoice1C
  {
    /// <summary>
    /// Номер счета.
    /// </summary>
    public string НомерВходящегоДокумента { get; set; }

    /// <summary>
    /// Дата счета.
    /// </summary>
    public DateTime ДатаВходящегоДокумента { get; set; }

    /// <summary>
    /// Организация.
    /// </summary>
    public string Организация_Key { get; set; }

    /// <summary>
    /// Контрагент.
    /// </summary>
    public string Контрагент_Key { get; set; }

    /// <summary>
    /// Договор.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ДоговорКонтрагента_Key { get; set; }

    /// <summary>
    /// Идентификатор.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Ref_Key { get; set; }

    /// <summary>
    /// Создать данные для создания входящего счета в 1С.
    /// </summary>
    /// <param name="number">Номер счета.</param>
    /// <param name="date">Дата счета.</param>
    /// <param name="businessUnitId">ИД организации в 1С.</param>
    /// <param name="counterpartyId">ИД контрагента в 1С.</param>
    /// <param name="contractId">ИД договора в 1С.</param>
    /// <returns></returns>
    public static IncomingInvoice1C Create(string number, DateTime date, string businessUnitId, 
                                           string counterpartyId, string contractId)
    {
      var incomingInvoice1C = new IncomingInvoice1C()
      {
        НомерВходящегоДокумента = number,
        ДатаВходящегоДокумента = date,
        Организация_Key = businessUnitId,
        Контрагент_Key = counterpartyId
      };

      if (!string.IsNullOrEmpty(contractId))
        incomingInvoice1C.ДоговорКонтрагента_Key = contractId;

      return incomingInvoice1C;
    }
  }
}
