using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingLetter;

namespace Sungero.SmartCapture
{
  partial class IncomingLetterClientHandlers
  {

    public override void Closing(Sungero.Presentation.FormClosingEventArgs e)
    {
      base.Closing(e);
      
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.Correspondent.IsRequired = false;
      
      ((Domain.Shared.IExtendedEntity)_obj).Params.Remove(Capture.PublicConstants.Module.IsVisualModeParamName);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void ContactValueInput(Sungero.RecordManagement.Client.IncomingLetterContactValueInputEventArgs e)
    {
      base.ContactValueInput(e);
      
      this._obj.State.Properties.Contact.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void InResponseToValueInput(Sungero.Docflow.Client.IncomingDocumentBaseInResponseToValueInputEventArgs e)
    {
      base.InResponseToValueInput(e);
      
      this._obj.State.Properties.InResponseTo.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      base.PreparedByValueInput(e);
      
      this._obj.State.Properties.PreparedBy.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void SubjectValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.SubjectValueInput(e);
      
      this._obj.State.Properties.Subject.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void AddresseeValueInput(Sungero.Docflow.Client.IncomingDocumentBaseAddresseeValueInputEventArgs e)
    {
      base.AddresseeValueInput(e);
      
      this._obj.State.Properties.Addressee.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void SignedByValueInput(Sungero.RecordManagement.Client.IncomingLetterSignedByValueInputEventArgs e)
    {
      base.SignedByValueInput(e);
      
      this._obj.State.Properties.SignedBy.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.BusinessUnit.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void DatedValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.DatedValueInput(e);
      
      // Для DateTime событие изменения отрабатывает, даже если даты одинаковые.
      // Поэтому еще раз сравниваем только даты без учёта времени.
      if (e.OldValue.HasValue && e.NewValue.HasValue && Equals(e.OldValue.Value.Date, e.NewValue.Value.Date))
        return;
      
      this._obj.State.Properties.Dated.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void InNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.InNumberValueInput(e);
      
      this._obj.State.Properties.InNumber.HighlightColor = Sungero.Core.Colors.Empty;
    }

    public override void CorrespondentValueInput(Sungero.Docflow.Client.IncomingDocumentBaseCorrespondentValueInputEventArgs e)
    {
      base.CorrespondentValueInput(e);
      
      this._obj.State.Properties.Correspondent.HighlightColor = Sungero.Core.Colors.Empty;
      this._obj.State.Properties.Contact.HighlightColor = Sungero.Core.Colors.Empty;
      this._obj.State.Properties.SignedBy.HighlightColor = Sungero.Core.Colors.Empty;
    }
    
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // В визуальном режиме поля содержание и корреспондент обязательны, при программном изменении - нет.
      // Чтобы в зависимости от режима изменять обязательность для возможности сохранять документ с незаполненными полями,
      // используется этот параметр. Добавляется на Refresh до отрабатывания базового события,
      // чтобы выполнились вычисления обязательности свойств, т.к. при отмене изменений параметры откатываются.
      ((Domain.Shared.IExtendedEntity)_obj).Params[Capture.PublicConstants.Module.IsVisualModeParamName] = true;
      
      base.Refresh(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }
  }
}