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
    
    public override void SetLifeCycleState()
    {
      _obj.LifeCycleState = LifeCycleState.Active;
    }
  }
}