using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingSharedHandlers
  {
    public virtual void ArioUrlChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue || e.NewValue == null)
        return;
      
      var trimmedArioUrl = e.NewValue.Trim(' ');
      if (e.NewValue == trimmedArioUrl)
        return;
      
      _obj.ArioUrl = trimmedArioUrl;
    }

  }
}