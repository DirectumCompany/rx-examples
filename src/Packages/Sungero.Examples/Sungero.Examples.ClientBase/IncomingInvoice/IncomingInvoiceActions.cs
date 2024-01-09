using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;

namespace Sungero.Examples.Client
{
  partial class IncomingInvoiceActions
  {
    public virtual void Open1CEntitySungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var getHyperlinkResult = Sungero.Examples.PublicFunctions.Module.Remote.GetIncomingInvoice1CHyperlink(_obj);
      
      var hyperlink = getHyperlinkResult.Hyperlink;
      var errorMessage = getHyperlinkResult.ErrorMessage;
      
      if (!string.IsNullOrEmpty(hyperlink))
        Hyperlinks.Open(hyperlink);
      else if (!string.IsNullOrEmpty(errorMessage))
        e.AddWarning(errorMessage);
      else
        e.AddWarning(Examples.Resources.OpenIncomingInvoice1CErrorNotFound);
    }

    public virtual bool CanOpen1CEntitySungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }


}