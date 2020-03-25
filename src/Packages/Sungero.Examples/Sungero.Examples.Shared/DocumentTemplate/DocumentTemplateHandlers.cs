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
    
    public override void DocumentKindsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.DocumentKindsChanged(e);
      
      var availableDocumentGroups = Functions.DocumentTemplate.GetAvailableDocumentGroups(_obj);
      var suitableDocumentGroups = _obj.DocumentGroups.Select(d => d.DocumentGroup).Where(dg => availableDocumentGroups.Contains(dg)).ToList();
      
      if (suitableDocumentGroups.Count < _obj.DocumentGroups.Count())
      {
        Functions.DocumentTemplate.TryToShowNotifyMessage(Functions.DocumentTemplate.GetIncompatibleDocumentGroupsExcludedHint(_obj));
        _obj.DocumentGroups.Clear();
        foreach (var documentGroup in suitableDocumentGroups)
          _obj.DocumentGroups.AddNew().DocumentGroup = documentGroup;
      }
      
      _obj.State.Properties.DocumentGroups.IsEnabled = availableDocumentGroups.Any();
    }

  }
}