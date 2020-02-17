using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.OutgoingTaxInvoice;

namespace Sungero.SmartCapture.Shared
{
  partial class OutgoingTaxInvoiceFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Sungero.Docflow.PublicConstants.OfficialDocument.IsVisualModeParamName);

      // При визуальной работе обязательность контрагента как в OutgoingTaxInvoice.
      // При программной работе поля делаем необязательными, чтобы сбросить обязательность,
      // если она изменилась в вызове текущего метода в базовой сущности.
      _obj.State.Properties.Counterparty.IsRequired = isVisualMode;
      
      // Содержание в базовой сущности необязательно, но может в будущем измениться,
      // Поэтому при программной работе - явно сбрасываем необязательность.
      // При визуальной работе - обязательность содержания определится в вызове текущего метода в базовой сущности.
      if (!isVisualMode)
        _obj.State.Properties.Subject.IsRequired = false;
    }
    
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool repeatRegister)
    {
      var smartCaptureNumerationSucceed = Capture.PublicFunctions.Module.IsSmartCaptureNumerationSucceed(_obj);
      
      if (smartCaptureNumerationSucceed)
        Sungero.Capture.PublicFunctions.Module.EnableRequisitesForVerification(_obj);
      else
        base.ChangeDocumentPropertiesAccess(isEnabled, repeatRegister);
      
      Sungero.SmartCapture.Functions.Module.EnableRegistrationNumberAndDate(_obj);
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