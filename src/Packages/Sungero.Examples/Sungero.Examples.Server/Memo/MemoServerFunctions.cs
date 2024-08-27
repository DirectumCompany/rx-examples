using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Memo;

namespace Sungero.Examples.Server
{
  partial class MemoFunctions
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
      this.UpdateMemoSignMark(versionId);
      return base.ConvertToPdfWithMarks(versionId);
    }

    /// <summary>
    /// Обновить отметку для служебной записки.
    /// </summary>
    [Public]
    public virtual void UpdateMemoSignMark(long versionId)
    {
      var signatures = this.GetDocumentSignatures(versionId);
      var yIndent = 15d;
      foreach (var signature in signatures)
      {
        var mark = this.GetOrCreateSignatureBasedMark(Sungero.Examples.Constants.Docflow.Memo.SignMarkKindSid, signature);
        yIndent += 2.3;
        mark.XIndent = 10;
        mark.YIndent = yIndent;
        mark.Page = 1;
        mark.Save();
      }
    }
    
    /// <summary>
    /// Получить или создать отметку определенного вида.
    /// </summary>
    /// <param name="markKindSid">Sid вида отметки.</param>
    /// <param name="signature">Подпись.</param>
    /// <returns>Отметка указанного вида.</returns>
    public virtual IMark GetOrCreateSignatureBasedMark(string markKindSid, Sungero.Domain.Shared.ISignature signature)
    {
      var mark = Marks.GetAll(m => m.DocumentId == _obj.Id && m.MarkKind.Sid == markKindSid && m.VersionId == _obj.LastVersion.Id)
        .Where(dm => dm.AdditionalParams.Any(a => a.Name == PublicConstants.Docflow.Memo.MarkSignatureIdKey && 
                                             a.Value == signature.Id.ToString())).FirstOrDefault();
      if (mark != null)
        return mark;
      
      mark = Marks.Create();
      var additionalParam = mark.AdditionalParams.AddNew();
      additionalParam.Name = Constants.Docflow.Memo.MarkSignatureIdKey;
      additionalParam.Value = signature.Id.ToString();
      mark.DocumentId = _obj.Id;
      mark.VersionId = _obj.LastVersion.Id;
      mark.MarkKind = Sungero.Docflow.PublicFunctions.MarkKind.GetMarkKind(markKindSid);
      return mark;
    }
    
    /// <summary>
    /// Получить экземпляр отметки об электронной подписи документа.
    /// </summary>
    /// <returns>Отметка об электронной подписи документа.</returns>
    /// <remarks>Если отметки об электронной подписи документа не существует, то будет создана отметка для простановки по якорю.</remarks>
    public override Sungero.Docflow.IMark GetOrCreateSignatureMark()
    {
      var signature = this.GetDocumentSignatures(_obj.LastVersion.Id).First();
      return this.GetOrCreateSignatureBasedMark(Sungero.Examples.Constants.Docflow.Memo.SignMarkKindSid, signature);
    }
    
    /// <summary>
    /// Получить отметку для служеьной записки с состоянием "Утверждено".
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Изображение отметки в виде html.</returns>
    public virtual string GetMemoSignMarkAsHtml(long versionId, long signatureId)
    {
      var signature = Functions.Memo.GetDocumentSignatures(_obj, versionId).Where(s => s.Id == signatureId).First();
      return GetDocumentHtmlStamp(signature);
    }
    
    /// <summary>
    /// Получить список всех подписей документа.
    /// </summary>
    /// <param name="versionId">ИД версии документа.</param>
    /// <returns>Коллекция подписей документа.</returns>
    public System.Collections.Generic.IEnumerable<Sungero.Domain.Shared.ISignature> GetDocumentSignatures(long versionId)
    {
      var version = _obj.Versions.FirstOrDefault(x => x.Id == versionId);
      var versionSignatures = Signatures.Get(version)
        .Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Approval)
        .GroupBy(s => s.Signatory).Select(s => s.OrderBy(d => d.SigningDate).FirstOrDefault());
      return versionSignatures;
    }
    
    /// <summary>
    /// Получить список отметок об ЭП в формате html.
    /// </summary>
    /// <param name="versionId">ИД версии документа.</param>
    /// <returns>Список отметок об ЭП.</returns>
    public List<string> GetDocumentHtmlStamps(long versionId)
    {
      var signatures = this.GetDocumentSignatures(versionId);
      var htmlStamps = new List<string>();
      foreach (var signature in signatures)
        htmlStamps.Add(GetDocumentHtmlStamp(signature));
      return htmlStamps;
    }
    
    public static string GetDocumentHtmlStamp(Sungero.Domain.Shared.ISignature signature)
    {
      var htmlStamp = string.Empty;
      var defaultSignatureStampParams = Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(signature.SignCertificate != null);
      if (signature.SignCertificate != null)
        htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature, defaultSignatureStampParams);
      else
        htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(signature, defaultSignatureStampParams);
      return htmlStamp;
    }
    
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfAndAddSignatureMark(long versionId)
    {
      var info = Docflow.Structures.OfficialDocument.ConversionToPdfResult.Create();
      info.HasErrors = true;
      var version = _obj.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        info.HasConvertionError = true;
        info.ErrorMessage = Docflow.OfficialDocuments.Resources.NoVersionWithNumberErrorFormat(versionId);
        return info;
      }
      
      System.IO.Stream pdfDocumentStream = null;
      var versionBody = Docflow.PublicFunctions.OfficialDocument.GetBodyToConvertToPdf(_obj, version, true);
      using (var inputStream = new System.IO.MemoryStream(versionBody.Body))
      {
        try
        {
          var extension = version.BodyAssociatedApplication.Extension;
          var htmlStamps = this.GetDocumentHtmlStamps(versionId);
          
          // Конвертация в pdf документ.
          pdfDocumentStream = Sungero.Examples.Module.Docflow.IsolatedFunctions.PdfConverter.AddAllSignatureStamps(inputStream,
                                                                                                                   htmlStamps,
                                                                                                                   extension);
        }
        catch (Exception e)
        {
          if (e is AppliedCodeException)
            Logger.Error(Docflow.Resources.PdfConvertErrorFormat(_obj.Id), e.InnerException);
          else
            Logger.Error(string.Format("{0} {1}", Docflow.Resources.PdfConvertErrorFormat(_obj.Id), e.Message));
          
          info.HasConvertionError = true;
          info.HasLockError = false;
          info.ErrorMessage = e.Message;
        }
      }
      
      if (!string.IsNullOrWhiteSpace(info.ErrorMessage))
        return info;
      
      version.PublicBody.Write(pdfDocumentStream);
      version.AssociatedApplication = Content.AssociatedApplications.GetByExtension("pdf");
      pdfDocumentStream.Close();

      try
      {
        _obj.Save();
        info.HasErrors = false;
      }
      catch (Sungero.Domain.Shared.Exceptions.RepeatedLockException e)
      {
        Logger.Error(e.Message);
        info.HasConvertionError = false;
        info.HasLockError = true;
        info.ErrorMessage = e.Message;
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
        info.HasConvertionError = true;
        info.HasLockError = false;
        info.ErrorMessage = e.Message;
      }
      
      return info;
    }
    
  }
}