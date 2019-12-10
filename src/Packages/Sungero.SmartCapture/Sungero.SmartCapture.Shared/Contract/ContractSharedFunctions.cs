using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.Contract;

namespace Sungero.SmartCapture.Shared
{
  partial class ContractFunctions
  {
    public override void SetRequiredProperties()
    {
      
      base.SetRequiredProperties();
      
      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var documentParams = ((Domain.Shared.IExtendedEntity)_obj).Params;
      if (documentParams.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName))
        _obj.State.Properties.Subject.IsRequired = true;
      else
      {
        // Содержание, категория и контрагент обязательны, только если это указано в метаданных.
        _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired;
        _obj.State.Properties.DocumentGroup.IsRequired = _obj.Info.Properties.DocumentGroup.IsRequired;
        _obj.State.Properties.Counterparty.IsRequired = _obj.Info.Properties.Counterparty.IsRequired;
      }
      
    }
    
    public override void FillName()
    {
      base.FillName();
      
      Capture.PublicFunctions.Module.FillNameFromKindIfEmpty(_obj);
    }
  }
}