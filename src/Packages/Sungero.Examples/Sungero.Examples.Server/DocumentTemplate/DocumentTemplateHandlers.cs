using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.DocumentTemplate;

namespace Sungero.Examples
{
  partial class DocumentTemplateFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      // TODO Shtina: Добавить фильтрацию по категории договора
      
      
      return query;
    }
  }

  partial class DocumentTemplateDocumentGroupsDocumentGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DocumentGroupsDocumentGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(g => Functions.DocumentTemplate.GetAvailableDocumentGroups(_root).Contains(g));
    }
  }

}