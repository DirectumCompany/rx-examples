using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.Contract;

namespace Sungero.SmartCapture.Shared
{
  partial class ContractFunctions
  {   
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
  }
}