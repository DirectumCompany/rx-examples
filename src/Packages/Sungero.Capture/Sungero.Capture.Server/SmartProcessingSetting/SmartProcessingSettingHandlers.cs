using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;
using MessageTypes = Sungero.Capture.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Валидация адреса сервиса Ario.
      var isSafeFromUI = e.Params.Contains(Constants.SmartProcessingSetting.SaveFromUIParamName);
      var isForceSave = e.Params.Contains(Constants.SmartProcessingSetting.ForceSaveParamName);
      var arioUrlValidationMessages = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (isSafeFromUI && arioUrlValidationMessages.Any())
      {
        var errorMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Error);
        foreach (var message in errorMessages)
          e.AddError(message.Text);
        
        if (!isForceSave)
        {
          var warningMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Warning);
          foreach (var message in warningMessages)
            e.AddError(message.Text, _obj.Info.Actions.ForceSave);
        }
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.LowerConfidenceLimit = 40;
      _obj.UpperConfidenceLimit = 80;
      
      _obj.Name = SmartProcessingSettings.Resources.SmartProcessingSettings;
      _obj.PercentLabel = SmartProcessingSettings.Resources.PercentLabelValue;
      _obj.LimitsDescription = SmartProcessingSettings.Resources.LimitsDecriptionValue;
    }
  }

}