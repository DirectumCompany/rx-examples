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
    /// Собрать ФИО из фамилии, имени и отчества.
    /// </summary>
    /// <param name="surname">Фамилия.</param>
    /// <param name="name">Имя.</param>
    /// <param name="patronymic">Отчество.</param>
    /// <returns>ФИО.</returns>
    public string ConcatFullName(string surname, string name, string patronymic)
    {
      var parts = new List<string>();

      if (!string.IsNullOrWhiteSpace(surname))
        parts.Add(surname);
      if (!string.IsNullOrWhiteSpace(name))
        parts.Add(name);
      if (!string.IsNullOrWhiteSpace(patronymic))
        parts.Add(patronymic);

      return string.Join(" ", parts);
    }
    
    /// <summary>
    /// Получить дату из строки с датой и временем.
    /// </summary>
    /// <param name="source">Исходная строка.</param>
    /// <returns>Дата.</returns>
    public string GetShortDate(string source)
    {
      DateTime dateTmp;
      Calendar.TryParseDate(source, out dateTmp);
      return dateTmp != null ? dateTmp.ToShortDateString() : source;
    }
  }
}