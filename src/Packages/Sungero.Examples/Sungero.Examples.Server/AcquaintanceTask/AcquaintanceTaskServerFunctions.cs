using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.AcquaintanceTask;

namespace Sungero.Examples.Server
{
  partial class AcquaintanceTaskFunctions
  {
    /// <summary>
    /// Напомнить участникам о невыполненных заданиях.
    /// </summary>
    [Remote]
    public virtual void RemindParticipants()
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
        this.SendNotificationToEmployee(performer, subject, text);
        
        sentCount++;
      }
      
      Logger.Debug($"RemindParticipants. Sent {sentCount} notifications");
    }
    
    /// <summary>
    /// Отправить уведомление сотруднику.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="subject">Тема уведомления.</param>
    /// <param name="text">Текст уведомления.</param>
    public virtual void SendNotificationToEmployee(Sungero.Examples.IEmployee employee, string subject, string text)
    {
      if (employee == null)
        return;
      
      var channel = employee.NotificationChannel;
      
      if (channel == null || channel == Sungero.Examples.Employee.NotificationChannel.DoNotNotify)
      {
        Logger.Debug($"SendNotificationToEmployee. Cannot send message to Employee={employee.Id} because of his notify channel settings");
        return;
      }
      
      Logger.Debug($"SendNotificationToEmployee. Send message to Employee={employee.Id} via channel={channel.Value}");
      if (channel == Sungero.Examples.Employee.NotificationChannel.Email)
      {
        if (!string.IsNullOrEmpty(employee.Email))
        {
          Sungero.Notifications.PublicFunctions.Module.SendEmail(employee.Email, subject, text);
        }
        else
        {
          Logger.Debug($"SendNotificationToEmployee. Cannot send message to Employee={employee.Id} because of an empty e-mail");
        }
      }
      else if (channel == Sungero.Examples.Employee.NotificationChannel.SMS)
      {
        Logger.Debug($"SendNotificationToEmployee. SMS sending not implemented");
        // TODO: Реализовать отправку SMS
      }
      else if (channel == Sungero.Examples.Employee.NotificationChannel.Omni)
      {
        Logger.Debug($"SendNotificationToEmployee. Omni sending not implemented");
        // TODO: Реализовать отправку через Омни
      }
    }

  }
}