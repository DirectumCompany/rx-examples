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
    public decimal Probability { get; set; }
  }
  
  partial class MockCounterparty
  {
    public string Name { get; set; }
    public string Tin { get; set; }
    public string Trrc { get; set; }
  }
  
}