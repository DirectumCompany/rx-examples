using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SupAgreement;

namespace Sungero.SmartCapture.Client
{
  partial class SupAgreementActions
  {
    public override void AssignNumber(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Sungero.SmartCapture.Functions.Module.RemoveNeedValidateRegisterFormatParameter(_obj, e);
      
      base.AssignNumber(e);
    }

    public override bool CanAssignNumber(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanAssignNumber(e);
    }

    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Sungero.SmartCapture.Functions.Module.RemoveNeedValidateRegisterFormatParameter(_obj, e);
      
      base.Register(e);
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e);
    }

  }

}