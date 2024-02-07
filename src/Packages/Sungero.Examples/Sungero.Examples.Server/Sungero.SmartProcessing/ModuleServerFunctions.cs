using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Module.SmartProcessing.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Определить ведущий документ в комплекте.
    /// </summary>
    /// <param name="documents">Комплект документов.</param>
    /// <returns>Ведущий документ.</returns>
    [Public]
    public virtual Sungero.Docflow.IOfficialDocument GetLeadingDocument(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var documentPriority = new Dictionary<Sungero.Docflow.IOfficialDocument, int>();
      var documentTypePriorities = Sungero.SmartProcessing.PublicFunctions.Module.GetPackageDocumentTypePriorities();
      int priority;
      foreach (var document in documents)
      {
        documentTypePriorities.TryGetValue(document.GetType(), out priority);
        documentPriority.Add(document, priority);
      }
      
      var leadingDocument = documentPriority
        .OrderByDescending(p => p.Value)
        .FirstOrDefault().Key;
      return leadingDocument;
    }
  }
}