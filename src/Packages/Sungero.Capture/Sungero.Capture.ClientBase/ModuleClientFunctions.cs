using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using ArioClassNames = Sungero.Capture.Constants.Module.ArioClassNames;
using ArioGrammarNames = Sungero.Capture.Constants.Module.ArioGrammarNames;
using InstanceInfosTagNames = Sungero.Capture.Constants.Module.InstanceInfosTagNames;

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
      var element = filesXDoc.Element(Constants.Module.DeviceInfoTagNames.MailSourceInfo);
      if (element != null)
        return Constants.Module.CaptureSourceType.Mail;
      return Constants.Module.CaptureSourceType.Folder;
    }
    
    /// <summary>
    /// Задать основные настройки захвата.
    /// </summary>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="lowerConfidenceLimit">Нижняя граница доверия извлеченным фактам.</param>
    /// <param name="upperConfidenceLimit">Верхняя граница доверия извлеченным фактам.</param>
    public static void SetCaptureMainSettings(string arioUrl, string lowerConfidenceLimit, string upperConfidenceLimit)
    {
      Sungero.Capture.Functions.Module.Remote.SetCaptureMainSettings(arioUrl, lowerConfidenceLimit, upperConfidenceLimit);
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
        throw new ApplicationException(Resources.EmptyMailPackage);
      
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
        var classificationAndExtractionResponse = TryClassifyAndExtractFacts(attachment.Path, arioUrl, firstPageClassifierName, typeClassifierName, false);
        if (string.IsNullOrWhiteSpace(classificationAndExtractionResponse.Error))
        {
          Logger.DebugFormat("Captured Package Process. Create documents by recognition results. {0}", fileName);
          var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResponse.Response,
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
      mailFiles.Attachments = new List<Structures.Module.IFileDto>();
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return mailFiles;
      }
      
      var fileDescriptionTagName = Constants.Module.InputFilesTagNames.FileDescription;
      var mailBodyHtmlName = Constants.Module.MailBodyName.Html;
      var mailBodyTxtName = Constants.Module.MailBodyName.Txt;
      
      // Тело письма.
      var fileElements = filesXDoc
        .Element(Constants.Module.InputFilesTagNames.InputFilesSection)
        .Element(Constants.Module.InputFilesTagNames.Files)
        .Elements();
      var htmlBodyElement = fileElements
        .FirstOrDefault(x => string.Equals(x.Element(fileDescriptionTagName).Value, mailBodyHtmlName, StringComparison.InvariantCultureIgnoreCase));
      var txtBodyElement = fileElements
        .FirstOrDefault(x => string.Equals(x.Element(fileDescriptionTagName).Value, mailBodyTxtName, StringComparison.InvariantCultureIgnoreCase));
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
      var attachments = fileElements
        .Where(x => !string.Equals(x.Element(fileDescriptionTagName).Value, mailBodyHtmlName, StringComparison.InvariantCultureIgnoreCase) &&
               !string.Equals(x.Element(fileDescriptionTagName).Value, mailBodyTxtName, StringComparison.InvariantCultureIgnoreCase))
        .ToList();
      
      // Фильтрация картинок из тела письма.
      if (mailFiles.Body != null && !string.IsNullOrEmpty(mailFiles.Body.Path) && hasHtmlBody)
        attachments = FilterEmailBodyInlineImages(mailFiles.Body.Path, attachments);

      foreach (var attachment in attachments)
      {
        var fileDescription = attachment.Element(fileDescriptionTagName).Value;
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
        .Element(InstanceInfosTagNames.CaptureInstanceInfoList)
        .Element(InstanceInfosTagNames.MailCaptureInstanceInfo);
      
      if (mailCaptureInstanceInfoElement == null)
        return result;
      
      result.Subject = GetAttributeStringValue(mailCaptureInstanceInfoElement, InstanceInfosTagNames.Subject);
      
      var fromElement = mailCaptureInstanceInfoElement.Element(InstanceInfosTagNames.From);
      if (fromElement == null)
        return result;
      
      result.FromEmail = GetAttributeStringValue(fromElement, InstanceInfosTagNames.FromTags.Address);
      result.Name = GetAttributeStringValue(fromElement, InstanceInfosTagNames.FromTags.Name);
      
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
      if (Path.GetExtension(path).ToLower() != Constants.Module.HtmlExtension.WithDot)
        return;

      try
      {
        var mailBody = File.ReadAllText(path);
        mailBody = System.Text.RegularExpressions.Regex.Replace(mailBody, @"<img([^\>]*)>", string.Empty);
        
        // В некоторых случаях Aspose не может распознать файл как html, поэтому добавляем тег html, если его нет.
        if (!mailBody.Contains(Constants.Module.HtmlTags.MaskForSearch))
          mailBody = string.Concat(Constants.Module.HtmlTags.StartTag, mailBody, Constants.Module.HtmlTags.EndTag);
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
    public virtual Structures.Module.IFileDto CreateFileInfoFromXelement(System.Xml.Linq.XElement xmlElement, string folder)
    {
      var file = Structures.Module.FileDto.Create();
      file.Path = Path.Combine(folder, Path.GetFileName(xmlElement.Element(Constants.Module.InputFilesTagNames.FileName).Value));
      file.Description = xmlElement.Element(Constants.Module.InputFilesTagNames.FileDescription).Value;
      
      return file;
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
        throw new ApplicationException(Resources.EmptyScanPackage);
      
      foreach (var packagePath in packagesPaths)
      {
        var classificationAndExtractionResponse = TryClassifyAndExtractFacts(packagePath, arioUrl, firstPageClassifierName, typeClassifierName);
        Logger.DebugFormat("Begin package processing. Path: {0}", packagePath);
        var originalFile = new Structures.Module.FileDto();
        originalFile.Path = packagePath;
        
        var documents = Functions.Module.Remote.CreateDocumentsByRecognitionResults(classificationAndExtractionResponse.Response,
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
        .Element(Constants.Module.InputFilesTagNames.InputFilesSection)
        .Element(Constants.Module.InputFilesTagNames.Files)
        .Elements();
      
      if (!fileElements.Any())
        throw new ApplicationException(Resources.NoFilesInfoInPackage);
      
      foreach (var fileElement in fileElements)
      {
        var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element(Constants.Module.InputFilesTagNames.FileName).Value));
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
    public virtual Structures.Module.ArioResponse TryClassifyAndExtractFacts(string filePath,
                                                                             string arioUrl,
                                                                             string firstPageClassifierName,
                                                                             string typeClassifierName,
                                                                             bool throwOnError = true)
    {
      var classificationAndExtractionResponse = Structures.Module.ArioResponse.Create();
      if (!CanArioProcessFile(filePath))
      {
        classificationAndExtractionResponse.Error = Resources.CantProcessFileByArio;
        return classificationAndExtractionResponse;
      }
      
      var processResult = ProcessPackage(filePath, arioUrl, firstPageClassifierName, typeClassifierName);
      var nativeError = ArioExtensions.ArioConnector.GetErrorMessageFromClassifyAndExtractFactsResult(processResult);
      classificationAndExtractionResponse.Response = processResult;
      if (nativeError == null || string.IsNullOrWhiteSpace(nativeError.Message))
        return classificationAndExtractionResponse;
      
      if (throwOnError)
        throw new ApplicationException(nativeError.Message);
      
      Logger.Error(nativeError.Message);
      classificationAndExtractionResponse.Error = nativeError.Message;
      return classificationAndExtractionResponse;
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
        { ArioClassNames.Letter, ArioGrammarNames.Letter},
        { ArioClassNames.ContractStatement, ArioGrammarNames.ContractStatement},
        { ArioClassNames.Waybill, ArioGrammarNames.Waybill},
        { ArioClassNames.UniversalTransferDocument, ArioGrammarNames.UniversalTransferDocument},
        { ArioClassNames.UniversalTransferCorrectionDocument, ArioGrammarNames.UniversalTransferCorrectionDocument},
        { ArioClassNames.TaxInvoice, ArioGrammarNames.TaxInvoice},
        { ArioClassNames.TaxinvoiceCorrection, ArioGrammarNames.TaxinvoiceCorrection},
        { ArioClassNames.IncomingInvoice, ArioGrammarNames.IncomingInvoice},
        { ArioClassNames.Contract, ArioGrammarNames.Contract}
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
      HighlightPropertiesAndFacts(document, exactlyRecognizedProperties, Sungero.Core.Colors.Parse(Constants.Module.HighlightsColorCodes.Green));
      
      var notExactlyRecognizedProperties = Sungero.Capture.PublicFunctions.Module.Remote.GetRecognitionResultProperties(document, false);
      HighlightPropertiesAndFacts(document, notExactlyRecognizedProperties, Sungero.Core.Colors.Parse(Constants.Module.HighlightsColorCodes.Yellow));
    }
    
    /// <summary>
    /// Подсветить указанные свойства в карточке документа и факты в теле.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="propertyNamesAndPositions">Список имён свойств и позиций подсветки.</param>
    /// <param name="color">Цвет.</param>
    public virtual void HighlightPropertiesAndFacts(Sungero.Docflow.IOfficialDocument document, List<string> propertyNamesAndPositions, Sungero.Core.Color color)
    {
      var posColor = color == Sungero.Core.Colors.Parse(Constants.Module.HighlightsColorCodes.Yellow)
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
      var originalFile = new Structures.Module.FileDto();
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
    
    /// <summary>
    /// Создать классификатор.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <param name="minProbability">Минимальная вероятность.</param>
    public static void CreateClassifier(string classifierName, string minProbability)
    {
      Logger.DebugFormat("Begin create classifier with name \"{0}\".", classifierName);
      try
      {
        var arioUrl = Functions.Module.Remote.GetArioUrl();
        var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
        var classifier = arioConnector.GetClassifierByName(classifierName);
        if (classifier != null)
        {
          Logger.ErrorFormat("Already exists classifier with name: \"{0}\".", classifierName);
          return;
        }
        // Некорректно обрабатывается minProbability если использовать запятую в качестве разделителя.
        arioConnector.CreateClassifier(classifierName, minProbability.Replace(',', '.'), true);
        Logger.DebugFormat("Successful create classifier with name \"{0}\".", classifierName);
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Create classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Импорт классификатора из файла модели.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <param name="filePath">Путь к файлу модели.</param>
    public static void ImportClassifierModel(string classifierName, string filePath)
    {
      Logger.DebugFormat("Begin import classifier with name \"{0}\" from folder {1}.", classifierName, filePath);
      try
      {
        var arioUrl = Functions.Module.Remote.GetArioUrl();
        var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
        var classifier = arioConnector.GetClassifierByName(classifierName);
        if (classifier == null)
        {
          Logger.ErrorFormat("Cant find classifier with name: \"{0}\".", classifierName);
          return;
        }

        arioConnector.ImportClassifierModel(classifier.Id.ToString(), filePath);
        
        if (ShowModelsInfo(classifierName))
          Logger.DebugFormat("Successful import classifier with name \"{0}\" from folder {1}.", classifierName, filePath);
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Import classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Экспорт модели классификатора.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <param name="modelId">Id модели.</param>
    /// <param name="filePath">Путь к файлу модели.</param>
    public static void ExportClassifierModel(string classifierName, string modelId, string filePath)
    {
      Logger.DebugFormat("Begin export classifier with name \"{0}\" into file {1}.", classifierName, filePath);
      try
      {
        var arioUrl = Functions.Module.Remote.GetArioUrl();
        var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
        var classifier = arioConnector.GetClassifierByName(classifierName);
        if (classifier == null)
        {
          Logger.ErrorFormat("Cant find classifier with name: \"{0}\".", classifierName);
          return;
        }

        var model = arioConnector.ExportClassifierModel(classifier.Id.ToString(), modelId);
        File.WriteAllBytes(filePath, model);
        Logger.DebugFormat("Successful export classifier with name \"{0}\" into file {1}.", classifierName, filePath);
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Export classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Отобразить список моделей классификатора.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    public static void ShowClassifierModels(string classifierName)
    {
      Logger.DebugFormat("Begin showing models for classifier with name \"{0}\".", classifierName);
      try
      {
        if (ShowModelsInfo(classifierName))
          Logger.DebugFormat("Successful showing models for classifier with name \"{0}\".", classifierName);
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Showing models for classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Опубликовать модель классификатора.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <param name="modelId">Id модели.</param>
    public static void PublishClassifierModel(string classifierName, string modelId)
    {
      Logger.DebugFormat("Begin publish model with Id {0} for classifier with name \"{1}\".", modelId, classifierName);
      try
      {
        var arioUrl = Functions.Module.Remote.GetArioUrl();
        var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
        var classifier = arioConnector.GetClassifierByName(classifierName);
        if (classifier == null)
        {
          Logger.ErrorFormat("Cant find classifier with name: \"{0}\"", classifierName);
          return;
        }

        var model = arioConnector.PublishClassifierModel(classifier.Id.ToString(), modelId);
        if (model == null)
        {
          Logger.ErrorFormat("Error for publish model with Id {0} for classifier with name \"{1}\".", modelId, classifierName);
          return;
        }
        
        if (ShowModelsInfo(classifierName))
          Logger.DebugFormat("Successful publish model with Id {0} for classifier with name \"{1}\".", modelId, classifierName);
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Publish classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Обучение классификатора.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <param name="filePath">Путь к папке с dataset для обучения.</param>
    public static void TrainClassifierModel(string classifierName, string filePath)
    {
      Logger.DebugFormat("Begin train classifier with name \"{0}\" from folder {1}.", classifierName, filePath);
      try
      {
        var arioUrl = Functions.Module.Remote.GetArioUrl();
        var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
        var classifier = arioConnector.GetClassifierByName(classifierName);
        if (classifier == null)
        {
          Logger.ErrorFormat("Cant find classifier with name: \"{0}\".", classifierName);
          return;
        }

        var trainTask = arioConnector.TrainClassifierFromFolder(classifier.Id.ToString(), Path.GetFullPath(filePath));
        var trainTaskInfo = arioConnector.GetTrainTaskInfo(trainTask.Id.ToString());

        // Статусы задачи асинхронного обучения:
        // New = 0
        // InProgress = 1
        // Completed = 2
        // Error = 3
        // Trained = 4
        // Aborted = 5
        int[] stateCodes = {2, 3, 5};
        while (!stateCodes.Contains(trainTaskInfo.Task.State))
        {
          Logger.DebugFormat("[{0}] Training in process. Classifier name: \"{1}\", training task Id: {2}.", Calendar.Now, classifierName, trainTaskInfo.Task.Id);
          System.Threading.Thread.Sleep(10000);
          trainTaskInfo = arioConnector.GetTrainTaskInfo(trainTaskInfo.Task.Id.ToString());
        }
        
        switch (trainTaskInfo.Task.State)
        {
          case 2:
            System.Threading.Thread.Sleep(20000);
            ShowModelsInfo(classifierName);
            Logger.DebugFormat("Successful classifier training with name \"{0}\" from folder {1}.", classifierName, filePath);
            break;
          case 3:
            Logger.DebugFormat("Classifier training task with Id {0} completed with an error. {1}", trainTaskInfo.Task.Id, trainTaskInfo.Result ?? string.Empty);
            break;
          case 5:
            Logger.DebugFormat("Classifier training task with Id {0} was aborted. {1}", trainTaskInfo.Task.Id, trainTaskInfo.Result ?? string.Empty);
            break;
        }
      }
      catch (Exception e)
      {
        Logger.ErrorFormat("Training classifier error: {0}", GetInnerExceptionsMessages(e));
      }
    }
    
    /// <summary>
    /// Отобразить информацию о моделях классификатора.
    /// </summary>
    /// <param name="classifierName">Имя классификатора.</param>
    /// <returns>True, при успешном отображении.</returns>
    private static bool ShowModelsInfo(string classifierName)
    {
      var arioUrl = Functions.Module.Remote.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var classifier = arioConnector.GetClassifierByName(classifierName);
      if (classifier == null)
      {
        Logger.ErrorFormat("Cant find classifier with name: \"{0}\".", classifierName);
        return false;
      }
      
      var models = arioConnector.GetModelsByClassifier(classifier.Id.ToString());
      
      Logger.Debug("-------------------------------------------------------------------------------------------------");
      Logger.DebugFormat("Classifier \"{0}\" with Id {1}, created {2}, min probability {3}. Models:",
                         classifier.Name, classifier.Id, classifier.Created, classifier.MinProbability);
      if (models.Any())
        foreach (var model in models)
          Logger.DebugFormat("{0} Model with Id {1}, created {2}. Train set count {3}, accuracy {4}.",
                             model.Classes != null ? "*CURRENT*" : "---------",
                             model.Id, model.Created,
                             model.Metrics.TrainSetCount, Math.Round(model.Metrics.Accuracy, 4));
        else
          Logger.Debug("Classifier has no models");
      Logger.Debug("-------------------------------------------------------------------------------------------------");
      
      return true;
    }
    
    /// <summary>
    /// Собрать цепочку InnerExceptions в одну строку.
    /// </summary>
    /// <param name="e">Исключение.</param>
    /// <returns>Строка InnerExceptions исключений.</returns>
    private static string GetInnerExceptionsMessages(Exception e)
    {
      var result = e.InnerException != null ?
        string.Concat(e.InnerException.Message.TrimEnd('.'), ". ",  GetInnerExceptionsMessages(e.InnerException)) :
        string.Empty;
      return string.IsNullOrEmpty(result) ? e.Message : result;
    }
    
    #endregion
  }
}