using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.Module
{
  partial class PackageClassificationResult
  {
    // Гуид документа.
    public string DocumentGuid { get; set; }
    
    // Класс документа, присвоенный классификатором.
    public string DocumentClass { get; set; }
  }
}