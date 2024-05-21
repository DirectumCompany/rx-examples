using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.ApprovalStage;

namespace Sungero.Examples.Server
{
  partial class ApprovalStageFunctions
  {
    public override List<IRecipient> GetStageRecipients(Sungero.Docflow.IApprovalTask task, List<IRecipient> additionalApprovers)
    {
      var recipients = base.GetStageRecipients(task, additionalApprovers);
      
      if (_obj.ApprovalRoles.Any(r => r.ApprovalRole.Type == Sungero.Examples.ApprovalRole.Type.InitDepEmpl))
      {
        var role = _obj.ApprovalRoles.Where(r => r.ApprovalRole != null && r.ApprovalRole.Type == Sungero.Examples.ApprovalRole.Type.InitDepEmpl)
          .Select(r => Sungero.Examples.ApprovalRoles.As(r.ApprovalRole))
          .Where(r => r != null)
          .FirstOrDefault();
        
        if (role != null)
          recipients.AddRange(Sungero.Examples.Functions.ApprovalRole.GetRolePerformers(role, task));
      }

      return recipients;
    }
  }
}