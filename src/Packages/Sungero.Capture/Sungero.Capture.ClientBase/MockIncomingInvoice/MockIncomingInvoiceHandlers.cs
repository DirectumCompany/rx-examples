using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingInvoice;

namespace Sungero.Capture
{
  partial class MockIncomingInvoiceClientHandlers
  {

    public virtual void VatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ContractValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Contract.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CurrencyValueInput(Sungero.Capture.Client.MockIncomingInvoiceCurrencyValueInputEventArgs e)
    {
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BuyerTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BuyerTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BuyerTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BuyerTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void BuyerNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.BuyerName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SellerTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SellerTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SellerTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SellerTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SellerNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SellerName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void DateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      this._obj.State.Properties.Date.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void NumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Number.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}