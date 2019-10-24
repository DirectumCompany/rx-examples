using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartCapture.Contract;

namespace Sungero.SmartCapture
{
  partial class ContractClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Поля Содержание и Контрагент обязательны для заполнения.
      _obj.State.Properties.Subject.IsRequired = true;
      _obj.State.Properties.Counterparty.IsRequired = true;
      
      // Поле Категория обязательна для заполнения, если на вид документа создана хотя бы одна категория.
      var hasAvailableCategories = Docflow.DocumentGroupBases.GetAllCached(g => g.Status == CoreEntities.DatabookEntry.Status.Active &&
                                                                           g.DocumentKinds.Any(d => Equals(d.DocumentKind, _obj.DocumentKind))).Any();
      _obj.State.Properties.DocumentGroup.IsRequired = _obj.DocumentKind != null && hasAvailableCategories;
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Sungero.Capture.PublicFunctions.Module.SwitchVerificationMode(_obj);
    }

  }
}