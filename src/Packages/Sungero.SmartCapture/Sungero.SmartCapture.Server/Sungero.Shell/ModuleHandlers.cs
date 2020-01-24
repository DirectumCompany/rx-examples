using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.SmartCapture.Module.Shell.Server
{
  partial class OnDocumentProcessingFolderHandlers
  {

    public override IQueryable<Sungero.Workflow.IAssignmentBase> OnDocumentProcessingDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      // Фильтр по типу.
      var typeFilterEnabled = _filter != null && (_filter.ProcessResolution ||
                                                  _filter.ConfirmSigning ||
                                                  _filter.SendActionItem ||
                                                  _filter.Send ||
                                                  _filter.CheckReturn ||
                                                  _filter.ProcessPackage ||
                                                  _filter.Other);
      
      var stageTypes = new List<Sungero.Core.Enumeration>();
      if (!typeFilterEnabled || _filter.ProcessResolution)
        stageTypes.Add(Sungero.Docflow.ApprovalReviewAssignmentCollapsedStagesTypesRe.StageType.ReviewingResult);
      if (!typeFilterEnabled || _filter.ConfirmSigning)
        stageTypes.Add(Sungero.Docflow.ApprovalReviewAssignmentCollapsedStagesTypesRe.StageType.ConfirmSign);
      if (!typeFilterEnabled || _filter.SendActionItem)
        stageTypes.Add(Sungero.Docflow.ApprovalReviewAssignmentCollapsedStagesTypesRe.StageType.Execution);
      if (!typeFilterEnabled || _filter.Send)
        stageTypes.Add(Sungero.Docflow.ApprovalReviewAssignmentCollapsedStagesTypesRe.StageType.Sending);
      
      var showExecution = !typeFilterEnabled || _filter.SendActionItem;
      var showCheckReturn = !typeFilterEnabled || _filter.CheckReturn;
      var showProcessPackage = !typeFilterEnabled || _filter.ProcessPackage;
      var showOther = !typeFilterEnabled || _filter.Other;
      
      var checkPackageSubjectRu = string.Empty;
      var checkDocumentSubjectRu = string.Empty;
      var checkPackageSubjectEn = string.Empty;
      var checkDocumentSubjectEn = string.Empty;
      using (Sungero.Core.CultureInfoExtensions.SwitchTo(System.Globalization.CultureInfo.GetCultureInfo("ru-RU")))
      {
        checkPackageSubjectRu = Sungero.Capture.Resources.CheckPackage;
        checkDocumentSubjectRu = Sungero.Capture.Resources.CheckDocument;
      }
      using (Sungero.Core.CultureInfoExtensions.SwitchTo(System.Globalization.CultureInfo.GetCultureInfo("en-US")))
      {
        checkPackageSubjectEn = Sungero.Capture.Resources.CheckPackage;
        checkDocumentSubjectEn = Sungero.Capture.Resources.CheckDocument;
      }
      
      var result = query.Where(q =>
                               // Рассмотрение.
                               Sungero.Docflow.ApprovalReviewAssignments.Is(q) && Sungero.Docflow.ApprovalReviewAssignments.As(q).CollapsedStagesTypesRe.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                               // Подписание.
                               Sungero.Docflow.ApprovalSigningAssignments.Is(q) && Sungero.Docflow.ApprovalSigningAssignments.As(q).CollapsedStagesTypesSig.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                               // Создание поручений.
                               (Sungero.Docflow.ApprovalExecutionAssignments.Is(q) && Sungero.Docflow.ApprovalExecutionAssignments.As(q).CollapsedStagesTypesExe.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                                showExecution && Sungero.RecordManagement.ReviewResolutionAssignments.Is(q)) ||
                               // Подготовка проекта резолюции
                               showExecution && Sungero.RecordManagement.PreparingDraftResolutionAssignments.Is(q) ||
                               // Регистрация.
                               Sungero.Docflow.ApprovalRegistrationAssignments.Is(q) && Sungero.Docflow.ApprovalRegistrationAssignments.As(q).CollapsedStagesTypesReg.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                               // Печать.
                               Sungero.Docflow.ApprovalPrintingAssignments.Is(q) && Sungero.Docflow.ApprovalPrintingAssignments.As(q).CollapsedStagesTypesPr.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                               // Отправка контрагенту.
                               Sungero.Docflow.ApprovalSendingAssignments.Is(q) && Sungero.Docflow.ApprovalSendingAssignments.As(q).CollapsedStagesTypesSen.Any(s => stageTypes.Contains(s.StageType.Value)) ||
                               // Контроль возврата.
                               showCheckReturn && Sungero.Docflow.ApprovalCheckReturnAssignments.Is(q) ||
                               // Проверка комплектов документов.
                               showProcessPackage && (q.Subject.Contains(checkPackageSubjectRu) ||
                                                      q.Subject.Contains(checkDocumentSubjectRu) ||
                                                      q.Subject.Contains(checkPackageSubjectEn) ||
                                                      q.Subject.Contains(checkDocumentSubjectEn)) ||
                               // Прочие задания.
                               showOther && (Sungero.Docflow.ApprovalSimpleAssignments.Is(q) || Sungero.Docflow.ApprovalCheckingAssignments.Is(q)));
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                   _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      return result;
    }
  }

  partial class ShellHandlers
  {
  }
}