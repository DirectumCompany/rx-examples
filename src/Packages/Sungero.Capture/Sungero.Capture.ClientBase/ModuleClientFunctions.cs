using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Directum.BarcodeRecognition;

namespace Sungero.Capture.Client
{
  public class ModuleFunctions
  {
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
      
      // Поиск документа по штрих-коду.
      Docflow.IOfficialDocument document = null;
      var documentIds = GetIdFromDocumentBarcode(filePath);
      if (documentIds != null)
        document = Capture.PublicFunctions.Module.Remote.GetDocument(documentIds.FirstOrDefault());
      
      if (document == null)
        document = Capture.PublicFunctions.Module.Remote.CreateSimpleDocument();
      else
        document.ExternalApprovalState = Docflow.OfficialDocument.ExternalApprovalState.Signed;
      
      document.CreateVersionFrom(filePath);
      document.Save();
      
      var task = Capture.PublicFunctions.Module.Remote.CreateSimpleTask(Resources.TaskNameFormat(document.Name), document.Id, int.Parse(responsibleId));
      if (task != null)
        task.Start();
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
      var mailBodyElement = fileElements.FirstOrDefault(e => e.Element("FileDescription").Value.Equals("body.txt", StringComparison.InvariantCultureIgnoreCase) ||
                                                        e.Element("FileDescription").Value.Equals("body.html", StringComparison.InvariantCultureIgnoreCase));
      if (mailBodyElement != null)
        filePaths.Add(Path.Combine(folder, Path.GetFileName(mailBodyElement.Element("FileName").Value)));
      
      foreach (var fileElement in fileElements)
      {
        var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
        var fileDescription = fileElement.Element("FileDescription").Value;
        if (fileDescription.Equals("body.html", StringComparison.InvariantCultureIgnoreCase) || fileDescription.Equals("body.txt", StringComparison.InvariantCultureIgnoreCase))
          continue;
        
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
    
    /// <summary>
    /// Получить ИД из штрих-кода файла.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <returns>Список ИД.</returns>
    [Public]
    public static List<int> GetIdFromDocumentBarcode(string path)
    {
      var recognitionParameters = new BarcodeRecognitionParameters(false, "Code128", true, "MaxQuality", 300);
      var barcodeNumber = new BarcodeRecognitionManager().RecognizeBarcode(path, recognitionParameters);
      if (!barcodeNumber.Any())
        return null;
      
      var result = barcodeNumber.Select(x => int.Parse(x.Barcode.Split(new string[] {" - ", "-"}, StringSplitOptions.None).Last()));
      return result.ToList();
    }
  }
}