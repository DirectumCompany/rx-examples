using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SimpleDocument;

namespace Sungero.SmartCapture
{
  partial class SimpleDocumentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }
    
  }

}