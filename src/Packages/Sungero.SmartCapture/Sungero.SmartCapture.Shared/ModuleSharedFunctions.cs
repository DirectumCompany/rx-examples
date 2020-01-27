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
      if (document.DocumentKind == null || document.DocumentKind.NumberingType != Sungero.Docflow.DocumentKind.NumberingType.Registrable)
        return;
      
      var verificationInProcess = document.VerificationState == Docflow.OfficialDocument.VerificationState.InProcess;
      var properties = document.State.Properties;
      if (document.RegistrationState == Docflow.OfficialDocument.RegistrationState.NotRegistered)
      {
        properties.RegistrationNumber.IsEnabled = verificationInProcess;
        properties.RegistrationDate.IsEnabled = verificationInProcess;
      }
    }
  }
}