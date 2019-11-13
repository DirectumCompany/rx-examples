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
    
    // Типы ошибок при адреса валидации сервиса Ario.
    public static class SettingsValidationMessageTypes
    {
      // Неправильный формат адреса.
      public const string Error = "Error";
      
      // Сервис недоступен.
      public const string Warning = "Warning";
    }
    
  }
}