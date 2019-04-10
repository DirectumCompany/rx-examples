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
    /// Создать документ на основе пакета, сформированного DCS.
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
      
      var filesXDoc = GetXDocumentFromFile(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return;
      }
      
      // При захвате из файловой системы (со сканера) создать документ на основе единственного файла в папке.
      var fileElement = filesXDoc.Element("InputFilesSection").Element("Files").Elements().FirstOrDefault();
      if (fileElement == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return;
      }
      
      var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
      if (!File.Exists(filePath))
      {
        Logger.Error(Resources.FileNotFoundFormat(filePath));
        return;
      }
      
      var document = Capture.PublicFunctions.Module.Remote.CreateSimpleDocument();
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
      
      var filesXDoc = GetXDocumentFromFile(filesInfo);
      if (filesXDoc == null)
      {
        Logger.Error(Resources.NoFilesInfoInPackage);
        return;
      }
      
      var fileElements = filesXDoc.Element("InputFilesSection").Element("Files").Elements();
      Sungero.Content.IElectronicDocument firstDoc = null;
      foreach (var fileElement in fileElements)
      {
        var filePath = Path.Combine(folder, Path.GetFileName(fileElement.Element("FileName").Value));
        var fileDescription = fileElement.Element("FileDescription").Value;
        if (fileDescription.Equals("body.html", StringComparison.InvariantCultureIgnoreCase))
          continue;
        
        if (!File.Exists(filePath))
        {
          Logger.Error(Resources.FileNotFoundFormat(filePath));
          return;
        }
        
        var document = Capture.PublicFunctions.Module.Remote.CreateSimpleDocument();
        document.CreateVersionFrom(filePath);
        if (firstDoc != null)
        {
          document.Relations.Add("Simple relation", firstDoc);
        }
        else
        {
          firstDoc = document;
        }
        document.Save();
      }
      var task = Capture.PublicFunctions.Module.Remote.CreateSimpleTask(Resources.TaskNameFormat(firstDoc.Name), firstDoc.Id, int.Parse(responsibleId));
      if (task != null)
        task.Start();
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
  }
}