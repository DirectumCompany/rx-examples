using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать документ на основе пакета документов со сканера.
    /// </summary>
    /// <param name="senderLine">Наименование линии.</param>
    /// <param name="instanceInfos">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <param name="deviceInfo">Путь к xml файлу DCS c информацией об устройствах ввода.</param>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    public static void ProcessCapturedPackage(string senderLine, string instanceInfos, string deviceInfo, string filesInfo, string folder,
                                              string responsibleId, string firstPageClassifierName, string typeClassifierName)
    {
      // Найти ответственного.
      Logger.Debug("Begin of captured package processing...");
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
        throw new ApplicationException(Resources.InvalidResponsibleId);
      
      // Получить имена файлов.
      var filePath = GetScannedPackagePath(filesInfo, folder);
      var sourceFileName = GetScannedPackageName(instanceInfos);
      if (string.IsNullOrEmpty(filePath))
        throw new ApplicationException(Resources.FileNotFoundFormat(filePath));
      
      // Разделить пакет на документы.
      Logger.DebugFormat("Begin of package \"{0}\" splitting and classification...", sourceFileName);
      var arioUrl = Functions.Module.Remote.GetArioUrl();
      if (string.IsNullOrEmpty(arioUrl))
        throw new ApplicationException(Resources.EmptyArioUrl);
      
      var jsonClassificationResults = ProcessPackage(filePath, arioUrl, firstPageClassifierName, typeClassifierName);
      Logger.DebugFormat("End of package \"{0}\" splitting and classification.", sourceFileName);
      
      // Принудительно обвалить захват, если Ario вернул ошибку. DCS запишет в лог и перезапустит процесс.
      var ErrorMessage = ArioExtensions.ArioConnector.GetErrorMessageFromClassifyAndExtractFactsResult(jsonClassificationResults);
      if (ErrorMessage != null && !string.IsNullOrWhiteSpace(ErrorMessage.Message))
        throw new ApplicationException(ErrorMessage.Message);
      
      // Обработать пакет.
      Logger.DebugFormat("Begin of splitted package \"{0}\" processing...", sourceFileName);
      Functions.Module.Remote.ProcessSplitedPackage(sourceFileName, jsonClassificationResults, responsible);
      Logger.DebugFormat("End of splitted package \"{0}\" processing.", sourceFileName);
      Logger.Debug("End of captured package processing.");
    }
    
    /// <summary>
    /// Разделить пакет на документы, классифицировать и извлечь из документов факты с помощью сервиса Ario.
    /// </summary>
    /// <param name="filePath">Путь к пакету.</param>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <returns>Json с результатом классификации и извлечения фактов.</returns>
    public static string ProcessPackage(string filePath, string arioUrl, string firstPageClassifierName, string typeClassifierName)
    {
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var fpClassifier = arioConnector.GetClassifierByName(firstPageClassifierName);
      if (fpClassifier == null)
        throw new ApplicationException(Resources.ClassifierNotFoundFormat(firstPageClassifierName));
      
      var typeClassifier = arioConnector.GetClassifierByName(typeClassifierName);
      if (typeClassifier == null)
        throw new ApplicationException(Resources.ClassifierNotFoundFormat(firstPageClassifierName));
      
      var fpClassifierId = fpClassifier.Id.ToString();
      var typeClassifierId = typeClassifier.Id.ToString();
      
      Logger.DebugFormat("First page classifier: name - \"{0}\", id - {1}.", firstPageClassifierName, fpClassifierId);
      Logger.DebugFormat("Type classifier: name - \"{0}\", id - {1}.", typeClassifierName, typeClassifierId);
      
      var ruleMapping = GetClassRuleMapping();
      return arioConnector.ClassifyAndExtractFacts(File.ReadAllBytes(filePath), Path.GetFileName(filePath), typeClassifierId, fpClassifierId, ruleMapping);
    }
    
    /// <summary>
    /// Получить соответствие класса и имени правила его обработки.
    /// </summary>
    /// <returns></returns>
    [Public]
    public static System.Collections.Generic.Dictionary<string, string> GetClassRuleMapping()
    {
      return new Dictionary<string, string>()
      {
        { "Входящее письмо", "Letter"},
        { Constants.Module.LetterClassName, "Letter"},
        { Constants.Module.ContractStatementClassName, "ContractStatement"},
        { Constants.Module.WaybillClassName, "Waybill"},
        { Constants.Module.UniversalTransferDocumentClassName, "GeneralTransferDocument"},
        { Constants.Module.IncomingTaxInvoiceClassName, "TaxInvoice"}
      };
    }
    
    /// <summary>
    /// Получить путь к пакету документов со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Путь к пакету документов со сканера.</returns>
    public static string GetScannedPackagePath(string filesInfo, string folder)
    {
      if (!File.Exists(filesInfo))
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      var fileElement = filesXDoc
        .Element("InputFilesSection")
        .Element("Files")
        .Elements()
        .FirstOrDefault();
      if (fileElement == null)
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
            
      var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
      if (!File.Exists(filePath))
      {
        Logger.Error(Resources.FileNotFoundFormat(filePath));
        return string.Empty;
      }
      
      return filePath;
    }
    
    /// <summary>
    /// Получить имя пакета документов со сканера.
    /// </summary>
    /// <param name="instanceInfos">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <returns>Путь к пакету документов со сканера.</returns>
    public static string GetScannedPackageName(string instanceInfos)
    {
      if (!File.Exists(instanceInfos))
        throw new ApplicationException(Resources.NoFilesInfoInPackage);  
      
      var filesXDoc = System.Xml.Linq.XDocument.Load(instanceInfos);
      var fileElement = filesXDoc
        .Element("CaptureInstanceInfoList")
        .Element("FileSystemCaptureInstanceInfo")
        .Element("Files")
        .Elements()
        .FirstOrDefault();
      if (fileElement == null)
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      
      return fileElement.Element("FileDescription").Value;
    }
    
    /// <summary>
    /// Включить демо-режим.
    /// </summary>
    public static void SwitchToCaptureMockMode()
    {
      Sungero.Capture.Functions.Module.Remote.InitCaptureMockMode();
    }
    
    /// <summary>
    /// Задать основные настройки захвата.
    /// </summary>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="minFactProbability">Минимальная вероятность для факта.</param>
    /// <param name="trustedFactProbability">Доверительная вероятность для факта.</param>
    public static void SetCaptureMainSettings(string arioUrl, string minFactProbability, string trustedFactProbability)
    {
      Sungero.Capture.Functions.Module.Remote.SetCaptureMainSettings(arioUrl, minFactProbability, trustedFactProbability);
    }
  }
}