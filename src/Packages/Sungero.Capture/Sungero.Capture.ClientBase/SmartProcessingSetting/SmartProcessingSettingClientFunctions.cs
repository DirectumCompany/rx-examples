using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture.Client
{
  partial class SmartProcessingSettingFunctions
  {

    /// <summary>
    /// Выбрать классификатор.
    /// </summary>
    public void ChooseClassifier()
    {
      var dialog = Dialogs.CreateInputDialog("Выбор классификатора по типу");
      
      var classifier = dialog.AddSelect("Классификатор", true);
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      dialog.Show();
    }

  }
}