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
      /// "Создать PDF-документ с отметками" для входящих счетов с состоянием "Оплачен"
      /// добавляется отметка "Утверждено" на преобразованный PDF-документ.
      this.UpdateContractApprovedMark();
      return base.ConvertToPdfWithMarks(versionId);
    }
    
    /// <summary>
    /// Получить отметку для договора.
    /// </summary>
    [Public]
    public virtual void UpdateContractApprovedMark()
    {
      if (_obj.LastVersionApproved ?? false)
      {
        var mark = GetOrCreateMark(Constants.Contracts.Contract.ApprovedMarkKindSid);
        mark.XIndent = 0.3;
        mark.YIndent = -2;
        mark.Page = 1;
        mark.Rotation = 90;
        mark.Save();
      }
    }
    
    /// <summary>
    /// Получить отметку для договора с состоянием "Утверждено".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    private static string GetApprovedMarkAsHtml(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(document, versionId, false);
      var html = string.Empty;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.Contracts.Resources.HtmlMarkTemplateApprove;
        html = html.Replace("{version}", versionId.ToString());
        var employee = Sungero.Company.Employees.As(signature.Signatory);
        html = html.Replace("{approvedOrganization}", employee.Department.BusinessUnit.Name);
      }
      return html;
    }
  }
}