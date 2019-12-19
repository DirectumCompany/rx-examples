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
      var isSafeFromUI = e.Params.Contains(Constants.SmartProcessingSetting.SaveFromUIParamName);
      if (!isSafeFromUI)
        return;
      
      // "Жёсткая" проверка адреса сервиса Ario.
      var arioUrlValidationMessages = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      var arioUrlErrorMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Error);
      if (arioUrlErrorMessages.Any())
        foreach (var message in arioUrlErrorMessages)
          e.AddError(_obj.Info.Properties.ArioUrl, message.Text);
      
      // "Жёсткая" проверка границ доверия.
      var confidenceLimitsValidationMessages = Functions.SmartProcessingSetting.ValidateConfidenceLimits(_obj);
      var confidenceLimitsErrorMessages = confidenceLimitsValidationMessages.Where(m => m.Type == MessageTypes.Error);
      if (confidenceLimitsErrorMessages.Any())
      {
        foreach (var message in confidenceLimitsValidationMessages)
        {
          e.AddError(_obj.Info.Properties.LowerConfidenceLimit, message.Text, _obj.Info.Properties.UpperConfidenceLimit);
          e.AddError(_obj.Info.Properties.UpperConfidenceLimit, message.Text, _obj.Info.Properties.LowerConfidenceLimit);
        }
      }
      
      // При наличии "Жёстких" ошибок не переходить к ForceSave.
      if (arioUrlErrorMessages.Any() || confidenceLimitsErrorMessages.Any())
        return;
      
      var isForceSave = e.Params.Contains(Constants.SmartProcessingSetting.ForceSaveParamName);
      if (!isForceSave)
      {
        // "Мягкая" проверка адреса сервиса Ario.
        var arioUrlWarningMessages = arioUrlValidationMessages.Where(m => m.Type == MessageTypes.Warning);
        foreach (var message in arioUrlWarningMessages)
          e.AddError(message.Text, _obj.Info.Actions.ForceSave);
        
        // "Мягкая" проверка классификаторов.
        var classifierValidationMessages = Functions.SmartProcessingSetting.ValidateClassifiers(_obj);
        var classifierErrorMessages = classifierValidationMessages.Where(m => m.Type == MessageTypes.Warning);
        
        if (classifierErrorMessages.Any())
          e.AddError(classifierErrorMessages.First().Text, _obj.Info.Actions.ForceSave);
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