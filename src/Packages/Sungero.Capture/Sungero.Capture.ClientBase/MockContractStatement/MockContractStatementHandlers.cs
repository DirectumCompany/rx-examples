using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockContractStatement;

namespace Sungero.Capture
{
  partial class MockContractStatementClientHandlers
  {

    public virtual void VatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CurrencyValueInput(Sungero.Capture.Client.MockContractStatementCurrencyValueInputEventArgs e)
    {
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BusinessUnitTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BusinessUnitTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BusinessUnitTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BusinessUnitTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BusinessUnitNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BusinessUnitName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CounterpartyTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.CounterpartyTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CounterpartyTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.CounterpartyTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CounterpartyNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.CounterpartyName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void LeadDocValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.LeadDoc.HighlightColor = Sungero.Core.Colors.Empty;
    }
  }

}