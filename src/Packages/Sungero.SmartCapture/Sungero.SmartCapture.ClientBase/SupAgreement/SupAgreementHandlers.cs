using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SupAgreement;

namespace Sungero.SmartCapture
{
  partial class SupAgreementClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Поля Содержание и Ведущий документ обязательны для заполнения.
      _obj.State.Properties.Subject.IsRequired = true;
      _obj.State.Properties.LeadingDocument.IsRequired = true;
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

  }
}