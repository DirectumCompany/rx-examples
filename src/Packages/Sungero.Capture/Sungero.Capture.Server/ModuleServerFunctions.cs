using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Company;
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
    /// <param name="сlassificationResults">Коллекция записей с результатом разделения.</param>
    /// <param name="responsible">Сотрудник, ответственного за проверку документов.</param>
    [Remote]
    public static void ProcessSplitedPackage(string sourceFileName, List<Sungero.Capture.Structures.Module.ClassifiedDocument> сlassificationResults, IEmployee responsible)
    {
      var letterRecord = сlassificationResults.FirstOrDefault(d => d.DocumentClass != null &&
                                                              d.DocumentClass.Equals(Constants.Module.LetterClassName, StringComparison.InvariantCultureIgnoreCase));
      IOfficialDocument leadingDocument;
      if (letterRecord != null)
      {
        // Если в пакете есть документ с классом письмо, то создаем письмо и делаем его ведущим документом.
        var responsibleDepartment = GetDepartment(responsible);
        leadingDocument = CreateIncomingLetter(letterRecord.DocumentGuid, responsibleDepartment);
        сlassificationResults.Remove(letterRecord);
      }
      else
      {
        // Иначе ведущий документ - первый документ в списке.
        leadingDocument = CreateDocumentByGuid(sourceFileName, 0, сlassificationResults.First().DocumentGuid, null);
        сlassificationResults = сlassificationResults.Skip(1).ToList();
      }
      
      var documents = new List<IOfficialDocument>();
      int addendumNumber = 1;
      foreach(var сlassificationResult in сlassificationResults)
        documents.Add(CreateDocumentByGuid(string.Empty, addendumNumber++, сlassificationResult.DocumentGuid, leadingDocument));
      
      if (leadingDocument != null)
        SendToResponsible(leadingDocument, documents, responsible);
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
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateDocumentByGuid(string name, int addendumNumber,string documentGuid, IOfficialDocument leadingDoc)
    {
      var documentBody = GetDocumentBody(documentGuid);
      var document = SimpleDocuments.Create();
      document.Name = string.IsNullOrWhiteSpace(name) ? Resources.DocumentNameFormat(addendumNumber) : name;
      document.CreateVersionFrom(documentBody, "pdf");
      if (leadingDoc != null)
      {
        if (IncomingDocumentBases.Is(leadingDoc))
        {
          document.Relations.AddFrom(Docflow.PublicConstants.Module.AddendumRelationName, leadingDoc);
          document.Name = string.IsNullOrWhiteSpace(name) ? Resources.AttachmentNameFormat(addendumNumber) : name;
        }
        else
        {
          document.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, leadingDoc);
        }
      }
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать письмо в Rx.
    /// </summary>
    /// <param name="documentGuid">Гуид документа в Арио.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateIncomingLetter(string documentGuid, Company.IDepartment department)
    {
      var documentBody = GetDocumentBody(documentGuid);
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      document.Subject = "<TODO>";
      document.Correspondent = Parties.Counterparties.GetAll().FirstOrDefault();
      document.BusinessUnit = department.BusinessUnit;
      document.Department = department;
      document.CreateVersionFrom(documentBody, "pdf");
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Получить тело документа из Арио.
    /// </summary>
    /// <param name="documentGuid">Гуид документа в Арио.</param>
    /// <returns>Тело документа.</returns>
    public static System.IO.Stream GetDocumentBody(string documentGuid)
    {
      var arioUrl = Functions.Module.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      return arioConnector.GetDocumentByGuid(documentGuid);
    }
    
    /// <summary>
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="taskName">Тема задачи.</param>
    /// <param name="documentId">ИД вкладываемого документа.</param>
    /// <param name="responsibleId">ИД ответственного.</param>
    /// <returns>Простая задача.</returns>
    [Remote, Public]
    public static void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents, Company.IEmployee responsible)
    {
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
      task.NeedsReview = false;
      task.Deadline = Calendar.Now.AddWorkingHours(4);
      task.Save();
      task.Start();
    }
    
    public static Company.IDepartment GetDepartment(Company.IEmployee employee)
    {
      if (employee == null)
        return null;
      var employeeDepartment = Company.Departments.GetAll(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                          d.RecipientLinks.Any(l => Equals(l.Member, employee)))
        .FirstOrDefault(department => department.BusinessUnit != null);
      return employeeDepartment;
    }
    
    [Remote, Public]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
  }
}