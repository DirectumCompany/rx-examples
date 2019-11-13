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
    public virtual List<Structures.SmartProcessingSetting.SettingsValidationMessage> ValidateArioUrl()
    {
      var messages = new List<Structures.SmartProcessingSetting.SettingsValidationMessage>();
      
      // Проверка что адрес Ario не "кривой".
      if (!System.Uri.IsWellFormedUriString(_obj.ArioUrl, UriKind.Absolute))
      {
        var result = Structures.SmartProcessingSetting.SettingsValidationMessage.Create();
        result.Type = Constants.SmartProcessingSetting.SettingsValidationMessageTypes.Error;
        result.Text = SmartProcessingSettings.Resources.InvalidArioUrl;
        messages.Add(result);
      }
      else if (!Functions.SmartProcessingSetting.Remote.CheckConnection(_obj))
      {
        var result = Structures.SmartProcessingSetting.SettingsValidationMessage.Create();
        result.Type = Constants.SmartProcessingSetting.SettingsValidationMessageTypes.Warning;
        result.Text = SmartProcessingSettings.Resources.ArioConnectionError;
        messages.Add(result);
      }
      
      return messages;
    }    
  }
}