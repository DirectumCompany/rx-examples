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
    /// Создать документы в Rx и отправить задачу на проверку.
    /// </summary>
    /// <param name="documentGuids">Список Гуидов документов, на которые Арио разделил пакет.</param>
    /// <param name="responsibleId">Ид сотрудника, ответственного за проверку документов.</param>
    [Remote, Public]
    public static void ProcessSplitedPackage(List<string> documentGuids, int responsibleId)
    {
      var documents = new List<IOfficialDocument>();
      var leadingDocument = CreateDocumentByGuid(documentGuids.First(), null);
      documentGuids = documentGuids.Skip(1).ToList();
      foreach (var documentGuid in documentGuids)
        documents.Add(CreateDocumentByGuid(documentGuid, leadingDocument));
      
      if (leadingDocument != null)
        SendToResponsible(leadingDocument, documents, responsibleId);
    }
    
    /// <summary>
    /// Получить адрес сервиса Арио.
    /// </summary>
    /// <returns>Адрес Арио.</returns>
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
    
    /// <summary>
    /// Создать документ в Rx, тело документа загружается из Арио.
    /// </summary>
    /// <param name="documentGuid">Гуид тела документа.</param>
    /// <param name="firstDoc">Ведущий документ.</param>
    /// <returns></returns>
    public static Docflow.IOfficialDocument CreateDocumentByGuid(string documentGuid, IOfficialDocument leadingDoc)
    {
      var arioUrl = Functions.Module.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var documentBody = arioConnector.GetDocumentByGuid(documentGuid);
      
      var document = SimpleDocuments.Create();
      document.Name = Resources.DocumentNameFormat(document.Id);
      document.CreateVersionFrom(documentBody, "pdf");
      if (leadingDoc != null)
        document.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, leadingDoc);
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="taskName">Тема задачи.</param>
    /// <param name="documentId">ИД вкладываемого документа.</param>
    /// <param name="responsibleId">ИД ответственного.</param>
    /// <returns>Простая задача.</returns>
    [Remote, Public]
    public static void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents, int responsibleId)
    {
      var responsible = Company.PublicFunctions.Module.Remote.GetEmployeeById(responsibleId);
      if (responsible == null)
      {
        Logger.Error(Capture.Resources.InvalidResponsibleId);
        return;
      }
      
      if (leadingDocument == null)
        return;
      
      var task = SimpleTasks.Create();
      task.Subject = Resources.TaskNameFormat(leadingDocument);
      var step = task.RouteSteps.AddNew();
      step.AssignmentType = Workflow.SimpleTask.AssignmentType.Assignment;
      step.Performer = responsible;
      
      // Вложить в задачу и выдать права на документы ответственному.
      leadingDocument.AccessRights.Grant(responsible, DefaultAccessRightsTypes.FullAccess);
      leadingDocument.Save();
      task.Attachments.Add(leadingDocument);
      foreach (var document in documents)
      {
        document.AccessRights.Grant(responsible, DefaultAccessRightsTypes.FullAccess);
        document.Save();
        task.Attachments.Add(document);
      }
      task.Save();
      task.Start();
    }
    
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
    
    [Remote, Public]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
  }
}