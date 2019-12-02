using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingLetter;

namespace Sungero.Examples
{
  partial class IncomingLetterServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }
  }


  partial class IncomingLetterInResponseToPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> InResponseToFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.InResponseToFiltering(query, e);
      
      return query;
    }
  }


}