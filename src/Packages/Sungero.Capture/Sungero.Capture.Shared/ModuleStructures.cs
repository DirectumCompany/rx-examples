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
  partial class RecognizedDocument
  {
    public int ClassificationResultId { get; set; }
    public string BodyGuid { get; set; }
    public string PredictedClass { get; set; }
    public List<Sungero.Capture.Structures.Module.Fact> Facts { get; set; }
    public string Message { get; set; }
    public IDocumentRecognitionInfo Info { get; set; }
  }
  
  /// <summary>
  /// Факт.
  /// </summary>
  partial class Fact
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Sungero.Capture.Structures.Module.FactField> Fields { get; set; }
  }
  
  /// <summary>
  /// Поле факта.
  /// </summary>
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
    public Sungero.Capture.Structures.Module.Fact Fact { get; set; }
  }
  
  partial class CounterpartyWithFact
  {
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    public Sungero.Capture.Structures.Module.Fact Fact { get; set; }
    public bool IsTrusted { get; set; }
  }
  
  partial class BusinessUnitWithFact
  {
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    public Sungero.Capture.Structures.Module.Fact Fact { get; set; }
    public bool IsTrusted { get; set; }
  }
  
  partial class BusinessUnitAndCounterparty
  {
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    public bool? IsBusinessUnitSeller { get; set; }    
  }
  
}