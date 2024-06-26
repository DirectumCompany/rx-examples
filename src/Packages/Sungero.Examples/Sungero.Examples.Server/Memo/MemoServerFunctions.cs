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
      var htmlStamp = string.Empty;
      foreach (var signature in signatures)
      {
        var defaultSignatureStampParams = Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(signature.SignCertificate != null);
        if (signature.SignCertificate != null)
          htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature, defaultSignatureStampParams);
        else
          htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(signature, defaultSignatureStampParams);
        htmlStamps.Add(htmlStamp);
      }
      
      return htmlStamps;
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