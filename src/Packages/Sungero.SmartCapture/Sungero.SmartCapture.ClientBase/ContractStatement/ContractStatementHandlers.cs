using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture
{
  partial class ContractStatementClientHandlers
  {

    public override void CounterpartyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}