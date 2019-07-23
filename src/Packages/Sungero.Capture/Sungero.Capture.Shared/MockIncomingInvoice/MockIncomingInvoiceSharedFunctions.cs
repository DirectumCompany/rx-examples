using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingInvoice;

namespace Sungero.Capture.Shared
{
  partial class MockIncomingInvoiceFunctions
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
        if (!string.IsNullOrWhiteSpace(_obj.Number))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.Number;
        
        if (_obj.Date != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Date.Value.ToString("d");
        
        if (!string.IsNullOrWhiteSpace(_obj.SellerName))
          name += " " + _obj.SellerName;
        else if(!string.IsNullOrWhiteSpace(_obj.BuyerName))
          name += " " + _obj.BuyerName;
        
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