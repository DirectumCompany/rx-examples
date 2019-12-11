using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingLetter;

namespace Sungero.SmartCapture.Shared
{
  partial class IncomingLetterFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();

      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);
      if (isVisualMode)
      {
        // При визуальной работе обязательность содержания и корреспондента как в IncomingLetter.
        _obj.State.Properties.Subject.IsRequired = true;
        _obj.State.Properties.Correspondent.IsRequired = true;
      }
      else
      {
        // При программной работе содержание и корреспондент - необязательные.
        // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
        _obj.State.Properties.Subject.IsRequired = false;
        _obj.State.Properties.Correspondent.IsRequired = false;        
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