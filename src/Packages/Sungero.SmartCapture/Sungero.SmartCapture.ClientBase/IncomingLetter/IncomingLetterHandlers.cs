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

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Подсветка полей.
      Sungero.Capture.PublicFunctions.Module.SetPropertiesColors(_obj);
    }

    public override void ContactValueInput(Sungero.RecordManagement.Client.IncomingLetterContactValueInputEventArgs e)
    {
      base.ContactValueInput(e);
      
      this._obj.State.Properties.Contact.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void InResponseToValueInput(Sungero.Docflow.Client.IncomingDocumentBaseInResponseToValueInputEventArgs e)
    {
      base.InResponseToValueInput(e);
      
      this._obj.State.Properties.InResponseTo.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      base.PreparedByValueInput(e);
      
      this._obj.State.Properties.PreparedBy.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void SubjectValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.SubjectValueInput(e);
      
      this._obj.State.Properties.Subject.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void AddresseeValueInput(Sungero.Docflow.Client.IncomingDocumentBaseAddresseeValueInputEventArgs e)
    {
      base.AddresseeValueInput(e);
      
      this._obj.State.Properties.Addressee.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void SignedByValueInput(Sungero.RecordManagement.Client.IncomingLetterSignedByValueInputEventArgs e)
    {
      base.SignedByValueInput(e);
      
      this._obj.State.Properties.SignedBy.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void BusinessUnitValueInput(Sungero.Docflow.Client.OfficialDocumentBusinessUnitValueInputEventArgs e)
    {
      base.BusinessUnitValueInput(e);
      
      this._obj.State.Properties.BusinessUnit.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void DatedValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.DatedValueInput(e);
      
      // Для DateTime событие изменения отрабатывает, даже если даты одинаковые.
      // Поэтому еще раз сравниваем только даты без учёта времени.
      if (e.OldValue.HasValue && e.NewValue.HasValue && Equals(e.OldValue.Value.Date, e.NewValue.Value.Date))
        return;
      
      this._obj.State.Properties.Dated.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void InNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.InNumberValueInput(e);
      
      this._obj.State.Properties.InNumber.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void CorrespondentValueInput(Sungero.Docflow.Client.IncomingDocumentBaseCorrespondentValueInputEventArgs e)
    {
      base.CorrespondentValueInput(e);
      
      this._obj.State.Properties.Correspondent.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
      this._obj.State.Properties.Contact.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
      this._obj.State.Properties.SignedBy.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }
    
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired ||
        (_obj.DocumentKind != null &&
         (_obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Registrable ||
          _obj.DocumentKind.GenerateDocumentName == true));
      
      // Восстановить обязательность корреспондента.
      _obj.State.Properties.Correspondent.IsRequired = true;
      
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