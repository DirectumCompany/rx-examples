using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class SmartProcessingSetting
  {
    // Сообщение при успешном подключении к Ario.
    public const string ArioConnectionSuccessMessage = "SmartService is running";
    
    // Имя параметра "Сохранение выполняется через UI".
    public const string SaveFromUIParamName = "Settings Saved From UI";
    
    // Имя параметра "Сохранить принудительно".
    public const string ForceSaveParamName = "Settings Force Saved";
    
    // Типы ошибок при валидации настроек.
    public static class SettingsValidationMessageTypes
    {
      // Ошибка. При обработке документа в логах ошибка. Настройки сохранить нельзя. 
      public const string Error = "Error";
      
      // "Мягкая" ошибка. При обработке документа в логах ошибка. Для сохранения требуется подтверждение.
      public const string SoftError = "SoftError";
      
      // Предупреждение. При обработке документа в логах предупреждение. Для сохранения требуется подтверждение.
      public const string Warning = "Warning";
    }
    
  }
}