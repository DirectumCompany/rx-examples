using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.SmartProcessingSetting
{
  /// <summary>
  /// Классификатор.
  /// </summary>
  partial class Classifier
  {
    // ИД.
    public int Id { get; set; }
    
    // Наименование.
    public string Name { get; set; }
  }
  
  /// <summary>
  /// Ошибка валидации настроек интеллектуальной обработки.
  /// </summary>
  partial class SettingsValidationMessage
  {
    // Тип ошибки (Error|SoftError|Warning).
    // Доступные типы Constants.SmartProcessingSetting.SettingsValidationMessageTypes.
    public string Type { get; set; }
    
    // Текст ошибки.
    public string Text { get; set; }
  }
}