using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Contract;

namespace Sungero.Examples.Client
{
  partial class ContractActions
  {
    public virtual void OpenEntity1CSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var getHyperlinkResult = Sungero.Examples.PublicFunctions.Module.Remote.GetSyncEntity1CHyperlink(_obj, Constants.Module.ContractsExtEntityType);
      
      var hyperlink = getHyperlinkResult.Hyperlink;
      var errorMessage = getHyperlinkResult.ErrorMessage;
      
      if (!string.IsNullOrEmpty(hyperlink))
        Hyperlinks.Open(hyperlink);
      else if (!string.IsNullOrEmpty(errorMessage))
        e.AddWarning(errorMessage);
      else
        e.AddWarning(Examples.Resources.OpenRecord1CError);
    }

    public virtual bool CanOpenEntity1CSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }

}