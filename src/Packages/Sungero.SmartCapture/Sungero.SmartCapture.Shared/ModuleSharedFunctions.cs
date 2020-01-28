using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.SmartCapture.Shared
{
  public class ModuleFunctions
  {
    // Dmitriev_IA: EnableRegistrationNumberAndDate() вынесен сюда для использования в перекрытых типах.
    
    /// <summary>
    /// Сделать доступными рег. номер и рег. дату незарегистрированного документа регистрируемого вида в процессе верификации.
    /// </summary>
    /// <param name="document">Документ.</param>
    public static void EnableRegistrationNumberAndDate(Docflow.IOfficialDocument document)
    {
      if (document.DocumentKind == null || document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.NotNumerable)
        return;
      
      var isRegistrable = document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Registrable;
      var isNumerable = document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable;
      var verificationInProcess = document.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess;
      if (!verificationInProcess)
        return;
      
      var properties = document.State.Properties;
      if (isNumerable ||
          isRegistrable && document.RegistrationState == Docflow.OfficialDocument.RegistrationState.NotRegistered)
      {
        properties.RegistrationNumber.IsEnabled = true;
        properties.RegistrationDate.IsEnabled = true;
      }
    }
  }
}