using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Mark;

namespace Sungero.Examples.Server
{
  partial class MarkFunctions
  {
    
    /// <summary>
    /// Получить содержимое отметки для простановки.
    /// </summary>
    /// <returns>Содержимое отметки в виде строки.</returns>
    public override string GetContent()
    {
      var content = string.Empty;
      var hasSignatureId = _obj.AdditionalParams.Any(a => a.Name == PublicConstants.Docflow.Memo.MarkSignatureIdKey);
      if (hasSignatureId)
      {
        var document = Sungero.Docflow.OfficialDocuments.Get(_obj.DocumentId.Value);
        content = this.GetContentWithSignerInfo(document, _obj.VersionId.Value);
        
        /* Приоритет простановки штампов: тэги, координаты.
         * Для простановки по координатам нужно возвращать html-контент.
         */
        if (!_obj.Tags.Any() && !this.StringLikeHtmlContent(content))
          content = Docflow.Resources.StringToHtmlDocumentWrapperFormat(content);
      }
      
      if (string.IsNullOrWhiteSpace(content))
        content = base.GetContent();
      return content;
    }
    
    /// <summary>
    /// Получить содержимое отметки с информацией о подписанте для простановки.
    /// </summary>
    /// <param name="document">Документ, на который будет проставлена отметка.</param>
    /// <param name="versionId">ИД версии документа.</param>
    /// <returns>Содержимое отметки в виде строки.</returns>
    private string GetContentWithSignerInfo(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var signatureIdString = _obj.AdditionalParams.Where(a => a.Name == PublicConstants.Docflow.Memo.MarkSignatureIdKey).First().Value;
      var signatureId = Convert.ToInt64(signatureIdString);
      var parameters = new object[] { document, versionId, signatureId};
      return Sungero.Docflow.PublicFunctions.Module.ExecuteMarkFunction(_obj.MarkKind.MarkContentClassName, _obj.MarkKind.MarkContentFunctionName, parameters).ToString();
    }
    
    /// <summary>
    /// Получить или создать отметку определенного вида.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="markKindSid">Sid вида отметки.</param>
    /// <param name="signature">Подпись.</param>
    /// <returns>Отметка указанного вида.</returns>
    public static IMark GetOrCreateMark(Sungero.Docflow.IOfficialDocument document, string markKindSid, Sungero.Domain.Shared.ISignature signature)
    {
      var mark = Marks.GetAll(m => m.DocumentId == document.Id && m.MarkKind.Sid == markKindSid && m.AdditionalParams.Any(a => a.Name == PublicConstants.Docflow.Memo.MarkSignatureIdKey)).FirstOrDefault();
      if (mark != null)
        return mark;
      mark = Marks.Create();
      mark.DocumentId = document.Id;
      mark.VersionId = document.LastVersion.Id;
      mark.MarkKind = Sungero.Docflow.PublicFunctions.MarkKind.GetMarkKind(markKindSid);
      return mark;
    }
  }
}