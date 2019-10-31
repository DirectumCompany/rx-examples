﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.ArioUrl.IsRequired = true;
      _obj.State.Properties.FirstPageClassifierName.IsRequired = true;
      _obj.State.Properties.TypeClassifierName.IsRequired = true;
    }

  }
}