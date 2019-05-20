using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockDocumentBase;

namespace Sungero.Capture.Shared
{
  partial class MockDocumentBaseFunctions
  {
    public override void SetRequiredProperties()
    {
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.DocumentRegister.IsRequired = false;
      _obj.State.Properties.RegistrationDate.IsRequired = false;
    }
    
    public override void FillName()
    {
      base.FillName();
      
      // Если имя формировать не из чего, то сформировать из краткого названия вида документа.
      if (_obj.Name == Docflow.Resources.DocumentNameAutotext)
        _obj.Name = _obj.DocumentKind.ShortName;
    }
  }
}