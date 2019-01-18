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
	public override Sungero.Docflow.Structures.OfficialDocument.GeneratePublicBodyInfo GeneratePublicBodyWithSignatureMark(int versionId, string signatureMark)
	{	 		
      var info = Docflow.Structures.OfficialDocument.GeneratePublicBodyInfo.Create();
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
          var pdfDocument = pdfConverter.GeneratePdfDocument(inputStream, extension);
          var htmlStampString = signatureMark;
          var pdfStamp = pdfConverter.CreatePageStampFromHtmlString(htmlStampString);
          pdfStamp.XIndent = 5;
          pdfStamp.YIndent = pdfDocument.Pages[1].Rect.Height - pdfStamp.PdfPage.PageInfo.Height - 5;
          var pages = new int[] {pdfDocument.Pages.Count};
          var doc = pdfConverter.AddStampToDocument(pdfDocument, pdfStamp, pages);
          pdfStamp.YIndent = pdfDocument.Pages[1].Rect.Height - 2*pdfStamp.PdfPage.PageInfo.Height - 5;          
          doc = pdfConverter.AddStampToDocument(doc, pdfStamp, pages);
          doc.Save(pdfDocumentStream);
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