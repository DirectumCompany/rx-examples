using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingLetter;

namespace Sungero.Examples
{
  partial class IncomingLetterSharedHandlers
  {

    public override void InResponseToChanged(Sungero.Docflow.Shared.IncomingDocumentBaseInResponseToChangedEventArgs e)
    {
      base.InResponseToChanged(e);
    }

  }
}