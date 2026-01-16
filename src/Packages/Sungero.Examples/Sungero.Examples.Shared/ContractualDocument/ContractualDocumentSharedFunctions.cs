using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.ContractualDocument;

namespace Sungero.Examples.Shared
{
  partial class ContractualDocumentFunctions
  {
    public override bool AllowedToAddMarksManually()
    {
      return false;
    }
  }
}