using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class Module
  {
    public const string SimpleRelationRelationName = "Simple relation";
        
    public const string ArioUrlKey = "ArioUrl";
    
    public const string MinFactProbabilityKey = "MinFactProbability";
    
    public const string TrustedFactProbabilityKey = "TrustedFactProbability";
    
    public const string CaptureMockModeKey = "CaptureMockMode";
    
    public const string LetterClassName = "Письмо";
    
    public const string ContractStatementClassName = "Акт выполненных работ";
    
    public const string WaybillClassName = "Товарная накладная";
    
    public const string IncomingTaxInvoiceClassName = "Счет-фактура";
    
    public const string UniversalTransferDocumentClassName = "Универсальный передаточный документ";
    
    public const string IncomingInvoiceClassName = "Входящий счет на оплату";
    
    public static class Initialize
    {
      public static readonly Guid MockIncomingLetterKindGuid = Guid.Parse("E37D0916-7814-441E-84EF-904B7B643497");
      
      public static readonly Guid MockContractStatementKindGuid = Guid.Parse("C149F090-CE5C-4786-BACE-AA38BB51635A");
      
      public static readonly Guid MockWaybillKindGuid = Guid.Parse("75AB134F-B73B-4933-984F-0B08BCE699EF");
      
      public static readonly Guid MockIncomingTaxInvoiceGuid = Guid.Parse("69A7B4C2-7D6C-4F36-B760-B6D048EE40A4");
    }
  }
}