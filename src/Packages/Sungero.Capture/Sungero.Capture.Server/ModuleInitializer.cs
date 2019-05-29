using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Capture.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // TODO Временно для разработки.
      var CaptureMockMode = ModuleFunctions.GetDocflowParamsValue(Constants.Module.CaptureMockModeKey);
      if (CaptureMockMode != null)
        Functions.Module.InitCaptureMockMode();
    }
  }
}
