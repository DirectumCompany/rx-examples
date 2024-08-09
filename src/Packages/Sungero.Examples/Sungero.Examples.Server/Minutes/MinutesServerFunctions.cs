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
    /// Получить отметку для протокола совещания.
    /// </summary>
    [Public]
    public virtual void GetMarkForMinutesDocument()
      => this.CreateAndSaveMark(Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindGuid, 2, 1, 1);

    /// <summary>
    /// Создать и сохранить отметку по заданным координатам.
    /// </summary>
    /// <param name="markKindSid">Сид отметки.</param>
    /// <param name="xIndent">Координата X.</param>
    /// <param name="yIndent">Координата Y.</param>
    /// <param name="page">Страница.</param>
    public virtual void CreateAndSaveMark(string markKindSid, double xIndent, double yIndent, int page)
    {
      var mark = GetOrCreateMark(markKindSid);
      mark.XIndent = xIndent;
      mark.YIndent = yIndent;
      mark.Page = page;
      mark.Save();
    }

    /// <summary>
    /// Получить отметку для протокола совещания.
    /// </summary>
    /// <returns>Изображение отметки в виде html.</returns>
    private static string GetMinutesMarkAsHtml(IOfficialDocument document, long versionId)
    {
      var signatures =
        Signatures.Get(document.LastVersion, q => q.Where(s => s.SignatureType == SignatureType.Endorsing))
        .Where(s => s.IsValid)
        .ToList();
      
      string htmlTemplate = Examples.Minuteses.Resources.HtmlMarkTemplateMinutes;
      
      string signatoriesHtml = "";
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        foreach(var signature in signatures)
        {
          signatoriesHtml += $"<tr><td colspan=\"2\"><span class=\"tg2\">" + Examples.Minuteses.Resources.HtmlMarkTemplateSignatory + $" <b>{signature.SignatoryFullName}</b></span></td></tr>";
          signatoriesHtml += $"<tr><td colspan=\"2\"><span class=\"tg2\">" + Examples.Minuteses.Resources.HtmlMarkTemplateSignatoryIdentifier + $" {signature.Signatory.Id}</span></td></tr>";
          signatoriesHtml += $"<tr><td colspan=\"2\"><span class=\"tg2\">"+ Examples.Minuteses.Resources.HtmlMarkTemplateSigningDate + $" {signature.SigningDate.ToString("g")}</span></td></tr>";
          signatoriesHtml += "<tr><td></td></tr>";
        }
      }
      
      string resultHtml = htmlTemplate.Replace("{0}", signatoriesHtml);
      return resultHtml;
    }
  }
}