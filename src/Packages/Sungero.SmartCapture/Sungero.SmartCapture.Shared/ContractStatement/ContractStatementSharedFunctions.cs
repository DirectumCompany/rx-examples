using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture.Shared
{
  partial class ContractStatementFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);

      // При визуальной работе обязательность контрагента как в ContractStatement.
      // При программной работе поля делаем необязательными, чтобы сбросить обязательность,
      // если она изменилась в вызове текущего метода в базовой сущности.
      _obj.State.Properties.Counterparty.IsRequired = isVisualMode;
      
      // Содержание в базовой сущности необязательно, но может в будущем измениться,
      // поэтому при программной работе - явно сбрасываем необязательность.
      // При визуальной работе - обязательность содержания определится в вызове текущего метода в базовой сущности.
      if (!isVisualMode)
        _obj.State.Properties.Subject.IsRequired = false;
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
    
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool repeatRegister)
    {
      var smartCaptureNumerationSucceed = _obj.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered &&
        _obj.VerificationState == Sungero.Docflow.OfficialDocument.VerificationState.InProcess &&
        _obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable &&
        _obj.DocumentRegister != null;
      
      if (smartCaptureNumerationSucceed)
        Sungero.Capture.PublicFunctions.Module.EnableRequisitesForVerification(_obj);
      else
        base.ChangeDocumentPropertiesAccess(isEnabled, repeatRegister);
    }
    
  }
}