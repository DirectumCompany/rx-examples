using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.OutgoingTaxInvoice;

namespace Sungero.SmartCapture.Shared
{
  partial class OutgoingTaxInvoiceFunctions
  {
    
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
    
  }
}