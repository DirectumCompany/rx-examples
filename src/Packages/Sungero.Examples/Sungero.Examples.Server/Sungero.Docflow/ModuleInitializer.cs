using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Examples.Module.Docflow.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      
      CreateApprovalRole(Sungero.Examples.ApprovalRole.Type.InitDepEmpl, Sungero.Examples.Module.Docflow.Resources.RoleInitiatorDepartmentEmployees);
    }
  }
}
