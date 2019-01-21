using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;

namespace Sungero.Examples.Server
{
  partial class IncomingInvoiceFunctions
  {
    /// <summary>
    /// Получить отметку "Принят к оплате".
    /// </summary>
    /// <returns>Строка в формате html.</returns>
    public override string GetSignatureMarkAsHtml(int versionId)
    {
      return Examples.IncomingInvoices.Resources.HtmlStampTemplate;
    }
  }
}