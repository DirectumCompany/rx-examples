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
    public static void ProcessCapturedPackage(string senderLine, string instanceInfos, string deviceInfo, string filesInfo, string folder,
                                              string responsibleId, string firstPageClassifierName, string typeClassifierName)
    {
      // Найти ответственного.
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(int.Parse(responsibleId));
      if (responsible == null)
      {
        Logger.Error(Resources.InvalidResponsibleId);
        return;
      }
      
      var filePath = GetScannedPackagePath(filesInfo, folder);
      var sourceFileName = GetScannedPackageName(instanceInfos);
      if (string.IsNullOrEmpty(filePath))
      {
        Logger.Error(Resources.FileNotFoundFormat(filePath));
        return;
      }
      
      // Разделить пакет на документы.
      var arioUrl = Functions.Module.Remote.GetArioUrl();
      var documentGuids = SplitPackage(filePath, arioUrl, firstPageClassifierName);
      
      // Обработать пакет.
      Functions.Module.Remote.ProcessSplitedPackage(sourceFileName, documentGuids, int.Parse(responsibleId));
    }
    
    /// <summary>
    /// Разделить пакет на документы с помощью сервиса Ario.
    /// </summary>
    /// <param name="filePath">Путь к пакету.</param>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <returns>Список Гуидов, по которым в Арио можно получить документы.</returns>
    public static List<string> SplitPackage(string filePath, string arioUrl, string firstPageClassifierName)
    {
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var fpClassifier = arioConnector.GetClassifierByName(firstPageClassifierName).Id.ToString();
      var classificationResults = arioConnector.Classify(File.ReadAllBytes(filePath), Path.GetFileName(filePath), fpClassifier, fpClassifier);
      var documentGuids = classificationResults.Select(r => r.DocumentGuid).ToList();
      return documentGuids;
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
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return string.Empty;
      }
      
      var filesXDoc = System.Xml.Linq.XDocument.Load(filesInfo);
      var fileElement = filesXDoc
        .Element("InputFilesSection")
        .Element("Files")
        .Elements()
        .FirstOrDefault();
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
    /// Получить путь к пакету документов со сканера.
    /// </summary>
    /// <param name="instanceInfos">Путь к xml файлу DCS c информацией об экземплярах захвата и о захваченных файлах.</param>
    /// <returns>Путь к пакету документов со сканера.</returns>
    public static string GetScannedPackageName(string instanceInfos)
    {
      if (!File.Exists(instanceInfos))
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return string.Empty;
      }

      var filesXDoc = System.Xml.Linq.XDocument.Load(instanceInfos);
      var fileElement = filesXDoc
        .Element("CaptureInstanceInfoList")
        .Element("FileSystemCaptureInstanceInfo")
        .Element("Files")
        .Elements()
        .FirstOrDefault();
      if (fileElement == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return string.Empty;
      }
      
      return fileElement.Element("FileDescription").Value;
    }
  }
}