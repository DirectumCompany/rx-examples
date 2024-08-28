using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Examples.Module.Docflow.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      
      CreateApprovalRole(Sungero.Examples.ApprovalRole.Type.InitDepEmpl, Sungero.Examples.Module.Docflow.Resources.RoleInitiatorDepartmentEmployees);
      CreateCustomMarkKinds();
    }
    
    public static void CreateCustomMarkKinds()
    {
      InitializationLogger.Debug("Init: Create custom mark kinds.");
      CreateMarkKind(Sungero.Examples.IncomingInvoices.Resources.PaymentMarkName, Sungero.Examples.Constants.Contracts.IncomingInvoice.PaymentMarkKindSid,
                     Sungero.Examples.Constants.Contracts.IncomingInvoice.PaymentMarkKindClass, Sungero.Examples.Constants.Contracts.IncomingInvoice.PaymentMarkKindMethod);

      CreateMarkKind(Sungero.Examples.Memos.Resources.SignedMarkName, Sungero.Examples.Constants.Docflow.Memo.SignMarkKindSid,
                     Sungero.Examples.Constants.Docflow.Memo.SignMarkKindClass, Sungero.Examples.Constants.Docflow.Memo.SignMarkKindMethod);
      
      CreateMarkKind(Sungero.Examples.ContractualDocuments.Resources.PaginalApproveMarkName, Sungero.Examples.Constants.Contracts.Contract.PaginalApproveMarkKindSid,
                     Sungero.Examples.Constants.Contracts.Contract.PaginalApproveMarkKindClass, Sungero.Examples.Constants.Contracts.Contract.PaginalApproveMarkKindMethod);
      
      CreateMarkKind(Sungero.Examples.ContractualDocuments.Resources.PaginalApproveMarkName, Sungero.Examples.Constants.Contracts.SupAgreement.PaginalApproveMarkKindSid,
                     Sungero.Examples.Constants.Contracts.SupAgreement.PaginalApproveMarkKindClass, Sungero.Examples.Constants.Contracts.SupAgreement.PaginalApproveMarkKindMethod);

      var minutesMarkKind = CreateMarkKind(Sungero.Examples.Minuteses.Resources.MinutesMarkStamp, Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindGuid,
                                           Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindClass, Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindMethod);
      
      if (!minutesMarkKind.OnBlankPage.GetValueOrDefault())
      {
        minutesMarkKind.OnBlankPage = true;
        minutesMarkKind.Save();
      }
    }
  }
}