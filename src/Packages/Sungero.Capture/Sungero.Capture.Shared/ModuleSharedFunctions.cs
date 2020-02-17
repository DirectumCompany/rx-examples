using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получить дату из строки с датой и временем.
    /// </summary>
    /// <param name="source">Исходная строка.</param>
    /// <returns>Дата.</returns>
    public string GetShortDate(string source)
    {
      if (!string.IsNullOrWhiteSpace(source))
      {
        DateTime dateTmp;
        Calendar.TryParseDateTime(source, out dateTmp);
        return dateTmp != null ? dateTmp.ToShortDateString() : source;
      }
      return string.Empty;
    }
    
  }
}