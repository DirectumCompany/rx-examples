using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.Examples.Module.RecordManagement.Server.RecordManagementBlocks
{


  partial class DocumentReviewTaskBloockSungeroHandlers
  {
    public virtual void DocumentReviewTaskBloockSungeroStartTask(Sungero.RecordManagement.IDocumentReviewTask task)
    {
      var attachments = _obj.AllAttachments.Select(a => Content.ElectronicDocuments.As(a)).Distinct().ToList();
      
      // Определить главный документ.
      var mainDocument = Content.ElectronicDocuments.As(SmartProcessing.PublicFunctions.Module.GetLeadingDocument(attachments.Cast<Sungero.Docflow.IOfficialDocument>()));
      var mainOfficialDocument = Sungero.Docflow.OfficialDocuments.As(mainDocument);
      task.DocumentForReviewGroup.All.Add(mainOfficialDocument);
      
      // Добавить вложения, которые не были добавлены при создании задачи.
      foreach (var attachment in attachments.Where(att => !task.Attachments.Any(x => Equals(x, att))))
      {
        if (RecordManagement.Functions.Module.NeedToAttachDocument(attachment, mainOfficialDocument))
          task.OtherGroup.All.Add(attachment);
      }
    }
  }
}