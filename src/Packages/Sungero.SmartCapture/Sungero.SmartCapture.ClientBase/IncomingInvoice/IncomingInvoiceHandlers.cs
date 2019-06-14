using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingInvoice;

namespace Sungero.SmartCapture
{
  partial class IncomingInvoiceClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
       base.Refresh(e);
         
      // Восстановить обязательность.
      _obj.State.Properties.Counterparty.IsRequired = true;
      _obj.State.Properties.Number.IsRequired = true;
      _obj.State.Properties.Date.IsRequired = true;
      _obj.State.Properties.TotalAmount.IsRequired = true;
      _obj.State.Properties.Currency.IsRequired = true;
      
      // Контрагент не дб задизейблен, если незаполнен.
      if (_obj.Counterparty == null)
        _obj.State.Properties.Counterparty.IsEnabled = true;
    }

  }
}