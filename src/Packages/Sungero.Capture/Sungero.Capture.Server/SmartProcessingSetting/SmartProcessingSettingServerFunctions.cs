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
    /// Проверить подключение к Ario.
    /// </summary>
    /// <returns>True, если сервис работает, иначе - False.</returns>
    [Remote]
    public virtual bool CheckConnection()
    {
      var arioConnector = new ArioExtensions.ArioConnector(_obj.ArioUrl);
      ArioExtensions.Models.ArioInfo serviceInfo = null;
      
      try
      {
        serviceInfo = arioConnector.GetInfo();
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
      }
      
      return serviceInfo != null && serviceInfo.State == Constants.SmartProcessingSetting.ArioConnectionSuccessMessage;
    }
  }
}