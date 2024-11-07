using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;

namespace Sungero.Examples.Shared
{
  partial class IncomingInvoiceFunctions
  {
    public override List<string> GetAvailableMarkKindsSids()
    {
      var marksKinds = base.GetAvailableMarkKindsSids();
      
      if (_obj.LifeCycleState == Sungero.Contracts.IncomingInvoice.LifeCycleState.Paid)
        marksKinds.Add(Sungero.Examples.Constants.Contracts.IncomingInvoice.PaymentMarkKindSid);
      
      return marksKinds;
    }
    
    public override void UpdateMarksBeforeConversion(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия
      /// "Создать PDF-документ с отметками" для входящих счетов с состоянием "Оплачен"
      /// добавляется отметка "Оплачено" на преобразованный PDF-документ.
      Functions.IncomingInvoice.Remote.UpdateInvoicePaymentMark(_obj);
    }
  }
}