using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.ContractStatement;

namespace Sungero.SmartCapture
{
  partial class ContractStatementServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);            
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      
      // Попытаться пронумеровать документ, если он еще не пронумерован.
      if (_obj.RegistrationState != Docflow.OfficialDocument.RegistrationState.Registered)
      {
        if (Capture.PublicFunctions.Module.RegisterDocument(_obj))
          _obj.VerificationState = Docflow.OfficialDocument.VerificationState.Completed;
      }
    }

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      // Сохранить подтверждённые пользователем значения.
      Capture.PublicFunctions.Module.StoreVerifiedPropertiesValues(_obj);
    }
  }

}