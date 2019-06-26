using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingLetter;

namespace Sungero.SmartCapture.Client
{
  partial class IncomingLetterActions
  {
    public override void Cancel(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Cancel(e);
    }

    public override bool CanCancel(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCancel(e);
    }

  }

}