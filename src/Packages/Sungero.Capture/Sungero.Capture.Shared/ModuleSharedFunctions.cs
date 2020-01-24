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
    /// Сделать доступными рег. номер и рег. дату незарегистрированного договорного документа в процессе верификации.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static void EnableRegistrationNumberAndDate(Docflow.IContractualDocumentBase document)
    {      
      if (document.VerificationState == Docflow.OfficialDocument.VerificationState.Completed)
        return;
      
      var properties = document.State.Properties;
      if (document.RegistrationState == Docflow.OfficialDocument.RegistrationState.NotRegistered)
      {
        properties.RegistrationNumber.IsEnabled = true;
        properties.RegistrationDate.IsEnabled = true;
      }      
    }
  }
}