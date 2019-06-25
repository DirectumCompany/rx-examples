﻿using System;
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
    /// <param name="responsibleId">ИД сотрудника, ответственного за обработку захваченных документов.</param>
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
      
      var arioUrl = Functions.Module.Remote.GetArioUrl();
      if (string.IsNullOrEmpty(arioUrl))
        throw new ApplicationException(Resources.EmptyArioUrl);
      
      var source = GetSourceType(deviceInfo);
      
      if (source == Constants.Module.CaptureSourceType.Mail)
        ProcessMailPackage(filesInfo, folder, arioUrl, firstPageClassifierName, typeClassifierName, responsible);
      if (source == Constants.Module.CaptureSourceType.Folder)
        ProcessScanPackage(filesInfo, folder, arioUrl, firstPageClassifierName, typeClassifierName, responsible);
    }
    
    /// <summary>
    /// Получить тип источника захвата.
    /// </summary>
    /// <param name="deviceInfo">Путь к xml файлу DCS c информацией об устройствах ввода.</param>
    /// <returns>Тип источника захвата.</returns>
    public static string GetSourceType(string deviceInfo)
    {
      if (!File.Exists(deviceInfo))
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      
      var filesXDoc = System.Xml.Linq.XDocument.Load(deviceInfo);
      var element = filesXDoc.Element("MailSourceInfo");
      if (element != null)
        return Constants.Module.CaptureSourceType.Mail;
      return Constants.Module.CaptureSourceType.Folder;
    }
    
    /// <summary>
    /// Обработать пакет пришедший со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку захваченных документов.</param>
    private static void ProcessScanPackage(string filesInfo, string folder,
                                           string arioUrl, string firstPageClassifierName, string typeClassifierName,
                                           Sungero.Company.IEmployee responsible)
    {
      var fileNames = GetScannedPackagesPaths(filesInfo, folder);
      if (!fileNames.Any())
        throw new ApplicationException("Files not found");
      
      foreach (var fileName in fileNames)
      {
        var classificationAndExtractionResult = TryClassifyAndExtractFacts(arioUrl, fileName, firstPageClassifierName, typeClassifierName);
        Logger.DebugFormat("Begin package processing. File: {0}", fileName);
        var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResult.Result, fileName, null, responsible, null);
        Functions.Module.Remote.SendToResponsible(documents, responsible);
        Logger.DebugFormat("End package processing. File: {0}", fileName);
        Logger.Debug("End of captured package processing.");
      }
    }
    
    /// <summary>
    /// Обработать пакет пришедший с почты.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку захваченных документов.</param>
    private static void ProcessMailPackage(string filesInfo, string folder,
                                           string arioUrl, string firstPageClassifierName, string typeClassifierName,
                                           Sungero.Company.IEmployee responsible)
    {
      var mailFilesPaths = GetCapturedMailFilesPaths(filesInfo, folder);
      if (string.IsNullOrWhiteSpace(mailFilesPaths.Body) && !mailFilesPaths.Attachments.Any())
        throw new ApplicationException("Files not found");
      
      // TODO Dmitriev: Создать входящее письмо.

      var relatedDocumentIds = new List<int>();
      foreach (var attachment in mailFilesPaths.Attachments)
      {
        var classificationAndExtractionResult = TryClassifyAndExtractFacts(arioUrl, attachment, firstPageClassifierName, typeClassifierName, false);
        if (classificationAndExtractionResult.Error == null ||
            string.IsNullOrWhiteSpace(classificationAndExtractionResult.Error))
        {
          //TODO Передать явно ведущий документ!!!
          var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResult.Result, attachment, null, responsible, null);
          relatedDocumentIds.AddRange(documents.RelatedDocumentIds);
        }
      }
      
      //TODO Передать явно ID ведущего документа!!!
      var documentsToSend = Structures.Module.DocumentsCreatedByRecognitionResults.Create();
      documentsToSend.LeadingDocumentId = 0;
      documentsToSend.RelatedDocumentIds = relatedDocumentIds;
      Functions.Module.Remote.SendToResponsible(documentsToSend, responsible);
    }
    
    /// <summary>
    /// Выполнить классификацию и распознавание для документа.
    /// </summary>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="fileName">Имя классифицируемого файла.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="throwOnError">Выбросить исключение, если возникла ошибка при классификации и распозновании.</param>
    /// <returns>Структура, содержащая json с результатами классификации и распознавания и сообщение об ошибке при наличии.</returns>
    private static Structures.Module.ClassificationAndExtractionResult TryClassifyAndExtractFacts(string arioUrl,
                                                                                                  string fileName,
                                                                                                  string firstPageClassifierName,
                                                                                                  string typeClassifierName,
                                                                                                  bool throwOnError = true)
    {
      var classificationAndExtractionResult = Structures.Module.ClassificationAndExtractionResult.Create();
      var filePath = System.IO.Path.GetFileName(fileName);
      Logger.DebugFormat("Begin classification and facts extraction. File: {0}", filePath);
      classificationAndExtractionResult.Result = ProcessPackage(filePath, arioUrl, firstPageClassifierName, typeClassifierName);
      Logger.DebugFormat("End classification and facts extraction. File: {0}", filePath);
      
      var nativeError = ArioExtensions.ArioConnector.GetErrorMessageFromClassifyAndExtractFactsResult(classificationAndExtractionResult.Result);
      classificationAndExtractionResult.Error = nativeError == null ? string.Empty : nativeError.Message;
      if (classificationAndExtractionResult.Error == null ||
          string.IsNullOrWhiteSpace(classificationAndExtractionResult.Error))
        return classificationAndExtractionResult;
      
      if (throwOnError)
        throw new ApplicationException(classificationAndExtractionResult.Error);
      
      Logger.Error(classificationAndExtractionResult.Error);
      return classificationAndExtractionResult;
    }
    
    /// <summary>
    /// Создать документ в DirectumRX на основе данных распознования.
    /// </summary>
    /// <param name="bodyFilePath">Путь до исходного файла.</param>
    /// <param name="jsonFilePath">Путь до файла json с результатом распознавания.</param>
    /// <param name="responsibleId">Id сотрудника ответственного за распознавание документов.</param>
    public static void CreateDocumentByRecognitionData(string bodyFilePath, string jsonFilePath, string responsibleId)
    {
      Logger.Debug("Start CreateDocumentByRecognitionData");
      
      if (!System.IO.File.Exists(bodyFilePath))
      {
        Logger.ErrorFormat("File does not exist {0}", bodyFilePath);
        return;
      }
      
      if (!System.IO.File.Exists(jsonFilePath))
      {
        Logger.ErrorFormat("File does not exist {0}", jsonFilePath);
        return;
      }
      
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
        throw new ApplicationException(Resources.InvalidResponsibleId);
      Logger.DebugFormat("Responsible: {0}", responsible.Person.ShortName);
      
      var arioUrl = Sungero.Capture.Functions.Module.Remote.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      
      // Загрузить документ в Ario с преобразованием в pdf.
      var convertionResults = arioConnector.ConvertDocumentToPdfAndGetGuid(System.IO.File.ReadAllBytes(bodyFilePath),
                                                                           System.IO.Path.GetFileName(bodyFilePath));
      if (convertionResults == null)
        return;
      var convertionResult = convertionResults.Results.FirstOrDefault();
      if (convertionResult == null)
        return;
      var docPdfGuid = convertionResult.Guid;
      Logger.DebugFormat("Document Ario Guid: {0}", docPdfGuid);
      
      // Заменить guid документа в исходном json'е на полученный из Ario.
      var modifiedJson = arioConnector.UpdateGuidInClassificationResults(System.IO.File.ReadAllText(jsonFilePath), docPdfGuid);
      Logger.Debug("Source Json updated.");
      
      // Обработать пакет.
      Logger.Debug("Start ProcessSplitedPackage");
      Functions.Module.Remote.CreateDocumentsByRecognitionResults(modifiedJson,
                                                                  System.IO.Path.GetFileName(bodyFilePath),
                                                                  null, responsible, null);
      Logger.Debug("Start ProcessSplitedPackage");
      Logger.Debug("End CreateDocumentByRecognitionData");
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
        { Constants.Module.GeneralCorrectionDocumentClassName, "GeneralCorrectionDocument"},
        { Constants.Module.TaxInvoiceClassName, "TaxInvoice"},
        { Constants.Module.TaxinvoiceCorrectionClassName, "TaxinvoiceCorrection"},
        { Constants.Module.IncomingInvoiceClassName, "IncomingInvoice"},
      };
    }
    
    /// <summary>
    /// Получить пути к пакетам документов со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Пути к пакетам документов со сканера.</returns>
    /// <remarks>Технически возможно, что документов будет несколько, но на практике приходит один.</remarks>
    public static List<string> GetScannedPackagesPaths(string filesInfo, string folder)
    {
      if (!File.Exists(filesInfo))
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      
      var filePaths = new List<string>();
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      var fileElements = filesXDoc
        .Element("InputFilesSection")
        .Element("Files")
        .Elements();
      
      if (!fileElements.Any())
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      foreach (var fileElement in fileElements)
      {
        var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
        if (!File.Exists(filePath))
        {
          Logger.Error(Resources.FileNotFoundFormat(filePath));
        }
        else
        {
          filePaths.Add(filePath);
        }
      }
      return filePaths;
    }
    
    /// <summary>
    /// Получить пути до захваченных с почты файлов.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Пути до захваченных с почты файлов.</returns>
    public static Structures.Module.CapturedMailFilesPaths GetCapturedMailFilesPaths(string filesInfo, string folder)
    {
      var mailFilesPaths = Structures.Module.CapturedMailFilesPaths.Create();
      mailFilesPaths.Attachments = new List<string>();
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return mailFilesPaths;
      }
      
      // Тело письма.
      var fileElements = filesXDoc.Element("InputFilesSection").Element("Files").Elements();
      var hasHtmlBody = fileElements.Any(x => string.Equals(x.Element("FileDescription").Value, "body.html", StringComparison.InvariantCultureIgnoreCase));
      var hasTxtBody = fileElements.Any(x => string.Equals(x.Element("FileDescription").Value, "body.txt", StringComparison.InvariantCultureIgnoreCase));
      var hasAssociatedAppForHtml = Sungero.Content.Shared.ElectronicDocumentUtils.GetAssociatedApplication("body.html") != null;
      if (hasAssociatedAppForHtml && hasHtmlBody)
        mailFilesPaths.Body= Path.Combine(folder, "body.html");
      else if (hasTxtBody)
        mailFilesPaths.Body= Path.Combine(folder, "body.txt");
      
      // Вложения.
      var attachments = fileElements.Where(x => !string.Equals(x.Element("FileDescription").Value, "body.html", StringComparison.InvariantCultureIgnoreCase) &&
                                           !string.Equals(x.Element("FileDescription").Value, "body.txt", StringComparison.InvariantCultureIgnoreCase));
      foreach (var attachment in attachments)
      {
        // Если для файла нет приложения-обработчика, то платформа не даст создать документ.
        var fileDescription = attachment.Element("FileDescription").Value;
        if (Sungero.Content.Shared.ElectronicDocumentUtils.GetAssociatedApplication(fileDescription) == null)
          continue;
        
        // Отбрасываем изображения из тела письма (например картинки из подписей).
        if (System.Text.RegularExpressions.Regex.IsMatch(fileDescription, @"^ATT\d+\s\d+\.\w+"))
          continue;
        
        var filePath = Path.Combine(folder, Path.GetFileName(attachment.Element("FileName").Value));
        if (!File.Exists(filePath))
        {
          Logger.Error(Resources.FileNotFoundFormat(filePath));
          continue;
        }
        
        mailFilesPaths.Attachments.Add(filePath);
      }
      
      return mailFilesPaths;
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
    /// Установить цвет у распознанных свойств в карточке документа.
    /// </summary>
    [Public]
    public virtual void SetPropertiesColors(Sungero.Docflow.IOfficialDocument document)
    {
      // Точно распознанные свойства документа подсветить зелёным цветом, неточно - жёлтым.
      // Точно и неточно распознанные свойства получить с сервера отдельными вызовами метода из-за ограничений платформы.
      var exactlyRecognizedProperties = Sungero.Capture.PublicFunctions.Module.Remote.GetRecognizedDocumentProperties(document, true);
      HighlightProperties(document, exactlyRecognizedProperties, Sungero.Core.Colors.Highlights.Green);
      
      var notExactlyRecognizedProperties = Sungero.Capture.PublicFunctions.Module.Remote.GetRecognizedDocumentProperties(document, false);
      HighlightProperties(document, notExactlyRecognizedProperties, Sungero.Core.Colors.Highlights.Yellow);
    }
    
    /// <summary>
    /// Подсветить указанные свойства в карточке документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="propertyNames">Список имён свойств.</param>
    /// <param name="color">Цвет.</param>
    public virtual void HighlightProperties(Sungero.Docflow.IOfficialDocument document, List<string> propertyNames, Sungero.Core.Color color)
    {
      foreach (var propertyName in propertyNames)
      {
        var property = document.GetType().GetProperty(propertyName);
        if (property != null)
          document.State.Properties[propertyName].HighlightColor = color;
      }
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