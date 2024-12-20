using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingLetter;

namespace Sungero.Examples.Server
{
  partial class IncomingLetterFunctions
  {
    /// <summary>
    /// Получить отметку о регистрации.
    /// </summary>
    /// <returns>Изображение отметки о регистрации в виде html.</returns>
    [Public]
    public override string GetRegistrationStampAsHtml(long versionId)
    {
      var regNumber = _obj.RegistrationNumber;
      var regDate = _obj.RegistrationDate;
      var businessUnit = _obj.BusinessUnit.Name;
      var department = _obj.Department.Name;
      
      if (regNumber == null || regDate == null)
        return string.Empty;
      
      string html;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = IncomingLetters.Resources.HtmlStampTemplateForRegistrationMarkCustom;
        html = html.Replace("{RegNumber}", regNumber.ToString());
        html = html.Replace("{RegDate}", regDate.Value.ToShortDateString());
        html = html.Replace("{BusinessUnit}", businessUnit);
        html = html.Replace("{Department}", department.ToUpper());
      }
      
      return html;
    }
    
    /// <summary>
    /// Преобразовать документ в PDF с наложением отметки о поступлении в новую версию.
    /// </summary>
    /// <param name="rightIndent">Значение отступа справа.</param>
    /// <param name="bottomIndent">Значение отступа снизу.</param>
    /// <returns>Результат преобразования.</returns>
    [Remote]
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult AddRegistrationStamp(double rightIndent, double bottomIndent)
    {
      return base.AddRegistrationStamp(5, 1);
    }
    
    /// <summary>
    /// Создать экземпляр отметки о поступлении для простановки по координатам от правого нижнего угла.
    /// </summary>
    /// <param name="rightIndent">Отступ справа, см.</param>
    /// <param name="bottomIndent">Отступ снизу, см.</param>
    /// <returns>Отметка о поступлении для простановки по координатам от правого нижнего угла.</returns>
    public override Sungero.Docflow.IMark GetOrCreateRightBottomCoordinatesBasedReceiptMark(double rightIndent, double bottomIndent)
    {
      return base.GetOrCreateRightBottomCoordinatesBasedReceiptMark(5, 1);
    }
  }
}