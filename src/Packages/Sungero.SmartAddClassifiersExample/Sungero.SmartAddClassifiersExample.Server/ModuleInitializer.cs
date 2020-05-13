using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.SmartAddClassifiersExample.Server
{
  public partial class ModuleInitializer
  {
    
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      InitializationLogger.Debug("Init: Fill smart processing additional classifiers.");
      FillSmartAdditionalClassifiers();
    }
    
    /// <summary>
    /// Заполнить доп. классификаторы в настройках интеллектуальной обработки.
    /// </summary>
    [Public]
    public virtual void FillSmartAdditionalClassifiers()
    {
      var smartProcessingSettings = Sungero.Docflow.PublicFunctions.SmartProcessingSetting.GetSettings();
      if (smartProcessingSettings != null)
      {
        // Для примера взять классификатор по типу документа.
        var classifierId = smartProcessingSettings.TypeClassifierId;
        var classifierName = smartProcessingSettings.TypeClassifierName;
        
        var additionalClassifier = smartProcessingSettings.AdditionalClassifiers.AddNew();
        additionalClassifier.ClassifierId = classifierId;
        additionalClassifier.ClassifierName = classifierName;
        
        smartProcessingSettings.Save();
      }
    }
    
  }
}
