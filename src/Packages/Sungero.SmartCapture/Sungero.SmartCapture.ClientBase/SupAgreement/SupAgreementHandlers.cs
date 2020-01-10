using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.SupAgreement;

namespace Sungero.SmartCapture
{
  partial class SupAgreementClientHandlers
  {

    public override void NoteValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.NoteValueInput(e);
      
      this._obj.State.Properties.Note.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CounterpartySignatoryValueInput(Sungero.Docflow.Client.ContractualDocumentBaseCounterpartySignatoryValueInputEventArgs e)
    {
      base.CounterpartySignatoryValueInput(e);
      
      this._obj.State.Properties.CounterpartySignatory.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      base.TotalAmountValueInput(e);
      
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CurrencyValueInput(Sungero.Docflow.Client.ContractualDocumentBaseCurrencyValueInputEventArgs e)
    {
      base.CurrencyValueInput(e);
      
      this._obj.State.Properties.Currency.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.BusinessUnit.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void OurSignatoryValueInput(Sungero.Docflow.Client.OfficialDocumentOurSignatoryValueInputEventArgs e)
    {
      base.OurSignatoryValueInput(e);
      
      this._obj.State.Properties.OurSignatory.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CounterpartyValueInput(Sungero.Docflow.Client.ContractualDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
      
      this._obj.State.Properties.Counterparty.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void Closing(Sungero.Presentation.FormClosingEventArgs e)
    {
      base.Closing(e);
      
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.LeadingDocument.IsRequired = false;
      _obj.State.Properties.Counterparty.IsRequired = false;
      
      ((Domain.Shared.IExtendedEntity)_obj).Params.Remove(Capture.PublicConstants.Module.IsVisualModeParamName);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // В визуальном режиме поля Содержание, Ведущий документ и Контрагент обязательны, при программном изменении - нет.
      // Чтобы в зависимости от режима изменять обязательность для возможности сохранять документ с незаполненными полями,
      // используется этот параметр. Добавляется на Refresh до отрабатывания базового события,
      // чтобы выполнились вычисления обязательности свойств, т.к. при отмене изменений параметры откатываются.
      ((Domain.Shared.IExtendedEntity)_obj).Params[Capture.PublicConstants.Module.IsVisualModeParamName] = true;
      
      base.Refresh(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

  }
}