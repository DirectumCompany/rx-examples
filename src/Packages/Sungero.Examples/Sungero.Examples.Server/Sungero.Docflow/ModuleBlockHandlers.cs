using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.Examples.Module.Docflow.Server.DocflowBlocks
{
  partial class ApprovalTaskBlockSungeroHandlers
  {

    public virtual void ApprovalTaskBlockSungeroStartTask(Sungero.Docflow.IApprovalTask task)
    {
      var attachments = _obj.AllAttachments.Select(a => Content.ElectronicDocuments.As(a)).Distinct();

      // Определить главный документ.
      var mainDocument = Content.ElectronicDocuments.As(SmartProcessing.PublicFunctions.Module.GetLeadingDocument(attachments.Cast<Sungero.Docflow.IOfficialDocument>()));
      var mainOfficialDocument = Sungero.Docflow.OfficialDocuments.As(mainDocument); 
      
      // Проверить наличие регламента.
      var availableApprovalRules = Sungero.Docflow.PublicFunctions.ApprovalRuleBase.Remote.GetAvailableRulesByDocument(mainOfficialDocument);
      if (availableApprovalRules.Any())
      {
        task.DocumentGroup.All.Add(mainOfficialDocument);
        // Добавить вложения, которые не были добавлены при создании задачи.
        foreach (var attachment in attachments.Where(att => !task.Attachments.Any(x => Equals(x, att))))
        {
          if (RecordManagement.Functions.Module.NeedToAttachDocument(attachment, mainOfficialDocument))
            task.OtherGroup.All.Add(attachment);
        }
      }
      else
      {
        task.Abort();
      }
    }
  }

}