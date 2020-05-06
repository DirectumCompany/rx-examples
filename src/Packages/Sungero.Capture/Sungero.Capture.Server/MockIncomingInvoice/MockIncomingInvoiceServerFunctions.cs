using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingInvoice;

namespace Sungero.Capture.Server
{
  partial class MockIncomingInvoiceFunctions
  {
    #region Интеллектуальная обработка
    
    public override void FillDocumentDate(Sungero.Commons.IEntityRecognitionInfo recognitionInfo, Sungero.Docflow.Structures.Module.IRecognizedDocumentDate recognizedDate, string fieldName, string propertyName)
    {
      if (recognizedDate.Fact == null)
        return;
      
      _obj.Date = recognizedDate.Date;
      Docflow.PublicFunctions.Module.LinkFactAndProperty(recognitionInfo,
                                                         recognizedDate.Fact,
                                                         fieldName,
                                                         propertyName,
                                                         recognizedDate.Date,
                                                         recognizedDate.Probability);
    }
      
    #endregion
  }
}