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
    
    public const string TaxInvoiceClassName = "Счет-фактура";
    
    public const string TaxinvoiceCorrectionClassName = "Корректировочный счет-фактура";
    
    public const string UniversalTransferDocumentClassName = "Универсальный передаточный документ";
    
    public const string GeneralCorrectionDocumentClassName = "Универсальный корректировочный документ";
    
    public const string IncomingInvoiceClassName = "Входящий счет на оплату";
    
    public const string ContractClassName = "Договор";
    
    [Sungero.Core.Public]
    public const string PropertiesAlreadyColoredParamName = "PropertiesAlreadyColored";

    public const char PositionsDelimiter = '#';
    
    public const char PositionElementDelimiter = '|';
    
    public const char PropertyAndPositionDelimiter = '-';
    
    public const string GreenHighlightsColorCode = "#E3EFD0";
    
    public const string YellowHighlightsColorCode = "#FFFBCC";
    
    public const string RedHighlightsColorCode = "#FAC6B6";
    
    public static class Initialize
    {
      public static readonly Guid MockIncomingLetterKindGuid = Guid.Parse("E37D0916-7814-441E-84EF-904B7B643497");
      
      public static readonly Guid MockContractStatementKindGuid = Guid.Parse("C149F090-CE5C-4786-BACE-AA38BB51635A");
      
      public static readonly Guid MockWaybillKindGuid = Guid.Parse("75AB134F-B73B-4933-984F-0B08BCE699EF");
      
      public static readonly Guid MockIncomingTaxInvoiceGuid = Guid.Parse("69A7B4C2-7D6C-4F36-B760-B6D048EE40A4");
      
      public static readonly Guid MockIncomingInvoiceGuid = Guid.Parse("a2f02dd2-ae5b-4659-b9d1-cfcfa1f56734");
      
      public static readonly Guid MockContractGuid = Guid.Parse("3803bce4-67ff-4c9c-af63-7e305ae7ed69");
    }
    
    public static class CaptureSourceType
    {
      public const string Folder = "folder";
      
      public const string Mail = "mail";
    }
    
    public const string DocumentNumberingBySmartCaptureResultParamName = "DocumentNumberingBySmartCaptureResult";
    public const string FindByBarcodeParamName = "FindByBarcode";
    
  }
}