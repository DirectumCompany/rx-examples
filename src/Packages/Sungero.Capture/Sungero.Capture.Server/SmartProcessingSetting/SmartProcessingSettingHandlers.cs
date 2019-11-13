using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture
{
  partial class SmartProcessingSettingServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Валидация адреса сервиса Ario.
      var isSafeFromUI = e.Params.Contains(Constants.SmartProcessingSetting.SaveFromUIParamName);
      var validationError = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (!string.IsNullOrEmpty(validationError.Text))
      {
        if (isSafeFromUI && validationError.Type == Constants.SmartProcessingSetting.ArioUrlValidationErrorTypes.WrongFormat)
          e.AddError(validationError.Text);
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