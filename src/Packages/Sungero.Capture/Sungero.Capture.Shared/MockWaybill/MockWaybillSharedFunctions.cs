﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockWaybill;

namespace Sungero.Capture.Shared
{
  partial class MockWaybillFunctions
  {
    /// <summary>
    /// Заполнить имя.
    /// </summary>
    public override void FillName()
    {
      if (_obj != null && _obj.DocumentKind != null && !_obj.DocumentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.OfficialDocuments.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      var name = string.Empty;
      
      /* Имя в формате:
        <Вид документа> №<номер> от <дата> <контрагент> "<содержание>".
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.RegistrationDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
        
        if (!string.IsNullOrWhiteSpace(_obj.Supplier))
          name += " " + _obj.Supplier;
        else if(!string.IsNullOrWhiteSpace(_obj.Shipper))
          name += " " + _obj.Shipper;
        else if(!string.IsNullOrWhiteSpace(_obj.Payer))
          name += " " + _obj.Payer;
        else if(!string.IsNullOrWhiteSpace(_obj.Consignee))
          name += " " + _obj.Consignee;
        
        if (!string.IsNullOrWhiteSpace(_obj.Subject))
          name += " \"" + _obj.Subject + "\"";
        
        if (_obj.DocumentKind != null)
          name = _obj.DocumentKind.ShortName + name;
      }
      
      name = Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      
      _obj.Name = Docflow.PublicFunctions.OfficialDocument.AddClosingQuote(name, _obj);
    }
    
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      
      var properties = _obj.State.Properties;
      
      properties.RegistrationNumber.IsEnabled = true;
      properties.RegistrationDate.IsEnabled = true;
    }
  }
}