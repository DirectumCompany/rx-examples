using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Structures.Module
{
  /// <summary>
  /// Результат получения ссылки на запись 1С.
  /// </summary>
  [Public]
  partial class GetHyperlink1CResult
  {
    // Ссылка на запись 1С.
    public string Hyperlink { get; set; }
    
    // Текст ошибки.
    public string ErrorMessage { get; set; }
  }
  
  /// <summary>
  /// Данные о входящем счете 1С.
  /// </summary>
  [Public]
  partial class IncomingInvoice1C
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

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string Комментарий { get; set; }
    
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Ref_Key { get; set; }
  }
}