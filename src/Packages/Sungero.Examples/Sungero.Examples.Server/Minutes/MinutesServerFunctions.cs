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
    /// Преобразовать документ в PDF с простановкой отметок.
    /// </summary>
    /// <param name="versionId">ИД версии, на которую будут проставлены отметки.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfWithMarks(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия
      /// "Создать PDF-документ с отметками" для протокола
      /// добавляется отметка всех подписантов на преобразованный PDF-документ.
      this.UpdateMinutesMark();
      return base.ConvertToPdfWithMarks(versionId);
    }

    /// <summary>
    /// Получить отметку для протокола.
    /// </summary>
    [Public]
    public virtual void UpdateMinutesMark()
    {
      var mark = GetOrCreateMark(Sungero.Examples.Constants.Meetings.Minutes.MinutesMarkKindGuid);
      mark.XIndent = 2;
      mark.YIndent = 1;
      mark.Page = 1;
      mark.Save();
    }

    /// <summary>
    /// Получить отметку для протокола.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    private static string GetMinutesMarkAsHtml(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var signatures =
        Signatures.Get(document.LastVersion, q => q.Where(s => s.SignatureType == SignatureType.Endorsing))
        .Where(s => s.IsValid)
        .ToList();
      
      var signatoriesHtml = string.Empty;   
      foreach(var signature in signatures)
      {
        signatoriesHtml += Examples.Minuteses.Resources.HtmlMarkTemplateSignatoryFormat(signature.SignatoryFullName);
        signatoriesHtml += Examples.Minuteses.Resources.HtmlMarkTemplateSignatoryIdentifierFormat(signature.Signatory.Id);
        signatoriesHtml += Examples.Minuteses.Resources.HtmlMarkTemplateSigningDateFormat(signature.SigningDate.ToString("g"));
        signatoriesHtml += "<tr><td></td></tr>";
      }
      
      string resultHtml = Examples.Minuteses.Resources.HtmlMarkTemplateMinutesFormat(signatoriesHtml);
      return resultHtml;
    }
  }
}