using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingInvoice;

namespace Sungero.SmartCapture
{
  partial class IncomingInvoiceClientHandlers
  {

    public override void ContractValueInput(Sungero.Contracts.Client.IncomingInvoiceContractValueInputEventArgs e)
    {
      base.ContractValueInput(e);
      
      this._obj.State.Properties.Contract.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CurrencyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCurrencyValueInputEventArgs e)
    {
      base.CurrencyValueInput(e);
      
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void DateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.DateValueInput(e);
      
      this._obj.State.Properties.Date.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void NumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.NumberValueInput(e);
      
      this._obj.State.Properties.Number.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.BusinessUnit.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CounterpartyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}