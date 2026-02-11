using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.AcquaintanceTask;
using Sungero.Notifications;
using Sungero.Workflow;

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
      var notificationGuid = Guid.NewGuid();
      
      foreach (var assignment in incompleteAssignments)
      {
        var performer = Sungero.Examples.Employees.As(assignment.Performer);
        if (performer == null)
          continue;
        
        var subject = assignment.Subject;
        var text = $"У вас невыполненное задание: {assignment.Subject}";
        this.SendNotificationToEmployee(performer, subject, text, assignment, notificationGuid);
      }
    }
    
    /// <summary>
    /// Отправить уведомление сотруднику.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="subject">Тема уведомления.</param>
    /// <param name="text">Текст уведомления.</param>
    public virtual void SendNotificationToEmployee(Sungero.Examples.IEmployee employee, string subject,
       string text, Workflow.IAssignment assignment, Guid notificationGuid)
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
          this.SendEmail(employee, subject, text, assignment, notificationGuid);
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
          this.SendSms(employee, text, assignment, notificationGuid);
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

    private void SendSms(IEmployee employee, string text, IAssignment assignment, Guid notificationGuid)
    {
      var deliveryParameters = Notifications.PublicFunctions.Module.CreateDefaultSmsDeliveryParameters();
      var processingParameters = Notifications.PublicFunctions.Module.CreateDefaultProcessingParameters();
      processingParameters.ExtendedProperties = new Dictionary<string, string>
      {
        { Constants.RecordManagement.AcquaintanceTask.AcquaintanceTaskIdParamName, assignment.MainTask.Id.ToString() },
        { Constants.RecordManagement.AcquaintanceTask.MailingGuidParamName, notificationGuid.ToString() }
      };
      var smsMessage = new Notifications.Structures.Module.SmsMessage();
      smsMessage.Recipient = employee.Phone;
      smsMessage.Text = text;
      Notifications.PublicFunctions.Module.SendSms(smsMessage, deliveryParameters, processingParameters);
    }

    private void SendEmail(IEmployee employee, string subject, string text, IAssignment assignment, Guid notificationGuid)
    {
      var deliveryParameters = Notifications.PublicFunctions.Module.CreateDefaultEmailDeliveryParameters();
      var processingParameters = Notifications.PublicFunctions.Module.CreateDefaultProcessingParameters();
      processingParameters.Callback.ClassName = "Sungero.Examples.Server.AcquaintanceTaskFunctions";
      processingParameters.Callback.Method = "CreateResultNotification";
      processingParameters.Callback.Parameters = new Dictionary<string, string>
      {
        { Constants.RecordManagement.AcquaintanceTask.MailingGuidParamName, notificationGuid.ToString() }
      };
      processingParameters.ExtendedProperties = new Dictionary<string, string>
      {
        { Constants.RecordManagement.AcquaintanceTask.AcquaintanceTaskIdParamName, assignment.MainTask.Id.ToString() },
        { Constants.RecordManagement.AcquaintanceTask.MailingGuidParamName , notificationGuid.ToString() }
      };
      var email = Sungero.Notifications.Structures.Module.EmailMessage.Create();
      email.Subject = subject;
      email.Text = text;
      var splittedEmails = Sungero.Commons.PublicFunctions.Module.SplitEmails(employee.Email);
      var addresses = string.Join(";", splittedEmails);
      email.Recipient = addresses;
      email.Attachments = this.GetTaskAttachments();
      Notifications.PublicFunctions.Module.SendEmail(email, deliveryParameters, processingParameters);
    }

    /// <summary>
    /// Получить вложения документов для отправки по email.
    /// </summary>
    /// <returns>Список вложений.</returns>
    private List<Sungero.Notifications.Structures.Module.IMailAttachment> GetTaskAttachments()
    {
      var attachments = new List<Sungero.Notifications.Structures.Module.IMailAttachment>();
      
      var mainDocument = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (mainDocument != null)
      {
        var mainAttachment = Notifications.PublicFunctions.Module.CreateMailAttachment(mainDocument);
        if (mainAttachment != null)
          attachments.Add(mainAttachment);
      }
      
      foreach (var addendum in _obj.AddendaGroup.OfficialDocuments)
      {
        var addendumAttachment = Notifications.PublicFunctions.Module.CreateMailAttachment(addendum);
        if (addendumAttachment != null)
          attachments.Add(addendumAttachment);
      }

      return attachments;
    }

    private static void CreateResultNotification(string mailingGuid)
    {
      var entries = NotificationEntries.GetAll()
        .Where(e => e.ExtendedProperties.Any(p => p.Name == Constants.RecordManagement.AcquaintanceTask.MailingGuidParamName &&
                                                  p.Value == mailingGuid))
        .ToList();
      if (entries.Any(e => e.MessageStatus != Notifications.PublicConstants.NotificationEntry.MessageStatus.Delivered &&
                           e.MessageStatus != Notifications.PublicConstants.NotificationEntry.MessageStatus.Error))
        return;

      var acquaintanceTaskId = entries.FirstOrDefault().ExtendedProperties
        .FirstOrDefault(p => p.Name == Constants.RecordManagement.AcquaintanceTask.AcquaintanceTaskIdParamName)?.Value ?? "0";
      var acquaintanceTask = RecordManagement.AcquaintanceTasks.Get(long.Parse(acquaintanceTaskId));
      var taskDate = RecordManagement.AcquaintanceTasks.Get(long.Parse(acquaintanceTaskId)).Created.Value.ToShortDateString();
      var errorMessageCount = entries.Count(e => e.MessageStatus == Notifications.PublicConstants.NotificationEntry.MessageStatus.Error);
      var postedMessageCount = entries.Count(e => e.MessageStatus == Notifications.PublicConstants.NotificationEntry.MessageStatus.Delivered);
      var notNotificatedCount = ((Workflow.Server.Task)acquaintanceTask).Assignments
        .Count(a => Employees.As(a.Performer).NotificationChannel == Examples.Employee.NotificationChannel.DoNotNotify);

      var subject = Docflow.PublicFunctions.Module.TrimQuotes($"Результат отправки уведомлений по задаче от {taskDate}.");
      var activeText = Docflow.PublicFunctions.Module.
        TrimQuotes($"Уведомления о задании от {taskDate} {Hyperlinks.Get(acquaintanceTask)} отправлены.\n\n" +
                   $"Список:\n- Отправлено: {postedMessageCount} из {entries.Count}\n" +
                   $"- Ошибок: {errorMessageCount}\n- Отключены уведомления: {notNotificatedCount}\n");
      var task = SimpleTasks.CreateWithNotices(subject, acquaintanceTask.Author);
      task.ActiveText = activeText;
      task.Subject = subject;
      task.Save();
      task.Start();
    }
  }
}