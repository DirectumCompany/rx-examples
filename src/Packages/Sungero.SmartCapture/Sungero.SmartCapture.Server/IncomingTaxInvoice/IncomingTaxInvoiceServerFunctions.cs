using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.IncomingTaxInvoice;

namespace Sungero.SmartCapture.Server
{
  partial class IncomingTaxInvoiceFunctions
  {
    [Public]
    public override void FillProperties(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, Sungero.Company.IEmployee responsible, object additionalInfo)
    {
      var overrideStructure = (Capture.Structures.Module.OverrideStructure)additionalInfo;
      base.FillProperties(recognitionResult, responsible, overrideStructure.Parties);
      _obj.Note = overrideStructure.Note;
    }

  }
}