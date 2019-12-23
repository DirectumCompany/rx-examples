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
      var arioUrlValidationMessage = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      var isArioUrlValidationMessageError = arioUrlValidationMessage != null && arioUrlValidationMessage.Type == MessageTypes.Error;
      if (isArioUrlValidationMessageError)
        e.AddError(_obj.Info.Properties.ArioUrl, arioUrlValidationMessage.Text);
      
      // "Жёсткая" проверка границ доверия.
      var confidenceLimitsValidationMessage = Functions.SmartProcessingSetting.ValidateConfidenceLimits(_obj);
      var isConfidenceLimitsValidationMessageError = confidenceLimitsValidationMessage != null && confidenceLimitsValidationMessage.Type == MessageTypes.Error;
      if (isConfidenceLimitsValidationMessageError)
      {
        e.AddError(_obj.Info.Properties.LowerConfidenceLimit, confidenceLimitsValidationMessage.Text, _obj.Info.Properties.UpperConfidenceLimit);
        e.AddError(_obj.Info.Properties.UpperConfidenceLimit, confidenceLimitsValidationMessage.Text, _obj.Info.Properties.LowerConfidenceLimit);
      }
      
      // При наличии "Жёстких" ошибок не переходить к ForceSave.
      if (isArioUrlValidationMessageError || isConfidenceLimitsValidationMessageError)
        return;
      
      var isForceSave = e.Params.Contains(Constants.SmartProcessingSetting.ForceSaveParamName);
      if (!isForceSave)
      {
        // "Мягкая" проверка адреса сервиса Ario.
        if (arioUrlValidationMessage != null && arioUrlValidationMessage.Type == MessageTypes.Warning)
          e.AddError(arioUrlValidationMessage.Text, _obj.Info.Actions.ForceSave);
        
        // "Мягкая" проверка классификаторов.
        var classifierValidationMessage = Functions.SmartProcessingSetting.ValidateClassifiers(_obj);
        if (classifierValidationMessage != null && classifierValidationMessage.Type == MessageTypes.Warning)
          e.AddError(classifierValidationMessage.Text, _obj.Info.Actions.ForceSave);
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