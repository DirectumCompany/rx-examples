using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture.Client
{
  partial class SmartProcessingSettingActions
  {
    public virtual void CheckConnection(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.SmartProcessingSetting.Remote.CheckConnection(_obj))
        Dialogs.NotifyMessage(Sungero.Capture.SmartProcessingSettings.Resources.ArioConnectionEstablished);
      else
        e.AddWarning(Sungero.Capture.SmartProcessingSettings.Resources.ArioConnectionError);
    }

    public virtual bool CanCheckConnection(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.AccessRights.CanUpdate();
    }

  }

}