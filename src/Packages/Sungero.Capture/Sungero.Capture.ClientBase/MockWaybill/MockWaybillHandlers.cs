using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockWaybill;

namespace Sungero.Capture
{
  partial class MockWaybillGoodsClientHandlers
  {

    public virtual void GoodsTotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void GoodsVatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void GoodsPriceValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.Price.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void GoodsCountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.Count.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void GoodsUnitNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.UnitName.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void GoodsNameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Name.HighlightColor = Sungero.Core.Colors.Empty;
    }
  }

  partial class MockWaybillClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.MockWaybill.ChangeGoodsVerificationView(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Functions.MockWaybill.ChangeGoodsVerificationView(_obj);
    }

    public virtual void VatAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.VatAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void CurrencyValueInput(Sungero.Capture.Client.MockWaybillCurrencyValueInputEventArgs e)
    {
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void PayerTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.PayerTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void PayerTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.PayerTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void PayerValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Payer.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SupplierTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SupplierTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SupplierTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.SupplierTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void SupplierValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Supplier.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ConsigneeTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ConsigneeTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ConsigneeTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ConsigneeTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ConsigneeValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Consignee.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ShipperTrrc.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.ShipperTin.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ShipperValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Shipper.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public virtual void ContractValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Contract.HighlightColor = Sungero.Core.Colors.Empty;
    }

  }
}