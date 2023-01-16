using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using Sungero.Examples.Module.Docflow.Structures.Module;

namespace Sungero.Examples.Module.Docflow.Isolated.PdfConverter
{
  public partial class IsolatedFunctions
  {
    [Public]
    public virtual Stream AddAllSignatureStamps(Stream inputStream, List<string> htmlStamps, string extension)
    {
      var pdfDocumentStream = new MemoryStream();
      try
      {
        var pdfConverter = new AsposeExtensions.Converter();
        inputStream.CopyTo(pdfDocumentStream);
        var pdfDocument = pdfConverter.GeneratePdfDocument(pdfDocumentStream, extension);
        // Координаты отсчитываются от нижнего левого угла.
        var horizontalCoord = 312;
        var verticalCoord = pdfDocument.Pages[1].Rect.Height - 100;
        // Отметка об ЭП проставляется только на первой странице.
        var pages = new int[] { 1 };
        
        foreach (var htmlStamp in htmlStamps)
        {
          var pdfStamp = pdfConverter.CreateMarkFromHtml(htmlStamp);
          pdfStamp.XIndent = horizontalCoord;
          // Отступ сверху на высоту штампа.
          pdfStamp.YIndent = verticalCoord - pdfStamp.PdfPage.PageInfo.Height;
          pdfDocumentStream = pdfConverter.GetPdfDocumentWithStamp(pdfDocument, pdfStamp, pages, false);
          verticalCoord = verticalCoord - pdfStamp.PdfPage.PageInfo.Height - 5;
        }
        
        //pdfDocument.Save(pdfDocumentStream);
        return pdfDocumentStream;
      }
      catch (Exception ex)
      {
        pdfDocumentStream.Close();
        Logger.Error("Cannot add stamp", ex);
        throw new AppliedCodeException("Cannot add stamp");
      }
    }
  }
}