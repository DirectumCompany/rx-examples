using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Capture.Structures.Module
{
  /// <summary>
  /// Распознанный в Ario документ.
  /// </summary>
  [Public]
  partial class RecognitionResult
  {
    // ИД результата распознования документа в Арио.
    public int ClassificationResultId { get; set; }
    
    // Guid pdf версии документа.
    public string BodyGuid { get; set; }
    
    // Класс документа.
    public string PredictedClass { get; set; }
    
    // Извлеченные из документа факты.
    public List<Sungero.Capture.Structures.Module.IFact> Facts { get; set; }
    
    // Примечание от Арио.
    public string Message { get; set; }
    
    // Запись в справочнике для сохранения результов распознования документа.
    public IDocumentRecognitionInfo Info { get; set; }
    
    // Исходный документ.
    public Sungero.Capture.Structures.Module.IFileDto File { get; set; }
    
    // Признак того, что документ был получен службой захвата из электронной почты.
    public bool SendedByEmail { get; set; }
  }
  
  /// <summary>
  /// Файл из службы захвата.
  /// </summary>
  [Public]
  partial class FileDto
  {
    // Файл.
    public byte[] Data { get; set; }
    
    // Путь к файлу.
    public string Path { get; set; }
    
    // Имя исходного файла до обработки службой ввода документов.
    public string Description { get; set; }
  }
  
  /// <summary>
  /// Факт.
  /// </summary>
  [Public]
  partial class Fact
  {
    // ИД факта в Арио.
    public int Id { get; set; }
    
    // Название факта.
    public string Name { get; set; }
    
    // Список полей.
    public List<Sungero.Capture.Structures.Module.IFactField> Fields { get; set; }
  }
  
  /// <summary>
  /// Поле факта.
  /// </summary>
  [Public]
  partial class FactField
  {
    // ИД поля в Арио.
    public int Id { get; set; }
    
    // Название поля.
    public string Name { get; set; }
    
    // Значение поля.
    public string Value { get; set; }
    
    // Вероятность.
    public double Probability { get; set; }
  }
  
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
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
  }
  
    /// <summary>
  /// Наименования факта и полей для организаций.
  /// </summary>
  [Public]
  partial class CounterpartyFactNames
  {
    // Наименование факта с данными организации.
    public string Fact { get; set; }
    
    // Наименование поля с наименованием организации.
    public string NameField { get; set; }
    
    // Наименование поля с организационно-правовой формой организации.
    public string LegalFormField { get; set; }
  }
  
  /// <summary>
  /// Наименования факта и полей для контактных лиц.
  /// </summary>
  [Public]
  partial class ContactFactNames
  {
    // Наименование факта с данными контактного лица.
    public string Fact { get; set; }
    
    // Наименование поля с фамилией контактного лица.
    public string SurnameField { get; set; }
    
    // Наименование поля с именем контактного лица.
    public string NameField { get; set; }
    
    // Наименование поля с отчетством контактного лица.
    public string PatronymicField { get; set; }
  }
 
  /// <summary>
  /// Подписант (контакт или сотрудник) и сопоставленный с ним факт.
  /// </summary>
  [Public]
  partial class SignatoryFactMatching
  {
    // Сотрудник.
    public Sungero.Company.IEmployee Employee { get; set; }
    
    // Контактное лицо.
    public Sungero.Parties.IContact Contact { get; set; }
    
    // Факт, по полям которого было найдено контактное лицо.
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    
    // Доверять ли найденному значению.
    public bool IsTrusted { get; set; }
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
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    
    // Доверять ли найденному значению.
    public bool IsTrusted { get; set; }
  }
  
  /// <summary>
  /// Результат подбора сторон сделки для документа.
  /// </summary>  
  [Public]
  partial class DocumentParties
  {
    // НОР.
    public Sungero.Capture.Structures.Module.ICounterpartyFactMatching BusinessUnit { get; set; }
    
    // Контрагент.
    public Sungero.Capture.Structures.Module.ICounterpartyFactMatching Counterparty { get; set; }
    
    // НОР подобранная из ответственного сотрудника.
    public Sungero.Company.IBusinessUnit ResponsibleEmployeeBusinessUnit { get; set; }
  }

  /// <summary>
  /// Контрагент, НОР и сопоставленный с ними факт с типом "Контрагент".
  /// </summary>
  [Public]
  partial class CounterpartyFactMatching
  {
    // НОР.
    public Sungero.Company.IBusinessUnit BusinessUnit { get; set; }
    
    // Контрагент.
    public Sungero.Parties.ICounterparty Counterparty { get; set; }
    
    // Факт с типом контрагент, по полям которого осуществлялся поиск.
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
    
    // Тип найденного значения (Buyer, Seller и т.д.).
    public string Type { get; set; }
    
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
    public Sungero.Capture.Structures.Module.IFileDto Body { get; set; }
    
    // Вложенные в письмо файлы.
    public List<Sungero.Capture.Structures.Module.IFileDto> Attachments { get; set; }
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
  /// Результат распознавания суммы.
  /// </summary>
  [Public]
  partial class RecognizedAmount
  {
    // Сумма.
    public double Amount { get; set; }
    
    // Признак - есть значение.
    public bool HasValue { get; set; }
    
    // Признак - можно доверять.
    public bool IsTrusted { get; set; }

    // Факт
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
  }  
  
  /// <summary>
  /// Результат распознавания валюты.
  /// </summary>
  [Public]
  partial class RecognizedCurrency
  {
    // Валюта.
    public Commons.ICurrency Currency { get; set; }
    
    // Признак - есть значение.
    public bool HasValue { get; set; }
    
    // Признак - можно доверять.
    public bool IsTrusted { get; set; }
    
    // Факт
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
  }
  
  /// <summary>
  /// Результат распознавания номера документа.
  /// </summary>
  [Public]
  partial class RecognizedDocumentNumber
  {
    public string Number { get; set; }
    
    public bool IsTrusted { get; set; }
    
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
  }
  
  /// <summary>
  /// Результат распознавания даты документа.
  /// </summary>
  [Public]
  partial class RecognizedDocumentDate
  {
    public DateTime? Date { get; set; }
    
    public bool IsTrusted { get; set; }
    
    public Sungero.Capture.Structures.Module.IFact Fact { get; set; }
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