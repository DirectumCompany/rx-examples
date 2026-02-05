using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
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
        { "AcquaintanceTaskId", assignment.MainTask.Id.ToString() },
        { "MailingGuid", notificationGuid.ToString() }
      };
      var smsMessage = new Notifications.Structures.Module.SmsMessage();
      smsMessage.Recipient = employee.Phone;
      smsMessage.Text = text;
      var message = Notifications.PublicFunctions.Module.ConvertSmsMessageToMessage(smsMessage);
      Notifications.PublicFunctions.Module.SendMessage(message, deliveryParameters, processingParameters);
    }

    private void SendEmail(IEmployee employee, string subject, string text, IAssignment assignment, Guid notificationGuid)
    {
      var deliveryParameters = Notifications.PublicFunctions.Module.CreateDefaultEmailDeliveryParameters();
      var processingParameters = Notifications.PublicFunctions.Module.CreateDefaultProcessingParameters();
      processingParameters.Callback.ClassName = "Sungero.Examples.Server.AcquaintanceTaskFunctions";
      processingParameters.Callback.Method = "CreateResultNotification";
      processingParameters.Callback.Parameters = new Dictionary<string, string>
      {
        {"notificationGuid", notificationGuid.ToString() }
      };
      processingParameters.ExtendedProperties = new Dictionary<string, string>
      {
        { "AcquaintanceTaskId", assignment.MainTask.Id.ToString() },
        { "MailingGuid", notificationGuid.ToString() }
      };
      var email = new Notifications.Structures.Module.EmailMessage();
      email.Subject = subject;
      email.Text = text;
      email.Recipient = employee.Email;
      email.IsHtmlBody = false;
      email.SenderName = string.Empty;
      email.SenderAddress = string.Empty;
      email.CC = new List<string>();
      email.Bcc = new List<string>();
      email.Priority = 0;
      var message = Notifications.PublicFunctions.Module.ConvertEmailMessageToMessage(email);
      Notifications.PublicFunctions.Module.SendMessage(message, deliveryParameters, processingParameters);
    }

    private static void CreateResultNotification(string notificationGuid)
    {
      var entries = NotificationEntries.GetAll()
        .Where(e => e.ExtendedProperties.Any(p => p.Value == notificationGuid)).ToList();
      if (entries.Any(e => e.ProcessingStatus != Notifications.NotificationEntry.ProcessingStatus.Posted
          && e.ProcessingStatus != Notifications.NotificationEntry.ProcessingStatus.Error))
        return;

      var notificationExtendedProperty = entries.FirstOrDefault().ExtendedProperties.FirstOrDefault();
      var acquaintanceTask = RecordManagement.AcquaintanceTasks.Get(long.Parse(notificationExtendedProperty.Value));
      var taskDate = acquaintanceTask.Created.Value.ToShortDateString();
      var subject = Docflow.PublicFunctions.Module
        .TrimQuotes($"Результат отправки уведомлений по задаче от {taskDate}: {acquaintanceTask.Subject}.");

      var errorMessageCount = entries.Count(e => e.ProcessingStatus == Notifications.NotificationEntry.ProcessingStatus.Error);
      var postedMessageCount = entries.Count(e => e.ProcessingStatus == Notifications.NotificationEntry.ProcessingStatus.Posted);
      var notNotificatedCount = ((Workflow.Server.Task)acquaintanceTask).Assignments.Count(a
        => Employees.As(a.Performer).NotificationChannel == Examples.Employee.NotificationChannel.DoNotNotify);

      var activeText = Docflow.PublicFunctions.Module.
        TrimQuotes($"Уведомление о задании от {taskDate} {Hyperlinks.Get(acquaintanceTask)} доставлено.\n\n" +
                   $"Список:\n- Доставлено: {postedMessageCount} из {entries.Count}\n" +
                   $"- Ошибок: {errorMessageCount}\n- Отключены уведомления: {notNotificatedCount}\n");
      var task = SimpleTasks.CreateWithNotices(subject, acquaintanceTask.Author);
      task.ActiveText = activeText;
      task.Subject = subject;
      task.Save();
      task.Start();
    }
  }
}