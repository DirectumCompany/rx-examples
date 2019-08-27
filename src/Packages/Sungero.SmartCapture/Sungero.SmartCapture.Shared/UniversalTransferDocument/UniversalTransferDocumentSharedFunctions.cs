using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.UniversalTransferDocument;

namespace Sungero.SmartCapture.Shared
{
  partial class UniversalTransferDocumentFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Содержание обязательно, только если это указано в метаданных.
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired;
    }
    
    public override List<Sungero.Parties.ICounterparty> GetCounterparties()
    {
      if (_obj.Counterparty == null)
        return null;
      
      return base.GetCounterparties();
    }
    
    public override void FillName()
    {
      base.FillName();
      
      Capture.PublicFunctions.Module.FillNameFromKind(_obj);
    }
    
  }
}