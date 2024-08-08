using System;
using Sungero.Core;

namespace Sungero.Examples.Constants.Contracts
{
  public static class Contract
  {
    // Sid отметки "Утверждено".
    [Public]
    public const string ApprovedMarkKindSid = "3cdb9932-708f-4079-bc50-890b700202c6";
    
    // Полное имя класса, из которого вызывается метод получения отметки "Утверждено".
    [Public]
    public const string ApprovedMarkKindClass = "Sungero.Examples.Server.ContractFunctions";
    
    // Имя метода получения отметки "Утверждено".
    [Public]
    public const string ApprovedMarkKindMethod = "GeApprovedMarkAsHtml";
  }
}