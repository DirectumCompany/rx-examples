using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingLetter;

namespace Sungero.SmartCapture
{
  partial class IncomingLetterClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired ||
        (_obj.DocumentKind != null &&
         (_obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Registrable ||
          _obj.DocumentKind.GenerateDocumentName == true));
    }
  }
}