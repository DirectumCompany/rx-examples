using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Module.RecordManagementUI.Client
{
  partial class IncomingDocumentsFolderHandlers
  {

    public virtual void IncomingDocumentsValidateFilterPanel(Sungero.Domain.Client.ValidateFilterPanelEventArgs e)
    {
      if (_filter.ManualPeriod && (_filter.DateRangeFrom == null || _filter.DateRangeTo == null) && _filter.Counterparty == null && _filter.DocumentRegister == null)
        e.AddError(Resources.FilterPanelValidationError, _filter.Info.Counterparty, _filter.Info.DocumentRegister);
    }
  }


}