using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingLetter;

namespace Sungero.Capture.Shared
{
  partial class MockIncomingLetterFunctions
  {
    public override void FillName()
    {
      /* Имя в формате:
        <Вид документа> от <корреспондент> №<номер> от <дата> "<содержание>".
      */
     if (_obj.DocumentKind == null)
       return;
     
     var name = _obj.DocumentKind.ShortName;
     
     using (TenantInfo.Culture.SwitchTo())
     {
       if (_obj.Correspondent != null)
         name += Sungero.RecordManagement.IncomingLetters.Resources.CorrespondentFrom + _obj.Correspondent;
       
       if (!string.IsNullOrWhiteSpace(_obj.InNumber))
         name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.InNumber;
       
       if (_obj.Dated != null)
         name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Dated;
       
       if (!string.IsNullOrWhiteSpace(_obj.Subject))
         name += " \"" + _obj.Subject + "\"";
     }
     
     _obj.Name = name;
    }
  }
}