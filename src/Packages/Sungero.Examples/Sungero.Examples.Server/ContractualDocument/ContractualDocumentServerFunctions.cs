using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.ContractualDocument;

namespace Sungero.Examples.Server
{
  partial class ContractualDocumentFunctions
  {
    /// <summary>
    /// Преобразовать документ в PDF с простановкой отметок.
    /// </summary>
    /// <param name="versionId">ИД версии, на которую будут проставлены отметки.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfWithMarks(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия
      /// "Создать PDF-документ с отметками" для договорных документов с состоянием "Утверждено"
      /// добавляется отметка "Утверждено" на преобразованный PDF-документ.
      this.UpdateContractPaginalApproveMark();
      return base.ConvertToPdfWithMarks(versionId);
    }
    
    /// <summary>
    /// Получить отметку для договорных документов.
    /// </summary>
    [Public]
    public virtual void UpdateContractPaginalApproveMark()
    {
      var markKindSid = Contracts.Is(_obj) ? Sungero.Examples.PublicConstants.Contracts.Contract.PaginalApproveMarkKindSid :
        Sungero.Examples.PublicConstants.Contracts.SupAgreement.PaginalApproveMarkKindSid;
      if (_obj.LastVersionApproved == true)
      {
        var mark = GetOrCreateMark(markKindSid);
        mark.XIndent = 0.3;
        mark.YIndent = -2;
        mark.Page = 0;
        mark.RotateAngle = 90;
        mark.Save();
      }
      else
      {
        var paginalApproveMark = GetVersionMarks(_obj.LastVersion.Id, markKindSid).SingleOrDefault();
        Docflow.PublicFunctions.Module.DeleteMark(_obj, paginalApproveMark);
      }
    }
    
    /// <summary>
    /// Получить отметку для договорных документов с состоянием "Утверждено".
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    [Public]
    public virtual string GetContractualApprovedMarkAsHtml(long versionId)
    {
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(_obj, versionId, false);
      var html = string.Empty;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.ContractualDocuments.Resources.HtmlMarkTemplatePaginalApprove;
        html = html.Replace("{version}", versionId.ToString());
        var employee = Sungero.Company.Employees.As(signature.Signatory);
        html = html.Replace("{approvedOrganization}", employee.Department.BusinessUnit.Name);
      }
      return html;
    }
  }
}