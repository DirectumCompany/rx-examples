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
    
    public static class MailBodyName
    {
      public const string Html = "body.html";
      
      public const string Txt = "body.txt";
    }
    
    public static class InputFiles
    {
      public const string InputFilesSectionName = "InputFilesSection";
      
      public const string FilesSectionName = "Files";
      
      public const string FileDescriptionTagName = "FileDescription";
      
      public const string FileNameTagName = "FileName";
    }
    
    public static class MailInstanceInfos
    {
      public const string CaptureInstanceInfoListSectionName = "CaptureInstanceInfoList";
      
      public const string MailCaptureInstanceInfoSectionName = "MailCaptureInstanceInfo";
      
      public const string SubjectTag = "Subject";
      
      public const string FromSectionName = "From";
      
      public const string FromSectionAddressTag = "Address";
      
      public const string FromSectionNameTag = "Name";
    }
    
    public static class ArioGrammarNames
    {
      public const string Letter = "Letter";
      
      public const string ContractStatement = "ContractStatement";
      
      public const string Waybill = "Waybill";
      
      public const string UniversalTransferDocument = "GeneralTransferDocument";
      
      public const string UniversalTransferCorrectionDocument = "GeneralCorrectionDocument";
      
      public const string TaxInvoice = "TaxInvoice";
      
      public const string TaxinvoiceCorrection = "TaxinvoiceCorrection";
      
      public const string IncomingInvoice = "IncomingInvoice";
      
      public const string Contract = "Contract";
    }
    
    public static class FieldNames
    {
      public static class Letter
      {
        public const string Addressee = "Addressee";
        
        public const string CorrespondentLegalForm = "CorrespondentLegalForm";
        
        public const string CorrespondentName = "CorrespondentName";
        
        public const string Date = "Date";
        
        public const string Number = "Number";
        
        public const string ResponseToDate = "ResponseToDate";
        
        public const string ResponseToNumber = "ResponseToNumber";
        
        public const string Subject = "Subject";
      }
      
      public static class LetterPerson
      {
        public const string Surname = "Surname";
        
        public const string Type = "Type";
      }
      
      public static class Counterparty
      {
        public const string LegalForm = "LegalForm";
        
        public const string Name = "Name";
        
        public const string TIN = "TIN";
        
        public const string TRRC = "TRRC";
        
        public const string CounterpartyType = "CounterpartyType";
        
        public const string SignatorySurname = "SignatorySurname";
        
        public const string SignatoryName = "SignatoryName";
        
        public const string SignatoryPatrn = "SignatoryPatrn";
      }
      
      public static class Counterparty
      {
        public const string TIN = "TIN";
        
        public const string TRRC = "TRRC";
      }
      
      public static class Document
      {
        public const string Date = "Date";
        
        public const string Number = "Number";
      }
      
      public static class DocumentAmount
      {
        public const string Amount = "Amount";
        
        public const string Currency = "Currency";
        
        public const string VatAmount = "VatAmount";
      }
      
      public static class FinancialDocument
      {
        public const string DocumentBaseName = "DocumentBaseName";
        
        public const string Date = "Date";
        
        public const string Number = "Number";
        
        public const string CorrectionDate = "CorrectionDate";
        
        public const string CorrectionNumber = "CorrectionNumber";
      }
      
      public static class Goods
      {
        public const string Name = "Name";
        
        public const string Count = "Count";
        
        public const string UnitName = "UnitName";
        
        public const string Price = "Price";
        
        public const string VatAmount = "VatAmount";
        
        public const string Amount = "Amount";
      }
    }
    
    public static class FactNames
    {
      public const string Letter = "Letter";
      
      public const string LetterPerson = "LetterPerson";
      
      public const string Counterparty = "Counterparty";
      
      public const string Document = "Document";
      
      public const string DocumentAmount = "DocumentAmount";
      
      public const string FinancialDocument = "FinancialDocument";
      
      public const string Goods = "Goods";
    }
  }
}