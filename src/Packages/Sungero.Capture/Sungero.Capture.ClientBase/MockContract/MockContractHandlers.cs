using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockContract;

namespace Sungero.Capture
{
  partial class MockContractClientHandlers
  {

    public virtual void VatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CurrencyValueInput(Sungero.Capture.Client.MockContractCurrencyValueInputEventArgs e)
    {
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SecondPartySignatoryValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SecondPartySignatory.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SecondPartyTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SecondPartyTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SecondPartyTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SecondPartyTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SecondPartyNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SecondPartyName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void FirstPartySignatoryValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.FirstPartySignatory.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void FirstPartyTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.FirstPartyTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void FirstPartyTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.FirstPartyTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void FirstPartyNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.FirstPartyName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void IsStandardValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      this._obj.State.Properties.IsStandard.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}