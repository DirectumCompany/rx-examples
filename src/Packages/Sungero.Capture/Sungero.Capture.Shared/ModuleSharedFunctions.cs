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
    /// Определить пронумерован ли документ при захвате.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True - документ успешно пронумерован при захвате, False - иначе.</returns>
    [Public]
    public virtual bool IsSmartCaptureNumerationSucceed(Sungero.Docflow.IOfficialDocument document)
    {
      return document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered &&
             document.VerificationState == Sungero.Docflow.OfficialDocument.VerificationState.InProcess &&
             (document.DocumentKind == null || document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable) &&
             document.DocumentRegister != null;
    }
  }
}