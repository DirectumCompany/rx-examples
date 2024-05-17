using System;
using Aspose.Pdf;
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
    /// <summary>
    /// Добавить в документ отметки обо всех электронных подписях.
    /// </summary>
    /// <param name="inputStream">Поток с исходным документом.</param>
    /// <param name="htmlStamps">Отметки об ЭП.</param>
    /// <param name="extension">Расширение документа.</param>
    /// <returns>Поток с документом с проставленными отметками.</returns>
    [Public]
    public virtual Stream AddAllSignatureStamps(Stream inputStream, List<string> htmlStamps, string extension)
    {
      System.IO.Stream outputStream = null;
      try
      {
        var pdfConverter = this.CreatePdfConverter();
        var pdfDocumentStream = pdfConverter.GeneratePdf(inputStream, extension);
        outputStream  = this.AddHtmlStamps(pdfDocumentStream, htmlStamps);
        return outputStream;
      }
      catch (Exception ex)
      {
        outputStream?.Close();
        Logger.Error("Cannot add stamp", ex);
        throw new AppliedCodeException("Cannot add stamp");
      }
    }
    
    /// <summary>
    /// Добавить в документ заданные отметки об ЭП.
    /// </summary>
    /// <param name="pdfDocument">Исходный документ.</param>
    /// <param name="pdfConverter">Конвертер в PDF.</param>
    /// <param name="htmlStamps">Отметки об ЭП.</param>
    /// <param name="outputStream">Поток, куда записывать результат.</param>
    /// <returns>Поток с документом с проставленными отметками.</returns>
    public virtual Stream AddHtmlStamps(Stream pdfDocumentStream, List<string> htmlStamps)
    {
      using (var pdfDocument = new Aspose.Pdf.Document(pdfDocumentStream))
      {
        Stream outputStream = new MemoryStream();
        pdfDocumentStream.Position = 0;
        pdfDocumentStream.CopyTo(outputStream);
        
        // Координаты отсчитываются от нижнего левого угла.
        const int horizontalCoord = 312;
        const int verticalOffset = 100;
        const int verticalSpacing = 5;
        var verticalCoord = pdfDocument.Pages[1].Rect.Height - verticalOffset;
        
        // Отметка об ЭП проставляется только на первой странице.
        var firstPageIndex = 1;
        
        foreach (var htmlStamp in htmlStamps)
        {
          var pdfStamper = this.CreatePdfStamper();
          var pdfStamp = pdfStamper.CreateMarkFromHtml(htmlStamp);
          pdfStamp.XIndent = horizontalCoord;
          
          // Отступ сверху на высоту штампа.
          pdfStamp.YIndent = verticalCoord - pdfStamp.PdfPage.PageInfo.Height;
          outputStream = pdfStamper.AddStampToDocumentPage(outputStream, firstPageIndex, pdfStamp);
          verticalCoord = verticalCoord - pdfStamp.PdfPage.PageInfo.Height - verticalSpacing;
        }
        
        return outputStream;
      }
    }
  }
}