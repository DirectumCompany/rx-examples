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
    /// <param name="sourceFileName">Имя исходного файла, полученного с DCS.</param>
    /// <param name="documentGuids">Список Гуидов документов, на которые Арио разделил пакет.</param>
    /// <param name="responsibleId">Ид сотрудника, ответственного за проверку документов.</param>
    [Remote, Public]
    public static void ProcessSplitedPackage(string sourceFileName, List<string> documentGuids, int responsibleId)
    {
      var documents = new List<IOfficialDocument>();
      var leadingDocument = CreateDocumentByGuid(sourceFileName, 0, documentGuids.First(), null);
      documentGuids = documentGuids.Skip(1).ToList();
      int addendumNumber = 1;
      foreach (var documentGuid in documentGuids)
        documents.Add(CreateDocumentByGuid(string.Empty, addendumNumber++, documentGuid, leadingDocument));
      
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
    /// <param name="name">Имя документа.</param>
    /// <param name="addendumNumber">Номер приложения.</param>
    /// <param name="documentGuid">Гуид тела документа.</param>
    /// <param name="firstDoc">Ведущий документ.</param>
    /// <returns></returns>
    public static Docflow.IOfficialDocument CreateDocumentByGuid(string name, int addendumNumber,string documentGuid, IOfficialDocument leadingDoc)
    {
      var arioUrl = Functions.Module.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      var documentBody = arioConnector.GetDocumentByGuid(documentGuid);
      
      var document = SimpleDocuments.Create();
      document.Name = string.IsNullOrWhiteSpace(name) ? Resources.DocumentNameFormat(addendumNumber) : name;
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
           
    [Remote, Public]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
  }
}