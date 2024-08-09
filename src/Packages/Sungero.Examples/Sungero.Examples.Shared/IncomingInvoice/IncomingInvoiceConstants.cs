using System;
using Sungero.Core;

namespace Sungero.Examples.Constants.Contracts
{
  public static class IncomingInvoice
  {
    // Sid отметки "Оплачено".
    [Public]
    public const string PaymentMarkKindSid = "A89D9F46-9065-422A-B6CA-DED9D29EEDC7";
    
    // Полное имя класса, из которого вызывается метод получения отметки "Оплачено".
    [Public]
    public const string PaymentMarkKindClass = "Sungero.Examples.Server.IncomingInvoiceFunctions";
    
    // Имя метода получения отметки "Оплачено".
    [Public]
    public const string PaymentMarkKindMethod = "GetPaymentMarkAsHtml";
  }
}