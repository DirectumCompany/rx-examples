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
    /// Отправить уведомление сотруднику.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="subject">Тема уведомления.</param>
    /// <param name="text">Текст уведомления.</param>
    [Public, Remote]
    public virtual void SendNotificationToEmployee(Sungero.Examples.IEmployee employee, string subject, string text)
    {
      if (employee == null)
        return;
      
      var channel = employee.NotificationChannel;
      
      if (channel == null || channel == Sungero.Examples.Employee.NotificationChannel.DoNotNotify)
        return;
      
      Logger.Debug($"SendNotificationToEmployee. Send message to Employee={employee.Id} via channel={channel.Value}");
      if (channel == Sungero.Examples.Employee.NotificationChannel.Email)
      {
        if (!string.IsNullOrEmpty(employee.Email))
        {
          Sungero.Notifications.PublicFunctions.Module.SendEmail(employee.Email, subject, text);
        }
      }
      else if (channel == Sungero.Examples.Employee.NotificationChannel.SMS)
      {
        // TODO: Реализовать отправку SMS
      }
      else if (channel == Sungero.Examples.Employee.NotificationChannel.Omni)
      {
        // TODO: Реализовать отправку через Омни
      }
    }

  }
}