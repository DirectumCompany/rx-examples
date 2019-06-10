﻿using System;
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
      
      // Содержание обязательно, только если это указано в метаданных.
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired;
    }
    
    public override List<Sungero.Parties.ICounterparty> GetCounterparties()
    {
      if (_obj.Counterparty == null)
        return null;
      
      return base.GetCounterparties();
    }
    
    public override void FillName()
    {
      base.FillName();
      
      // Если имя формировать не из чего, то сформировать из краткого названия вида документа.
      if (_obj.Name == Docflow.Resources.DocumentNameAutotext)
        _obj.Name = _obj.DocumentKind.ShortName;
    }
  }
}