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

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
                 
      if (_obj.VerificationState != VerificationState.InProcess)
        _obj.State.Properties.VerificationState.IsVisible = false;
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }
    
  }

}