using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
// TODO Dmitriev_IA: см. 81819
// using Directum.BarcodeRecognition;

namespace Sungero.Capture.Client
{
  public class ModuleFunctions
  {
    
    public static int GetFirstPageClassifierId(string arioUrl, string firstPageClassifierName)
    {
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      return arioConnector.GetClassifierByName(firstPageClassifierName).Id;
    }
    
    public static List<string> SplitPackage(string filePath, string arioUrl, string firstPageClassifierName)
    {
      var documentGuids = new List<string>();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var fpClassifier = GetFirstPageClassifierId(arioUrl, firstPageClassifierName).ToString();      
      var classificationResults = arioConnector.Classify(File.ReadAllBytes(filePath), Path.GetFileName(filePath), fpClassifier, fpClassifier);
      foreach (var classificationResult in classificationResults)
      {
        documentGuids.Add(classificationResult.DocumentGuid);
      }

      return documentGuids;
    }
    
    public static void ProcessCapturedPackege(string filePath, int responsibleId)
    {
      var arioUrl = Functions.Module.Remote.GetArioUrl();
      var classifierName = "TestRX Classifier";
      var documentGuids = SplitPackage(filePath, arioUrl, classifierName);
      Functions.Module.Remote.ProcessSplitedPackage(documentGuids, responsibleId);
    }
    
    /// <summary>
    /// Создать документ на основе пакета документов со сканера.
    /// </summary>
    /// <param name="senderLine">Наименование линии.</param>
    /// <param name="instanceInfos">Путь к xml файлу DCTS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <param name="deviceInfo">Путь к xml файлу DCTS c информацией об устройствах ввода.</param>
    /// <param name="filesInfo">Путь к xml файлу DCTS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    public static void ImportDocument(string senderLine, string instanceInfos, string deviceInfo, string filesInfo, string folder, string responsibleId)
    {
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
      {
        Logger.Error(Resources.InvalidResponsibleId);
        return;
      }
      
      var filePath = GetScannedPackagePath(filesInfo, folder);
      if (string.IsNullOrEmpty(filePath))
      {
        Logger.Error(Resources.FileNotFoundFormat(filePath));
        return;
      }
      ProcessCapturedPackege(filePath, int.Parse(responsibleId));
      // Поиск документа по штрих-коду.
      /*Docflow.IOfficialDocument document = null;
      // TODO Dmitriev_IA: BarcodeRecognition потребовал Aspose.Pdf версии 18.10.0.0.
      //                   Сейчас в платформе 17.5.0.0. Падает ошибка при загрузке сборок. (см. 81819)
        document = Capture.PublicFunctions.Module.Remote.GetDocument(documentIds.FirstOrDefault())      
     */
      if (documentIds.Any())
    }
    
    public static void ImportDocumentFromEmail(string senderLine, string instanceInfos, string deviceInfo, string filesInfo, string folder, string responsibleId)
    {      
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
      {
        Logger.Error(Resources.InvalidResponsibleId);
        return;
      }
      
      var filePaths = GetMailPackagePaths(filesInfo, folder);
      Sungero.Content.IElectronicDocument firstDoc = null;
      foreach (var filePath in filePaths)
      {
        var document = Capture.PublicFunctions.Module.Remote.CreateSimpleDocument();
        document.CreateVersionFrom(filePath);
        if (firstDoc != null)
        {
          document.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, firstDoc);
        }
        else
        {
          firstDoc = document;
        }
        document.Save();
      }
      
      var task = Capture.PublicFunctions.Module.Remote.CreateSimpleTask(Resources.TaskNameFormat(firstDoc.Name), firstDoc.Id, responsible.Id);
      if (task != null)
        task.Start();
    }
    
