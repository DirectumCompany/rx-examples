using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow.OfficialDocument;
using Sungero.Examples.OfficialDocument;

namespace Sungero.Examples.Server
{
  partial class OfficialDocumentFunctions
  {
    /// <summary>
    /// Преобразовать документ в PDF с простановкой отметок.
    /// </summary>
    /// <param name="versionId">ИД версии, на которую будут проставлены отметки.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfWithMarks(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия 
      /// "Создать PDF-документ с отметками" для входящих счетов с состоянием "Оплачен"
      /// добавляется отметка "Оплачено" на преобразованный PDF-документ.
      if (IncomingInvoices.Is(_obj))
        Sungero.Examples.PublicFunctions.IncomingInvoice.GetMarkForIncomingInvoiceDocument(IncomingInvoices.As(_obj));
      
      return Sungero.Docflow.Functions.Module.ConvertToPdfWithMarks(_obj, versionId);
    }    
  }
}