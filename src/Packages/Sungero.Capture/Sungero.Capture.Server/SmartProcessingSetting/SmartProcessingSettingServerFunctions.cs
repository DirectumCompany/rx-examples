using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture.Server
{
  partial class SmartProcessingSettingFunctions
  {
    /// <summary>
    /// Создать настройки интеллектуальной обработки документов.
    /// </summary>
    [Remote, Public]
    public static void CreateSmartProcessingSettings()
    {
      var smartProcessingSettings = SmartProcessingSettings.Create();
      smartProcessingSettings.Save();
    }
    
    /// <summary>
    /// Получить настройки интеллектуальной обработки документов.
    /// </summary>
    /// <returns>Настройки.</returns>
    [Remote, Public]
    public static ISmartProcessingSetting GetSmartProcessingSettings()
    {
      return SmartProcessingSettings.GetAllCached().SingleOrDefault();
    }
  }
}