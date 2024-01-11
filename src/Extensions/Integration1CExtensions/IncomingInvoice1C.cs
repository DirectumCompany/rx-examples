using System;

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
    public string ДоговорКонтрагента_Key { get; set; }


    public static IncomingInvoice1C Create(string number, DateTime date, string businessUnitKey, 
                                           string counterpartyKey, string contractKey)
    {
      return new IncomingInvoice1C()
      {
        НомерВходящегоДокумента = number,
        ДатаВходящегоДокумента = date,
        Организация_Key = businessUnitKey,
        Контрагент_Key = counterpartyKey,
        ДоговорКонтрагента_Key = contractKey
      };
    }
  }
}
