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
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Sungero.Docflow.PublicConstants.OfficialDocument.IsVisualModeParamName);

      // При визуальной работе обязательность контрагента, номера, даты, суммы и валюты как в IncommingInvoice.
      // При программной работе контрагента, номер, дату, сумму и валюту делаем необязательными.
      // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
      _obj.State.Properties.Counterparty.IsRequired = isVisualMode;
      _obj.State.Properties.Number.IsRequired = isVisualMode;
      _obj.State.Properties.Date.IsRequired = isVisualMode;
      _obj.State.Properties.TotalAmount.IsRequired = isVisualMode;
      _obj.State.Properties.Currency.IsRequired = isVisualMode;
      
      // Содержание в базовой сущности необязательно, но может в будущем измениться,
      // Поэтому при программной работе - явно сбрасываем необязательность.
      // При визуальной работе - обязательность содержания определится в вызове текущего метода в базовой сущности.
      if (!isVisualMode)
        _obj.State.Properties.Subject.IsRequired = false;
    }
    
    public override void FillName()
    {
      base.FillName();
      
      FillNameFromKindIfEmpty();
    }
    
    [Public]
    public override bool IsVerificationModeSupported()
    {
      return true;
    }
    
  }
}