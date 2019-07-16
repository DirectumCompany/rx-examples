﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.Waybill;

namespace Sungero.SmartCapture
{
  partial class WaybillServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      // Сохранить подтверждённые пользователем значения.
      Capture.PublicFunctions.Module.StoreVerifiedPropertiesValues(_obj);
    }
  }

}