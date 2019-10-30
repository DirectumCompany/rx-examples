using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.SmartCapture.Module.Docflow.Client
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Показать настройки интеллектуальной обработки документов.
    /// </summary>
    public virtual void ShowSmartProcessingSettings()
    {
      var smartProcessingSettings = Capture.PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
      smartProcessingSettings.Show();
    }

  }
}