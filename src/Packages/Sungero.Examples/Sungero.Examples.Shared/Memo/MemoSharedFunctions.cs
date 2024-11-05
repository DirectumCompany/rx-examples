using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Memo;

namespace Sungero.Examples.Shared
{
  partial class MemoFunctions
  {
    public override bool AllowedToAddMarksManually()
    {
      return false;
    }
  }
}