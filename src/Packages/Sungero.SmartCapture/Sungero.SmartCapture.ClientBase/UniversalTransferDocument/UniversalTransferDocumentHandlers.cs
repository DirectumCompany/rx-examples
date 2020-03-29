using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.UniversalTransferDocument;

namespace Sungero.SmartCapture
{
  partial class UniversalTransferDocumentClientHandlers
  {

    public override void RegistrationNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.RegistrationNumberValueInput(e);
      
      this._obj.State.Properties.RegistrationNumber.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CorrectedValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCorrectedValueInputEventArgs e)
    {
      base.CorrectedValueInput(e);
      
      this._obj.State.Properties.Corrected.HighlightColor = Sungero.Core.Colors.Empty;
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