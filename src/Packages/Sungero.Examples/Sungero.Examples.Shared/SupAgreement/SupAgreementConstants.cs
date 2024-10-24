using System;
using Sungero.Core;

namespace Sungero.Examples.Constants.Contracts
{
  public static class SupAgreement
  {
    // Sid отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindSid = "16d5e673-4444-4b09-aacf-36a7c3a30022";
    
    // Полное имя класса, из которого вызывается метод получения отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindClass = "Sungero.Examples.Functions.SupAgreement";
    
    // Имя метода получения отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindMethod = "GetApprovedMarkAsHtml";
  }
}