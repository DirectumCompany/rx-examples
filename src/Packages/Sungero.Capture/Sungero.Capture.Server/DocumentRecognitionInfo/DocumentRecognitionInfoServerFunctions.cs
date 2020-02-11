using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.DocumentRecognitionInfo;

namespace Sungero.Capture.Server
{
  partial class DocumentRecognitionInfoFunctions
  {
    /// <summary>
    /// Получить результат распознавания документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Результат распознавания.</returns>
    [Remote(IsPure=true)]
    public static IDocumentRecognitionInfo GetDocumentRecognitionInfo(Sungero.Docflow.IOfficialDocument document)
    {
      return DocumentRecognitionInfos.GetAll(x => x.DocumentId == document.Id).FirstOrDefault();
    }
  }
}