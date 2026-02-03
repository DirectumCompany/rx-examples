using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;

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
            var notificationGuid = Guid.NewGuid();
            foreach (var assignment in incompleteAssignments)
            {
                var performer = Sungero.Examples.Employees.As(assignment.Performer);
                if (performer == null)
                    continue;

                var subject = assignment.Subject;
                var text = $"У вас невыполненное задание: {assignment.Subject}";
                this.SendNotificationToEmployee(performer, subject, text, assignment, notificationGuid);
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

        private void SendSms(Sungero.Examples.IEmployee employee, string text,
          Workflow.IAssignment assignment, Guid notificationGuid)
        {
            var deliveryParameters = Notifications.PublicFunctions.Module.CreateDefaultSmsDeliveryParameters();
            var processingParameters = Notifications.PublicFunctions.Module.CreateDefaultProcessingParameters();
            processingParameters.ExtendedProperties = new Dictionary<string, string>
            {
               { assignment.MainTask.Id.ToString(), notificationGuid.ToString() }
            };
            var smsMessage = new Notifications.Structures.Module.SmsMessage();
            smsMessage.Recipient = employee.Phone;
            smsMessage.Text = text;
            Notifications.PublicFunctions.Module.SendSms(smsMessage, deliveryParameters, processingParameters);
        }

        private void SendEmail(Sungero.Examples.IEmployee employee, string subject, string text,
          Workflow.IAssignment assignment, Guid notificationGuid)
        {
            var deliveryParameters = Notifications.PublicFunctions.Module.CreateDefaultEmailDeliveryParameters();
            var processingParameters = Notifications.PublicFunctions.Module.CreateDefaultProcessingParameters();
            processingParameters.ExtendedProperties = new Dictionary<string, string>
            {
               { assignment.MainTask.Id.ToString(), notificationGuid.ToString() }
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
            Notifications.PublicFunctions.Module.SendEmail(email, deliveryParameters, processingParameters);
        }
    }
}