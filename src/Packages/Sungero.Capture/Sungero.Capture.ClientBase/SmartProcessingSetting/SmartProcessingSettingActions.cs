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
    public virtual void ForceSave(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
    }

    public virtual bool CanForceSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void CheckConnection(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Валидация адреса сервиса Ario.
      var validationError = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (!string.IsNullOrEmpty(validationError.Text))
      {
        if (validationError.Type == Constants.SmartProcessingSetting.ArioUrlValidationErrorTypes.WrongFormat)
          e.AddError(validationError.Text);
        if (validationError.Type == Constants.SmartProcessingSetting.ArioUrlValidationErrorTypes.ServiceIsDown)
          e.AddWarning(validationError.Text);
        return;
      }
      
      Dialogs.NotifyMessage(SmartProcessingSettings.Resources.ArioConnectionEstablished);
    }

    public virtual bool CanCheckConnection(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.AccessRights.CanUpdate();
    }

  }

}