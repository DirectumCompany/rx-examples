using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;

namespace Sungero.Examples.Shared
{
  partial class IncomingInvoiceFunctions
  {
    public override List<string> GetAvailableMarkKindsSids()
    {
      var marksKinds = new List<string>()
      {
        Docflow.Constants.MarkKind.ElectronicSignatureMarkKindSid,
        Docflow.Constants.MarkKind.RegistrationDateMarkKindSid,
        Docflow.Constants.MarkKind.RegistrationNumberMarkKindSid
      };
      
      if (_obj.LifeCycleState == Sungero.Contracts.IncomingInvoice.LifeCycleState.Paid)
        marksKinds.Add(Sungero.Examples.Constants.Contracts.IncomingInvoice.PaymentMarkKindSid);
      
      return marksKinds;
    }
  }
}