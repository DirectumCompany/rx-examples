﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.Memo;

namespace Sungero.Examples.Server
{
	partial class MemoFunctions
	{
		
		public System.Collections.Generic.IEnumerable<Sungero.Domain.Shared.ISignature> GetDocumentSignatures(int versionId)
		{
			var version = _obj.Versions.FirstOrDefault(x => x.Id == versionId);
			var versionSignatures = Signatures.Get(version)
				.Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Approval);
			return versionSignatures;	
		}
		
		public List<string> GetDocumentHtmlStamps(int versionId)
		{
			var signatures = GetDocumentSignatures(versionId);
			var htmlStamps = new List<string>();
			var htmlStamp = string.Empty;			
			foreach (var signature in signatures)
			{
				if (signature.SignCertificate != null)										
					htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature);
				else
					htmlStamp = Docflow.PublicFunctions.Module.GetSignatureMarkForSignatureAsHtml(signature);
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
					var xCoord = 5;
					var yCoord = pdfDocument.Pages[1].Rect.Height - 5;
					var htmlStamps = this.GetDocumentHtmlStamps(versionId);
					var pages = new int[] {1};
					foreach (var htmlStamp in htmlStamps)
					{
						var pdfStamp = pdfConverter.CreateMarkFromHtml(htmlStamp);
						pdfStamp.XIndent = xCoord;
						pdfStamp.YIndent = yCoord - pdfStamp.PdfPage.PageInfo.Height;						
						pdfConverter.AddStampToDocument(pdfDocument, pdfStamp, pages);
						yCoord = yCoord - pdfStamp.PdfPage.PageInfo.Height - 5;						
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