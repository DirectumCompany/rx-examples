using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Capture.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      InitializationLogger.Debug("Init: Create smart processing settings.");
      CreateSmartProcessingSettings();
    }
    
    /// <summary>
    /// Создать настройки интеллектуальной обработки документов.
    /// </summary>
    public static void CreateSmartProcessingSettings()
    {
      var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.Remote.GetSmartProcessingSettings();
      if (smartProcessingSettings == null)
        PublicFunctions.SmartProcessingSetting.Remote.CreateSmartProcessingSettings();
    }
  }

}
