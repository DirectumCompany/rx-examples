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
      
      task.Attachments.Add(document);
      GrantRightsToDocument(documentId, responsibleId, DefaultAccessRightsTypes.FullAccess);
      
      task.Save();
      
      return task;
    }
  }
}