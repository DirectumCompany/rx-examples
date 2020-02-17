using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.UniversalTransferDocument;

namespace Sungero.SmartCapture.Shared
{
  partial class UniversalTransferDocumentFunctions
  {
    
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
    
  }
}