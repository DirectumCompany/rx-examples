using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class Module
  {
    // Имя типа связи "Прочие".
    [Sungero.Core.Public]
    public const string SimpleRelationRelationName = "Simple relation";
    
    // Ключ параметра демо-режима.
    [Sungero.Core.Public]
    public const string CaptureMockModeKey = "CaptureMockMode";
    
    // Ключ параметра адреса сервиса Ario.
    [Sungero.Core.Public]
    public const string ArioUrlKey = "ArioUrl";
    
    // Ключ параметра минимально допустимой вероятности для поля факта, извлеченного Ario.
    // Факт с полем, вероятность которого ниже минимально допустимой, отбрасывается как недостоверный.
    [Sungero.Core.Public]
    public const string MinFactProbabilityKey = "MinFactProbability";
    
    // Ключ параметра вероятности для поля факта, извлеченного Ario, выше которой факт считается достоверным.
    [Sungero.Core.Public]
    public const string TrustedFactProbabilityKey = "TrustedFactProbability";
    
    // Наименования классов из классификатора Ario.
    public static class ArioClassNames
    {
      [Sungero.Core.Public]
      public const string Letter = "Письмо";
      
      [Sungero.Core.Public]
      public const string ContractStatement = "Акт выполненных работ";
      
      [Sungero.Core.Public]
      public const string Waybill = "Товарная накладная";
      
      [Sungero.Core.Public]
      public const string TaxInvoice = "Счет-фактура";
      
      [Sungero.Core.Public]
      public const string TaxinvoiceCorrection = "Корректировочный счет-фактура";
      
      [Sungero.Core.Public]
      public const string UniversalTransferDocument = "Универсальный передаточный документ";
      
      [Sungero.Core.Public]
      public const string UniversalTransferCorrectionDocument = "Универсальный корректировочный документ";
      
      [Sungero.Core.Public]
      public const string IncomingInvoice = "Входящий счет на оплату";
      
      [Sungero.Core.Public]
      public const string Contract = "Договор";
      
      [Sungero.Core.Public]
      public const string SupAgreement = "Дополнительное соглашение";
    }
    
    // Наименование правил для извлечения фактов Ario.
    public static class ArioGrammarNames
    {
      [Sungero.Core.Public]
      public const string Letter = "Letter";
      
      [Sungero.Core.Public]
      public const string ContractStatement = "ContractStatement";
      
      [Sungero.Core.Public]
      public const string Waybill = "Waybill";
      
      [Sungero.Core.Public]
      public const string UniversalTransferDocument = "GeneralTransferDocument";
      
      [Sungero.Core.Public]
      public const string UniversalTransferCorrectionDocument = "GeneralCorrectionDocument";
      
      [Sungero.Core.Public]
      public const string TaxInvoice = "TaxInvoice";
      
      [Sungero.Core.Public]
      public const string TaxinvoiceCorrection = "TaxinvoiceCorrection";
      
      [Sungero.Core.Public]
      public const string IncomingInvoice = "IncomingInvoice";
      
      [Sungero.Core.Public]
      public const string Contract = "Contract";
      
      [Sungero.Core.Public]
      public const string SupAgreement = "SupAgreement";
    }
    
    // Имя параметра: подсвечены ли свойства.
    [Sungero.Core.Public]
    public const string PropertiesAlreadyColoredParamName = "PropertiesAlreadyColored";
    
    // Имя параметра: удалось ли пронумеровать документ.
    [Sungero.Core.Public]
    public const string DocumentNumberingBySmartCaptureResultParamName = "DocumentNumberingBySmartCaptureResult";
    
    // Имя параметра: найден ли документ по штрихкоду.
    [Sungero.Core.Public]
    public const string FindByBarcodeParamName = "FindByBarcode";
    
    // Имя параметра: заблокирован ли документ.
    [Sungero.Core.Public]
    public const string DocumentIsLockedParamName = "DocumentIsLocked";
    
    [Sungero.Core.Public]
    public const char PositionsDelimiter = '#';
    
    [Sungero.Core.Public]
    public const char PositionElementDelimiter = '|';
    
    [Sungero.Core.Public]
    public const char PropertyAndPositionDelimiter = '-';
    
    // Коды цвета подсветки свойств.
    public static class PropertiesHighlightColorCodes
    {
      [Sungero.Core.Public]
      public const string Green = "#E3EFD0";
      
      [Sungero.Core.Public]
      public const string Yellow = "#FFFBCC";
      
      [Sungero.Core.Public]
      public const string Red = "#FAC6B6";
    }
    
    // Коды цвета подсветки полей в предпросмотре.
    public static class PreviewHighlightColorCodes
    {
      [Sungero.Core.Public]
      public const string Green = "#74B014";
      
      [Sungero.Core.Public]
      public const string Yellow = "#FFEA00";
    }
    
    // Наименования полей для фактов Ario.
    public static class FieldNames
    {
      public static class Letter
      {
        [Sungero.Core.Public]
        public const string Addressee = "Addressee";
        
        [Sungero.Core.Public]
        public const string CorrespondentLegalForm = "CorrespondentLegalForm";
        
        [Sungero.Core.Public]
        public const string CorrespondentName = "CorrespondentName";
        
        [Sungero.Core.Public]
        public const string Date = "Date";
        
        [Sungero.Core.Public]
        public const string Number = "Number";
        
        [Sungero.Core.Public]
        public const string ResponseToDate = "ResponseToDate";
        
        [Sungero.Core.Public]
        public const string ResponseToNumber = "ResponseToNumber";
        
        [Sungero.Core.Public]
        public const string Subject = "Subject";
      }
      
      public static class LetterPerson
      {
        [Sungero.Core.Public]
        public const string Surname = "Surname";
        
        [Sungero.Core.Public]
        public const string Type = "Type";
      }
      
      public static class Counterparty
      {
        [Sungero.Core.Public]
        public const string LegalForm = "LegalForm";
        
        [Sungero.Core.Public]
        public const string Name = "Name";
        
        [Sungero.Core.Public]
        public const string TIN = "TIN";
        
        [Sungero.Core.Public]
        public const string TRRC = "TRRC";
        
        [Sungero.Core.Public]
        public const string CounterpartyType = "CounterpartyType";
        
        [Sungero.Core.Public]
        public const string SignatorySurname = "SignatorySurname";
        
        [Sungero.Core.Public]
        public const string SignatoryName = "SignatoryName";
        
        [Sungero.Core.Public]
        public const string SignatoryPatrn = "SignatoryPatrn";
      }
      
      public static class Document
      {
        [Sungero.Core.Public]
        public const string Date = "Date";
        
        [Sungero.Core.Public]
        public const string Number = "Number";
      }
      
      public static class DocumentAmount
      {
        [Sungero.Core.Public]
        public const string Amount = "Amount";
        
        [Sungero.Core.Public]
        public const string Currency = "Currency";
        
        [Sungero.Core.Public]
        public const string VatAmount = "VatAmount";
      }
      
      public static class FinancialDocument
      {
        [Sungero.Core.Public]
        public const string DocumentBaseName = "DocumentBaseName";
        
        [Sungero.Core.Public]
        public const string DocumentBaseNumber = "DocumentBaseNumber";
        
        [Sungero.Core.Public]
        public const string DocumentBaseDate = "DocumentBaseDate";
        
        [Sungero.Core.Public]
        public const string Date = "Date";
        
        [Sungero.Core.Public]
        public const string Number = "Number";
        
        [Sungero.Core.Public]
        public const string CorrectionDate = "CorrectionDate";
        
        [Sungero.Core.Public]
        public const string CorrectionNumber = "CorrectionNumber";
      }
      
      public static class Goods
      {
        [Sungero.Core.Public]
        public const string Name = "Name";
        
        [Sungero.Core.Public]
        public const string Count = "Count";
        
        [Sungero.Core.Public]
        public const string UnitName = "UnitName";
        
        [Sungero.Core.Public]
        public const string Price = "Price";
        
        [Sungero.Core.Public]
        public const string VatAmount = "VatAmount";
        
        [Sungero.Core.Public]
        public const string Amount = "Amount";
      }
      
      public static class Person
      {
        [Sungero.Core.Public]
        public const string Surname = "Surname";
        
        [Sungero.Core.Public]
        public const string Name = "Name";
        
        [Sungero.Core.Public]
        public const string Patrn = "Patrn";
      }
    }
    
    // Наименования фактов Ario.
    public static class FactNames
    {
      [Sungero.Core.Public]
      public const string Letter = "Letter";
      
      [Sungero.Core.Public]
      public const string LetterPerson = "LetterPerson";
      
      [Sungero.Core.Public]
      public const string Counterparty = "Counterparty";
      
      [Sungero.Core.Public]
      public const string Document = "Document";
      
      [Sungero.Core.Public]
      public const string DocumentAmount = "DocumentAmount";
      
      [Sungero.Core.Public]
      public const string FinancialDocument = "FinancialDocument";
      
      [Sungero.Core.Public]
      public const string Goods = "Goods";
    }
    
    // Типы персоны: "Подписант", "Исполнитель".
    public static class LetterPersonTypes
    {
      [Sungero.Core.Public]
      public const string Signatory = "SIGNATORY";
      
      [Sungero.Core.Public]
      public const string Responsible = "RESPONSIBLE";
    }
    
    // Типы контрагента.
    public static class CounterpartyTypes
    {
      [Sungero.Core.Public]
      public const string Consignee = "CONSIGNEE";
      
      [Sungero.Core.Public]
      public const string Payer = "PAYER";
      
      [Sungero.Core.Public]
      public const string Shipper = "SHIPPER";
      
      [Sungero.Core.Public]
      public const string Supplier = "SUPPLIER";
      
      [Sungero.Core.Public]
      public const string Buyer = "BUYER";
      
      [Sungero.Core.Public]
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
      [Sungero.Core.Public]
      public const string Folder = "folder";
      
      [Sungero.Core.Public]
      public const string Mail = "mail";
    }
    
    // Наименования для тела письма с электронной почты.
    public static class MailBodyName
    {
      [Sungero.Core.Public]
      public const string Html = "body.html";
      
      [Sungero.Core.Public]
      public const string Txt = "body.txt";
    }
    
    // Названия тегов файла DeviceInfo.xml с информацией об устройствах ввода.
    public static class DeviceInfoTagNames
    {
      // Корневой узел для электронной почты.
      [Sungero.Core.Public]
      public const string MailSourceInfo = "MailSourceInfo";
      
      // Узел c информацией об отправляемых в конечную систему файлах.
      [Sungero.Core.Public]
      public const string Files = "Files";
      
      // Узел c именем файла без пути.
      [Sungero.Core.Public]
      public const string FileDescription = "FileDescription";
      
      // Узел c именем захваченного файла относительно папки InputFiles.
      [Sungero.Core.Public]
      public const string FileName = "FileName";
    }
    
    // Названия тегов файла InputFiles.xml с информацией об отправляемых в систему файлах службой DCS.
    public static class InputFilesTagNames
    {
      // Корневой узел.
      [Sungero.Core.Public]
      public const string InputFilesSection = "InputFilesSection";
      
      // Узел c информацией об отправляемых в конечную систему файлах.
      [Sungero.Core.Public]
      public const string Files = "Files";
      
      // Узел c именем файла без пути.
      [Sungero.Core.Public]
      public const string FileDescription = "FileDescription";
      
      // Узел c именем захваченного файла относительно папки InputFiles.
      [Sungero.Core.Public]
      public const string FileName = "FileName";
    }
    
    // Названия тегов файла InstanceInfos.xml с информацией об экземплярах ввода и о захваченных файлах службой DCS.
    public static class InstanceInfosTagNames
    {
      // Корневой узел для ввода из файловой системы.
      [Sungero.Core.Public]
      public const string CaptureInstanceInfoList = "CaptureInstanceInfoList";
      
      // Корневой узел для ввода с почтового сервера.
      [Sungero.Core.Public]
      public const string MailCaptureInstanceInfo = "MailCaptureInstanceInfo";
      
      // Узел c темой почтового сообщения, полученного по электронной почте.
      [Sungero.Core.Public]
      public const string Subject = "Subject";
      
      // Узел c информацией об отправителе письма.
      [Sungero.Core.Public]
      public const string From = "From";
      
      public static class FromTags
      {
        // Узел c адресом отправителя письма.
        [Sungero.Core.Public]
        public const string Address = "Address";
        
        // Узел c именем отправителя письма.
        [Sungero.Core.Public]
        public const string Name = "Name";
      }
    }
    
    // Html расширение.
    public static class HtmlExtension
    {
      [Sungero.Core.Public]
      public const string WithDot = ".html";
      
      [Sungero.Core.Public]
      public const string WithoutDot = "html";
    }
    
    // Pdf расширение.
    [Sungero.Core.Public]
    public const string PdfExtension = "pdf";
    
    // Html теги.
    public static class HtmlTags
    {
      [Sungero.Core.Public]
      public const string MaskForSearch = "<html";
      
      [Sungero.Core.Public]
      public const string StartTag = "<html>";
      
      [Sungero.Core.Public]
      public const string EndTag = "</html>";
    }
    
    
  }
}