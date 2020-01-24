using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.SmartCapture.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Удалить параметр NeedValidateRegisterFormat.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="e">Аргумент действия.</param>
    public static void RemoveNeedValidateRegisterFormatParameter(Sungero.Docflow.IOfficialDocument document,
                                                                 Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если документ в процессе верификации, то игнорировать изменение полей регистрационных данных.
      if (document.VerificationState == Sungero.Docflow.OfficialDocument.VerificationState.InProcess)
        e.Params.Remove(Sungero.Docflow.Constants.OfficialDocument.NeedValidateRegisterFormat);
    }
  }
}