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
    /// Получить отметку для договора.
    /// </summary>
    [Public]
    public virtual void GetMarkForIncomingInvoiceDocument()
    {
      if (_obj.LastVersionApproved ?? false)
        this.CreateAndSaveMark(Sungero.Examples.PublicConstants.Contracts.Contract.ApprovedMarkKindSid, 0.3, -2, 90, 1);
    }
    
    /// <summary>
    /// Создать и сохранить отметку по заданным координатам.
    /// </summary>
    /// <param name="markKindSid">Сид отметки.</param>
    /// <param name="xIndent">Координата X.</param>
    /// <param name="yIndent">Координата Y.</param>
    /// <param name="rotation">Поворот.</param>
    /// <param name="page">Страница.</param>
    public virtual void CreateAndSaveMark(string markKindSid, double xIndent, double yIndent, int rotation, int page)
    {
      var mark = GetOrCreateMark(markKindSid);
      mark.XIndent = xIndent;
      mark.YIndent = yIndent;
      mark.Page = page;
      mark.Rotation = rotation;
      mark.Save();
    }
    
    /// <summary>
    /// Получить отметку для договора с состоянием "Утверждено".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    private static string GeApprovedMarkAsHtml(IOfficialDocument document, long versionId)
    {
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(document, versionId, false);
      string html;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.Contracts.Resources.HtmlMarkTemplateApprove;
        html = html.Replace("{version}", versionId.ToString());
        var employee = Sungero.Company.Employees.As(signature.Signatory);
        html = html.Replace("{approvedBy}", employee.Department.BusinessUnit.Name);
      }
      return html;
    }
  }
}