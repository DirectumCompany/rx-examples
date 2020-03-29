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

  }
}