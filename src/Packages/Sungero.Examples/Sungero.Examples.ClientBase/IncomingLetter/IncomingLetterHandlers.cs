using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingLetter;

namespace Sungero.Examples
{
  partial class IncomingLetterFilteringClientHandler
  {

    public override void ValidateFilterPanel(Sungero.Domain.Client.ValidateFilterPanelEventArgs e)
    {
      base.ValidateFilterPanel(e);
      
      if (_filter.ManualPeriod && (_filter.DateRangeFrom == null || _filter.DateRangeTo == null) && _filter.Counterparty == null && _filter.DocumentRegister == null)
        e.AddError(Sungero.Examples.Module.RecordManagementUI.Resources.FilterPanelValidationError, _filter.Info.Counterparty, _filter.Info.DocumentRegister);
    }
  }

  partial class IncomingLetterClientHandlers
  {

  }
}