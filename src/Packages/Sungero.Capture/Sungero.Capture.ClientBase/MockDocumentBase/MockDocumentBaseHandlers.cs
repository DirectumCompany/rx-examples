using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockDocumentBase;

namespace Sungero.Capture
{
  partial class MockDocumentBaseClientHandlers
  {

    public override void RegistrationDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.RegistrationDateValueInput(e);
      
      this._obj.State.Properties.RegistrationDate.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void RegistrationNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.RegistrationNumberValueInput(e);
      
      this._obj.State.Properties.RegistrationNumber.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void SubjectValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.SubjectValueInput(e);
      
      this._obj.State.Properties.Subject.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
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

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      PublicFunctions.Module.SetPropertiesColors(_obj);
    }

    public override void DocumentKindValueInput(Sungero.Docflow.Client.OfficialDocumentDocumentKindValueInputEventArgs e)
    {
    }

  }
}