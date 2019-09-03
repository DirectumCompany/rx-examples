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
    #region Захват
    
    /// <summary>
    /// Обработать пакет документов со сканера или почты.
    /// </summary>
    /// <param name="senderLine">Наименование линии.</param>
    /// <param name="instanceInfo">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <param name="deviceInfo">Путь к xml файлу DCS c информацией об устройствах ввода.</param>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="responsibleId">ИД сотрудника, ответственного за обработку захваченных документов.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    public virtual void ProcessCapturedPackage(string senderLine, string instanceInfo, string deviceInfo, string filesInfo, string folder,
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
      
      // Захват с почты.
      var source = GetSourceType(deviceInfo);
      if (source == Constants.Module.CaptureSourceType.Mail)
        ProcessEmailPackage(filesInfo, folder, instanceInfo, arioUrl, firstPageClassifierName, typeClassifierName, responsible);
      
      // Захват с папки (сканера).
      if (source == Constants.Module.CaptureSourceType.Folder)
        ProcessScanPackage(filesInfo, folder, arioUrl, firstPageClassifierName, typeClassifierName, responsible);
    }
    
    /// <summary>
    /// Получить тип источника захвата.
    /// </summary>
    /// <param name="deviceInfo">Путь к xml файлу DCS c информацией об устройствах ввода.</param>
    /// <returns>Тип источника захвата.</returns>
    public virtual string GetSourceType(string deviceInfo)
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
    /// Задать основные настройки захвата.
    /// </summary>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="minFactProbability">Минимальная вероятность для факта.</param>
    /// <param name="trustedFactProbability">Доверительная вероятность для факта.</param>
    public static void SetCaptureMainSettings(string arioUrl, string minFactProbability, string trustedFactProbability)
    {
      Sungero.Capture.Functions.Module.Remote.SetCaptureMainSettings(arioUrl, minFactProbability, trustedFactProbability);
    }
    
    #endregion
    
    #region Обработка пакета с эл. почты
    
    /// <summary>
    /// Обработать пакет пришедший с эл.почты.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="instanceInfo">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку захваченных документов.</param>
    public virtual void ProcessEmailPackage(string filesInfo, string folder, string instanceInfo,
                                            string arioUrl, string firstPageClassifierName, string typeClassifierName,
                                            Sungero.Company.IEmployee responsible)
    {
      Logger.Debug("Captured Package Process. Captured package type is MAIL.");
      var mailFiles = GetCapturedMailFiles(filesInfo, folder);
      if ((mailFiles.Body == null || !File.Exists(mailFiles.Body.Path)) && !mailFiles.Attachments.Any())
        throw new ApplicationException("Captured Package Process. Mail body and attached files does not exists.");
      
      // Для писем без тела не создавать простой документ.
      var mailInfo = GetMailInfo(instanceInfo);
      var emailBody = Docflow.SimpleDocuments.Null;
      if (mailFiles.Body != null && !string.IsNullOrEmpty(mailFiles.Body.Path))
      {
        RemoveImagesFromEmailBody(mailFiles.Body.Path);
        mailFiles.Body.Data = System.IO.File.ReadAllBytes(mailFiles.Body.Path);
        
        emailBody = Functions.Module.Remote.CreateSimpleDocumentFromEmailBody(mailInfo, mailFiles.Body, responsible);
        Logger.Debug("Captured Package Process. Document from e-mail body created.");
      }
      else
      {
        Logger.Debug("Captured Package Process. E-mail body is empty, document from e-mail body was not created.");
      }
      
      var package = new List<Docflow.IOfficialDocument>();
      var notRecognizedDocuments = new List<Docflow.IOfficialDocument>();
      if (emailBody != null)
        notRecognizedDocuments.Add(emailBody);
      
      foreach (var attachment in mailFiles.Attachments)
      {
        var fileName = attachment.Description;
        Logger.DebugFormat("Captured Package Process. Attachment: {0}", fileName);
        attachment.Data = System.IO.File.ReadAllBytes(attachment.Path);
        
        Logger.DebugFormat("Captured Package Process. Try classify and extract facts. {0}", fileName);
        var classificationAndExtractionResult = TryClassifyAndExtractFacts(attachment.Path, arioUrl, firstPageClassifierName, typeClassifierName, false);
        if (string.IsNullOrWhiteSpace(classificationAndExtractionResult.Error))
        {
          Logger.DebugFormat("Captured Package Process. Create documents by recognition results. {0}", fileName);
          var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResult.Result,
                                                                                      attachment,
                                                                                      responsible,
                                                                                      true, mailInfo.FromEmail);
          package.AddRange(documents);
        }
        else
        {
          Logger.DebugFormat("Captured Package Process. Has some errors with classification and facts extraction. {0}", fileName);
          var document = Functions.Module.Remote.CreateSimpleDocumentFromFile(attachment, true, responsible);
          Logger.DebugFormat("Captured Package Process. Simple document created. {0}", fileName);
          notRecognizedDocuments.Add(document);
        }
      }
      
      var documentsCreatedByRecognitionResults = Functions.Module.Remote.ProcessPackageAfterCreationDocuments(package, notRecognizedDocuments, false);
      Logger.Debug("Captured Package Process. Send documents to responsible.");

      Functions.Module.Remote.SendToResponsible(documentsCreatedByRecognitionResults, emailBody, responsible);
      Logger.Debug("Captured Package Process. Done.");
    }
    
    /// <summary>
    /// Получить пути до захваченных с почты файлов.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Пути до захваченных с почты файлов.</returns>
    public virtual Structures.Module.CapturedMailFiles GetCapturedMailFiles(string filesInfo, string folder)
    {
      var mailFiles = Structures.Module.CapturedMailFiles.Create();
      mailFiles.Attachments = new List<Structures.Module.IFileInfo>();
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return mailFiles;
      }
      
      // Тело письма.
      var fileElements = filesXDoc.Element("InputFilesSection").Element("Files").Elements();
      var htmlBodyElement = fileElements.FirstOrDefault(x => string.Equals(x.Element("FileDescription").Value, "body.html", StringComparison.InvariantCultureIgnoreCase));
      var txtBodyElement = fileElements.FirstOrDefault(x => string.Equals(x.Element("FileDescription").Value, "body.txt", StringComparison.InvariantCultureIgnoreCase));
      var hasHtmlBody = htmlBodyElement != null;
      var hasTxtBody = txtBodyElement != null;
      
      // Не создавать документ для писем с пустым телом.
      // В некоторых случаях (например, при отправке из Outlook в Яндекс) для писем с пустым телом генерируется фейковое тело,
      // представляющее из себя только перевод строки. Такие тела заносить также не нужно.
      
      // Получить текст из тела письма.
      var bodyText = string.Empty;
      if (hasHtmlBody)
      {
        var htmlBodyInfo = CreateFileInfoFromXelement(htmlBodyElement, folder);
        bodyText = AsposeExtensions.HtmlTagReader.GetTextFromHtml(htmlBodyInfo.Path);
      }
      else if (hasTxtBody)
      {
        var txtBodyInfo = CreateFileInfoFromXelement(txtBodyElement, folder);
        bodyText = File.ReadAllText(txtBodyInfo.Path);
      }
      
      // Очистить текст из тела письма от спецсимволов, чтобы определить, пуст ли он.
      var clearBodyText = bodyText.Trim(new[] {' ', '\r', '\n', '\0'});
      if (!string.IsNullOrWhiteSpace(clearBodyText))
      {
        var bodyElement = hasHtmlBody ? htmlBodyElement : txtBodyElement;
        mailFiles.Body = CreateFileInfoFromXelement(bodyElement, folder);
      }
      
      // Вложения.
      var attachments = fileElements.Where(x => !string.Equals(x.Element("FileDescription").Value, "body.html", StringComparison.InvariantCultureIgnoreCase) &&
                                           !string.Equals(x.Element("FileDescription").Value, "body.txt", StringComparison.InvariantCultureIgnoreCase)).ToList();
      
      // Фильтрация картинок из тела письма.
      if (mailFiles.Body != null && !string.IsNullOrEmpty(mailFiles.Body.Path) && hasHtmlBody)
        attachments = FilterEmailBodyInlineImages(mailFiles.Body.Path, attachments);

      foreach (var attachment in attachments)
      {
        var fileDescription = attachment.Element("FileDescription").Value;
        mailFiles.Attachments.Add(CreateFileInfoFromXelement(attachment, folder));
      }
      
      return mailFiles;
    }
    
    /// <summary>
    /// Получить информацию о захваченном письме.
    /// </summary>
    /// <param name="instanceInfo">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <returns>Информация о захваченном письме.</returns>
    public virtual Structures.Module.CapturedMailInfo GetMailInfo(string instanceInfo)
    {
      var result = Structures.Module.CapturedMailInfo.Create();
      
      if (!File.Exists(instanceInfo))
        throw new ApplicationException(Resources.FileNotFoundFormat(instanceInfo));
      
      var infoXDoc = System.Xml.Linq.XDocument.Load(instanceInfo);
      if (infoXDoc == null)
        return result;
      
      var mailCaptureInstanceInfoElement = infoXDoc
        .Element("CaptureInstanceInfoList")
        .Element("MailCaptureInstanceInfo");
      
      if (mailCaptureInstanceInfoElement == null)
        return result;
      
      result.Subject = GetAttributeStringValue(mailCaptureInstanceInfoElement, "Subject");
      
      var fromElement = mailCaptureInstanceInfoElement.Element("From");
      if (fromElement == null)
        return result;
      
      result.FromEmail = GetAttributeStringValue(fromElement, "Address");
      result.Name = GetAttributeStringValue(fromElement, "Name");
      
      return result;
    }
    
    /// <summary>
    /// Отфильтровать изображения, пришедшие в теле письма.
    /// </summary>
    /// <param name="htmlBodyPath">Путь до тела письма.</param>
    /// <param name="attachments">Вложения.</param>
    /// <returns>Отфильтрованный список вложений.</returns>
    public virtual List<System.Xml.Linq.XElement> FilterEmailBodyInlineImages(string htmlBodyPath, List<System.Xml.Linq.XElement> attachments)
    {
      var inlineImagesCount = AsposeExtensions.HtmlTagReader.GetInlineImagesCount(htmlBodyPath);
      return attachments.Skip(inlineImagesCount).ToList();
    }
    
    /// <summary>
    /// Удалить изображения из тела письма.
    /// </summary>
    /// <param name="path">Пусть к html-файлу письма.</param>
    public virtual void RemoveImagesFromEmailBody(string path)
    {
      // Нет смысла удалять изображения в файлах, расширение которых не html.
      if (Path.GetExtension(path).ToLower() != ".html")
        return;

      try
      {
        var mailBody = File.ReadAllText(path);
        mailBody = System.Text.RegularExpressions.Regex.Replace(mailBody, @"<img([^\>]*)>", string.Empty);
        
        // В некоторых случаях Aspose не может распознать файл как html, поэтому добавляем тег html, если его нет.
        if (!mailBody.Contains("<html"))
          mailBody = string.Format("<html>{0}</html>", mailBody);
        File.WriteAllText(path, mailBody);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("RemoveImagesFromEmailBody: Cannot remove images from email body.", ex);
      }
    }
    
    /// <summary>
    /// Получить значение атрибута XElement.
    /// </summary>
    /// <param name="element">XElement.</param>
    /// <param name="attributeName">Имя атрибута.</param>
    /// <returns>Строковое значение атрибута. null, если атрибут отсутствует.</returns>
    public virtual string GetAttributeStringValue(System.Xml.Linq.XElement element, string attributeName)
    {
      var attribute = element.Attribute(attributeName);
      if (attribute != null)
        return attribute.Value;
      return null;
    }
    
    /// <summary>
    /// Создать информацию о файле на основе xml элемента.
    /// </summary>
    /// <param name="xmlElement">Xml элемент.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Информация о файле.</returns>
    public virtual Structures.Module.IFileInfo CreateFileInfoFromXelement(System.Xml.Linq.XElement xmlElement, string folder)
    {
      var fileInfo = Structures.Module.FileInfo.Create();
      fileInfo.Path = Path.Combine(folder, Path.GetFileName(xmlElement.Element("FileName").Value));
      fileInfo.Description = xmlElement.Element("FileDescription").Value;
      
      return fileInfo;
    }
    
    #endregion
    
    #region Обработка пакета со сканера
    
    /// <summary>
    /// Обработать пакет пришедший со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку захваченных документов.</param>
    public virtual void ProcessScanPackage(string filesInfo, string folder,
                                           string arioUrl, string firstPageClassifierName, string typeClassifierName,
                                           Sungero.Company.IEmployee responsible)
    {
      var packagesPaths = GetScannedPackagesPaths(filesInfo, folder);
      if (!packagesPaths.Any())
        throw new ApplicationException("Package not found");
      
      foreach (var packagePath in packagesPaths)
      {
        var classificationAndExtractionResult = TryClassifyAndExtractFacts(packagePath, arioUrl, firstPageClassifierName, typeClassifierName);
        Logger.DebugFormat("Begin package processing. Path: {0}", packagePath);
        var originalFile = new Structures.Module.FileInfo();
        originalFile.Path = packagePath;
        
        var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResult.Result,
                                                                                    originalFile,
                                                                                    responsible,
                                                                                    false, string.Empty);
        
        var documentsCreatedByRecognitionResults = Functions.Module.Remote.ProcessPackageAfterCreationDocuments(documents, null, true);
        Logger.Debug("Captured Package Process. Send documents to responsible.");
        Functions.Module.Remote.SendToResponsible(documentsCreatedByRecognitionResults, null, responsible);
        Logger.Debug("Captured Package Process. Done.");
      }
    }
    
    /// <summary>
    /// Получить пути к пакетам документов со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Пути к пакетам документов со сканера.</returns>
    /// <remarks>Технически возможно, что документов будет несколько, но на практике приходит один.</remarks>
    public virtual List<string> GetScannedPackagesPaths(string filesInfo, string folder)
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
        if (File.Exists(filePath))
          filePaths.Add(filePath);
        else
          Logger.Error(Resources.FileNotFoundFormat(filePath));
      }
      
      return filePaths;
    }
    
    #endregion
    
    #region Классификация и распознавание
    
    /// <summary>
    /// Выполнить классификацию и распознавание для документа.
    /// </summary>
    /// <param name="filePath">Путь к классифицируемому файлу.</param>
    /// <param name="arioUrl">Host Ario.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <param name="throwOnError">Выбросить исключение, если возникла ошибка при классификации и распозновании.</param>
    /// <returns>Структура, содержащая json с результатами классификации и распознавания и сообщение об ошибке при наличии.</returns>
    public virtual Structures.Module.ClassificationAndExtractionResult TryClassifyAndExtractFacts(string filePath,
                                                                                                  string arioUrl,
                                                                                                  string firstPageClassifierName,
                                                                                                  string typeClassifierName,
                                                                                                  bool throwOnError = true)
    {
      var classificationAndExtractionResult = Structures.Module.ClassificationAndExtractionResult.Create();
      if (!CanArioProcessFile(filePath))
      {
        classificationAndExtractionResult.Error = Resources.CantProcessFileByArio;
        return classificationAndExtractionResult;
      }
      
      var processResult = ProcessPackage(filePath, arioUrl, firstPageClassifierName, typeClassifierName);
      var nativeError = ArioExtensions.ArioConnector.GetErrorMessageFromClassifyAndExtractFactsResult(processResult);
      classificationAndExtractionResult.Result = processResult;
      if (nativeError == null || string.IsNullOrWhiteSpace(nativeError.Message))
        return classificationAndExtractionResult;
      
      if (throwOnError)
        throw new ApplicationException(nativeError.Message);
      
      Logger.Error(nativeError.Message);
      classificationAndExtractionResult.Error = nativeError.Message;
      return classificationAndExtractionResult;
    }
    
    /// <summary>
    /// Разделить пакет на документы, классифицировать и извлечь из документов факты с помощью сервиса Ario.
    /// </summary>
    /// <param name="filePath">Путь к пакету.</param>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типу.</param>
    /// <returns>Json с результатом классификации и извлечения фактов.</returns>
    public virtual string ProcessPackage(string filePath, string arioUrl, string firstPageClassifierName, string typeClassifierName)
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
      
      var fileName = System.IO.Path.GetFileName(filePath);
      var ruleMapping = GetClassRuleMapping();
      Logger.DebugFormat("Begin classification and facts extraction. File: {0}", fileName);
      var result = arioConnector.ClassifyAndExtractFacts(File.ReadAllBytes(filePath), fileName, typeClassifierId, fpClassifierId, ruleMapping);
      Logger.DebugFormat("End classification and facts extraction. File: {0}", fileName);
      return result;
    }
    
    /// <summary>
    /// Определить может ли Ario обработать файл.
    /// </summary>
    /// <param name="fileName">Имя или путь до файла.</param>
    /// <returns>True - может, False - иначе.</returns>
    public virtual bool CanArioProcessFile(string fileName)
    {
      var ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
      var allowedExtensions = new List<string>()
      {
        "jpg", "jpeg", "png", "bmp", "gif",
        "tif", "tiff", "pdf", "doc", "docx",
        "dot", "dotx", "rtf", "odt", "ott",
        "txt", "xls", "xlsx", "ods", "pdf"
      };
      return allowedExtensions.Contains(ext);
    }
    
    /// <summary>
    /// Получить соответствие класса и наименования правила извлечения фактов.
    /// </summary>
    /// <returns></returns>
    [Public]
    public virtual System.Collections.Generic.Dictionary<string, string> GetClassRuleMapping()
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
        { Constants.Module.ContractClassName, "Contract"}
      };
    }
    
    #endregion
    
    #region Верификация
    
    /// <summary>
    /// Включить режим верификации.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public virtual void SwitchVerificationMode(Sungero.Docflow.IOfficialDocument document)
    {
      // Активировать / скрыть вкладку, подсветить свойства карточки и факты в теле только один раз при открытии.
      // Либо в событии Showing либо в Refresh.
      // Вызов в Refresh необходим, т.к. при отмене изменений не вызывается Showing.
      var formParams = ((Sungero.Domain.Shared.IExtendedEntity)document).Params;
      if (formParams.ContainsKey(Capture.PublicConstants.Module.PropertiesAlreadyColoredParamName))
        return;
      else
        formParams.Add(Capture.PublicConstants.Module.PropertiesAlreadyColoredParamName, true);
      
      // Активировать / скрыть вкладку.
      if (document.VerificationState != Docflow.OfficialDocument.VerificationState.InProcess)
      {
        document.State.Pages.PreviewPage.IsVisible = false;
        return;
      }
      document.State.Pages.PreviewPage.IsVisible = true;
      document.State.Pages.PreviewPage.Activate();
      
      // Точно распознанные свойства документа подсветить зелёным цветом, неточно - жёлтым.
      // Точно и неточно распознанные свойства получить с сервера отдельными вызовами метода из-за того, что получение списка структур с
      // атрибутом Public с помощью Remote-функции невозможно из-за ограничений платформы, а в данном случае Public необходим, так как
      // данная функция используется за пределами модуля.
      var exactlyRecognizedProperties = Sungero.Capture.PublicFunctions.Module.Remote.GetRecognitionResultProperties(document, true);
      HighlightPropertiesAndFacts(document, exactlyRecognizedProperties, Sungero.Core.Colors.Parse(Constants.Module.GreenHighlightsColorCode));
      
      var notExactlyRecognizedProperties = Sungero.Capture.PublicFunctions.Module.Remote.GetRecognitionResultProperties(document, false);
      HighlightPropertiesAndFacts(document, notExactlyRecognizedProperties, Sungero.Core.Colors.Parse(Constants.Module.YellowHighlightsColorCode));
    }
    
    /// <summary>
    /// Подсветить указанные свойства в карточке документа и факты в теле.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="propertyNamesAndPositions">Список имён свойств и позиций подсветки.</param>
    /// <param name="color">Цвет.</param>
    public virtual void HighlightPropertiesAndFacts(Sungero.Docflow.IOfficialDocument document, List<string> propertyNamesAndPositions, Sungero.Core.Color color)
    {
      var posColor = color == Sungero.Core.Colors.Parse(Constants.Module.YellowHighlightsColorCode)
        ? Sungero.Core.Colors.Common.Yellow
        : Sungero.Core.Colors.Common.Green;
      foreach (var propertyNameAndPosition in propertyNamesAndPositions)
      {
        // Подсветка полей карточки.
        var splitedPropertyNameAndPosition = propertyNameAndPosition.Split(Constants.Module.PropertyAndPositionDelimiter);
        var propertyName = splitedPropertyNameAndPosition[0];
        var property = document.GetType().GetProperties().Where(p => p.Name == propertyName).LastOrDefault();
        if (property != null)
          document.State.Properties[propertyName].HighlightColor = color;
        
        // Подсветка фактов в теле документа.
        if (splitedPropertyNameAndPosition.Count() > 1 && !string.IsNullOrWhiteSpace(splitedPropertyNameAndPosition[1]))
        {
          var fieldsPositions = splitedPropertyNameAndPosition[1].Split(Constants.Module.PositionsDelimiter);
          foreach (var fieldPosition in fieldsPositions)
          {
            var pos = fieldPosition.Split(Constants.Module.PositionElementDelimiter);
            if (pos.Count() < 7)
              continue;
            
            document.State.Controls.Preview.HighlightAreas.Add(posColor,
                                                               int.Parse(pos[0]),
                                                               double.Parse(pos[1]),
                                                               double.Parse(pos[2]),
                                                               double.Parse(pos[3]),
                                                               double.Parse(pos[4]),
                                                               double.Parse(pos[5]),
                                                               double.Parse(pos[6]));
          }
        }
      }
    }
    
    /// <summary>
    /// Разблокировать реквизиты для верификации после нумерации.
    /// </summary>
    /// <param name="document">Документ для верификации.</param>
    [Public]
    public virtual void EnableRequisitesForVerification(Sungero.Docflow.IAccountingDocumentBase document)
    {
      var smartCaptureNumerationSucceed = document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered &&
        document.VerificationState == Sungero.Docflow.OfficialDocument.VerificationState.InProcess &&
        document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable &&
        document.DocumentRegister != null;
      
      if (document.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess &&
          smartCaptureNumerationSucceed)
      {
        // Проверить возможность изменения реквизитов.
        if (!Sungero.Docflow.PublicFunctions.OfficialDocument.CanChangeRequisitesOrCancelRegistration(document))
          return;
        
        if (!document.AccessRights.CanUpdate() ||
            document.VerificationState != Docflow.OfficialDocument.VerificationState.InProcess ||
            document.DocumentKind.NumberingType != Docflow.DocumentKind.NumberingType.Numerable)
          return;

        var properties = document.State.Properties;
        properties.Name.IsEnabled = !document.DocumentKind.GenerateDocumentName.Value;
        properties.DocumentKind.IsEnabled = true;
        properties.Subject.IsEnabled = true;
        properties.BusinessUnit.IsEnabled = true;
        properties.Department.IsEnabled = true;
        properties.Counterparty.IsEnabled = true;
        properties.Assignee.IsEnabled = true;
        
        properties.DeliveryMethod.IsEnabled = true;
        properties.DocumentRegister.IsEnabled = true;
        properties.CaseFile.IsEnabled = true;
        properties.PlacedToCaseFileDate.IsEnabled = true;
        properties.RegistrationNumber.IsEnabled = true;
        properties.RegistrationDate.IsEnabled = true;
      }
    }
    
    #endregion
    
    #region Настройка и тесты
    
    /// <summary>
    /// Включить демо-режим.
    /// </summary>
    public static void SwitchToCaptureMockMode()
    {
      Sungero.Capture.Functions.Module.Remote.InitCaptureMockMode();
    }
    
    /// <summary>
    /// Создать документ в DirectumRX на основе данных распознования.
    /// </summary>
    /// <param name="bodyFilePath">Путь до исходного файла.</param>
    /// <param name="jsonFilePath">Путь до файла json с результатом распознавания.</param>
    /// <param name="responsibleId">Id сотрудника ответственного за распознавание документов.</param>
    /// <remarks> Функция добавлена для автотестов.</remarks>
    public static void CreateDocumentByRecognitionData(string bodyFilePath, string jsonFilePath, string responsibleId)
    {
      Logger.Debug("Start CreateDocumentByRecognitionData");
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
        throw new ApplicationException(Resources.InvalidResponsibleId);
      Logger.DebugFormat(Calendar.Now.ToString() + " Responsible: {0}", responsible.Person.ShortName);

      // Обработать пакет.
      Logger.Debug(Calendar.Now.ToString() + " Start ProcessSplitedPackage");
      var originalFile = new Structures.Module.FileInfo();
      originalFile.Path = System.IO.Path.GetFileName(bodyFilePath);
      var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(System.IO.File.ReadAllText(jsonFilePath),
                                                                                  originalFile,
                                                                                  responsible,
                                                                                  false, string.Empty);
      
      var documentsCreatedByRecognitionResults = Functions.Module.Remote.ProcessPackageAfterCreationDocuments(documents, null, true);
      Functions.Module.Remote.SendToResponsible(documentsCreatedByRecognitionResults,  null, responsible);

      Logger.Debug(Calendar.Now.ToString() + " End ProcessSplitedPackage");
      Logger.Debug("End CreateDocumentByRecognitionData");
    }
    
    #endregion
  }
}