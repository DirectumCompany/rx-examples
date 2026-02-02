using System.Linq;
using Sungero.Notifications;
using System.Collections.Generic;
using Sungero.Core;
using Sungero.Workflow;
using Sungero.Notifications.NotificationEntry;

namespace Sungero.Examples.Module.Notifications.Server
{
    partial class ModuleFunctions
    {
        public override void FinalizeMessageSending(INotificationEntry entry,
          Sungero.Notifications.Structures.Module.IProcessResult processResult)
        {
            base.FinalizeMessageSending(entry, processResult);
            var notificationExtendedProperty = entry.ExtendedProperties.FirstOrDefault();
            var notificationEntries = NotificationEntries.GetAll()
                      .Where(e => e.ExtendedProperties.Any(p => p.Name == notificationExtendedProperty.Name
                                                         && p.Value == notificationExtendedProperty.Value))
                      .ToList();
            if (notificationEntries.Any(e => e.ProcessingStatus != ProcessingStatus.Posted 
                && e.ProcessingStatus != ProcessingStatus.Error))
                return;

            this.CreateResultNotification(entry, notificationEntries);
        }

        private void CreateResultNotification(INotificationEntry entry, List<INotificationEntry> notificationEntries)
        {
            var notificationExtendedProperty = entry.ExtendedProperties.FirstOrDefault();
            var asqTask = Sungero.RecordManagement.AcquaintanceTasks.Get(long.Parse(notificationExtendedProperty.Name));
            var taskDate = asqTask.Created.Value.ToShortDateString();
            var subject = Sungero.Docflow.PublicFunctions.Module.
              TrimQuotes($"Результат отправки уведомлений по задаче от {taskDate}: {asqTask.Subject}.");
            var errorMessageCount = notificationEntries.Count(e => e.ProcessingStatus == ProcessingStatus.Error);
            var postedNotificationCount = notificationEntries.Count(e => e.ProcessingStatus == ProcessingStatus.Posted);
            var notNotif = ((Workflow.Server.Task)asqTask).Assignments.Count(a 
              => Employees.As(a.Performer).NotificationChannel == Employee.NotificationChannel.DoNotNotify);
            var activeText = Sungero.Docflow.PublicFunctions.Module.
              TrimQuotes($"Уведомление о задании от {taskDate} {Hyperlinks.Get(asqTask)} доставлено.\n\n" +
                        $"Список:\n- Доставлено: {postedNotificationCount} из {notificationEntries.Count}\n" +
                        $"- Ошибок: {errorMessageCount}\n- Отключены уведомления: {notNotif}\n");

            var task = SimpleTasks.CreateWithNotices(subject, asqTask.Author);
            task.ActiveText = activeText;
            task.Subject = subject;
            task.Save();
            task.Start();
        }
    }
}