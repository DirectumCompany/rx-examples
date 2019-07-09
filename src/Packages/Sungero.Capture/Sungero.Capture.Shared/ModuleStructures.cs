using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.Module
{
  /// <summary>
  /// Распознанный в Ario документ.
  /// </summary>
  [Public]
  partial class RecognizedDocument
  {
    public int ClassificationResultId { get; set; }
    public string BodyGuid { get; set; }
    public string PredictedClass { get; set; }
    public List<Sungero.Capture.Structures.Module.IFact> Facts { get; set; }
    public string Message { get; set; }
    public IDocumentRecognitionInfo Info { get; set; }
    public Sungero.Capture.Structures.Module.IFileInfo OriginalFile { get; set; }
  }
  
  [Public]
  partial class FileInfo
  {
    public byte[] Data { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
  }
  
  /// <summary>
  /// Факт.
  /// </summary>
  [Public]
  partial class Fact
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Sungero.Capture.Structures.Module.IFactField> Fields { get; set; }
  }
  
  /// <summary>
  /// Поле факта.
  /// </summary>
  [Public]
  partial class FactField
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public double Probability { get; set; }
  }
  
  partial class MockCounterparty
  {
    public string Name { get; set; }
    public string Tin { get; set; }
    public string Trrc { get; set; }
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
  }
  
  partial class CounterpartyWithFact
  {
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    public bool IsTrusted { get; set; }
  }
  
  partial class ContactWithFact
  {
    public Sungero.Parties.IContact Contact { get; set; }
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    public bool IsTrusted { get; set; }
  }
  
  partial class BusinessUnitWithFact
  {
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    public bool IsTrusted { get; set; }
  }
  
  partial class BusinessUnitAndCounterparty
  {
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    public bool? IsBusinessUnitSeller { get; set; }
  }

  partial class BusinessUnitAndCounterpartyWithFact
  {
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    public string Type { get; set; }
    public bool IsTrusted { get; set; }
  }

  
  /// <summary>
  /// Файлы захваченного письма.
  /// </summary>
  partial class CapturedMailFiles
  {
    public Sungero.Capture.Structures.Module.IFileInfo Body { get; set; }
    public List<Sungero.Capture.Structures.Module.IFileInfo> Attachments { get; set; }
  }
  
  /// <summary>
  /// Информация о захваченном письме.
  /// </summary>
  partial class CapturedMailInfo
  {
    public string Name { get; set; }
    public string FromEmail { get; set; }
    public string Subject { get; set; }
  }
  
  /// <summary>
  /// Результат классификации и распознования документа.
  /// </summary>
  partial class ClassificationAndExtractionResult
  {
    public string Result { get; set; }
    public string Error { get; set; }
  }
  
  /// <summary>
  /// Документы, созданные по результатам распознавания.
  /// </summary>
  partial class DocumentsCreatedByRecognitionResults
  {
    public int LeadingDocumentId { get; set; }
    public List<int> RelatedDocumentIds { get; set; }
  }
}