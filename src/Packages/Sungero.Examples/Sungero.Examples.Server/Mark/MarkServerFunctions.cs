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
      if (_obj.SignatureIdSungero != null)
      {
        var document = Sungero.Docflow.OfficialDocuments.Get(_obj.DocumentId.Value);
        content = this.GetContent(document, _obj.VersionId.Value);
        
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
    /// Получить содержимое отметки для простановки.
    /// </summary>
    /// <param name="document">Документ, на который будет проставлена отметка.</param>
    /// <param name="versionId">ИД версии документа.</param>
    /// <returns>Содержимое отметки в виде строки.</returns>
    private string GetContent(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var parameters = new object[] { document, versionId, _obj.SignatureIdSungero};
      return Sungero.Docflow.PublicFunctions.Module.ExecuteMarkFunction(_obj.MarkKind.MarkContentClassName, _obj.MarkKind.MarkContentFunctionName, parameters).ToString();
    }
    
    /// <summary>
    /// Получить отметку определённого вида или создать, если ее не существует.
    /// </summary>
    /// <param name="document">TODO</param>
    /// <param name="markKindSid">Sid вида отметки.</param>
    /// <returns>Отметка указанного вида.</returns>
    public static IMark CreateMark(Sungero.Docflow.IOfficialDocument document, string markKindSid)
    {
      var mark = Marks.Create();
      mark.DocumentId = document.Id;
      mark.VersionId = document.LastVersion.Id;
      mark.MarkKind = Sungero.Docflow.PublicFunctions.MarkKind.GetMarkKind(markKindSid);
      return mark;
    }
  }
}