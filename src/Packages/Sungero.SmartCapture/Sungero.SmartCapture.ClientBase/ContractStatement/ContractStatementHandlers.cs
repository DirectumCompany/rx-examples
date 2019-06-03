using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture
{
  partial class ContractStatementClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.Subject.IsRequired = _obj.Info.Properties.Subject.IsRequired ||
        (_obj.DocumentKind != null &&
         (_obj.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Registrable ||
          _obj.DocumentKind.GenerateDocumentName == true));
      
      // Восстановить обязательность контрагента.
      _obj.State.Properties.Counterparty.IsRequired = true;
      
      // Контрагент не дб задизейблен, если незаполнен.
      if (_obj.Counterparty == null)
        _obj.State.Properties.Counterparty.IsEnabled = true;
    }

  }
}