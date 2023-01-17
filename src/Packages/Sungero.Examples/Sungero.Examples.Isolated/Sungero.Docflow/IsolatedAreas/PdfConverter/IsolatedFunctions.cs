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
        using (var pdfDocumentStream = new MemoryStream())
        {
          var pdfConverter = new AsposeExtensions.Converter();
          inputStream.CopyTo(pdfDocumentStream);
          var pdfDocument = pdfConverter.GeneratePdfDocument(pdfDocumentStream, extension);
          outputStream  = this.AddHtmlStamps(pdfDocument, pdfConverter, htmlStamps, outputStream);
        }
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
    public virtual Stream AddHtmlStamps(Aspose.Pdf.Document pdfDocument, Sungero.AsposeExtensions.Converter pdfConverter, List<string> htmlStamps, Stream outputStream)
    {
      const int horizontalCoord = 312;
      const int verticalOffset = 100;
      const int verticalSpacing = 5;
      
      // Координаты отсчитываются от нижнего левого угла.
      var verticalCoord = pdfDocument.Pages[1].Rect.Height - verticalOffset;
      
      // Отметка об ЭП проставляется только на первой странице.
      var pages = new int[] { 1 };
      
      foreach (var htmlStamp in htmlStamps)
      {
        var pdfStamp = pdfConverter.CreateMarkFromHtml(htmlStamp);
        pdfStamp.XIndent = horizontalCoord;
        
        // Отступ сверху на высоту штампа.
        pdfStamp.YIndent = verticalCoord - pdfStamp.PdfPage.PageInfo.Height;
        outputStream = pdfConverter.GetPdfDocumentWithStamp(pdfDocument, pdfStamp, pages, false);
        verticalCoord = verticalCoord - pdfStamp.PdfPage.PageInfo.Height - verticalSpacing;
      }
      
      return outputStream;
    }
  }
}