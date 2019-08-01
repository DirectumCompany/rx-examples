﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.OutgoingTaxInvoice;

namespace Sungero.SmartCapture
{
  partial class OutgoingTaxInvoiceServerHandlers
  {

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      
      // Зарегистрировать документ.
      Capture.PublicFunctions.Module.RegisterDocument(_obj);
    }

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      // Сохранить подтверждённые пользователем значения.
      Capture.PublicFunctions.Module.StoreVerifiedPropertiesValues(_obj);
    }
  }

}