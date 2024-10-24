using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Minutes;

namespace Sungero.Examples.Server
{
  partial class MinutesFunctions
  {
    /// <summary>
    /// Получить экземпляр отметки со всеми подписями для протокола.
    /// </summary>
    [Public, Remote]
    public override Sungero.Docflow.IMark GetOrCreateSignatureMark()
    {
      var mark = GetOrCreateMark(Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindGuid);
      mark.XIndent = 2;
      mark.YIndent = 1;
      mark.Page = -1;
      return mark;
    }
    
    /// <summary>
    /// Удалить экземпляр отметки со всеми подписями для протокола.
    /// </summary>
    [Public, Remote]
    public override void DeleteSignatureMark()
    {
      this.DeleteVersionMark(_obj.LastVersion.Id, Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindGuid);
    }
    
    /// <summary>
    /// Получить отметку со всеми подписями для протокола.
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    public virtual string GetMinutesMarkAsHtml(long versionId)
    {
      /// Получаем все подписи с типом "Согласовано" и "Утверждено"
      /// Группируем подписи по владельцам
      /// Для каждой группы выбираем одну с приоритетом для "Утверджено"
      var signatures = Signatures.Get(_obj.LastVersion, q => q.Where(s => s.SignatureType == SignatureType.Endorsing || s.SignatureType == SignatureType.Approval))
        .Where(s => s.IsValid)
        .GroupBy(s => s.Signatory.Id)
        .Select(g => g.OrderByDescending(s => s.SignatureType == SignatureType.Approval).First())
        .ToList();
      
      var html = new System.Text.StringBuilder();
      foreach(var signature in signatures)
      {
        html.Append(Examples.Minuteses.Resources.HtmlMarkTemplateSignatoryFormat(signature.SignatoryFullName));
        html.Append(Examples.Minuteses.Resources.HtmlMarkTemplateSignatoryIdentifierFormat(signature.Signatory.Id));
        html.Append(Examples.Minuteses.Resources.HtmlMarkTemplateSigningDateFormat(signature.SigningDate.ToString("g")));
        html.Append(Examples.Minuteses.Resources.HtmlMarkTemplateSeparator);
      }
      
      return Examples.Minuteses.Resources.HtmlMarkTemplateMinutesFormat(html.ToString());
    }
  }
}