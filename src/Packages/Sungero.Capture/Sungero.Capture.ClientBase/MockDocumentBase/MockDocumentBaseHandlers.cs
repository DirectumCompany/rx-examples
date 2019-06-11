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

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Точно распознанные свойства документа подсветить зелёным цветом, неточно - жёлтым.
      // Точно и неточно распознанные свойства получить с сервера отдельными вызовами метода из-за ограничений платформы.
      var exactlyRecognizedProperties = PublicFunctions.Module.Remote.GetRecognizedDocumentProperties(_obj, true);
      Functions.MockDocumentBase.HighlightProperties(_obj, exactlyRecognizedProperties, Sungero.Core.Colors.Highlights.Green);
      
      var notExactlyRecognizedProperties = PublicFunctions.Module.Remote.GetRecognizedDocumentProperties(_obj, false);
      Functions.MockDocumentBase.HighlightProperties(_obj, notExactlyRecognizedProperties, Sungero.Core.Colors.Highlights.Yellow);

    }

    public override void DocumentKindValueInput(Sungero.Docflow.Client.OfficialDocumentDocumentKindValueInputEventArgs e)
    {
    }

  }
}