using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.AcquaintanceTask;

namespace Sungero.Examples.Client
{
  partial class AcquaintanceTaskActions
  {
    public virtual void RemindParticipants(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var incompleteAssignments = Sungero.Workflow.Assignments.GetAll()
        .Where(a => Equals(a.Task, _obj) && 
               a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .ToList();
      
      Logger.Debug($"RemindParticipants. Found {incompleteAssignments.Count} incomplete assignments");
      
      var sentCount = 0;
      foreach (var assignment in incompleteAssignments)
      {
        var performer = Sungero.Examples.Employees.As(assignment.Performer);
        if (performer == null)
          continue;
        
        var subject = assignment.Subject;
        var text = $"У вас невыполненное задание: {assignment.Subject}";
        Functions.AcquaintanceTask.Remote.SendNotificationToEmployee(_obj, performer, subject, text);
        
        sentCount++;
      }
      
      Logger.Debug($"RemindParticipants. Sent {sentCount} notifications");
    }

    public virtual bool CanRemindParticipants(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}