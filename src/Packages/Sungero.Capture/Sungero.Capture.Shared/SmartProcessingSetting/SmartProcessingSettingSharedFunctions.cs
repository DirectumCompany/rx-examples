using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;
using MessageTypes = Sungero.Capture.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;
using SettingsValidationMessageStructure = Sungero.Capture.Structures.SmartProcessingSetting.SettingsValidationMessage;

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
    public virtual Structures.SmartProcessingSetting.SettingsValidationMessage ValidateArioUrl()
    {
      // Проверка что адрес Ario не "кривой".
      if (!System.Uri.IsWellFormedUriString(_obj.ArioUrl, UriKind.Absolute))
        return SettingsValidationMessageStructure.Create(MessageTypes.Error, SmartProcessingSettings.Resources.InvalidArioUrl);
        
      if (!Functions.SmartProcessingSetting.Remote.CheckConnection(_obj))
        return SettingsValidationMessageStructure.Create(MessageTypes.SoftError, SmartProcessingSettings.Resources.ArioConnectionError);
      
      return null;
    }
    
    /// <summary>
    /// Получить ответственного по имени линии.
    /// </summary>
    /// <param name="senderLineName">Наименование линии.</param>
    /// <returns>Ответственный.</returns>
    [Public]
    public virtual Sungero.Company.IEmployee GetDocumentProcessingResponsible(string senderLineName)
    {
      var recipient = _obj.CaptureSources.Where(x => x.SenderLineName.Trim() == senderLineName.Trim()).Select(x => x.Responsible).FirstOrDefault();
      var responsible = Company.Employees.Null;
      if (Company.Employees.Is(recipient))
        responsible = Company.Employees.As(recipient);
      
      if (Roles.Is(recipient))
        responsible = Company.Employees.As(Roles.As(recipient).RecipientLinks.FirstOrDefault().Member);
      
      return responsible;
    }
    
    /// <summary>
    /// Проверить классификаторы.
    /// </summary>
    /// <returns>Тип и текст ошибки, если она была обнаружена.</returns>
    public virtual Structures.SmartProcessingSetting.SettingsValidationMessage ValidateClassifiers()
    {
      if (!_obj.FirstPageClassifierId.HasValue || !_obj.TypeClassifierId.HasValue)
        return SettingsValidationMessageStructure.Create(MessageTypes.SoftError, SmartProcessingSettings.Resources.SetCorrectClassifiers);
      
      var classifiers = Functions.SmartProcessingSetting.Remote.GetArioClassifiers(_obj);
      var firstPageClassifier = classifiers
        .Where(a => a.Id == _obj.FirstPageClassifierId.Value && a.Name == _obj.FirstPageClassifierName)
        .FirstOrDefault();
      var typeClassifier = classifiers
        .Where(a => a.Id == _obj.TypeClassifierId.Value && a.Name == _obj.TypeClassifierName)
        .FirstOrDefault();
      
      if (firstPageClassifier == null || typeClassifier == null)
        return SettingsValidationMessageStructure.Create(MessageTypes.SoftError, SmartProcessingSettings.Resources.SetCorrectClassifiers);
      
      if (firstPageClassifier.Id == typeClassifier.Id)
        return SettingsValidationMessageStructure.Create(MessageTypes.Warning, SmartProcessingSettings.Resources.SetCorrectClassifiers);
      
      return null;
    }
    
    /// <summary>
    /// Проверить границы доверия к извлечённым фактам.
    /// </summary>
    /// <returns>Тип и текст ошибки, если она была обнаружена.</returns>
    public virtual Structures.SmartProcessingSetting.SettingsValidationMessage ValidateConfidenceLimits()
    {
      // Нижняя граница 0..99 включительно и < верхней. Верхняя 1..100 включительно.
      if (_obj.LowerConfidenceLimit >= 0 && _obj.LowerConfidenceLimit < _obj.UpperConfidenceLimit &&
          _obj.UpperConfidenceLimit <= 100)
        return null;
      
      // Однотипная ошибка для всех случаев.
      return SettingsValidationMessageStructure.Create(MessageTypes.Error, SmartProcessingSettings.Resources.SetCorrectConfidenceLimits);
    }
  }
}