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
      Functions.AcquaintanceTask.Remote.RemindParticipants(_obj);
    }

    public virtual bool CanRemindParticipants(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
  }
}