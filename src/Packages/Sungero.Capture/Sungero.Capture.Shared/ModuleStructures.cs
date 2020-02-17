using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.Module
{
  
  /// <summary>
  /// Контрагент для демо документов.
  /// </summary>
  partial class MockCounterparty
  {
    // Наименование контрагента.
    public string Name { get; set; }
    
    // ИНН.
    public string Tin { get; set; }
    
    // КПП.
    public string Trrc { get; set; }
    
    // Факт, из полей которого были излеченны данные контрагента.
    public Sungero.Docflow.Structures.Module.IFact Fact { get; set; }
  }
 
  /// <summary>
  /// Договорной документ и сопоставленный с ним факт.
  /// </summary>
  [Public]
  partial class ContractFactMatching
  {
    // Договорной документ.
    public Sungero.Contracts.IContractualDocument Contract { get; set; }
    
    // Факт, по полям которого был найден договорной документ.
    public Sungero.Docflow.Structures.Module.IFact Fact { get; set; }
    
    // Доверять ли найденному значению.
    public bool IsTrusted { get; set; }
  }
  
  /// <summary>
  /// Файлы захваченного письма.
  /// </summary>
  [Public]
  partial class CapturedMailFiles
  {
    // Тело письма.
    public Sungero.Docflow.Structures.Module.IFileDto Body { get; set; }
    
    // Вложенные в письмо файлы.
    public List<Sungero.Docflow.Structures.Module.IFileDto> Attachments { get; set; }
  }
  
  /// <summary>
  /// Информация о захваченном письме.
  /// </summary>
  [Public]
  partial class CapturedMailInfo
  {
    // Имя отправителя.
    public string Name { get; set; }
    
    // Адрес отправителя.
    public string FromEmail { get; set; }
    
    // Тема письма.
    public string Subject { get; set; }
  }
  
  /// <summary>
  /// Ответ от Арио.
  /// </summary>
  [Public]
  partial class ArioResponse
  {
    // Ответ. Json строка.
    public string Response { get; set; }
    
    // Текст ошибки, если не удалось обработать запрос.
    public string Error { get; set; }
  }
  
  /// <summary>
  /// Документы, созданные по результатам распознавания.
  /// </summary>
  [Public]
  partial class DocumentsCreatedByRecognitionResults
  {
    // Ид ведущего документа.
    public int LeadingDocumentId { get; set; }
    
    // Ид остальных документов в пакете.
    public List<int> RelatedDocumentIds { get; set; }
    
    // Ид документов, которые не удалось зарегестрировать или пронумеровать.
    public List<int> DocumentWithRegistrationFailureIds { get; set; }
    
    // Ид документов, которые были найдены по штрихкоду.
    public List<int> DocumentFoundByBarcodeIds { get; set; }
    public List<int> LockedDocumentIds { get; set; }
  }
  
  /// <summary>
  /// Параметры отображения фокусировки подстветки в предпросмотре.
  /// </summary>
  [Public]
  partial class HighlightActivationStyle
  {
    public string UseBorder { get; set; }
    
    public string BorderColor { get; set; }
    
    public double BorderWidth { get; set; }
    
    public string UseFilling { get; set; }
    
    public string FillingColor { get; set; }
  }

}