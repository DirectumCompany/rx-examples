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

      if (Guid.Parse(e.NewValue) != Constants.Docflow.DocumentTemplate.ContractTypeGuid)
        _obj.DocumentGroups.Clear();
      
      // Принудительно обновляем доступность поля категорий, т.к. в десктопе не всегда отрабатывает рефреш.
      var availableDocumentGroups = Functions.DocumentTemplate.GetAvailableDocumentGroups(_obj);
      _obj.State.Properties.DocumentGroups.IsEnabled = availableDocumentGroups.Any();
    }
    
    public override void DocumentKindsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.DocumentKindsChanged(e);
      
      var availableDocumentGroups = Functions.DocumentTemplate.GetAvailableDocumentGroups(_obj);
      var suitableDocumentGroups = _obj.DocumentGroups
        .Select(d => d.DocumentGroup)
        .Where(dg => availableDocumentGroups.Contains(dg));
      if (suitableDocumentGroups.Count() < _obj.DocumentGroups.Count())
        _obj.DocumentGroups.Clear();
      
      // Принудительно обновляем доступность поля категорий, т.к. в десктопе не всегда отрабатывает рефреш.
      _obj.State.Properties.DocumentGroups.IsEnabled = availableDocumentGroups.Any();
    }

  }
}