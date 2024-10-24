using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Contract;

namespace Sungero.Examples.Server
{
  partial class ContractFunctions
  {
    /// <summary>
    /// Получить отметку для договорных документов с состоянием "Утверждено".
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    public virtual string GetApprovedMarkAsHtml(long versionId)
    {
      return PublicFunctions.ContractualDocument.GetContractualApprovedMarkAsHtml(_obj, versionId);
    }
  }
}