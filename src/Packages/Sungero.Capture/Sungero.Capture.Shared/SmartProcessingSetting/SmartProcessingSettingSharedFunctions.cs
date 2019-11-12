using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture.Shared
{
  partial class SmartProcessingSettingFunctions
  {
    /// <summary>
    /// Получить настройки интеллектуальной обработки документов.
    /// </summary>
    /// <returns>Настройки.</returns>
    [Public]
    public static ISmartProcessingSetting GetSmartProcessingSettings()
    {
      return SmartProcessingSettings.GetAllCached().SingleOrDefault();
    }
    
    /// <summary>
    /// Проверить адрес сервиса Ario.
    /// </summary>
    /// <returns>Тип и текст ошибки, если она была обнаружена.</returns>
    public virtual Structures.SmartProcessingSetting.ArioUrlValidationError ValidateArioUrl()
    {
      var result = Structures.SmartProcessingSetting.ArioUrlValidationError.Create();
      if (!System.Uri.IsWellFormedUriString(_obj.ArioUrl, UriKind.Absolute))
      {
        result.Type = Constants.SmartProcessingSetting.ArioUrlValidationErrorTypes.WrongFormat;
        result.Text = SmartProcessingSettings.Resources.ArioUrlIsNotValid;
      }
      return result;
    }
  }
}