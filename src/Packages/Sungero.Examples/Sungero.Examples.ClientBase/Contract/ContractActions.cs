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
    public virtual void OpenRecord1CSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var hyperlink = string.Empty;
      
      try
      {
        hyperlink = Sungero.Examples.PublicFunctions.Module.GetSyncEntity1CHyperlink(_obj);

        if (!string.IsNullOrEmpty(hyperlink))
          Hyperlinks.Open(hyperlink);
        else
          e.AddWarning("Нет связанной записи в 1С.");
      }
      catch (Exception ex)
      {
        e.AddWarning(ex.Message);
      }        
    }

    public virtual bool CanOpenRecord1CSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}