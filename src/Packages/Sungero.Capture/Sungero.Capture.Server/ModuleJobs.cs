using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Workflow;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace Sungero.Capture.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Фоновый процесс для мониторинга выполненных задач верификации.
    /// </summary>
    public virtual void ChangeVerificationState()
    {
      var documentIds = Docflow.OfficialDocuments.GetAll()
        .Where(d => d.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess)
        .Where(d => d.RegistrationState == Docflow.OfficialDocument.RegistrationState.Registered ||
               d.RegistrationState == Docflow.OfficialDocument.RegistrationState.NotRegistered &&
               d.DocumentKind.NumberingType == Docflow.DocumentKind.NumberingType.NotNumerable)
        .Select(d => d.Id).ToList();
      
      // processedIds - ид документов, статус которых уже был изменен. В одной задаче на верификацию может придти пакет документов,
      // для всех них сразу изменяем статус и сохраняем в этот список, чтобы в дальнейшем не обрабатывать их в цикле по documentIds.
      var processedIds = new List<int?>();
      var verificationTasks = SimpleTasks.GetAll()
        .Where(t => t.MainTaskId.HasValue && t.MainTaskId == t.Id &&
               (t.Subject.Contains(Resources.CheckPackage) ||
                t.Subject.Contains(Resources.CheckDocument)));
      
      foreach(var documentId in documentIds)
      {
        if (processedIds.Contains(documentId))
          continue;
        
        // Берем задачи по документу, чтобы отсеять все задачи, уже обработанные ФП.
        var documentVerificationTasks = verificationTasks
          .Where(vt => vt.AttachmentDetails.Any(att => att.AttachmentId == documentId))
          .OrderByDescending(s => s.Started);
        if (!documentVerificationTasks.Any())
          continue;
        
        // Документ верифицирован, если последняя задача была выполнена.
        var task = documentVerificationTasks.First();
        if (task.Status != Workflow.Task.Status.Completed)
          continue;
        
        // Дополнительно проверять вложения на возможность смены статуса верификации.
        var attachmentIds = task.AttachmentDetails
          .Where(d => d.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess)
          .Where(d => d.RegistrationState == Docflow.OfficialDocument.RegistrationState.Registered ||
                      d.RegistrationState == Docflow.OfficialDocument.RegistrationState.NotRegistered &&
                      d.DocumentKind.NumberingType == Docflow.DocumentKind.NumberingType.NotNumerable)
          .Select(att => att.EntityId).ToList();
        
        processedIds.AddRange(attachmentIds);
        var subTasks = Tasks.GetAll()
          .Where(t => t.MainTaskId.HasValue &&
                 t.MainTaskId.Value == task.Id &&
                 t.Status != Workflow.Task.Status.Completed &&
                 t.Status != Workflow.Task.Status.Aborted);
        if (!subTasks.Any())
        {
          foreach (var id in attachmentIds)
          {
            if (id == null)
              continue;
            
            var document = OfficialDocuments.Get((int)id);
            var hasEmptyRequiredProperties = false;
            var originalMetadata = document.GetEntityMetadata().GetOriginal();
            foreach (var p in originalMetadata.Properties)
            {
              if (p.IsRequired && p.GetPropertyValue<object>(document) == null)
              {
                Logger.DebugFormat(Resources.EntityRequiredPropertyIsEmptyFormat(document.Id, p.Name));
                hasEmptyRequiredProperties = true;
                break;
              }
            }

            if (!hasEmptyRequiredProperties)
            {
              document.VerificationState = Docflow.OfficialDocument.VerificationState.Completed;
              document.Save();
            }
          }
        }
      }
    }

  }
}