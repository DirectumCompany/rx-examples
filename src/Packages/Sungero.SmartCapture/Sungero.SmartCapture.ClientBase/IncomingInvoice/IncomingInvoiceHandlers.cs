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
      
      this._obj.State.Properties.Contract.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void CurrencyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCurrencyValueInputEventArgs e)
    {
      base.CurrencyValueInput(e);
      
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      base.TotalAmountValueInput(e);
      
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void DateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.DateValueInput(e);
      
      this._obj.State.Properties.Date.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void NumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.NumberValueInput(e);
      
      this._obj.State.Properties.Number.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.BusinessUnit.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void CounterpartyValueInput(Sungero.Docflow.Client.AccountingDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
       base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.SetPropertiesColors(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
       base.Refresh(e);
         
      // Восстановить обязательность.
      _obj.State.Properties.Counterparty.IsRequired = true;
      _obj.State.Properties.Number.IsRequired = true;
      _obj.State.Properties.Date.IsRequired = true;
      _obj.State.Properties.TotalAmount.IsRequired = true;
      _obj.State.Properties.Currency.IsRequired = true;
      
      // Контрагент не дб задизейблен, если незаполнен.
      if (_obj.Counterparty == null)
        _obj.State.Properties.Counterparty.IsEnabled = true;
      
      // При открытии карточки подсвечиваются распознанные свойства.
      // При отмене изменений подсветки свойств не происходит (не вызывается Showing, также чистятся e.Params).
      // Принудительно обновить подсветку полей после отмены изменений.
      // В остальных случаях параметр будет добавлен при подсветке свойств.
      if (!e.Params.Contains(Capture.PublicConstants.Module.PropertiesAlreadyColoredParamName))
      {
        Sungero.Capture.PublicFunctions.Module.SetPropertiesColors(_obj);
        e.Params.AddOrUpdate(Capture.PublicConstants.Module.PropertiesAlreadyColoredParamName, true);
      }
    }

  }
}