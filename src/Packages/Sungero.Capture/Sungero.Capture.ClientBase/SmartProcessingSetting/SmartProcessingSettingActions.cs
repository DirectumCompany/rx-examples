using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;
using MessageTypes = Sungero.Capture.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;

namespace Sungero.Capture.Client
{
  partial class SmartProcessingSettingActions
  {
    public virtual void ForceSave(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.AddOrUpdate(Constants.SmartProcessingSetting.ForceSaveParamName, true);
      _obj.Save();
    }

    public virtual bool CanForceSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void CheckConnection(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Валидация адреса сервиса Ario.
      var arioUrlValidationMessages = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (arioUrlValidationMessages.Any())
      {
        var errorMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Error);
        foreach (var message in errorMessages)
          e.AddError(message.Text);
        
        var warningMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Warning);
        foreach (var message in warningMessages)
          e.AddWarning(message.Text);
        
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