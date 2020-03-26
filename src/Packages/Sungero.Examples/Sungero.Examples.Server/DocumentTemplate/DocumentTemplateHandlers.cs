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
      // Фильтр при создании документа из шаблона.
      if (_createFromTemplateContext != null)
      {
        // Фильтр по состоянию.
        query = query.Where(d => d.Status == Status.Active);
        
        // Фильтр по критериям.
        if (Docflow.OfficialDocuments.Is(_createFromTemplateContext))
        {
          var document = Docflow.OfficialDocuments.As(_createFromTemplateContext);
          query = (IQueryable<T>)Functions.DocumentTemplate.FilterTemplatesByCriteria(query,
                                                                                      document,
                                                                                      document.DocumentKind,
                                                                                      document.BusinessUnit,
                                                                                      document.Department,
                                                                                      document.DocumentGroup,
                                                                                      true);
        }
      }
      else if (_filter != null)
      {
        // Фильтр в списке.
        // Фильтр по состоянию.
        if (_filter.Active != _filter.Closed)
          query = query.Where(d => _filter.Active && d.Status == Status.Active ||
                              _filter.Closed && d.Status == Status.Closed);
        
        // Фильтр по критериям.
        query = (IQueryable<T>)Functions.DocumentTemplate.FilterTemplatesByCriteria(query,
                                                                                    null,
                                                                                    _filter.DocumentKind,
                                                                                    _filter.BusinessUnit,
                                                                                    _filter.Department,
                                                                                    null,
                                                                                    false);
      }
      
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