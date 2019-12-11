using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingInvoice;

namespace Sungero.SmartCapture.Shared
{
  partial class IncomingInvoiceFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);
      if (isVisualMode)
      {
        // При визуальной работе обязательность контрагента, номера, даты, суммы и валюты как в IncommingInvoice.        
        _obj.State.Properties.Counterparty.IsRequired = true;
        _obj.State.Properties.Number.IsRequired = true;
        _obj.State.Properties.Date.IsRequired = true;
        _obj.State.Properties.TotalAmount.IsRequired = true;
        _obj.State.Properties.Currency.IsRequired = true;
      }
      else
      {
        // При программной работе контрагента, номер, дату, сумму и валюту делаем необязательными.
        // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
        _obj.State.Properties.Counterparty.IsRequired = false;
        _obj.State.Properties.Number.IsRequired = false;
        _obj.State.Properties.Date.IsRequired = false;
        _obj.State.Properties.TotalAmount.IsRequired = false;
        _obj.State.Properties.Currency.IsRequired = false;
        // Содержание в базовой сущности необязательно, но может в будущем измениться,
        // Поэтому при программной работе - явно сбрасываем необязательность.
        // При визуальной работе - обязательность содержания определится в вызове текущего метода в базовой сущности.
        _obj.State.Properties.Subject.IsRequired = false;        
      }
    }
    
    public override void FillName()
    {
      base.FillName();
      
      Capture.PublicFunctions.Module.FillNameFromKindIfEmpty(_obj);
    }
    
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
    
  }
}