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
}