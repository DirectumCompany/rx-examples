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
    /// Поставить отметку об ЭП.
    /// </summary>
    /// <remarks>
    /// Для служебной записки отметка ставится в верхнем левом углу последней страницы.
	/// Отметка состоит из всех утверждающих подписей документа.    
    /// </remarks>
	public override Sungero.Docflow.Structures.OfficialDocument.СonversionToPdfResult GeneratePublicBodyWithSignatureMark(int versionId, string signatureMark)
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
          var htmlStampString = signatureMark;
          // Создание штампа в левом верхнем углу последней страницы.
          var pdfStamp = pdfConverter.CreatePageMarkFromHtmlString(htmlStampString);
          // Координаты отсчитываются от левого нижнего угла.
          pdfStamp.XIndent = 5;
          pdfStamp.YIndent = pdfDocument.Pages[1].Rect.Height - pdfStamp.PdfPage.PageInfo.Height - 5;
          var pages = new int[] {pdfDocument.Pages.Count};
          var doc = pdfConverter.AddStampToDocument(pdfDocument, pdfStamp, pages);
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
	
	/// <summary>
    /// Получить отметку об ЭП.
    /// </summary>
    /// <returns>Изображение отметки об ЭП в виде html.</returns>
    /// <remarks>Отметка собирается из всех подписей документа</remarks>
    public override string GetSignatureMarkAsHtml(int versionId)
    {    	
    	//Получение всех утверждающих подписей документа в порядке убывания.    	
    	var version = _obj.Versions.FirstOrDefault(x => x.Id == versionId);
    	if (version == null)
    		return null;    	
    	var versionSignatures = Signatures.Get(version)
    		.Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Approval);
    	if (!versionSignatures.Any())
    		return null;
    	var signatures = versionSignatures.OrderByDescending(x => x.SigningDate);
    	
    	// На основании каждой подписи создается html таблица с данными о подписи. 
    	var htmlTablesList = new List<string>();
    	foreach(var signature in signatures)
    	{
    		if (signature.SignCertificate == null)
    			continue;
    		var certificateSubject = Docflow.PublicFunctions.Module.GetCertificateSubject(signature);
    		var signatoryName = string.Format("{0} {1}", certificateSubject.Surname, certificateSubject.GivenName).Trim();
    		if (string.IsNullOrEmpty(signatoryName))
    			signatoryName = certificateSubject.CounterpartyName;
    		
    		string htmlTable = Sungero.Examples.Memos.Resources.HtmlMarkTable;
    		htmlTable = htmlTable.Replace("{SignatoryFullName}", signatoryName);
    		htmlTable = htmlTable.Replace("{Thumbprint}", signature.SignCertificate.Thumbprint.ToLower());
    		htmlTable = htmlTable.Replace("{Validity}", string.Format("{0} {1} {2} {3}",
    		                                                Company.Resources.From,
    		                                                signature.SignCertificate.NotBefore.Value.ToShortDateString(),
    		                                                Company.Resources.To,
    		                                                signature.SignCertificate.NotAfter.Value.ToShortDateString())
    		                   );
    		htmlTablesList.Add(htmlTable);
    		
    	}
    	
    	// Компановка html таблиц в единый html документ
    	var htmlTables = string.Join(Environment.NewLine, htmlTablesList);
    	string htmlBody = Sungero.Examples.Memos.Resources.HtmlMarkBody;    	
    	var htmlResult = htmlBody.Replace("{content}", htmlTables);
    	return htmlResult;
    }
	       
  }
}