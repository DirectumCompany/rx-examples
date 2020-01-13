using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingCaptureSourcesClientHandlers
  {

    public virtual void CaptureSourcesSenderLineNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (string.IsNullOrEmpty(e.NewValue))
        return;
      
      e.NewValue = e.NewValue.Trim();
      
      var errorMessage = PublicFunctions.SmartProcessingSetting.ValidateSenderLineName(e.NewValue);
      if (errorMessage != null)
        e.AddError(errorMessage);
    }
  }

  partial class SmartProcessingSettingClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.ArioUrl.IsRequired = true;
      
      e.Params.AddOrUpdate(Constants.SmartProcessingSetting.SaveFromUIParamName, true);
    }

  }
}