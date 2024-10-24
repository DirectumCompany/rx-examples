using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Contract;

namespace Sungero.Examples.Server
{
  partial class ContractFunctions
  {
    /// <summary>
    /// Преобразовать документ в PDF с простановкой отметок.
    /// </summary>
    /// <param name="versionId">ИД версии, на которую будут проставлены отметки.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfWithMarks(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия
      /// "Создать PDF-документ с отметками" для договоров с состоянием "Утверждено"
      /// добавляется отметка "Утверждено" на преобразованный PDF-документ.
      this.UpdateContractPaginalApproveMark();
      return base.ConvertToPdfWithMarks(versionId);
    }
    
    /// <summary>
    /// Получить отметку для договора.
    /// </summary>
    [Public]
    public virtual void UpdateContractPaginalApproveMark()
    {
      if (_obj.LastVersionApproved == true)
      {
        var mark = GetOrCreateMark(Constants.Contracts.Contract.PaginalApproveMarkKindSid);
        mark.XIndent = 0.3;
        mark.YIndent = -2;
        mark.Page = 0;
        mark.RotateAngle = 90;
        mark.Save();
      }
      else
      {
        var paginalApproveMark = GetVersionMarks(_obj.LastVersion.Id, Constants.Contracts.Contract.PaginalApproveMarkKindSid).SingleOrDefault();
        Docflow.PublicFunctions.Module.DeleteMark(_obj, paginalApproveMark);
      }
    }
    
    /// <summary>
    /// Получить отметку для договора с состоянием "Утверждено".
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    public virtual string GetApprovedMarkAsHtml(long versionId)
    {
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(_obj, versionId, false);
      var html = string.Empty;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.Contracts.Resources.HtmlMarkTemplatePaginalApprove;
        html = html.Replace("{version}", versionId.ToString());
        var employee = Sungero.Company.Employees.As(signature.Signatory);
        html = html.Replace("{approvedOrganization}", employee.Department.BusinessUnit.Name);
      }
      return html;
    }
  }
}