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
      var resources = Sungero.Capture.SmartProcessingSettings.Resources;
      if (!SmartProcessingSettings.AccessRights.CanUpdate())
      {
        Dialogs.ShowMessage(resources.ClassifiersSelectionNotAvailable);
        return null;
      }
      
      // Проверка адреса сервиса Ario.
      var arioUrlValidationMessage = Functions.SmartProcessingSetting.ValidateArioUrl(_obj);
      if (arioUrlValidationMessage != null)
      {
        Dialogs.ShowMessage(arioUrlValidationMessage.Text, MessageType.Information);
        return null;
      }
      
      var classifiers = Functions.SmartProcessingSetting.Remote.GetArioClassifiers(_obj);
      if (!classifiers.Any())
      {
        Dialogs.NotifyMessage(Sungero.Capture.SmartProcessingSettings.Resources.ClassifierSelectionError);
        return null;
      }
      
      var dialog = Dialogs.CreateInputDialog(dialogTitle);
      var classifierDisplayNames = classifiers.OrderBy(x => x.Name).Select(x => resources.ClassifierDisplayNameTemplateFormat(x.Name, x.Id).ToString());
      var classifier = dialog.AddSelect(resources.Classifier, true).From(classifierDisplayNames.ToArray());
      
      dialog.SetOnRefresh(e =>
                          {
                            var selectedClassifier = dialogTitle == resources.SelectTypeClassifierDialogTitle
                              ? resources.ClassifierDisplayNameTemplateFormat(_obj.FirstPageClassifierName, _obj.FirstPageClassifierId)
                              : resources.ClassifierDisplayNameTemplateFormat(_obj.TypeClassifierName, _obj.TypeClassifierId);
                            if (classifier.Value == selectedClassifier)
                              e.AddWarning(resources.SelectedSameClassifierWarning);
                          });
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      if (dialog.Show() == DialogButtons.Ok)
        return classifiers.SingleOrDefault(x => classifier.Value == resources.ClassifierDisplayNameTemplateFormat(x.Name, x.Id));
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