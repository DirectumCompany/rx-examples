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
    
    /// <summary>
    /// Получить строковое значение свойства.
    /// </summary>
    /// <param name="propertyValue">Значение свойства.</param>
    /// <returns></returns>
    /// <remarks>Для свойств типа сущность будет возвращена строка с Ид сущности.</remarks>
    public static string GetPropertyValueAsString(object propertyValue)
    {
      if (propertyValue == null)
        return string.Empty;
      
      var propertyStringValue = propertyValue.ToString();
      if (propertyValue is Sungero.Domain.Shared.IEntity)
        propertyStringValue = ((Sungero.Domain.Shared.IEntity)propertyValue).Id.ToString();
      return propertyStringValue;
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