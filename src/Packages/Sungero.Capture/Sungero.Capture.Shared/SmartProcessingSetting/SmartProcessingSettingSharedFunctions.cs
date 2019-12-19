using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;
using MessageTypes = Sungero.Capture.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;

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
        result.Type = MessageTypes.Error;
        result.Text = SmartProcessingSettings.Resources.InvalidArioUrl;
        messages.Add(result);
      }
      else if (!Functions.SmartProcessingSetting.Remote.CheckConnection(_obj))
      {
        var result = Structures.SmartProcessingSetting.SettingsValidationMessage.Create();
        result.Type = MessageTypes.Warning;
        result.Text = SmartProcessingSettings.Resources.ArioConnectionError;
        messages.Add(result);
      }
      
      return messages;
    }
    
    /// <summary>
    /// Проверить классификаторы.
    /// </summary>
    /// <returns>True, если классификаторы заполнены и найдены по Ид и наименованию, иначе - False.</returns>
    public virtual List<Structures.SmartProcessingSetting.SettingsValidationMessage> ValidateClassifiers()
    {
      var messages = new List<Structures.SmartProcessingSetting.SettingsValidationMessage>();
      var message = Structures.SmartProcessingSetting.SettingsValidationMessage.Create();
      message.Type = MessageTypes.Warning;
      
      if (!_obj.FirstPageClassifierId.HasValue || !_obj.TypeClassifierId.HasValue)
      {
        message.Text = Sungero.Capture.SmartProcessingSettings.Resources.SetCorrectClassifiers;
        messages.Add(message);
        return messages;
      }
      
      var arioClassifiers = Functions.SmartProcessingSetting.Remote.GetArioClassifiers(_obj);
      var arioFirstPageClassifier = arioClassifiers.Where(a => a.Id == _obj.FirstPageClassifierId.Value &&
                                                          a.Name == _obj.FirstPageClassifierName).FirstOrDefault();
      var arioTypeClassifier = arioClassifiers.Where(a => a.Id == _obj.TypeClassifierId.Value &&
                                                     a.Name == _obj.TypeClassifierName).FirstOrDefault();
      
      if (arioFirstPageClassifier == null && arioTypeClassifier == null)
      {
        message.Text = Resources.ClassifiersNotFoundFormat(_obj.FirstPageClassifierName, _obj.TypeClassifierName);
        messages.Add(message);
      }
      else if (arioFirstPageClassifier == null)
      {
        message.Text = Resources.ClassifierNotFoundFormat(_obj.FirstPageClassifierName);
        messages.Add(message);
      }
      else if (arioTypeClassifier == null)
      {
        message.Text = Resources.ClassifierNotFoundFormat(_obj.TypeClassifierName);
        messages.Add(message);
      }
      
      return messages;
    }
    
    /// <summary>
    /// Проверить границы доверия к извлечённым фактам.
    /// </summary>
    /// <returns>Тип и текст ошибки, если она была обнаружена.</returns>
    public virtual List<Structures.SmartProcessingSetting.SettingsValidationMessage> ValidateConfidenceLimits()
    {
      var messages = new List<Structures.SmartProcessingSetting.SettingsValidationMessage>();
      
      // Нижняя граница 0..99 включительно и < верхней. Верхняя 1..100 включительно.
      if (_obj.LowerConfidenceLimit >= 0 && _obj.LowerConfidenceLimit < _obj.UpperConfidenceLimit &&
          _obj.UpperConfidenceLimit <= 100)
        return messages;
      
      // Однотипная ошибка для всех случаев.
      var result = Structures.SmartProcessingSetting.SettingsValidationMessage.Create();
      result.Type = MessageTypes.Error;
      result.Text = SmartProcessingSettings.Resources.SetCorrectConfidenceLimits;
      messages.Add(result);

      return messages;
    }
  }
}