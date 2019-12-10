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

    public override void Closing(Sungero.Presentation.FormClosingEventArgs e)
    {
      base.Closing(e);
      
      _obj.State.Properties.Subject.IsRequired = false;
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Поле Содержание обязательно для заполнения в зависимости от типа нумерации/регистрации.
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired ||
        (_obj.DocumentKind != null &&
         (_obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Registrable ||
          _obj.DocumentKind.GenerateDocumentName == true));
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }
    
  }

}