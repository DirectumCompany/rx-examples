using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.ArioUrl = Sungero.Capture.SmartProcessingSettings.Resources.UrlTemplate;
      _obj.LowerConfidenceLimit = 40;
      _obj.UpperConfidenceLimit = 80;
      
      _obj.PercentLabel = Sungero.Capture.SmartProcessingSettings.Resources.PercentLabelValue;
      _obj.LimitsDescription = Sungero.Capture.SmartProcessingSettings.Resources.LimitsDecriptionValue;
    }
  }

}