    /// <summary>
    /// Получить список путей к документам пришедшим из почты.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCTS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Список путей к документам пришедшим из почты, первым идет путь к файлу с телом письма, если он есть.</returns>
    public static List<string> GetMailPackagePaths(string filesInfo, string folder)
    {
      var filePaths = new List<string>();
      var filesXDoc = GetXDocumentFromFile(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return filePaths;
      }
      
      var fileElements = filesXDoc.Element("InputFilesSection").Element("Files").Elements();
      
      var mailBodyElements = fileElements.Where(e => e.Element("FileDescription").Value.Equals("body.txt", StringComparison.InvariantCultureIgnoreCase) ||
                                                        e.Element("FileDescription").Value.Equals("body.html", StringComparison.InvariantCultureIgnoreCase));
      
      if(mailBodyElements.Count() > 0)
      {
        foreach (var mailBodyElement in mailBodyElements)
        {
          var fileDescription = mailBodyElement.Element("FileDescription").Value;
          if (Sungero.Content.Shared.ElectronicDocumentUtils.GetAssociatedApplication(fileDescription) != null)
          {
            filePaths.Add(Path.Combine(folder, Path.GetFileName(mailBodyElement.Element("FileName").Value)));
            break;
          }            
        }
      }
      
      var fileAttachments = fileElements.Where(f => !mailBodyElements.Contains(f));
      foreach (var fileAttachment in fileAttachments)
      {
        var fileDescription = fileAttachment.Element("FileDescription").Value;
        
        //Если для файла нет приложения-обработчика, то платформа не даст создать документ.
        var application = Sungero.Content.Shared.ElectronicDocumentUtils.GetAssociatedApplication(fileDescription);
        if (application == null)
          continue;
        
        // Отбрасываем изображения из тела письма (например картинки из подписей).
        if (System.Text.RegularExpressions.Regex.IsMatch(fileDescription, @"^ATT\d+\s\d+\.\w+"))
          continue;
                        
        var filePath = Path.Combine(folder, Path.GetFileName(fileAttachment.Element("FileName").Value));        
        if (!File.Exists(filePath))
        {
          Logger.Error(Resources.FileNotFoundFormat(filePath));
          continue;
        }
        filePaths.Add(filePath);
      }      
      return filePaths;
    }
    
    /// <summary>
    /// Получить путь к пакету документов со сканера.
    /// </summary>
    /// <param name="filesInfo">Путь к xml файлу DCTS c информацией об импортируемых файлах.</param>
    /// <param name="folder">Путь к папке хранения файлов, переданных в пакете.</param>
    /// <returns>Путь к пакету документов со сканера.</returns>
    public static string GetScannedPackagePath(string filesInfo, string folder)
    {
      var filesXDoc = GetXDocumentFromFile(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return string.Empty;
      }
      
      // При захвате из файловой системы (со сканера) создать документ на основе единственного файла в папке.
      var fileElement = filesXDoc.Element("InputFilesSection").Element("Files").Elements().FirstOrDefault();
      if (fileElement == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return string.Empty;
      }
      
      var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
      if (!File.Exists(filePath))
      {
        Logger.Error(Resources.FileNotFoundFormat(filePath));
        return string.Empty;
      }
      
      return filePath;
    }
    
    /// <summary>
    /// Получить XDocument из xml файла.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <returns>XElement документ.</returns>
    public static System.Xml.Linq.XDocument GetXDocumentFromFile(string path)
    {
      if (!File.Exists(path))
        return null;
      
      return System.Xml.Linq.XDocument.Load(path);
    }
    
    // TODO Dmitriev_IA: см. 81819
    //    /// <summary>
    //    /// Получить список ИД из штрих-кодов файла.
    //    /// </summary>
    //    /// <param name="path">Путь к файлу.</param>
    //    /// <returns>Список ИД.</returns>
    //    [Public]
    //    public static List<int> GetIdsFromFileBarcodes(string path)
    //    {
    //      var recognitionParameters = new BarcodeRecognitionParameters(false, "Code128", true, "MaxQuality", 300);
    //      var barcodeInfos = new BarcodeRecognitionManager().RecognizeBarcode(path, recognitionParameters);
    //      if (!barcodeInfos.Any())
    //        return new List<int>();
    //      
    //      return barcodeInfos.Select(x => int.Parse(x.Barcode.Split(new string[] {" - ", "-"}, StringSplitOptions.None).Last())).ToList();
    //    }   
  }
}