using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SupAgreement;

namespace Sungero.SmartCapture
{
  partial class SupAgreementSharedHandlers
  {

    public override void CounterpartySignatoryChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartySignatoryChangedEventArgs e)
    {
      base.CounterpartySignatoryChanged(e);
      
      this._obj.State.Properties.CounterpartySignatory.HighlightColor = Sungero.Core.Colors.Empty;
    }
  }

}