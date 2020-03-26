using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.DocumentTemplate;

namespace Sungero.Examples
{

  partial class DocumentTemplateSharedHandlers
  {

    public override void DocumentTypeChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.DocumentTypeChanged(e);
      Functions.DocumentTemplate.RemoveIncompatibleDocumentGroups(_obj);
    }
    
    public override void DocumentKindsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.DocumentKindsChanged(e);
      Functions.DocumentTemplate.RemoveIncompatibleDocumentGroups(_obj);
    }

  }
}