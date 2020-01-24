using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingTaxInvoice;

namespace Sungero.SmartCapture.Client
{
  partial class IncomingTaxInvoiceActions
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