using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.UniversalTransferDocument;

namespace Sungero.SmartCapture
{
  partial class UniversalTransferDocumentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
         
      // Восстановить обязательность контрагента.
      _obj.State.Properties.Counterparty.IsRequired = true;
    }

  }
}