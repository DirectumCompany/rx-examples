using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Workflow;

namespace Sungero.Capture.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получить простой документ.
    /// </summary>
    /// <param name="documentId">ИД документа.</param>
    /// <returns>Простой документ.</returns>
    [Remote, Public]
    public static ISimpleDocument GetSimpleDocument(int documentId)
    {
      var document = SimpleDocuments.GetAll(x => x.Id == documentId).FirstOrDefault();
      return document;
    }
    
    /// <summary>
    /// Получить документ.
    /// </summary>
    /// <param name="documentId">ИД документа.</param>
    /// <returns>Документ.</returns>
    [Remote, Public]
    public static IOfficialDocument GetDocument(int documentId)
    {
      var document = OfficialDocuments.GetAll(x => x.Id == documentId).FirstOrDefault();
      return document;
    }
    
    /// <summary>
    /// Создать простой документ.
    /// </summary>
    /// <returns>Простой документ.</returns>
    [Remote, Public]
    public static ISimpleDocument CreateSimpleDocument()
    {
      var document = SimpleDocuments.Create();
      document.Name = Resources.DocumentNameFormat(document.Id);
      document.Save();
      
      return document;
    }
    
    /// <summary>
    /// Выдать права на документ.
    /// </summary>
    /// <param name="documentId">ИД документа.</param>
    /// <param name="responsibleId">ИД ответственного.</param>
    /// <param name="rightType">Тип прав.</param>
    [Remote, Public]
    public static void GrantRightsToDocument(int documentId, int responsibleId, Guid rightType)
    {
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(responsibleId);
      if (responsible == null)
      {
        Logger.Error(Capture.Resources.InvalidResponsibleId);
        return;
      }
      
      var document = GetSimpleDocument(documentId);
      if (document == null)
      {
        Logger.Error(Capture.Resources.DocumentNotFoundFormat(documentId));
        return;
      }
      
      document.AccessRights.Grant(responsible, rightType);
    }
    
    /// <summary>
    /// Создать простую задачу.
    /// </summary>
    /// <param name="taskName">Тема задачи.</param>
    /// <param name="documentId">ИД вкладываемого документа.</param>
    /// <param name="responsibleId">ИД ответственного.</param>
    /// <returns>Простая задача.</returns>
    [Remote, Public]
    public static ISimpleTask CreateSimpleTask(string taskName, int documentId, int responsibleId)
    {
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(responsibleId);
      if (responsible == null)
      {
        Logger.Error(Capture.Resources.InvalidResponsibleId);
        return null;
      }
      
      var document = GetSimpleDocument(documentId);
      if (document == null)
      {
        Logger.Error(Capture.Resources.DocumentNotFoundFormat(documentId));
        return null;
      }
      
      var task = SimpleTasks.Create();
      task.Subject = taskName;
      
      var step = task.RouteSteps.AddNew();
      step.AssignmentType = Workflow.SimpleTask.AssignmentType.Assignment;
      step.Performer = responsible;
      
      GrantRightsToDocument(documentId, responsibleId, DefaultAccessRightsTypes.FullAccess);
      document.Save();
      
      task.Attachments.Add(document);
      task.Save();
      
      return task;
    }
    
    [Remote, Public]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
    
    [Remote]
    public static string GetArioUrl()
    {
      var key = Constants.Module.ArioUrlKey;
      var command = string.Format(Queries.Module.SelectArioUrl, key);
      var commandExecutionResult = Docflow.PublicFunctions.Module.ExecuteScalarSQLCommand(command);
      var arioUrl = string.Empty;
      if (!(commandExecutionResult is DBNull) && commandExecutionResult != null)
        arioUrl = commandExecutionResult.ToString();
      return arioUrl;
    }
    
    [Remote, Public]
    public static void ProcessSplitedPackage(List<string> documentGuids, int responsibleId)
    {
      var documents = new List<IOfficialDocument>();
      var firstDoc = CreateDocumentByGuid(documentGuids.First(), null);
      documents.Add(firstDoc);
      documentGuids = documentGuids.Skip(1).ToList();
      foreach(var documentGuid in documentGuids)
      {
        documents.Add(CreateDocumentByGuid(documentGuid, firstDoc));
      }
      
      if (documents.Any())
      {
        var task = CreateSimpleTask(Resources.TaskNameFormat(firstDoc.Name), firstDoc.Id, responsibleId);
        if (task != null)
          task.Start();
      }
    }
    
    public static Docflow.IOfficialDocument CreateDocumentByGuid(string documentGuid, IOfficialDocument firstDoc)
    {
      var arioConnector = new ArioExtensions.ArioConnector("http://smart:61100");
      var documentBody = arioConnector.GetDocumentByGuid(documentGuid);
      var document = CreateSimpleDocument();
      document.CreateVersionFrom(documentBody, "pdf");
      if (firstDoc != null)
      {
        document.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, firstDoc);
      }
      document.Save();
      return document;      
    }
    
  }
}