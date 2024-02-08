using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Module.RecordManagement.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Определить, нужно ли добавлять документ во вложения задачи.
    /// </summary>
    /// <param name="attachment">Вложения.</param>
    /// <param name="mainOfficialDocument">Выбранный главный документ.</param>
    /// <returns>True, если нужно.</returns>
    [Public]
    public static bool NeedToAttachDocument(Content.IElectronicDocument attachment, Sungero.Docflow.IOfficialDocument mainOfficialDocument)
    {
      if (Sungero.Docflow.ExchangeDocuments.Is(attachment))
        return false;
      return !mainOfficialDocument.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName).Any(ad => ad.Id == attachment.Id);
    }
  }
}