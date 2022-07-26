using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.ApprovalStage;

namespace Sungero.Examples.Shared
{
  partial class ApprovalStageFunctions
  {
    /// <summary>
    /// Получить список ролей, доступных для этого этапа.
    /// </summary>
    /// <returns>Список ролей.</returns>
    public override List<Enumeration?> GetPossibleRoles()
    {
      var roleTypes = base.GetPossibleRoles();
      
      if (_obj.StageType == Sungero.Examples.ApprovalStage.StageType.Approvers)
        roleTypes.Add(Sungero.Examples.ApprovalRole.Type.InitDepEmpl);
      
      return roleTypes;
    }
  }
}