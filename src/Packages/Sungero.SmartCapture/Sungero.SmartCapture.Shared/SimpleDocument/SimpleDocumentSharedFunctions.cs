using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SimpleDocument;

namespace Sungero.SmartCapture.Shared
{
  partial class SimpleDocumentFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Изменить обязательность полей в зависимости от того, программная или визульная работа.
      var isVisualMode = ((Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Capture.PublicConstants.Module.IsVisualModeParamName);
      if (!isVisualMode)
      {
        // При программной работе содержание делаем необязательными.
        // Чтобы сбросить обязательность, если она изменилась в вызове текущего метода в базовой сущности.
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