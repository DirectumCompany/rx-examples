﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture
{
  partial class ContractStatementClientHandlers
  {

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      base.TotalAmountValueInput(e);
      
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void RegistrationNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.RegistrationNumberValueInput(e);
      
      this._obj.State.Properties.RegistrationNumber.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void RegistrationDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.RegistrationDateValueInput(e);
      
      this._obj.State.Properties.RegistrationDate.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void LeadingDocumentValueInput(Sungero.Docflow.Client.OfficialDocumentLeadingDocumentValueInputEventArgs e)
    {
      base.LeadingDocumentValueInput(e);
      
      this._obj.State.Properties.LeadingDocument.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CurrencyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCurrencyValueInputEventArgs e)
    {
      base.CurrencyValueInput(e);
      
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CounterpartyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.ActivateVerivicationMode(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Восстановить обязательность контрагента.
      _obj.State.Properties.Counterparty.IsRequired = true;
      
      // Контрагент не дб задизейблен, если незаполнен.
      if (_obj.Counterparty == null)
        _obj.State.Properties.Counterparty.IsEnabled = true;
      
      Sungero.Capture.PublicFunctions.Module.ActivateVerivicationMode(_obj);
    }

  }
}