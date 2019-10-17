using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingTaxInvoice;

namespace Sungero.Capture
{
  partial class MockIncomingTaxInvoiceClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      if (!_obj.VerificationState.HasValue || _obj.VerificationState.Value == VerificationState.Completed)
        _obj.State.Controls.GoodsPreview.IsVisible = false;
    }

    public virtual void CurrencyValueInput(Sungero.Capture.Client.MockIncomingTaxInvoiceCurrencyValueInputEventArgs e)
    {
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void VatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
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

    public virtual void ConsigneeTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ConsigneeTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ConsigneeTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ConsigneeTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ConsigneeNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ConsigneeName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ShipperTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ShipperTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ShipperName.HighlightColor = Sungero.Core.Colors.Empty;
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

    public virtual void RevisionValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Revision.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}