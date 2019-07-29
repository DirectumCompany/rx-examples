using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture.Client
{
  partial class ContractStatementActions
  {
    public override void CancelRegistration(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CancelRegistration(e);
      
      // Отменить подсветку рег.номера и даты.
      this._obj.State.Properties.RegistrationNumber.HighlightColor = Sungero.Core.Colors.Empty;
      this._obj.State.Properties.RegistrationDate.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override bool CanCancelRegistration(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCancelRegistration(e);
    }

  }

}