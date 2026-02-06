using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.AcquaintanceTask;
using Sungero.Notifications;

namespace Sungero.Examples.Server
{
  partial class AcquaintanceTaskFunctions
  {
    /// <summary>
    /// Напомнить участникам о невыполненных заданиях.
    /// </summary>
    /// <returns>Текст ошибки, если возникла.</returns>
    [Remote]
    public virtual string RemindParticipants()
    {
      var incompleteAssignments = Sungero.Workflow.Assignments.GetAll()
        .Where(a => Equals(a.Task, _obj) && 
               a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .ToList();
      
      Logger.Debug($"RemindParticipants. Found {incompleteAssignments.Count} incomplete assignments");
      
      foreach (var assignment in incompleteAssignments)
      {
        var performer = Sungero.Examples.Employees.As(assignment.Performer);
        if (performer == null)
          continue;
        
        var subject = assignment.Subject;
        var text = $"У вас невыполненное задание: {assignment.Subject}";
        try
        {
          this.SendNotificationToEmployee(performer, subject, text);
        }
        catch (Exception ex)
        {
          Logger.Error(ex, $"RemindParticipants. Error while sending notification. Assignment={assignment.Id}, Employee={performer.Id}");
          return this.GetServiceErrorMessage(ex.Message);
        }
      }
      
      return string.Empty;
    }
    
    /// <summary>
    /// Получить текст ошибки из сообщения сервиса.
    /// </summary>
    /// <param name="errorText">Текст ошибки.</param>
    /// <returns>Сообщение ошибки.</returns>
    private string GetServiceErrorMessage(string errorText)
    {
      if (string.IsNullOrWhiteSpace(errorText))
        return errorText;
      
      var jsonStart = errorText.IndexOf('{');
      var jsonEnd = errorText.LastIndexOf('}');
      if (jsonStart < 0 || jsonEnd <= jsonStart)
        return errorText;
      
      var jsonText = errorText.Substring(jsonStart, jsonEnd - jsonStart + 1);
      const string messageKey = "\"message\"";
      var messageIndex = jsonText.IndexOf(messageKey, StringComparison.OrdinalIgnoreCase);
      if (messageIndex < 0)
        return errorText;
      
      var colonIndex = jsonText.IndexOf(':', messageIndex + messageKey.Length);
      if (colonIndex < 0)
        return errorText;
      
      var quoteStart = jsonText.IndexOf('"', colonIndex + 1);
      if (quoteStart < 0)
        return errorText;
      
      var quoteEnd = jsonText.IndexOf('"', quoteStart + 1);
      if (quoteEnd <= quoteStart)
        return errorText;
      
      var message = jsonText.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
      return string.IsNullOrWhiteSpace(message) ? errorText : message;
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
      
      if (channel == null || channel == Examples.Employee.NotificationChannel.DoNotNotify)
      {
        Logger.Debug($"SendNotificationToEmployee. Cannot send message to Employee={employee.Id} because of his notify channel settings");
        return;
      }
      
      Logger.Debug($"SendNotificationToEmployee. Send message to Employee={employee.Id} via channel={channel.Value}");
      if (channel == Examples.Employee.NotificationChannel.Email)
      {
        if (!string.IsNullOrEmpty(employee.Email))
        {
          var splittedEmails = Sungero.Commons.PublicFunctions.Module.SplitEmails(employee.Email);
          var addresses = string.Join(";", splittedEmails);
          var eMail = Notifications.PublicFunctions.Module.CreateEmailMessage(addresses, subject, text, false, "", "", new List<string>(), new List<string>(), 0);
          Notifications.PublicFunctions.Module.SendEmail(eMail);
        }
        else
        {
          Logger.Debug($"SendNotificationToEmployee. Cannot send message to Employee={employee.Id} because of an empty e-mail");
        }
      }
      else if (channel == Examples.Employee.NotificationChannel.SMS)
      {
        if (!string.IsNullOrEmpty(employee.Phone))
        {
          var sms = Notifications.PublicFunctions.Module.CreateSmsMessage(employee.Phone, text);
          Notifications.PublicFunctions.Module.SendSms(sms);
        }
        else
        {
          Logger.Debug($"SendNotificationToEmployee. Cannot send message to Employee={employee.Id} because of an empty phone");
        }
      }
      else if (channel == Examples.Employee.NotificationChannel.Omni)
      {
        Logger.Debug($"SendNotificationToEmployee. Omni sending not implemented");
        // TODO: Реализовать отправку через Омни
      }
    }

  }
}