using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Examples.DocumentTemplate;

namespace Sungero.Examples.Shared
{
  partial class DocumentTemplateFunctions
  {
    
    /// <summary>
    /// Получить список категорий договоров, доступных для выбора в шаблоне.
    /// </summary>
    /// <returns>Список категорий договоров.</returns>
    public virtual List<IDocumentGroupBase> GetAvailableDocumentGroups()
    {
      var kinds = _obj.DocumentKinds.Select(k => k.DocumentKind).ToList();
      var contractKinds = Docflow.PublicFunctions.DocumentKind.GetAvailableDocumentKinds(typeof(Contracts.IContractBase)).ToList();
      if (_obj.DocumentType == Constants.Docflow.DocumentTemplate.ContractTypeGuid && (!kinds.Any() || (kinds.Any() && kinds.All(k => contractKinds.Contains(k)))))
        return Contracts.PublicFunctions.ContractCategory.GetFilteredContractCategoris(kinds);
      return new List<IDocumentGroupBase>();
    }
    
    /// <summary>
    /// Очистить несовместимые категории договоров.
    /// </summary>
    public virtual void RemoveIncompatibleDocumentGroups()
    {
      var availableDocumentGroups = Functions.DocumentTemplate.GetAvailableDocumentGroups(_obj);
      var suitableDocumentGroups = _obj.DocumentGroups.Select(d => d.DocumentGroup).Where(dg => availableDocumentGroups.Contains(dg)).ToList();
      
      if (suitableDocumentGroups.Count < _obj.DocumentGroups.Count())
      {
        Docflow.PublicFunctions.Module.TryToShowNotifyMessage(Examples.DocumentTemplates.Resources.IncompatibleCategoriesExcluded);
        _obj.DocumentGroups.Clear();
        foreach (var documentGroup in suitableDocumentGroups)
          _obj.DocumentGroups.AddNew().DocumentGroup = documentGroup;
      }
      
      _obj.State.Properties.DocumentGroups.IsEnabled = availableDocumentGroups.Any();
    }
    
  }
}