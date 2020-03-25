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
    
    public virtual string GetIncompatibleDocumentGroupsExcludedHint()
    {
      return Sungero.Examples.DocumentTemplates.Resources.IncompatibleCategoriesExcluded;
    }
    
    /// <summary>
    /// Показать сообщение Dialog.NotifyMessage через Reflection.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    [Public]
    public static void TryToShowNotifyMessage(string message)
    {
      var dialogs = Type.GetType("Sungero.Core.Dialogs, Sungero.Domain.ClientBase");
      if (dialogs != null)
        dialogs.InvokeMember("NotifyMessage", System.Reflection.BindingFlags.InvokeMethod, null, null, new string[1] { message });
    }
    
    /// <summary>
    /// Получить список групп документов, доступных для выбора в шаблоне.
    /// </summary>
    /// <returns>Список групп документов.</returns>
    public virtual List<IDocumentGroupBase> GetAvailableDocumentGroups()
    {
      var contractKinds = _obj.DocumentKinds.Select(k => k.DocumentKind).ToList();
      return Sungero.Contracts.PublicFunctions.ContractCategory.GetFilteredContractCategoris(contractKinds);
    }

  }
}