using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class Module
  {
    // Имя типа связи "Прочие".
    public const string SimpleRelationRelationName = "Simple relation";
    
    // Ключ параметра адреса веб сервиса проверки контрагентов.
    public const string ArioUrlKey = "ArioUrl";
    
    // Ключ параметра минимально допустимой вероятности для поля факта, извлеченного Ario.
    // Факт с полем, вероятность которого ниже минимально допустимой, отбрасывается как недостоверный.
    public const string MinFactProbabilityKey = "MinFactProbability";
    
    // Ключ параметра вероятности для поля факта, извлеченного Ario, выше которой факт считается достоверным.
    public const string TrustedFactProbabilityKey = "TrustedFactProbability";
    
    // Ключ параметра демо-режима.
    public const string CaptureMockModeKey = "CaptureMockMode";
    
    // Наименования классов из классификатора Ario.
    public static class ArioClassNames
    {
      public const string Letter = "Письмо";
      
      public const string ContractStatement = "Акт выполненных работ";
      
      public const string Waybill = "Товарная накладная";
      
      public const string TaxInvoice = "Счет-фактура";
      
      public const string TaxinvoiceCorrection = "Корректировочный счет-фактура";
      
      public const string UniversalTransferDocument = "Универсальный передаточный документ";
      
      public const string UniversalTransferCorrectionDocument = "Универсальный корректировочный документ";
      
      public const string IncomingInvoice = "Входящий счет на оплату";
      
      public const string Contract = "Договор";
    }
    
    // Наименование правил для извлечения фактов Ario.
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
    
    // Имя параметра: подсвечены ли свойства.
    [Sungero.Core.Public]
    public const string PropertiesAlreadyColoredParamName = "PropertiesAlreadyColored";
    
    // Имя параметра: удалось ли пронумеровать документ.
    public const string DocumentNumberingBySmartCaptureResultParamName = "DocumentNumberingBySmartCaptureResult";
    
    // Имя параметра: найден ли документ по штрихкоду.
    public const string FindByBarcodeParamName = "FindByBarcode";
    
    public const char PositionsDelimiter = '#';
    
    public const char PositionElementDelimiter = '|';
    
    public const char PropertyAndPositionDelimiter = '-';
    
    // Коды цвета подсветки свойств.
    public static class HighlightsColors
    {
      public const string Green = "#E3EFD0";
      
      public const string Yellow = "#FFFBCC";
      
      public const string Red = "#FAC6B6";
    }
    
    // Наименования полей для фактов Ario.
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
    
    // Наименования фактов Ario.
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
    
    // Типы персоны: "Подписант", "Исполнитель".
    public static class LetterPersonTypes
    {
      public const string Signatory = "SIGNATORY";
      
      public const string Responsible = "RESPONSIBLE";
    }
    
    // Типы контрагента.
    public static class CounterpartyTypes
    {
      public const string Consignee = "CONSIGNEE";
      
      public const string Payer = "PAYER";
      
      public const string Shipper = "SHIPPER";
      
      public const string Supplier = "SUPPLIER";
      
      public const string Buyer = "BUYER";
      
      public const string Seller = "SELLER";
    }
    
    public static class Initialize
    {
      public static readonly Guid MockIncomingLetterKindGuid = Guid.Parse("E37D0916-7814-441E-84EF-904B7B643497");
      public static readonly Guid MockContractStatementKindGuid = Guid.Parse("C149F090-CE5C-4786-BACE-AA38BB51635A");
      public static readonly Guid MockWaybillKindGuid = Guid.Parse("75AB134F-B73B-4933-984F-0B08BCE699EF");
      public static readonly Guid MockIncomingTaxInvoiceGuid = Guid.Parse("69A7B4C2-7D6C-4F36-B760-B6D048EE40A4");
      public static readonly Guid MockIncomingInvoiceGuid = Guid.Parse("a2f02dd2-ae5b-4659-b9d1-cfcfa1f56734");
      public static readonly Guid MockContractGuid = Guid.Parse("3803bce4-67ff-4c9c-af63-7e305ae7ed69");
    }
    
    // Типы источников захвата.
    public static class CaptureSourceType
    {
      public const string Folder = "folder";
      
      public const string Mail = "mail";
    }
    
    // Наименования для тела письма с электронной почты.
    public static class MailBodyName
    {
      public const string Html = "body.html";
      
      public const string Txt = "body.txt";
    }
    
    // Названия тегов файла DeviceInfo.xml с информацией об устройствах ввода.
    public static class DeviceInfoTagNames
    {
      // Корневой узел для электронной почты.
      public const string MailSourceInfo = "MailSourceInfo";
      
      // Узел c информацией об отправляемых в конечную систему файлах.
      public const string Files = "Files";
      
      // Узел c именем файла без пути.
      public const string FileDescription = "FileDescription";
      
      // Узел c именем захваченного файла относительно папки InputFiles.
      public const string FileName = "FileName";
    }
    
    // Названия тегов файла InputFiles.xml с информацией об отправляемых в систему файлах службой DCS.
    public static class InputFilesTagNames
    {
      // Корневой узел.
      public const string InputFilesSection = "InputFilesSection";
      
      // Узел c информацией об отправляемых в конечную систему файлах.
      public const string Files = "Files";
      
      // Узел c именем файла без пути.
      public const string FileDescription = "FileDescription";
      
      // Узел c именем захваченного файла относительно папки InputFiles.
      public const string FileName = "FileName";
    }
    
    // Названия тегов файла InstanceInfos.xml с информацией об экземплярах ввода и о захваченных файлах службой DCS.
    public static class InstanceInfosTagNames
    {
      // Корневой узел для ввода из файловой системы.
      public const string CaptureInstanceInfoList = "CaptureInstanceInfoList";
      
      // Корневой узел для ввода с почтового сервера.
      public const string MailCaptureInstanceInfo = "MailCaptureInstanceInfo";
      
      // Узел c темой почтового сообщения, полученного по электронной почте.
      public const string Subject = "Subject";
      
      // Узел c информацией об отправителе письма.
      public const string From = "From";
      
      public static class FromTags
      {
        // Узел c адресом отправителя письма.
        public const string Address = "Address";
        
        // Узел c именем отправителя письма.
        public const string Name = "Name";
      }
    }
    
    // Html расширение.
    public static class HtmlExtension
    {
      public const string WithPeriod = ".html";
      
      public const string WithoutPeriod = "html";
    }
    
    // Pdf расширение.
    public const string PdfExtension = "pdf";
    
    // Html теги.
    public static class HtmlTags
    {
      public const string MaskForSearch = "<html";
      
      public const string StartTag = "<html>";
      
      public const string EndTag = "</html>";
    }
    
    
  }
}