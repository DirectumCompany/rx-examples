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
    /// Запустить диалог выбора классификатора.
    /// </summary>
    /// <param name="dialogTitle">Заголовок диалога.</param>
    /// <returns>Классификатор.</returns>
    public Sungero.Capture.Structures.SmartProcessingSetting.Classifier RunClassifierSelectionDialog(string dialogTitle)
    {
      var classifiers = Functions.SmartProcessingSetting.Remote.GetClassifiers(_obj);
      if (!classifiers.Any())
      {
        Dialogs.NotifyMessage(SmartProcessingSettings.Resources.ClassifierSelectionError);
        return null;
      }

      var dialog = Dialogs.CreateInputDialog(dialogTitle);
      var classifierDisplayNames = classifiers.OrderBy(x => x.Name).Select(x => string.Format("{0} ({1})", x.Name, x.Id));
      var classifier = dialog.AddSelect(SmartProcessingSettings.Resources.Classifier, true).From(classifierDisplayNames.ToArray());
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      if (dialog.Show() == DialogButtons.Ok)
        return classifiers.SingleOrDefault(x => classifier.Value == string.Format("{0} ({1})", x.Name, x.Id));
      return null;
    }
    
    /// <summary>
    /// Выбрать классификатор по типам документов.
    /// </summary>
    public void SelectTypeClassifier()
    {
      var typeClassifier = RunClassifierSelectionDialog(SmartProcessingSettings.Resources.SelectTypeClassifierDialogTitle);
      if (typeClassifier != null)
      {
        _obj.TypeClassifierId = typeClassifier.Id;
        _obj.TypeClassifierName = typeClassifier.Name;
      }
    }
    
    /// <summary>
    /// Выбрать классификатор первых страниц.
    /// </summary>
    public void SelectFirstPageClassifier()
    {
      var firstPageClassifier = RunClassifierSelectionDialog(SmartProcessingSettings.Resources.SelectFirstPageClassifierDialogTitle);
      if (firstPageClassifier != null)
      {
        _obj.FirstPageClassifierId = firstPageClassifier.Id;
        _obj.FirstPageClassifierName = firstPageClassifier.Name;
      }
    }
  }
}