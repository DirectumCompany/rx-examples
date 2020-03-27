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
    
  }
}