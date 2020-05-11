using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartProcessing.Structures.Module;
using Sungero.Company;
using Sungero.Docflow;

namespace Sungero.Examples.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать входящее письмо (пример использования доп. классификатора).
    /// </summary>
    /// <param name="arioDocument">Распознанный в Ario документ.</param>
    /// <param name="responsible">Ответственный за верификацию.</param>
    /// <returns>Входящее письмо.</returns>
    [Public]
    public virtual IOfficialDocument CreateIncomingLetter(IArioDocument arioDocument,
                                                          IEmployee responsible)
    {
      // Входящее письмо.
      var document = RecordManagement.IncomingLetters.Create();
      Sungero.SmartProcessing.PublicFunctions.Module.FillIncomingLetterProperties(document, arioDocument, responsible);
      
      // Доп. классификатор.
      var arioAdditionalClasses = arioDocument.AdditionalClasses;
      if (arioAdditionalClasses.Count > 0)
        document.Note = string.Format("Доп. класс = {0}", arioAdditionalClasses.FirstOrDefault().PredictedClass);
      
      return document;
    }
  }
}