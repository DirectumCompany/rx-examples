using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ContractsExample.ApprovalCreateIncInvoice1CStage;

namespace Sungero.ContractsExample.Server
{
  partial class ApprovalCreateIncInvoice1CStageFunctions
  {

    /// <summary>
    /// Создание входящего счета в 1С.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения кода.</returns>
    public override Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      Logger.DebugFormat("ApprovalCreateIncInvoice1CStage. Start create incoming invoice in 1C, approval task (ID={0}) (StartId={1}) (Iteration={2}) (StageNumber={3}).",
                         approvalTask.Id, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber);
      
      var mainDocument = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (mainDocument == null)
      {
        Logger.ErrorFormat("ApprovalCreateIncInvoice1CStage. Primary document not found. Approval task (ID={0}) (StartId={1}) (Iteration={2}) (StageNumber={3}).",
                           approvalTask.Id, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber);
        return this.GetErrorResult(Docflow.Resources.PrimaryDocumentNotFoundError);
      }
      
      var invoices = this.GetIncomingInvoicesForSyncTo1C(approvalTask);
      if (!invoices.Any())
      {
        Logger.DebugFormat("ApprovalCreateIncInvoice1CStage. Incoming invoices not found. Approval task (ID={0}).", approvalTask.Id);
        return this.GetSuccessResult();
      }
      
      var needRetry = false;
      foreach (var invoice in invoices)
      {
        try
        {
          var created = Sungero.Examples.PublicFunctions.Module.CreateIncomingInvoice1C(invoice);
          if (created)
            Logger.DebugFormat("ApprovalCreateIncInvoice1CStage. Created incoming invoice in 1C. Approval task (ID={0}), Document (ID={1}).", approvalTask.Id, invoice.Id);
          else
            Logger.DebugFormat("ApprovalCreateIncInvoice1CStage. Not created incoming invoice in 1C. Approval task (ID={0}), Document (ID={1}).", approvalTask.Id, invoice.Id);
        }
        catch (Exception ex)
        {
          needRetry = true;
          Logger.ErrorFormat("ApprovalCreateIncInvoice1CStage. Error while creating incoming invoice in 1C. Approval task (ID={0}) (Iteration={1}) (StageNumber={2}) for document (ID={3})",
                             ex, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber, invoice.Id);
        }
      }
      
      if (needRetry)
        return this.GetRetryResult(string.Empty);
      
      return this.GetSuccessResult();
    }

    /// <summary>
    /// Получить входящие счета для синхронизации в 1С.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Список входящих счетов.</returns>
    /// <remarks>Получает основной документ задачи на согласование, если это входящий счет.
    /// Получает все входящие счета из группы "Приложения", но только со статусом "Принят к оплате".
    /// Счета из группы "Дополнительно" игнорируются.</remarks>
    [Public]
    public virtual List<Sungero.Examples.IIncomingInvoice> GetIncomingInvoicesForSyncTo1C(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = new List<Sungero.Examples.IIncomingInvoice>();
      
      var mainDocument = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (mainDocument != null && Sungero.Examples.IncomingInvoices.Is(mainDocument))
        result.Add(Sungero.Examples.IncomingInvoices.As(mainDocument));
      
      var addendaInvoices = approvalTask.AddendaGroup.OfficialDocuments
        .Where(x => Sungero.Examples.IncomingInvoices.Is(x) && x.LifeCycleState == Sungero.Contracts.IncomingInvoice.LifeCycleState.Active)
        .Select(x => Sungero.Examples.IncomingInvoices.As(x));
      result.AddRange(addendaInvoices);
      
      return result;
    }    
  }
}