using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.ApprovalRole;

namespace Sungero.Examples.Server
{
  partial class ApprovalRoleFunctions
  {
    /// <summary>
    /// Получить сотрудников роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Список сотрудников.</returns>
    public override List<Sungero.Company.IEmployee> GetRolePerformers(Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.Type == Sungero.Examples.ApprovalRole.Type.InitDepEmpl)
        return this.GetInitiatorDepartmentEmployeesRolePerformers(task);
      
      return base.GetRolePerformers(task);
    }
    
    /// <summary>
    /// Получить сотрудников роли "Сотрудники подразделения инициатора".
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Список сотрудников.</returns>
    public virtual List<Sungero.Company.IEmployee> GetInitiatorDepartmentEmployeesRolePerformers(Sungero.Docflow.IApprovalTask task)
    {
      var initiator = Sungero.Company.Employees.As(task.Author);
      if (initiator == null)
        return new List<Sungero.Company.IEmployee>();
      
      return initiator.Department.RecipientLinks
        .Where(r => Sungero.Company.Employees.Is(r.Member))
        .Select(r => Sungero.Company.Employees.As(r.Member))
        .ToList();      
    }
  }
}