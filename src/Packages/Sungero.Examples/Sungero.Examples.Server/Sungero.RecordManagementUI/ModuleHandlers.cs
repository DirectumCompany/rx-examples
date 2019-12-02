using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Module.RecordManagementUI.Server
{
  partial class IncomingDocumentsFolderHandlers
  {

    public virtual IQueryable<Sungero.Docflow.IIncomingDocumentBase> IncomingDocumentsDataQuery(IQueryable<Sungero.Docflow.IIncomingDocumentBase> query)
    {      
      if (_filter == null)
        return query;
      
      
      
      // Фильтр по журналу регистрации.
      if (_filter.DocumentRegister != null)
        query = query.Where(d => Equals(d.DocumentRegister, _filter.DocumentRegister));
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => Equals(d.DocumentKind, _filter.DocumentKind));
      
      // Фильтр по статусу. Если все галочки включены, то нет смысла добавлять фильтр.
      if (_filter.Registered != _filter.NotRegistered)
        query = query
          .Where(d => _filter.Registered && d.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered ||
                 _filter.NotRegistered && d.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.NotRegistered);
      
      // Фильтр по контрагенту.
      if (_filter.Counterparty != null)
        query = query.Where(d => Equals(d.Correspondent, _filter.Counterparty));
      
      // Фильтр "Подразделение".
      if (_filter.Department != null)
        query = query.Where(c => Equals(c.Department, _filter.Department));
      
      // Фильтр по интервалу времени
      var periodBegin = Calendar.UserToday.AddDays(-7);
      var periodEnd = Calendar.UserToday.EndOfDay();
      
      if (_filter.LastWeek)
        periodBegin = Calendar.UserToday.AddDays(-7);
      
      if (_filter.LastMonth)
        periodBegin = Calendar.UserToday.AddDays(-30);
      
      if (_filter.Last90Days)
        periodBegin = Calendar.UserToday.AddDays(-90);
      
      if (_filter.ManualPeriod)
      {
        periodBegin = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
        periodEnd = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
      }

      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin) ? periodBegin : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(j => (j.DocumentDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                j.DocumentDate == periodBegin) && j.DocumentDate != clientPeriodEnd);
      
      return query;
    }
  }

  partial class RecordManagementUIHandlers
  {
  }
}