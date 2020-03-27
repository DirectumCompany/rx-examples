using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingInvoice;

namespace Sungero.SmartCapture.Client
{
  partial class IncomingInvoiceActions
  {
    public override void AssignNumber(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Bug 96201
      // Сброс подсветки полей происходит в событии контрола InputValue.
      // Действия регистрации изменяют значения полей програмно, событие ValueInput не вызывается.
      var originalNumber = _obj.State.Properties.RegistrationNumber.OriginalValue;
      var originalDate = _obj.State.Properties.RegistrationDate.OriginalValue;
      
      Sungero.SmartCapture.Functions.Module.RemoveNeedValidateRegisterFormatParameter(_obj, e);
      
      base.AssignNumber(e);
      
      if(_obj.RegistrationNumber != originalNumber)
        _obj.State.Properties.RegistrationNumber.HighlightColor = Colors.Empty;
      if(_obj.RegistrationDate != originalDate)
        _obj.State.Properties.RegistrationDate.HighlightColor = Colors.Empty;
    }

    public override bool CanAssignNumber(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanAssignNumber(e);
    }

    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Bug 96201
      // Сброс подсветки полей происходит в событии контрола InputValue.
      // Действия регистрации изменяют значения полей програмно, событие ValueInput не вызывается.
      var originalNumber = _obj.State.Properties.RegistrationNumber.OriginalValue;
      var originalDate = _obj.State.Properties.RegistrationDate.OriginalValue;
      
      Sungero.SmartCapture.Functions.Module.RemoveNeedValidateRegisterFormatParameter(_obj, e);
      
      base.Register(e);
      
      if(_obj.RegistrationNumber != originalNumber)
        _obj.State.Properties.RegistrationNumber.HighlightColor = Colors.Empty;
      if(_obj.RegistrationDate != originalDate)
        _obj.State.Properties.RegistrationDate.HighlightColor = Colors.Empty;
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e);
    }

  }

}