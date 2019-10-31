using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.SmartProcessingSetting
{
  /// <summary>
  /// Классификатор.
  /// </summary>
  partial class Classifier
  {
    // ИД.
    public int Id { get; set; }
    
    // Наименование.
    public string Name { get; set; }
  }
}