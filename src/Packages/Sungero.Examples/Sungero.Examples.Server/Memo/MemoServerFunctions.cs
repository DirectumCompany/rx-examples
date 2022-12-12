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
    /// <param name="versionId">Ид версии документа.</param>
    /// <returns>Коллекция подписей документа.</returns>
    public System.Collections.Generic.IEnumerable<Sungero.Domain.Shared.ISignature> GetDocumentSignatures(int versionId)
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
    /// <param name="versionId">Ид версии документа.</param>
    /// <returns>Список отметок об ЭП.</returns>
    public List<string> GetDocumentHtmlStamps(int versionId)
    {
      var signatures = this.GetDocumentSignatures(versionId);
      var htmlStamps = new List<string>();
      var htmlStamp = string.Empty;
      foreach (var signature in signatures)
      {
        if (signature.SignCertificate != null)
          htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature);
        else
          htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(signature);
        htmlStamps.Add(htmlStamp);
      }
      
      return htmlStamps;
    }
    
    public override Sungero.Docflow.Structures.OfficialDocument.СonversionToPdfResult ConvertToPdfAndAddSignatureMark(int versionId)
    {
      var info = Docflow.Structures.OfficialDocument.СonversionToPdfResult.Create();
      info.HasErrors = true;
      var version = _obj.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        info.HasConvertionError = true;
        info.ErrorMessage = Docflow.OfficialDocuments.Resources.NoVersionWithNumberErrorFormat(versionId);
        return info;
      }
      
      var pdfDocumentStream = new System.IO.MemoryStream();
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);
        try
        {
          var pdfConverter = new AsposeExtensions.Converter();
          var extension = version.BodyAssociatedApplication.Extension;
          // Конвертация в pdf документ.
          var pdfDocument = pdfConverter.GeneratePdfDocument(inputStream, extension);
          // Координаты отсчитываются от нижнего левого угла.
          var horizontalCoord = 312;
          var verticalCoord = pdfDocument.Pages[1].Rect.Height - 100;
          var htmlStamps = this.GetDocumentHtmlStamps(versionId);
          // Отметка об эп проставляется только на первой странице.
          var pages = new int[] { 1 };
          foreach (var htmlStamp in htmlStamps)
          {
            var pdfStamp = pdfConverter.CreateMarkFromHtml(htmlStamp);
            pdfStamp.XIndent = horizontalCoord;
            // Отступ сверху на высоту штампа.
            pdfStamp.YIndent = verticalCoord - pdfStamp.PdfPage.PageInfo.Height;
            pdfConverter.GetPdfDocumentWithStamp(pdfDocument, pdfStamp, pages, false);
            verticalCoord = verticalCoord - pdfStamp.PdfPage.PageInfo.Height - 5;
          }
          pdfDocument.Save(pdfDocumentStream);
        }
        catch (Exception e)
        {
          if (e is AsposeExtensions.PdfConvertException)
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