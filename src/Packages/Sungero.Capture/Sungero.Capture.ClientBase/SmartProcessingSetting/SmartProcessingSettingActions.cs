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
      // Валидация адреса сервиса Ario.
      var validationError = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (!string.IsNullOrEmpty(validationError.Text))
      {
        if (validationError.Type == Constants.SmartProcessingSetting.ArioUrlValidationErrorTypes.WrongFormat)
          e.AddError(validationError.Text);
        return;
      }
      
      if (Functions.SmartProcessingSetting.Remote.CheckConnection(_obj))
        Dialogs.NotifyMessage(SmartProcessingSettings.Resources.ArioConnectionEstablished);
      else
        e.AddWarning(SmartProcessingSettings.Resources.ArioConnectionError);
    }

    public virtual bool CanCheckConnection(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.AccessRights.CanUpdate();
    }

  }

}