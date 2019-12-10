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
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);
      if (isVisualMode)
      {
        // При визуальной работе обязательность содержания и контрагента как в Contract.
        // Обязательность категории вычисляется по стандартной логике.
        _obj.State.Properties.Subject.IsRequired = true;
        _obj.State.Properties.Counterparty.IsRequired = true;
      }
      else
      {
        // При программной работе содержание, контрагента и категорию делаем необязательными.
        // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
        _obj.State.Properties.Subject.IsRequired = false;
        _obj.State.Properties.Counterparty.IsRequired = false;
        _obj.State.Properties.DocumentGroup.IsRequired = false;
      }
    }
    
    public override void FillName()
    {
      base.FillName();
      
      Capture.PublicFunctions.Module.FillNameFromKindIfEmpty(_obj);
    }
  }
}