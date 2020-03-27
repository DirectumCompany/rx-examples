using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Contracts;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow.DocumentKind;
using Sungero.Examples.DocumentTemplate;

namespace Sungero.Examples
{
  partial class DocumentTemplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var availableDocumentGroups = Functions.DocumentTemplate.GetAvailableDocumentGroups(_obj);
      _obj.State.Properties.DocumentGroups.IsEnabled = availableDocumentGroups.Any();
    }
  }

}