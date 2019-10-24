using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SupAgreement;

namespace Sungero.SmartCapture.Shared
{
  partial class SupAgreementFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Содержание, Ведущий документ и Контрагент обязательны, только если это указано в метаданных.
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired;
      _obj.State.Properties.LeadingDocument.IsRequired = _obj.Info.Properties.LeadingDocument.IsRequired;
      _obj.State.Properties.Counterparty.IsRequired = _obj.Info.Properties.Counterparty.IsRequired;
    }
    
    public override void FillName()
    {
      base.FillName();
      
      Capture.PublicFunctions.Module.FillNameFromKindIfEmpty(_obj);
    }
  }
}