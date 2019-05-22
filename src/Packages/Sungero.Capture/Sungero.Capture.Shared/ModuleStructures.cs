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
  partial class RecognitedDocument
  {
    public int ClassificationResultId { get; set; }
    public string BodyGuid { get; set; }
    public string PredictedClass { get; set; }
    public List<Sungero.Capture.Structures.Module.Fact> Facts { get; set; }
  }
  
  /// <summary>
  /// Факт.
  /// </summary>
  partial class Fact
  {
    public string Name { get; set; }
    public List<Sungero.Capture.Structures.Module.FactField> Fields { get; set; }
  }
  
  /// <summary>
  /// Поле факта.
  /// </summary>
  partial class FactField
  {
    public string Name { get; set; }
    public string Value { get; set; }
    public decimal Probability { get; set; }
  }
}