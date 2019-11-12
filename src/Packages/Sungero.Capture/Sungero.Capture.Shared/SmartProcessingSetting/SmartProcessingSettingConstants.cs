using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class SmartProcessingSetting
  {
    // Сообщение при успешном подключении к Ario.
    public const string ArioConnectionSuccessMessage = "SmartService is running";
    
    // Типы ошибок при адреса валидации сервиса Ario.
    public static class ArioUrlValidationErrorTypes
    {
      // Неправильный формат адреса.
      public const string WrongFormat = "Wrong format";
      
      // Сервис недоступен.
      public const string ServiceIsDown = "Service is down";
    }
    
  }
}