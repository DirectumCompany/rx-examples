using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.SmartProcessingSetting;

namespace Sungero.Capture.Server
{
  partial class SmartProcessingSettingFunctions
  {
    /// <summary>
    /// Создать настройки интеллектуальной обработки документов.
    /// </summary>
    [Remote, Public]
    public static void CreateSmartProcessingSettings()
    {
      var smartProcessingSettings = SmartProcessingSettings.Create();
      
      // Заполнить из Docflow_Params, если ранее настройки хранились там.
      var arioUrlKey = Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ArioUrlKey).ToString();
      if (!string.IsNullOrEmpty(arioUrlKey))
        smartProcessingSettings.ArioUrl = arioUrlKey;
      
      var minFactProbability = Functions.Module.GetDocflowParamsNumbericValue(Constants.Module.MinFactProbabilityKey);
      if (minFactProbability != 0)
        smartProcessingSettings.LowerConfidenceLimit = (int) minFactProbability;
      
      var trustedFactProbability = Functions.Module.GetDocflowParamsNumbericValue(Constants.Module.TrustedFactProbabilityKey);
      if (trustedFactProbability != 0)
        smartProcessingSettings.UpperConfidenceLimit = (int) trustedFactProbability;
      
      smartProcessingSettings.Save();
      
      // Удалить параметры с настройками из Docflow_Params.
      Docflow.PublicFunctions.Module.ExecuteSQLCommand(Queries.Module.DeleteSmartSettingsFromDocflowParams);
    }
    
    /// <summary>
    /// Проверить подключение к Ario.
    /// </summary>
    /// <returns>True, если сервис работает, иначе - False.</returns>
    [Remote]
    public virtual bool CheckConnection()
    {
      var arioConnector = new ArioExtensions.ArioConnector(_obj.ArioUrl);
      ArioExtensions.Models.ArioInfo serviceInfo = null;
      
      try
      {
        serviceInfo = arioConnector.GetInfo();
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
      }
      
      return serviceInfo != null && serviceInfo.State == Constants.SmartProcessingSetting.ArioConnectionSuccessMessage;
    }
    
    /// <summary>
    /// Получить список классификаторов из Арио.
    /// </summary>
    /// <returns>Список классификаторов.</returns>
    [Remote]
    public virtual List<Structures.SmartProcessingSetting.Classifier> GetClassifiers()
    {
      var classifiers = new List<Structures.SmartProcessingSetting.Classifier>();
      try
      {
        var arioConnector = new ArioExtensions.ArioConnector(_obj.ArioUrl);
        classifiers = arioConnector.GetClassifiers().Select(x => Structures.SmartProcessingSetting.Classifier.Create(x.Id, x.Name)).ToList();
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
      }
      return classifiers;
    }
  }
}