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
    
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      
      var properties = _obj.State.Properties;
      
      properties.DeliveryMethod.IsEnabled = needShow;
      properties.DeliveryMethod.IsVisible = needShow;
      
      properties.CaseFile.IsEnabled = needShow;
      properties.CaseFile.IsVisible = needShow;
      
      properties.PlacedToCaseFileDate.IsEnabled = needShow;
      properties.PlacedToCaseFileDate.IsVisible = needShow;
    }
    
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
    
  }
}