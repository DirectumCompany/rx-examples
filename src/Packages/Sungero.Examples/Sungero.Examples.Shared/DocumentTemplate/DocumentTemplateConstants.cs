using System;
using Sungero.Core;

namespace Sungero.Examples.Constants.Docflow
{
  public static class DocumentTemplate
  {
    // GUID для значения "Договор" у свойства Тип документа.
    [Sungero.Core.Public]
    public static readonly Guid ContractTypeGuid = Guid.Parse("f37c7e63-b134-4446-9b5b-f8811f6c9666");
  }
}