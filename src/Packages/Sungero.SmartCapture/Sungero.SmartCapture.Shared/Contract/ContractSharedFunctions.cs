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
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);

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
      var smartCaptureNumerationSucceed = _obj.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered &&
        _obj.VerificationState == Sungero.Docflow.OfficialDocument.VerificationState.InProcess &&
        (_obj.DocumentKind == null || _obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable) &&
        _obj.DocumentRegister != null;
      
      if (smartCaptureNumerationSucceed)
        Sungero.Capture.PublicFunctions.Module.EnableRequisitesForVerification(_obj);
      else
        base.ChangeDocumentPropertiesAccess(isEnabled, repeatRegister);
      
      Sungero.SmartCapture.Functions.Module.EnableRegistrationNumberAndDate(_obj);
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