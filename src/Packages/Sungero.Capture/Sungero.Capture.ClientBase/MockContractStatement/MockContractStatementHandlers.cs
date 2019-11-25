using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockContractStatement;

namespace Sungero.Capture
{
  partial class MockContractStatementGoodsClientHandlers
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

  partial class MockContractStatementClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.MockContractStatement.ChangeGoodsVerificationView(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Functions.MockContractStatement.ChangeGoodsVerificationView(_obj);
    }

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