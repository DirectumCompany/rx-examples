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
      
      // Изменить обязательность полей в зависимости от того, программная или визуальная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Sungero.Docflow.PublicConstants.OfficialDocument.IsVisualModeParamName);

      // При визуальной работе обязательность содержания и контрагента как в Contract.
      // Обязательность категории вычисляется по стандартной логике.
      // При программной работе содержание, контрагента и категорию делаем необязательными.
      // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
      _obj.State.Properties.Subject.IsRequired = isVisualMode;
      _obj.State.Properties.Counterparty.IsRequired = isVisualMode;
      if (!isVisualMode)
        _obj.State.Properties.DocumentGroup.IsRequired = false;
    }
    
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool repeatRegister)
    {
      var smartCaptureNumerationSucceed = IsSmartCaptureNumerationSucceed();
      
      if (smartCaptureNumerationSucceed)
        EnableRequisitesForVerification();
      else
        base.ChangeDocumentPropertiesAccess(isEnabled, repeatRegister);
      
      EnableRegistrationNumberAndDate();
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