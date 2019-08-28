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
    
    /// <summary>
    /// Заполнить имя документа из короткого имени вида документа при захвате.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <remarks>Если имя документа не сформировалось автоматически, 
    /// то заполнить его из короткого имени вида документа.</remarks>
    [Public]
    public static void FillNameFromKindIfEmpty(Sungero.Docflow.IOfficialDocument document)
    {
      if ((document.Name == Docflow.Resources.DocumentNameAutotext || string.IsNullOrEmpty(document.Name)) && 
          document.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess)
        document.Name = document.DocumentKind.ShortName;
    }
  }
}