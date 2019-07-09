using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sungero.Company;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Capture.Structures.Module;
using Sungero.Parties;
using Sungero.RecordManagement;
using Sungero.Workflow;

namespace Sungero.Capture.Server
{
  public class ModuleFunctions
  {
    #region Инициализация
    
    /// <summary>
    /// Инициализация демо-режима.
    /// </summary>
    [Remote]
    public static void InitCaptureMockMode()
    {
      // Создать типы документов.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(RecordManagement.Resources.IncomingLetterTypeName,
                                                                              Capture.Server.MockIncomingLetter.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(FinancialArchive.Resources.ContractStatementTypeName,
                                                                              Capture.Server.MockContractStatement.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Contracts, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(FinancialArchive.Resources.WaybillDocumentTypeName,
                                                                              Capture.Server.MockWaybill.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(FinancialArchive.Resources.IncomingTaxInvoiceTypeName,
                                                                              Capture.Server.MockIncomingTaxInvoice.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Incoming, true);
      
      // Создать виды документов.
      var actions = new[] { OfficialDocuments.Info.Actions.SendActionItem, OfficialDocuments.Info.Actions.SendForFreeApproval };
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(RecordManagement.Resources.IncomingLetterKindName,
                                                                              RecordManagement.Resources.IncomingLetterKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true, false,
                                                                              Sungero.Capture.Server.MockIncomingLetter.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockIncomingLetterKindGuid);

      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(FinancialArchive.Resources.ContractStatementKindName,
                                                                              FinancialArchive.Resources.ContractStatementKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Contracts, true, false,
                                                                              Capture.Server.MockContractStatement.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockContractStatementKindGuid);

      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(FinancialArchive.Resources.WaybillDocumentKindName,
                                                                              FinancialArchive.Resources.WaybillDocumentKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Contracts, true, false,
                                                                              Sungero.Capture.Server.MockWaybill.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockWaybillKindGuid);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(FinancialArchive.Resources.IncomingTaxInvoiceKindName,
                                                                              FinancialArchive.Resources.IncomingTaxInvoiceKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true, false,
                                                                              Capture.Server.MockIncomingTaxInvoice.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockIncomingTaxInvoiceGuid);
      
      // Добавить параметр признака активации демо-режима.
      Sungero.Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Sungero.Capture.Constants.Module.CaptureMockModeKey, string.Empty);
    }
    
    /// <summary>
    /// Задать основные параметры захвата.
    /// </summary>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="minFactProbability">Минимальная вероятность для факта.</param>
    /// <param name="trustedFactProbability">Доверительная вероятность для факта.</param>
    [Remote]
    public static void SetCaptureMainSettings(string arioUrl, string minFactProbability, string trustedFactProbability)
    {
      // Добавить параметр адреса сервиса Ario.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.ArioUrlKey, arioUrl);
      
      // Добавить параметр минимальной вероятности для факта.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.MinFactProbabilityKey, minFactProbability);
      
      // Добавить параметр доверительной вероятности для факта.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.TrustedFactProbabilityKey, trustedFactProbability);
    }
    
    #endregion
    
    #region Общий процесс обработки захваченных документов
    
    /// <summary>
    /// Создать документы в RX.
    /// </summary>
    /// <param name="recognitionResults">Json результаты классификации и извлечения фактов.</param>
    /// <param name="originalFile">Исходный файл, полученный с DCS.</param>
    /// <param name="leadingDocument">Ведущий документ. Если не передан будет определен автоматически.</param>
    /// <param name="responsible">Сотрудник, ответственного за проверку документов.</param>
    /// <returns>Список Id созданных документов.</returns>
    [Remote]
    public virtual Structures.Module.DocumentsCreatedByRecognitionResults CreateDocumentsByRecognitionResults(string recognitionResults, Structures.Module.IFileInfo originalFile,
                                                                                                              IOfficialDocument leadingDocument, IEmployee responsible)
    {
      var result = Structures.Module.DocumentsCreatedByRecognitionResults.Create();
      
      var recognizedDocuments = GetRecognizedDocuments(recognitionResults, originalFile);
      var package = new List<IOfficialDocument>();
      foreach (var recognizedDocument in recognizedDocuments)
      {
        var document = CreateDocumentByRecognizedDocument(recognizedDocument,  responsible);
        package.Add(document);
        recognizedDocument.Info.DocumentId = document.Id;
        recognizedDocument.Info.Save();
      }
      
      // Определить ведущий документ.
      if (leadingDocument == null)
        leadingDocument = GetLeadingDocument(package);
      result.LeadingDocumentId = leadingDocument.Id;
      
      // Связать приложения с ведущим документом.
      var addendums = package;
      if (addendums.Any(x => Equals(x, leadingDocument)))
        addendums.Remove(leadingDocument);
      var canBeLeadingDocument = !SimpleDocuments.Is(leadingDocument);
      var relation = canBeLeadingDocument
        ? Docflow.PublicConstants.Module.AddendumRelationName
        : Constants.Module.SimpleRelationRelationName;
      int simpleAddendumNumber = 1;
      foreach (var addendum in addendums)
      {
        // У простых документов, захваченных с почты имя не меняется.
        if (SimpleDocuments.Is(addendum) && string.IsNullOrEmpty(originalFile.Description))
        {
          addendum.Name = canBeLeadingDocument
            ? Resources.AttachmentNameFormat(simpleAddendumNumber)
            : Resources.DocumentNameFormat(simpleAddendumNumber);
          simpleAddendumNumber++;
        }
        
        addendum.Relations.AddFrom(relation, leadingDocument);
        addendum.Save();
      }
      
      result.RelatedDocumentIds = package.Select(x => x.Id).ToList();
      return result;
    }
    
    public virtual List<Structures.Module.IRecognizedDocument> GetRecognizedDocuments(string jsonClassificationResults, Structures.Module.IFileInfo originalFile)
    {
      var recognizedDocuments = new List<IRecognizedDocument>();
      if (string.IsNullOrWhiteSpace(jsonClassificationResults))
        return recognizedDocuments;
      
      var packageProcessResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      foreach (var packageProcessResult in packageProcessResults)
      {
        // Класс и гуид тела документа.
        var recognizedDocument = RecognizedDocument.Create();
        var clsResult = packageProcessResult.ClassificationResult;
        recognizedDocument.ClassificationResultId = clsResult.Id;
        recognizedDocument.BodyGuid = packageProcessResult.Guid;
        recognizedDocument.PredictedClass = clsResult.PredictedClass != null ? clsResult.PredictedClass.Name : string.Empty;
        recognizedDocument.Message = packageProcessResult.Message;
        recognizedDocument.OriginalFile = originalFile;
        var docInfo = DocumentRecognitionInfos.Create();
        docInfo.Name = recognizedDocument.PredictedClass;
        docInfo.RecognizedClass = recognizedDocument.PredictedClass;
        if (clsResult.PredictedProbability != null)
          docInfo.ClassProbability = (double)(clsResult.PredictedProbability);
        
        // Факты и поля фактов.
        recognizedDocument.Facts = new List<IFact>();
        var minFactProbability = GetDocflowParamsNumbericValue(Constants.Module.MinFactProbabilityKey);
        if (packageProcessResult.ExtractionResult.Facts != null)
        {
          var facts = packageProcessResult.ExtractionResult.Facts
            .Where(f => !string.IsNullOrWhiteSpace(f.Name))
            .Where(f => f.Fields.Any())
            .ToList();
          foreach (var fact in facts)
          {
            var fields = fact.Fields.Where(f => f != null)
              .Where(f => f.Probability >= minFactProbability)
              .Select(f => FactField.Create(f.Id, f.Name, f.Value, f.Probability));
            recognizedDocument.Facts.Add(Fact.Create(fact.Id, fact.Name, fields.ToList()));
            
            foreach (var factField in fact.Fields)
            {
              var fieldInfo = docInfo.Facts.AddNew();
              fieldInfo.FactId = fact.Id;
              fieldInfo.FieldId = factField.Id;
              fieldInfo.FactName = fact.Name;
              fieldInfo.FieldName = factField.Name;
              fieldInfo.FieldValue = factField.Value;
              fieldInfo.FieldProbability = factField.Probability;
              
              // Позиция подсветки фактов в теле документа.
              if (factField.Positions != null)
              {
                var positions = factField.Positions
                  .Where(p => p != null)
                  .Select(p => string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}",
                                             Constants.Module.PositionElementDelimiter,
                                             p.Page, p.Top, p.Left, p.Width, p.Height));
                fieldInfo.Position = string.Join(Constants.Module.PositionsDelimiter.ToString(), positions);
              }
            }
          }
        }
        docInfo.Save();
        recognizedDocument.Info = docInfo;
        recognizedDocuments.Add(recognizedDocument);
      }
      return recognizedDocuments;
    }
    
    /// <summary>
    /// Создать документ DirectumRX на основе классификации Ario.
    /// </summary>
    /// <param name="recognizedDocument">Результат классификации Ario.</param>
    /// <param name="sourceFileName">Путь до исходного файла, отправленного на распознование.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Документ, созданный на основе классификации.</returns>
    public virtual IOfficialDocument CreateDocumentByRecognizedDocument(Structures.Module.IRecognizedDocument recognizedDocument,
                                                                        IEmployee responsible)
    {
      // Входящее письмо.
      var recognizedClass = recognizedDocument.PredictedClass;
      var isMockMode = GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null;
      if (recognizedClass == Constants.Module.LetterClassName)
        return isMockMode
          ? CreateMockIncomingLetter(recognizedDocument)
          : CreateIncomingLetter(recognizedDocument, responsible);
      
      // Акт выполненных работ.
      if (recognizedClass == Constants.Module.ContractStatementClassName)
        return isMockMode
          ? CreateMockContractStatement(recognizedDocument)
          : CreateContractStatement(recognizedDocument, responsible);
      
      // Товарная накладная.
      if (recognizedClass == Constants.Module.WaybillClassName)
        return isMockMode
          ? CreateMockWaybill(recognizedDocument)
          : CreateWaybill(recognizedDocument, responsible);
      
      // Счет-фактура.
      if (recognizedClass == Constants.Module.TaxInvoiceClassName)
        return isMockMode
          ? CreateMockIncomingTaxInvoice(recognizedDocument)
          : CreateTaxInvoice(recognizedDocument, responsible, false);
      
      // Корректировочный счет-фактура.
      if (recognizedClass == Constants.Module.TaxinvoiceCorrectionClassName && !isMockMode)
        return CreateTaxInvoice(recognizedDocument, responsible, true);
      
      // УПД.
      if (recognizedClass == Constants.Module.UniversalTransferDocumentClassName && !isMockMode)
        return CreateUniversalTransferDocument(recognizedDocument, responsible, false);
      
      //УКД
      if (recognizedClass == Constants.Module.GeneralCorrectionDocumentClassName && !isMockMode)
        return CreateUniversalTransferDocument(recognizedDocument, responsible, true);
      
      // Счет на оплату.
      if (recognizedClass == Constants.Module.IncomingInvoiceClassName && !isMockMode)
        return CreateIncomingInvoice(recognizedDocument, responsible);
      
      // Все нераспознанные документы создать простыми.
      return CreateSimpleDocument(recognizedDocument);
    }
    
    /// <summary>
    /// Определить ведущий документ распознанного комплекта.
    /// </summary>
    /// <param name="package">Комплект документов.</param>
    /// <returns>Ведущий документ.</returns>
    public virtual IOfficialDocument GetLeadingDocument(List<IOfficialDocument> package)
    {
      var leadingDocument = package.FirstOrDefault();
      var incLetter = GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null
        ? package.Where(d => MockIncomingLetters.Is(d)).FirstOrDefault()
        : package.Where(d => IncomingLetters.Is(d)).FirstOrDefault();
      if (incLetter != null)
        return incLetter;
      
      var contractStatement = package.Where(d => MockContractStatements.Is(d)).FirstOrDefault();
      if (contractStatement != null)
        return contractStatement;
      
      var waybill = package.Where(d => MockWaybills.Is(d)).FirstOrDefault();
      if (waybill != null)
        return waybill;
      
      var incTaxInvoice = package.Where(d => MockIncomingTaxInvoices.Is(d)).FirstOrDefault();
      if (incTaxInvoice != null)
        return incTaxInvoice;
      
      return leadingDocument;
    }
    
    #endregion
    
    #region Фасад DirectumRX
    
    [Public, Remote]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
    
    public virtual void RegisterDocument(IOfficialDocument document)
    {
      // Присвоить номер, если вид документа - нумеруемый.
      var number = document.RegistrationNumber;
      var date = document.RegistrationDate;
      if (document.DocumentKind != null && document.DocumentKind.NumberingType == Docflow.DocumentKind.NumberingType.Numerable)
      {
        var isRegistered = Docflow.PublicFunctions.OfficialDocument.TryExternalRegister(document, number, date);
        if (isRegistered)
          return;
      }
      
      // Записать номер/дату в примечании, если вид не нумеруемый или регистрируемый или не получилось пронумеровать.
      if (date != null && string.IsNullOrWhiteSpace(number))
        document.Note = Exchange.Resources.IncomingNotNumeratedDocumentNoteFormat(date.Value.Date.ToString("d"), number) +
          Environment.NewLine + document.Note;
    }
    
    /// <summary>
    /// Получить значение параметра из docflow_params.
    /// </summary>
    /// <param name="paramName">Наименование параметра.</param>
    /// <returns>Значение параметра.</returns>
    public static object GetDocflowParamsValue(string paramName)
    {
      var command = string.Format(Queries.Module.SelectDocflowParamsValue, paramName);
      return Docflow.PublicFunctions.Module.ExecuteScalarSQLCommand(command);
    }
    
    /// <summary>
    /// Получить адрес сервиса Арио.
    /// </summary>
    /// <returns>Адрес Арио.</returns>
    [Remote]
    public static string GetArioUrl()
    {
      var commandExecutionResult = GetDocflowParamsValue(Constants.Module.ArioUrlKey);
      var arioUrl = string.Empty;
      if (!(commandExecutionResult is DBNull) && commandExecutionResult != null)
        arioUrl = commandExecutionResult.ToString();
      
      return arioUrl;
    }
    
    /// <summary>
    /// Получить сотрудника по имени.
    /// </summary>
    /// <param name="name">Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</param>
    /// <returns>Сотрудник.</returns>
    public static IEmployee GetEmployeeByName(string name)
    {
      var noBreakSpace = new string('\u00A0', 1);
      var space = new string('\u0020', 1);
      
      return Employees.GetAll()
        .Where(x => x.Person.ShortName.ToLower().Replace(noBreakSpace, space).Replace(". ", ".") ==
               name.ToLower().Replace(noBreakSpace, space).Replace(". ", ".") || x.Name.ToLower() == name.ToLower())
        .FirstOrDefault();
    }
    
    /// <summary>
    /// Получить контактное лицо по имени.
    /// </summary>
    /// <param name="name">Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</param>
    /// <param name="shortName">Имя в формате "Фамилия И.О.".</param>
    /// <param name="counterparty">Контрагент, владелец контакта.</param>
    /// <returns>Контактное лицо.</returns>
    public static IContact GetContactByName(string name, string personShortName, ICounterparty counterparty)
    {
      var noBreakSpace = new string('\u00A0', 1);
      var space = new string('\u0020', 1);
      
      name = name.ToLower().Replace(noBreakSpace, space).Replace(". ", ".");
      
      var contacts =  Contacts.GetAll()
        .Where(x => (x.Name.ToLower().Replace(noBreakSpace, space).Replace(". ", ".") == name) ||
               (x.Person != null && string.Equals(x.Person.ShortName, personShortName, StringComparison.InvariantCultureIgnoreCase)));
      
      if (counterparty != null)
        return contacts.Where(c => c.Company.Equals(counterparty)).FirstOrDefault();
      
      return   contacts.FirstOrDefault();
    }
    
    /// <summary>
    /// Получить полное имя из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</returns>
    public static string GetFullNameByFact(Sungero.Capture.Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var surname = GetFieldValue(fact, "Surname");
      var name = GetFieldValue(fact, "Name");
      var patronymic = GetFieldValue(fact, "Patrn");
      
      // Собрать ФИО из фамилии, имени и отчества.
      var parts = new List<string>();
      
      if (!string.IsNullOrWhiteSpace(surname))
        parts.Add(surname);
      if (!string.IsNullOrWhiteSpace(name))
        parts.Add(name);
      if (!string.IsNullOrWhiteSpace(patronymic))
        parts.Add(patronymic);
      
      return string.Join(" ", parts);
    }
    
    /// <summary>
    /// Получить сокращенное имя из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О.".</returns>
    public static string GetShortNameByFact(Sungero.Capture.Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var surname = GetFieldValue(fact, "Surname");
      var name = GetFieldValue(fact, "Name");
      var patronymic = GetFieldValue(fact, "Patrn");
      return Parties.PublicFunctions.Person.GetSurnameAndInitialsInTenantCulture(name, patronymic, surname);
    }
    
    /// <summary>
    /// Получить контактное лицо по извлечённому факту.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент - владелец контактного лица.</param>
    /// <returns>Контактное лицо.</returns>
    public static IContact GetContactByFact(Sungero.Capture.Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return Contacts.Null;
      
      var fullName = GetFullNameByFact(fact);
      var shortName = GetShortNameByFact(fact);
      return GetContactByName(fullName, shortName, counterparty);
    }
    
    /// <summary>
    /// Получить контактное лицо по извлечённому факту.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public static Structures.Module.ContactWithFact GetContactByFact(Sungero.Capture.Structures.Module.IFact fact, string propertyName, ICounterparty counterparty, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContactWithFact.Create(Sungero.Parties.Contacts.Null, fact, false);
      if (fact == null)
        return result;
      if (counterparty != null)
      {
        result = GetContactByVerifiedData(fact, propertyName, counterparty.Id.ToString() ,counterpartyPropertyName);
        if (result.Contact != null)
          return result;
      }
      
      var filteredContact =  GetContactByFact(fact, counterparty);
      if (filteredContact == null)
        return result;
      result.Contact = filteredContact;
      result.IsTrusted = GetField(fact, "Type").Probability > GetDocflowParamsNumbericValue(Constants.Module.TrustedFactProbabilityKey);
      return result;
    }
    
    /// <summary>
    /// Поиск контактного лица контрагента.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public static Structures.Module.ContactWithFact GetContactByVerifiedData(Structures.Module.IFact fact, string propertyName, string  counterpartyPropertyValue, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContactWithFact.Create(Contacts.Null, fact, false);
      var factLabel = GetFactLabel(fact, propertyName);
      var recognitionInfo = DocumentRecognitionInfos.GetAll()
      	.Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty) 
      	       && d.Facts.Any(f => f.PropertyName == counterpartyPropertyName && f.PropertyValue == counterpartyPropertyValue))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return result;
      
      var fieldRecognitionInfo = recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
      int contactId;
      if (!int.TryParse(fieldRecognitionInfo.VerifiedValue, out contactId))
        return result;
      
      var filteredContact = Contacts.GetAll(x => x.Id == contactId).FirstOrDefault();
      if (filteredContact != null)
      {
        result.Contact = filteredContact;
        result.IsTrusted = fieldRecognitionInfo.IsTrusted == true;        
      }
      return result;
    }
    
    /// <summary>
    /// Получить подразделение из настроек сотрудника.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <returns>Подразделение.</returns>
    public static Company.IDepartment GetDepartment(Company.IEmployee employee)
    {
      if (employee == null)
        return null;
      
      var department = Company.Departments.Null;
      var settings = Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(employee);
      if (settings != null)
        department = settings.Department;
      if (department == null)
        department = employee.Department;
      return department;
    }
    
    /// <summary>
    /// Получить ведущий документ по номеру и дате из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Документ с соответствующими номером и датой.</returns>
    /// <remarks>Будет возвращен первый попавшийся, если таких документов несколько.
    /// Будет возвращен null, если таких документов нет.</remarks>
    private static Sungero.Contracts.IContractualDocument GetLeadingDocument(Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return Sungero.Contracts.ContractualDocuments.Null;
      
      var docDate = GetFieldDateTimeValue(fact, "DocumentBaseDate");
      var number = GetFieldValue(fact, "DocumentBaseNumber");
      
      if (string.IsNullOrWhiteSpace(number))
        return Sungero.Contracts.ContractualDocuments.Null;
      
      return Sungero.Contracts.ContractualDocuments.GetAll(x => x.RegistrationNumber == number &&
                                                           x.RegistrationDate == docDate &&
                                                           (counterparty == null || x.Counterparty.Equals(counterparty))).FirstOrDefault();
    }
    
    /// <summary>
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="leadingDocument">Основной документ.</param>
    /// <param name="documents">Прочие документы.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Простая задача.</returns>
    [Public, Remote]
    public virtual void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents, Company.IEmployee responsible)
    {
      if (leadingDocument == null)
        return;
      
      var task = SimpleTasks.Create();
      task.Subject = Resources.CheckPackageTaskNameFormat(leadingDocument);
      task.ActiveText = Resources.CheckPackageTaskText;
      var step = task.RouteSteps.AddNew();
      step.AssignmentType = Workflow.SimpleTask.AssignmentType.Assignment;
      step.Performer = responsible;
      
      // Вложить в задачу и выдать права на документы ответственному.
      leadingDocument.AccessRights.Grant(responsible, DefaultAccessRightsTypes.FullAccess);
      leadingDocument.Save();
      task.Attachments.Add(leadingDocument);
      foreach (var document in documents)
      {
        document.AccessRights.Grant(responsible, DefaultAccessRightsTypes.FullAccess);
        document.Save();
        task.Attachments.Add(document);
      }
      task.NeedsReview = false;
      task.Deadline = Calendar.Now.AddWorkingHours(4);
      task.Save();
      task.Start();
    }
    
    /// <summary>
    /// Отправить документы ответственному.
    /// </summary>
    /// <param name="documentsCreatedByRecognition">Результат создания документов.</param>
    /// <param name="responsible">Сотрудник, ответственный за одработку распознанных документов.</param>
    [Remote]
    public virtual void SendToResponsible(Structures.Module.DocumentsCreatedByRecognitionResults documentsCreatedByRecognition,
                                          Sungero.Company.IEmployee responsible)
    {
      var leadingDocument = OfficialDocuments.GetAll()
        .FirstOrDefault(x => x.Id == documentsCreatedByRecognition.LeadingDocumentId);
      var relatedDocuments = OfficialDocuments.GetAll()
        .Where(x => documentsCreatedByRecognition.RelatedDocumentIds.Contains(x.Id))
        .ToList();
      SendToResponsible(leadingDocument, relatedDocuments, responsible);
    }
    
    #endregion
    
    #region Простой документ
    
    /// <summary>
    /// Создать документ в Rx, тело документа загружается из Арио.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateSimpleDocument(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = SimpleDocuments.Create();
      document.Name = !string.IsNullOrWhiteSpace(recognizedDocument.OriginalFile.Description) ? recognizedDocument.OriginalFile.Description : Resources.SimpleDocumentName;
      document.Note = recognizedDocument.Message;
      CreateVersion(document, recognizedDocument);
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать документ из тела email.
    /// </summary>
    /// <param name="mailInfo">Информация о захваченном письме.</param>
    /// <param name="bodyInfo">Путь до тела email.</param>
    /// <returns>ИД созданного документа.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromEmailBody(Structures.Module.CapturedMailInfo mailInfo, Structures.Module.IFileInfo bodyInfo)
    {
      if (!System.IO.File.Exists(bodyInfo.Path))
        throw new ApplicationException(Resources.FileNotFoundFormat(bodyInfo.Path));
      
      var document = Sungero.Docflow.SimpleDocuments.Create();
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      document.Name = string.Format("Email от {0}", mailInfo.FromEmail);
      if (!string.IsNullOrWhiteSpace(mailInfo.Subject))
        document.Name = string.Format("{0} \"{1}\"", document.Name, mailInfo.Subject);
      document.Subject = string.Format("Subject: {0}\nEmail from: {1} {2}", mailInfo.Subject, mailInfo.FromEmail, mailInfo.Name);
      
      var application = GetAssociatedApplicationByFileName(bodyInfo.Path);
      using (var body = new MemoryStream(bodyInfo.Data))
      {
        document.CreateVersion();
        var version = document.LastVersion;
        version.Body.Write(body);
        version.AssociatedApplication = application;
      }
      document.Save();
      
      return document;
    }
    
    /// <summary>
    /// Создать простой документ из файла.
    /// </summary>
    /// <param name="File">Файл.</param>
    /// <returns>Простой документ.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromFile(Structures.Module.IFileInfo fileInfo)
    {
      var document = Sungero.Docflow.SimpleDocuments.Create();
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      document.Name = Path.GetFileName(fileInfo.Description);
      document.Save();
      
      var application = GetAssociatedApplicationByFileName(fileInfo.Path);
      using (var body = new MemoryStream(fileInfo.Data))
      {
        document.CreateVersion();
        var version = document.LastVersion;
        version.Body.Write(body);
        version.AssociatedApplication = application;
      }
      document.Save();
      
      return document;
    }
    
    #endregion
    
    #region Входящее письмо
    
    /// <summary>
    /// Создать входящее письмо в RX.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateIncomingLetter(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      CreateVersion(document, recognizedDocument);
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      var subjectFact = GetOrderedFacts(facts, "Letter", "Subject").FirstOrDefault();
      var subject = GetFieldValue(subjectFact, "Subject");
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        LinkFactAndProperty(recognizedDocument, subjectFact, "Subject", props.Subject.Name, document.Subject);
      }
      
      // Адресат.
      var addresseeFact = GetOrderedFacts(facts, "Letter", "Addressee").FirstOrDefault();
      var addressee = GetFieldValue(addresseeFact, "Addressee");
      document.Addressee = GetEmployeeByName(addressee);
      LinkFactAndProperty(recognizedDocument, addresseeFact, "Addressee", props.Addressee.Name, document.Addressee);
      
      // Заполнить данные корреспондента.
      var correspondent = GetCounterparty(facts, props.Correspondent.Name);
      if (correspondent != null)
      {
        document.Correspondent = correspondent.Counterparty;
        LinkFactAndProperty(recognizedDocument, correspondent.Fact, null, props.Correspondent.Name, document.Correspondent, correspondent.IsTrusted);
      }
      
      // Дата номер.
      var dateFact = GetOrderedFacts(facts, "Letter", "Date").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, "Letter", "Number").FirstOrDefault();
      document.Dated = GetFieldDateTimeValue(dateFact, "Date");
      document.InNumber = GetFieldValue(numberFact, "Number");
      LinkFactAndProperty(recognizedDocument, dateFact, "Date", props.Dated.Name, document.Dated);
      LinkFactAndProperty(recognizedDocument, numberFact, "Number", props.InNumber.Name, document.InNumber);
      
      // Заполнить данные нашей стороны.
      // Убираем уже использованный факт для подбора контрагента, чтобы организация не искалась по тем же реквизитам что и контрагент.
      if (correspondent != null)
        facts.Remove(correspondent.Fact);
      var businessUnitsWithFacts = GetBusinessUnitsWithFacts(facts);
      
      var businessUnitWithFact = GetBusinessUnitWithFact(businessUnitsWithFacts, responsible, document.Addressee);
      document.BusinessUnit = businessUnitWithFact.BusinessUnit;
      LinkFactAndProperty(recognizedDocument, businessUnitWithFact.Fact, null, props.BusinessUnit.Name, document.BusinessUnit, businessUnitWithFact.IsTrusted);
      
      document.Department = document.Addressee != null
        ? GetDepartment(document.Addressee)
        : GetDepartment(responsible);
      
      // Заполнить подписанта.
      var personFacts = GetOrderedFacts(facts, "LetterPerson", "Surname");     
      var signatoryFact = personFacts.Where(x => GetFieldValue(x, "Type") == "SIGNATORY").FirstOrDefault();
      var signedBy = GetContactByFact(signatoryFact, document.Info.Properties.SignedBy.Name, document.Correspondent, document.Info.Properties.Correspondent.Name);
      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && signedBy.Contact != null)
      {
        LinkFactAndProperty(recognizedDocument, null, null, props.Correspondent.Name, signedBy.Contact.Company, signedBy.IsTrusted);
      }  
      document.SignedBy = signedBy.Contact;
      var isTrustedSignatory = IsTrustedField(signatoryFact, "Type");
      LinkFactAndProperty(recognizedDocument, signatoryFact, null, props.SignedBy.Name, document.SignedBy, isTrustedSignatory);
      
      // Заполнить контакт.      
      var responsibleFact = personFacts.Where(x => GetFieldValue(x, "Type") == "RESPONSIBLE").FirstOrDefault();
      var contact = GetContactByFact(responsibleFact, document.Info.Properties.Contact.Name, document.Correspondent, document.Info.Properties.Correspondent.Name);      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && contact.Contact != null)
      {
        LinkFactAndProperty(recognizedDocument, null, null, props.Correspondent.Name, contact.Contact.Company, contact.IsTrusted);
      }
      document.Contact = contact.Contact;
      var isTrustedContact = IsTrustedField(responsibleFact, "Type");
      LinkFactAndProperty(recognizedDocument, responsibleFact, null, props.Contact.Name, document.Contact, isTrustedContact);
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо с текстовыми полями.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingLetter(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockIncomingLetters.Create();
      var props = document.Info.Properties;
      var facts = recognizedDocument.Facts;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // Заполнить дату и номер письма со стороны корреспондента.
      var dateFact = GetOrderedFacts(facts, "Letter", "Date").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, "Letter", "Number").FirstOrDefault();
      document.InNumber = GetFieldValue(numberFact, "Number");
      document.Dated = Functions.Module.GetShortDate(GetFieldValue(dateFact, "Date"));
      LinkFactAndProperty(recognizedDocument, dateFact, "Date", props.Dated.Name, document.Dated);
      LinkFactAndProperty(recognizedDocument, numberFact, "Number", props.InNumber.Name, document.InNumber);
      
      // Заполнить данные корреспондента.
      var correspondentNameFacts = GetOrderedFacts(facts, "Letter", "CorrespondentName");
      if (correspondentNameFacts.Count() > 0)
      {
        var fact = correspondentNameFacts.First();
        document.Correspondent = GetCorrespondentName(fact, "CorrespondentName", "CorrespondentLegalForm");
        LinkFactAndProperty(recognizedDocument, fact, "CorrespondentName", props.Correspondent.Name, document.Correspondent);
      }
      if (correspondentNameFacts.Count() > 1)
      {
        var fact = correspondentNameFacts.Last();
        document.Recipient = GetCorrespondentName(fact, "CorrespondentName", "CorrespondentLegalForm");
        LinkFactAndProperty(recognizedDocument, fact, "CorrespondentName", props.Recipient.Name, document.Recipient);
      }
      
      // Заполнить ИНН/КПП для КА и НОР.
      var tinTrrcFacts = GetOrderedFacts(facts, "Counterparty", "TIN");
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.CorrespondentTin = GetFieldValue(fact, "TIN");
        document.CorrespondentTrrc = GetFieldValue(fact, "TRRC");
        LinkFactAndProperty(recognizedDocument, fact, "TIN", props.CorrespondentTin.Name, document.CorrespondentTin);
        LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.CorrespondentTrrc.Name, document.CorrespondentTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.RecipientTin = GetFieldValue(fact, "TIN");
        document.RecipientTrrc = GetFieldValue(fact, "TRRC");
        LinkFactAndProperty(recognizedDocument, fact, "TIN", props.RecipientTin.Name, document.RecipientTin);
        LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.RecipientTrrc.Name, document.RecipientTrrc);
      }
      
      // В ответ на.
      var responseToNumberFact = GetOrderedFacts(facts, "Letter", "ResponseToNumber").FirstOrDefault();
      var responseToNumber = GetFieldValue(responseToNumberFact, "ResponseToNumber");
      var responseToDateFact = GetOrderedFacts(facts, "Letter", "ResponseToDate").FirstOrDefault();
      var responseToDate = Functions.Module.GetShortDate(GetFieldValue(facts, "Letter", "ResponseToDate"));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? responseToNumber
        : string.Format("{0} {1} {2}", responseToNumber, Sungero.Docflow.Resources.From, responseToDate);
      LinkFactAndProperty(recognizedDocument, responseToNumberFact, "ResponseToNumber", props.InResponseTo.Name, document.InResponseTo);
      LinkFactAndProperty(recognizedDocument, responseToDateFact, "ResponseToDate", props.InResponseTo.Name, document.InResponseTo);
      
      // Заполнить подписанта.
      var personFacts = GetOrderedFacts(facts, "LetterPerson", "Surname");
      if (document.Signatory == null)
      {
        var signatoryFact = personFacts.Where(x => GetFieldValue(x, "Type") == "SIGNATORY").FirstOrDefault();
        document.Signatory = GetFullNameByFact(signatoryFact);
        var isTrusted = IsTrustedField(signatoryFact, "Type");
        LinkFactAndProperty(recognizedDocument, signatoryFact, null, props.Signatory.Name, document.Signatory, isTrusted);
      }
      
      // Заполнить контакт.
      if (document.Contact == null)
      {
        var responsibleFact = personFacts.Where(x => GetFieldValue(x, "Type") == "RESPONSIBLE").FirstOrDefault();
        document.Contact = GetFullNameByFact(responsibleFact);
        var isTrusted = IsTrustedField(responsibleFact, "Type");
        LinkFactAndProperty(recognizedDocument, responsibleFact, null, props.Contact.Name, document.Contact, isTrusted);
      }
      
      // Заполнить данные нашей стороны.
      var addresseeFacts = GetFacts(facts, "Letter", "Addressee");
      foreach (var fact in addresseeFacts)
      {
        var addressee = GetFieldValue(fact, "Addressee");
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? addressee : string.Format("{0}; {1}", document.Addressees, addressee);
      }
      foreach (var fact in addresseeFacts)
        LinkFactAndProperty(recognizedDocument, fact, null, props.Addressees.Name, document.Addressees, true);
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subjectFact = GetOrderedFacts(facts, "Letter", "Subject").FirstOrDefault();
      var subject = GetFieldValue(subjectFact, "Subject");
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        LinkFactAndProperty(recognizedDocument, subjectFact, "Subject", props.Subject.Name, document.Subject);
      }
      CreateVersion(document, recognizedDocument);
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Акт
    
    /// <summary>
    /// Создать акт выполненных работ (демо режим).
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <returns>Акт выполненных работ.</returns>
    public static Docflow.IOfficialDocument CreateMockContractStatement(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadDoc = GetLeadingDocumentName(leadingDocFact);
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadDoc.Name, document.LeadDoc, true);
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "Document");
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, "SELLER");
      if (seller != null)
      {
        document.CounterpartyName = seller.Name;
        document.CounterpartyTin = seller.Tin;
        document.CounterpartyTrrc = seller.Trrc;
        LinkFactAndProperty(recognizedDocument, seller.Fact, "Name", props.CounterpartyName.Name, seller.Name);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "LegalForm", props.CounterpartyName.Name, seller.Name);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "TIN", props.CounterpartyTin.Name, seller.Tin);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "TRRC", props.CounterpartyTrrc.Name, seller.Trrc);
      }
      var buyer = GetMostProbableMockCounterparty(facts, "BUYER");
      if (buyer != null)
      {
        document.BusinessUnitName = buyer.Name;
        document.BusinessUnitTin = buyer.Tin;
        document.BusinessUnitTrrc = buyer.Trrc;
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "Name", props.BusinessUnitName.Name, buyer.Name);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "LegalForm", props.BusinessUnitName.Name, buyer.Name);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "TIN", props.BusinessUnitTin.Name, buyer.Tin);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "TRRC", props.BusinessUnitTrrc.Name, buyer.Trrc);
      }
      
      // В актах могут прийти контрагенты без типа. Заполнить контрагентами без типа.
      if (seller == null || buyer == null)
      {
        var withoutTypeFacts = GetFacts(facts, "Counterparty", "Name")
          .Where(f => string.IsNullOrWhiteSpace(GetFieldValue(f, "CounterpartyType")))
          .OrderByDescending(x => x.Fields.First(f => f.Name == "Name").Probability);
        foreach (var fact in withoutTypeFacts)
        {
          var name = GetCorrespondentName(fact, "Name", "LegalForm");
          
          var tin = GetFieldValue(fact, "TIN");
          var trrc = GetFieldValue(fact, "TRRC");
          var type = GetFieldValue(fact, "CounterpartyType");
          
          if (string.IsNullOrWhiteSpace(document.CounterpartyName))
          {
            document.CounterpartyName = name;
            document.CounterpartyTin = tin;
            document.CounterpartyTrrc = trrc;
            LinkFactAndProperty(recognizedDocument, fact, "Name", props.CounterpartyName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "LegalForm", props.CounterpartyName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "TIN", props.CounterpartyTin.Name, tin);
            LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.CounterpartyTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BusinessUnitName))
          {
            document.BusinessUnitName = name;
            document.BusinessUnitTin = tin;
            document.BusinessUnitTrrc = trrc;
            LinkFactAndProperty(recognizedDocument, fact, "Name", props.BusinessUnitName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "LegalForm", props.BusinessUnitName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "TIN", props.BusinessUnitTin.Name, tin);
            LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.BusinessUnitTrrc.Name, trrc);
          }
        }
      }
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      var currencyCode = GetFieldValue(documentAmountFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentAmountFact, "Currency", props.Currency.Name, document.Currency.Name);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, "Goods", "Name"))
      {
        var good = document.Goods.AddNew();
        good.Name = GetFieldValue(fact, "Name");
        good.UnitName = GetFieldValue(fact, "UnitName");
        good.Count = GetFieldNumericalValue(fact, "Count");
        good.Price = GetFieldNumericalValue(fact, "Price");
        good.VatAmount = GetFieldNumericalValue(fact, "VatAmount");
        good.TotalAmount = GetFieldNumericalValue(fact, "Amount");
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognizedDocument, fact, "Name", string.Format(formatter, props.Goods.Properties.Name.Name), good.Name);
        LinkFactAndProperty(recognizedDocument, fact, "UnitName", string.Format(formatter, props.Goods.Properties.UnitName.Name), good.UnitName);
        LinkFactAndProperty(recognizedDocument, fact, "Count", string.Format(formatter, props.Goods.Properties.Count.Name), good.Count);
        LinkFactAndProperty(recognizedDocument, fact, "Price", string.Format(formatter, props.Goods.Properties.Price.Name), good.Price);
        LinkFactAndProperty(recognizedDocument, fact, "VatAmount", string.Format(formatter, props.Goods.Properties.VatAmount.Name), good.VatAmount);
        LinkFactAndProperty(recognizedDocument, fact, "Amount", string.Format(formatter, props.Goods.Properties.TotalAmount.Name), good.TotalAmount);
      }
      CreateVersion(document, recognizedDocument);
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать акт выполненных работ.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Акт выполненных работ.</returns>
    public virtual Docflow.IOfficialDocument CreateContractStatement(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = FinancialArchive.ContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "Document");
      
      // Заполнить контрагента/НОР по типу.
      var counterpartyAndBusinessUnit = GetCounterpartyAndBusinessUnit(recognizedDocument, responsible);
      document.BusinessUnit = counterpartyAndBusinessUnit.BusinessUnit;
      document.Counterparty = counterpartyAndBusinessUnit.Counterparty;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadingDocument = GetLeadingDocument(leadingDocFact, document.Counterparty);
      var isTrusted = IsTrustedField(leadingDocFact, "Type");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      CreateVersion(document, recognizedDocument);
      // Регистрация.
      RegisterDocument(document);
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки накладной в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockWaybill(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockWaybills.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = IsTrustedField(leadingDocFact, "Type");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var counterpartyFacts = GetFacts(facts, "Counterparty", "Name");
      var shipper = GetMostProbableMockCounterparty(facts, "SHIPPER");
      if (shipper != null)
      {
        document.Shipper = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "Name", props.Shipper.Name, shipper.Name);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "LegalForm", props.Shipper.Name, shipper.Name);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "TIN", props.ShipperTin.Name, shipper.Tin);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "TRRC", props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, "CONSIGNEE");
      if (consignee != null)
      {
        document.Consignee = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "Name", props.Consignee.Name, consignee.Name);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "LegalForm", props.Consignee.Name, consignee.Name);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "TIN", props.ConsigneeTin.Name, consignee.Tin);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "TRRC", props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var supplier = GetMostProbableMockCounterparty(facts, "SUPPLIER");
      if (supplier != null)
      {
        document.Supplier = supplier.Name;
        document.SupplierTin = supplier.Tin;
        document.SupplierTrrc = supplier.Trrc;
        LinkFactAndProperty(recognizedDocument, supplier.Fact, "Name", props.Supplier.Name, supplier.Name);
        LinkFactAndProperty(recognizedDocument, supplier.Fact, "LegalForm", props.Supplier.Name, supplier.Name);
        LinkFactAndProperty(recognizedDocument, supplier.Fact, "TIN", props.SupplierTin.Name, supplier.Tin);
        LinkFactAndProperty(recognizedDocument, supplier.Fact, "TRRC", props.SupplierTrrc.Name, supplier.Trrc);
      }
      
      var payer = GetMostProbableMockCounterparty(facts, "PAYER");
      if (payer != null)
      {
        document.Payer = payer.Name;
        document.PayerTin = payer.Tin;
        document.PayerTrrc = payer.Trrc;
        LinkFactAndProperty(recognizedDocument, payer.Fact, "Name", props.Payer.Name, payer.Name);
        LinkFactAndProperty(recognizedDocument, payer.Fact, "LegalForm", props.Payer.Name, payer.Name);
        LinkFactAndProperty(recognizedDocument, payer.Fact, "TIN", props.PayerTin.Name, payer.Tin);
        LinkFactAndProperty(recognizedDocument, payer.Fact, "TRRC", props.PayerTrrc.Name, payer.Trrc);
      }
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      var currencyCode = GetFieldValue(documentAmountFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentAmountFact, "Currency", props.Currency.Name, document.Currency.Name);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, "Goods", "Name"))
      {
        var good = document.Goods.AddNew();
        good.Name = GetFieldValue(fact, "Name");
        good.UnitName = GetFieldValue(fact, "UnitName");
        good.Count = GetFieldNumericalValue(fact, "Count");
        good.Price = GetFieldNumericalValue(fact, "Price");
        good.VatAmount = GetFieldNumericalValue(fact, "VatAmount");
        good.TotalAmount = GetFieldNumericalValue(fact, "Amount");
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognizedDocument, fact, "Name", string.Format(formatter, props.Goods.Properties.Name.Name), good.Name);
        LinkFactAndProperty(recognizedDocument, fact, "UnitName", string.Format(formatter, props.Goods.Properties.UnitName.Name), good.UnitName);
        LinkFactAndProperty(recognizedDocument, fact, "Count", string.Format(formatter, props.Goods.Properties.Count.Name), good.Count);
        LinkFactAndProperty(recognizedDocument, fact, "Price", string.Format(formatter, props.Goods.Properties.Price.Name), good.Price);
        LinkFactAndProperty(recognizedDocument, fact, "VatAmount", string.Format(formatter, props.Goods.Properties.VatAmount.Name), good.VatAmount);
        LinkFactAndProperty(recognizedDocument, fact, "Amount", string.Format(formatter, props.Goods.Properties.TotalAmount.Name), good.TotalAmount);
      }
      CreateVersion(document, recognizedDocument);
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать накладную.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки накладной в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Накладная.</returns>
    public virtual Docflow.IOfficialDocument CreateWaybill(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = FinancialArchive.Waybills.Create();
      var props = document.Info.Properties;
      var facts = recognizedDocument.Facts;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      
      // Заполнить контрагента/НОР по типу.
      var counterpartyAndBusinessUnit = GetCounterpartyAndBusinessUnit(recognizedDocument, responsible, "SUPPLIER", "PAYER");
      document.BusinessUnit = counterpartyAndBusinessUnit.BusinessUnit;
      document.Counterparty = counterpartyAndBusinessUnit.Counterparty;
      
      // Документ-основание.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadingDocument = GetLeadingDocument(leadingDocFact, document.Counterparty);
      var isTrusted = IsTrustedField(leadingDocFact, "Type");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      CreateVersion(document, recognizedDocument);
      
      // Регистрация.
      RegisterDocument(document);
      
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockIncomingTaxInvoices.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var shipper = GetMostProbableMockCounterparty(facts, "SHIPPER");
      if (shipper != null)
      {
        document.ShipperName = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "Name", props.ShipperName.Name, shipper.Name);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "LegalForm", props.ShipperName.Name, shipper.Name);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "TIN", props.ShipperTin.Name, shipper.Tin);
        LinkFactAndProperty(recognizedDocument, shipper.Fact, "TRRC", props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, "CONSIGNEE");
      if (consignee != null)
      {
        document.ConsigneeName = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "Name", props.ConsigneeName.Name, consignee.Name);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "LegalForm", props.ConsigneeName.Name, consignee.Name);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "TIN", props.ConsigneeTin.Name, consignee.Tin);
        LinkFactAndProperty(recognizedDocument, consignee.Fact, "TRRC", props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var seller = GetMostProbableMockCounterparty(facts, "SELLER");
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        LinkFactAndProperty(recognizedDocument, seller.Fact, "Name", props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "LegalForm", props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "TIN", props.SellerTin.Name, seller.Tin);
        LinkFactAndProperty(recognizedDocument, seller.Fact, "TRRC", props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, "BUYER");
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "Name", props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "LegalForm", props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "TIN", props.BuyerTin.Name, buyer.Tin);
        LinkFactAndProperty(recognizedDocument, buyer.Fact, "TRRC", props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      var currencyCode = GetFieldValue(documentAmountFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentAmountFact, "Currency", props.Currency.Name, document.Currency.Name);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, "Goods", "Name"))
      {
        var good = document.Goods.AddNew();
        good.Name = GetFieldValue(fact, "Name");
        good.UnitName = GetFieldValue(fact, "UnitName");
        good.Count = GetFieldNumericalValue(fact, "Count");
        good.Price = GetFieldNumericalValue(fact, "Price");
        good.VatAmount = GetFieldNumericalValue(fact, "VatAmount");
        good.TotalAmount = GetFieldNumericalValue(fact, "Amount");
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognizedDocument, fact, "Name", string.Format(formatter, props.Goods.Properties.Name.Name), good.Name);
        LinkFactAndProperty(recognizedDocument, fact, "UnitName", string.Format(formatter, props.Goods.Properties.UnitName.Name), good.UnitName);
        LinkFactAndProperty(recognizedDocument, fact, "Count", string.Format(formatter, props.Goods.Properties.Count.Name), good.Count);
        LinkFactAndProperty(recognizedDocument, fact, "Price", string.Format(formatter, props.Goods.Properties.Price.Name), good.Price);
        LinkFactAndProperty(recognizedDocument, fact, "VatAmount", string.Format(formatter, props.Goods.Properties.VatAmount.Name), good.VatAmount);
        LinkFactAndProperty(recognizedDocument, fact, "Amount", string.Format(formatter, props.Goods.Properties.TotalAmount.Name), good.TotalAmount);
      }
      CreateVersion(document, recognizedDocument);
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать счет-фактуру.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns></returns>
    public virtual Docflow.IOfficialDocument CreateTaxInvoice(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible, bool isAdjustment)
    {
      // Если НОР выступает продавцом, то создаем исходящую счет-фактуру, иначе - входящую.
      IAccountingDocumentBase document = null;
      var counterpartyAndBusinessUnit = GetCounterpartyAndBusinessUnit(recognizedDocument, responsible);
      if (counterpartyAndBusinessUnit.IsBusinessUnitSeller == true)
        document = FinancialArchive.OutgoingTaxInvoices.Create();
      else
        document = FinancialArchive.IncomingTaxInvoices.Create();
      
      document.BusinessUnit = counterpartyAndBusinessUnit.BusinessUnit;
      document.Counterparty = counterpartyAndBusinessUnit.Counterparty;
      
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadingDocument = GetLeadingDocument(leadingDocFact, document.Counterparty);
      var isTrusted = IsTrustedField(leadingDocFact, "Type");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      
      // Корректировочный документ.
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionDate").FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionNumber").FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, "CorrectionDate");
        var correctionNumber = GetFieldValue(correctionNumberFact, "CorrectionNumber");
        if (correctionDate != null && !string.IsNullOrEmpty(correctionNumber))
        {
          if (FinancialArchive.IncomingTaxInvoices.Is(document))
          {
            document.Corrected = FinancialArchive.IncomingTaxInvoices.GetAll()
              .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
          }
          else
          {
            document.Corrected = FinancialArchive.OutgoingTaxInvoices.GetAll()
              .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
          }
          LinkFactAndProperty(recognizedDocument, correctionDateFact, "CorrectionDate", props.Corrected.Name, document.Corrected, true);
          LinkFactAndProperty(recognizedDocument, correctionNumberFact, "CorrectionNumber", props.Corrected.Name, document.Corrected, true);
        }
      }
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      CreateVersion(document, recognizedDocument);
      
      // Регистрация.
      RegisterDocument(document);
      document.Save();
      return document;
    }
    
    #endregion
    
    #region УПД
    
    /// <summary>
    /// Создать УПД.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки УПД в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns></returns>
    public virtual Docflow.IOfficialDocument CreateUniversalTransferDocument(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible, bool isAdjustment)
    {
      var document = Sungero.FinancialArchive.UniversalTransferDocuments.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и контрагент.
      var counterpartyAndBusinessUnit = GetCounterpartyAndBusinessUnit(recognizedDocument, responsible);
      document.BusinessUnit = counterpartyAndBusinessUnit.BusinessUnit;
      document.Counterparty = counterpartyAndBusinessUnit.Counterparty;
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      
      // Корректировочный документ.
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionDate").FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionNumber").FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, "CorrectionDate");
        var correctionNumber = GetFieldValue(correctionNumberFact, "CorrectionNumber");
        document.Corrected = FinancialArchive.UniversalTransferDocuments.GetAll()
          .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
        LinkFactAndProperty(recognizedDocument, correctionDateFact, "CorrectionDate", props.Corrected.Name, document.Corrected, true);
        LinkFactAndProperty(recognizedDocument, correctionNumberFact, "CorrectionNumber", props.Corrected.Name, document.Corrected, true);
      }
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      CreateVersion(document, recognizedDocument);
      
      // Регистрация.
      RegisterDocument(document);
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Счет на оплату
    
    /// <summary>
    /// Создать счет на оплату.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Счет на оплату.</returns>
    public virtual Docflow.IOfficialDocument CreateIncomingInvoice(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = Contracts.IncomingInvoices.Create();
      var counterpartyAndBusinessUnit = GetCounterpartyAndBusinessUnit(recognizedDocument, responsible);
      document.BusinessUnit = counterpartyAndBusinessUnit.BusinessUnit;
      document.Counterparty = counterpartyAndBusinessUnit.Counterparty;
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadingDocument = GetLeadingDocument(leadingDocFact, document.Counterparty);
      var isTrusted = IsTrustedField(leadingDocFact, "Type");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Дата и номер.
      var DateFact = GetOrderedFacts(facts, "FinancialDocument", "Date").FirstOrDefault();
      var NumberFact = GetOrderedFacts(facts, "FinancialDocument", "Number").FirstOrDefault();
      document.Date = GetFieldDateTimeValue(DateFact, "Date");
      document.Number = GetFieldValue(NumberFact, "Number");
      LinkFactAndProperty(recognizedDocument, DateFact, "Date", props.Date.Name, document.RegistrationDate);
      LinkFactAndProperty(recognizedDocument, NumberFact, "Number", props.Number.Name, document.RegistrationNumber);
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      CreateVersion(document, recognizedDocument);
      
      // Регистрация.
      RegisterDocument(document);
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Заполнение свойств документа
    
    /// <summary>
    /// Заполнить сумму и валюту.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    public static void FillAmount(IAccountingDocumentBase document, Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var facts = recognizedDocument.Facts;
      var documentAmountFacts = GetOrderedFacts(facts, "DocumentAmount", "Amount");
      var documentAmountFact = documentAmountFacts.Where(a => a.Fields.Any(field => field.Name == "Currency")).FirstOrDefault();
      if (documentAmountFact == null)
        documentAmountFact = documentAmountFacts.FirstOrDefault();
      
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      var currencyCode = GetFieldValue(documentAmountFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      
      var props = document.Info.Properties;
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentAmountFact, "Currency", props.Currency.Name, document.Currency);
    }
    
    /// <summary>
    /// Заполнить регистрационные данные.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта.</param>
    public static void FillRegistrationData(IOfficialDocument document, Structures.Module.IRecognizedDocument recognizedDocument, string factName)
    {
      var facts = recognizedDocument.Facts;
      var regDateFact = GetOrderedFacts(facts, factName, "Date").FirstOrDefault();
      var regNumberFact = GetOrderedFacts(facts, factName, "Number").FirstOrDefault();
      document.RegistrationDate = GetFieldDateTimeValue(regDateFact, "Date");
      document.RegistrationNumber = GetFieldValue(regNumberFact, "Number");
      
      var props = document.Info.Properties;
      LinkFactAndProperty(recognizedDocument, regDateFact, "Date", props.RegistrationDate.Name, document.RegistrationDate);
      LinkFactAndProperty(recognizedDocument, regNumberFact, "Number", props.RegistrationNumber.Name, document.RegistrationNumber);
    }
    
    /// <summary>
    /// Заполнить регистрационные данные.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта.</param>
    public static IAccountingDocumentBase GetCorrectedDocument(Structures.Module.IRecognizedDocument recognizedDocument, string factName)
    {
      var facts = recognizedDocument.Facts;
      var dateFact = GetOrderedFacts(facts, factName, "CorrectionDate").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, factName, "CorrectionNumber").FirstOrDefault();
      var date = GetFieldDateTimeValue(dateFact, "Date");
      var number = GetFieldValue(numberFact, "Number");
      if (date != null && !string.IsNullOrEmpty(number))
      {
        return AccountingDocumentBases.GetAll().FirstOrDefault(d => d.RegistrationNumber.Equals(number, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == date);
      }
      return null;
    }
    
    #endregion
    
    #region Поиск контрагента/НОР
    
    private static List<IFact> GetCounterpartyFacts(List<Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetOrderedFacts(facts, "Counterparty", "Name")
        .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType);
      
      if (!counterpartyFacts.Any())
        counterpartyFacts = GetOrderedFacts(facts, "Counterparty", "TIN")
          .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType);
      
      return counterpartyFacts.ToList();
    }
    
    /// <summary>
    /// Получить наши организации и контрагентов с фактами.
    /// </summary>
    /// <param name="facts">Факты.</param>
    /// <param name="counterpartyTypeFrom">Тип контрагента-отправителя.</param>
    /// <param name="counterpartyTypeTo">Тип контрагента-получателя.</param>
    /// <returns>Наши организации и контрагенты, найденные по фактам.</returns>
    public static List<Structures.Module.BusinessUnitAndCounterpartyWithFact> GetBusinessUnitsAndCounterparties(List<Structures.Module.IFact> facts,
                                                                                                                string counterpartyTypeFrom = "SELLER",
                                                                                                                string counterpartyTypeTo = "BUYER")
    {
      var businessUnitsAndCounterparties = new List<Structures.Module.BusinessUnitAndCounterpartyWithFact>();
      
      var counterpartyFacts = GetCounterpartyFacts(facts, string.Empty);
      counterpartyFacts.AddRange(GetCounterpartyFacts(facts, counterpartyTypeFrom));
      counterpartyFacts.AddRange(GetCounterpartyFacts(facts, counterpartyTypeTo));
      foreach (var fact in counterpartyFacts)
      {
        var tin = GetFieldValue(fact, "TIN");
        var trrc = GetFieldValue(fact, "TRRC");
        var businessUnit = GetBusinessUnits(tin, trrc).FirstOrDefault();
        var counterparty = GetCounterparties(tin, trrc).FirstOrDefault();
        
        if (counterparty != null || businessUnit != null)
        {
          var businessUnitAndCounterparty = Structures.Module.BusinessUnitAndCounterpartyWithFact.Create(businessUnit, counterparty, fact, GetFieldValue(fact, "CounterpartyType"), false);
          businessUnitsAndCounterparties.Add(businessUnitAndCounterparty);
        }
      }
      
      return businessUnitsAndCounterparties;
    }
    
    /// <summary>
    /// Получить контрагента и НОР.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <param name="counterpartyTypeFrom">Тип контрагента-отправителя.</param>
    /// <param name="counterpartyTypeTo">Тип контрагента-получателя.</param>
    /// <returns>Наша организация и контрагент.</returns>
    /// <remarks>Типы контрагентов BUYER и SELLER используются в большем количестве типов, поэтому они выбраны по умолчанию.</remarks>
    public static Structures.Module.BusinessUnitAndCounterparty GetCounterpartyAndBusinessUnit(Structures.Module.IRecognizedDocument recognizedDocument,
                                                                                               IEmployee responsible,
                                                                                               string counterpartyTypeFrom = "SELLER",
                                                                                               string counterpartyTypeTo = "BUYER")
    {
      var result = new BusinessUnitAndCounterparty();
      var facts = recognizedDocument.Facts;
      var props = AccountingDocumentBases.Info.Properties;
      var businessUnitByResponsible = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
      Structures.Module.BusinessUnitAndCounterpartyWithFact businessUnitWithFact = null;
      
      var businessUnitsAndCounterparties = GetBusinessUnitsAndCounterparties(facts, counterpartyTypeFrom, counterpartyTypeTo);
      
      // Искать НОР.
      var businessUnits = businessUnitsAndCounterparties.Where(x => x.BusinessUnit != null);
      var buyerBusinessUnits = businessUnits.Where(x => x.Type == counterpartyTypeTo);
      var sellerBusinessUnits = businessUnits.Where(x => x.Type == counterpartyTypeFrom);
      var withoutTypeBusinessUnits = businessUnits.Where(x => x.Type == string.Empty);
      
      // Уточнить по ответственному
      businessUnitWithFact = buyerBusinessUnits.Where(x => Equals(x.BusinessUnit, businessUnitByResponsible)).FirstOrDefault();
      if (businessUnitWithFact == null)
        businessUnitWithFact = withoutTypeBusinessUnits.Where(x => Equals(x.BusinessUnit, businessUnitByResponsible)).FirstOrDefault();
      if (businessUnitWithFact == null)
        businessUnitWithFact = sellerBusinessUnits.Where(x => Equals(x.BusinessUnit, businessUnitByResponsible)).FirstOrDefault();
      if (businessUnitWithFact != null)
      {
        var isTypeEmpty = string.IsNullOrWhiteSpace(businessUnitWithFact.Type);
        businessUnitWithFact.IsTrusted = isTypeEmpty;
        if (isTypeEmpty)
          result.IsBusinessUnitSeller = businessUnitWithFact.Type == counterpartyTypeFrom;
        else
          result.IsBusinessUnitSeller = null;
      }
      
      // Общий пиоритет поиска НОР, если не смогли уточнить по ответственному:
      //   1. Явно найденная для типа контрагента counterpartyTypeTo. По умолчанию "BUYER".
      //   2. Явно найденная в контрагентах без типов.
      //   3. Явно найденная для типа контрагента counterpartyTypeFrom. По умолчанию "SELLER".
      if (businessUnitWithFact == null)
      {
        var buyerBusinessUnit = buyerBusinessUnits.FirstOrDefault();
        var sellerBusinessUnit = sellerBusinessUnits.FirstOrDefault();
        var withoutTypeBusinessUnit = withoutTypeBusinessUnits.FirstOrDefault();
        
        businessUnitWithFact = buyerBusinessUnit ?? withoutTypeBusinessUnit ?? sellerBusinessUnit;
        result.IsBusinessUnitSeller = sellerBusinessUnit == businessUnitWithFact;
      }
      
      // Заполнить из ответственного, если не смогли определить по фактам.
      if (businessUnitWithFact == null)
      {
        businessUnitWithFact = Structures.Module.BusinessUnitAndCounterpartyWithFact.Create(businessUnitByResponsible, null, null, string.Empty, false);
        result.IsBusinessUnitSeller = false;
      }
      
      result.BusinessUnit = businessUnitWithFact.BusinessUnit;
      LinkFactAndProperty(recognizedDocument, businessUnitWithFact.Fact, null, props.BusinessUnit.Name, result.BusinessUnit, businessUnitWithFact.IsTrusted);
      

      // Исключить из поиска НОР.
      var сounterparties = businessUnitsAndCounterparties.Where(x => !Equals(x, businessUnitWithFact) && x.Counterparty != null).ToList();

      // Исключить из поиска контрагентов с типом НОР.
      if (result.IsBusinessUnitSeller == true)
        сounterparties = сounterparties.Where(x => x.Type != counterpartyTypeFrom).ToList();
      else
        if (result.IsBusinessUnitSeller == false)
          сounterparties = сounterparties.Where(x => x.Type != counterpartyTypeTo).ToList();


      var buyerCounterparty = сounterparties.Where(x => x.Type == counterpartyTypeTo).FirstOrDefault();
      var sellerCounterparty = сounterparties.Where(x => x.Type == counterpartyTypeFrom).FirstOrDefault();
      var withoutTypeCounterparty = сounterparties.Where(x => x.Type == string.Empty).FirstOrDefault();

      var counterpartyWithFact = sellerCounterparty ?? withoutTypeCounterparty ?? buyerCounterparty;
      if (counterpartyWithFact != null)
      {
        result.Counterparty = counterpartyWithFact.Counterparty;
        LinkFactAndProperty(recognizedDocument, counterpartyWithFact.Fact, null,
                            props.Counterparty.Name, counterpartyWithFact.Counterparty, counterpartyWithFact.IsTrusted);
      }

      // Если НОР нашли без типа, то пытаемся уточнить по типу контрагента.
      result.IsBusinessUnitSeller = result.IsBusinessUnitSeller ?? !Equals(sellerCounterparty, counterpartyWithFact);

      return result;
    }
    
    public static Structures.Module.MockCounterparty GetMostProbableMockCounterparty(List<Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetOrderedFacts(facts, "Counterparty", "Name");
      var mostProbabilityFact = counterpartyFacts.Where(f =>  GetFieldValue(f, "CounterpartyType") == counterpartyType).FirstOrDefault();
      if (mostProbabilityFact == null)
        return null;

      var counterparty = Structures.Module.MockCounterparty.Create();
      counterparty.Name = GetCorrespondentName(mostProbabilityFact, "Name", "LegalForm");
      counterparty.Tin = GetFieldValue(mostProbabilityFact, "TIN");
      counterparty.Trrc = GetFieldValue(mostProbabilityFact, "TRRC");
      counterparty.Fact = mostProbabilityFact;
      return counterparty;
    }
    
    /// <summary>
    /// Поиск корреспондента по извлеченным фактам.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Корреспондент.</returns>
    public static Structures.Module.CounterpartyWithFact GetCounterparty(List<Structures.Module.IFact> facts, string propertyName)
    {
      var filteredCounterparties = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
        .Where(x => x.Note == null || !x.Note.Equals(BusinessUnits.Resources.BusinessUnitComment));
      
      var foundByName = new List<Structures.Module.CounterpartyWithFact>();
      var correspondentNames = new List<string>();
      
      // Получить ИНН/КПП и наименования + форму собственности контрагентов из фактов.
      foreach (var fact in GetFacts(facts, "Letter", "CorrespondentName"))
      {
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, propertyName);
        if (verifiedCounterparty != null)
          return verifiedCounterparty;

        var name = GetCorrespondentName(fact, "CorrespondentName", "CorrespondentLegalForm");
        correspondentNames.Add(name);
        var counterparties = filteredCounterparties
          .Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        foreach (var counterparty in counterparties)
        {
          var counterpartyWithFact = Structures.Module.CounterpartyWithFact.Create(counterparty, fact, false);
          foundByName.Add(counterpartyWithFact);
        }
      }
      
      // Если факты с ИНН/КПП не найдены, то вернуть корреспондента по наименованию.
      var correspondentTINs = GetFacts(facts, "Counterparty", "TIN");
      if (!correspondentTINs.Any())
        return foundByName.FirstOrDefault();
      else
      {
        // Поиск по ИНН/КПП.
        var foundByTin = new List<Structures.Module.CounterpartyWithFact>();
        foreach (var fact in correspondentTINs)
        {
        	var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, propertyName);
        	if (verifiedCounterparty != null)
        		return verifiedCounterparty;

          var tin = GetFieldValue(fact, "TIN");
          var trrc = GetFieldValue(fact, "TRRC");
          var counterparties = GetCounterparties(tin, trrc);
          foreach (var counterparty in counterparties)
          {
            var counterpartyWithFact = Structures.Module.CounterpartyWithFact.Create(counterparty, fact, true);
            foundByTin.Add(counterpartyWithFact);
          }
        }
        
        // Найден ровно 1.
        if (foundByTin.Count == 1)
          return foundByTin.First();
        
        // Найдено 0. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
        if (!foundByTin.Any())
          return foundByName
            .Where(x => string.IsNullOrEmpty(x.Counterparty.TIN))
            .Where(x => !CompanyBases.Is(x.Counterparty) || string.IsNullOrEmpty(CompanyBases.As(x.Counterparty).TRRC))
            .FirstOrDefault();

        // Найдено несколько. Уточнить поиск по наименованию.
        foundByName = foundByTin.Where(x => correspondentNames.Any(n => n == x.Counterparty.Name)).ToList();
        if (foundByName.Any())
          return foundByName.FirstOrDefault();
        else
          return foundByTin.FirstOrDefault();
      }
    }
    
    /// <summary>
    /// Получить контрагента по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связку контрагент + факт.</returns>
    public static Structures.Module.CounterpartyWithFact GetCounterpartyByVerifiedData(Structures.Module.IFact fact, string propertyName)
    {
      var factLabel = GetFactLabel(fact, propertyName);
      var recognitionInfo = DocumentRecognitionInfos.GetAll()
        .Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return null;
      
      var fieldRecognitionInfo = recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
      int counterpartyId;
      if (!int.TryParse(fieldRecognitionInfo.VerifiedValue, out counterpartyId))
        return null;
      
      var filteredCounterparty = Counterparties.GetAll(x => x.Id == counterpartyId).FirstOrDefault();
      if (filteredCounterparty == null)
        return null;
      
      return Structures.Module.CounterpartyWithFact.Create(filteredCounterparty, fact, fieldRecognitionInfo.IsTrusted == true);
    }
    
    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного и адресата.
    /// </summary>
    /// <param name="businessUnits">НОР, найденные по фактам.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="addressee">Адресат.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public static Capture.Structures.Module.BusinessUnitWithFact GetBusinessUnitWithFact(List<Capture.Structures.Module.BusinessUnitWithFact> businessUnitsWithFacts, IEmployee responsible, IEmployee addressee)
    {
      var businessUnitByAddressee = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(addressee);
      var businessUnitByAddresseeWithFact = Capture.Structures.Module.BusinessUnitWithFact.Create(businessUnitByAddressee, null, false);
      
      // Попытаться уточнить по адресату.
      var businessUnitWithFact = businessUnitsWithFacts.Any() && businessUnitByAddressee != null
        ? businessUnitByAddresseeWithFact
        : businessUnitsWithFacts.FirstOrDefault();
      if (businessUnitWithFact != null)
        return businessUnitWithFact;
      
      // Если факты с ИНН/КПП не найдены, и по наименованию не найдено, то вернуть НОР из адресата.
      if (businessUnitByAddresseeWithFact.BusinessUnit != null)
        return businessUnitByAddresseeWithFact;
      
      // Если и по адресату не найдено, то вернуть НОР из ответственного.
      var businessUnitByResponsible = Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      var businessUnitByResponsibleWithFact = Capture.Structures.Module.BusinessUnitWithFact.Create(businessUnitByResponsible, null, false);
      return businessUnitByResponsibleWithFact;
    }
    
    /// <summary>
    /// Получение списка НОР по извлеченным фактам.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <returns>Список НОР и соответствующих им фактов.</returns>
    public static List<Capture.Structures.Module.BusinessUnitWithFact> GetBusinessUnitsWithFacts(List<Structures.Module.IFact> facts)
    {
      // Получить ИНН/КПП и наименования/ФС корреспондентов из фактов.
      var businessUnitsByName = new List<Capture.Structures.Module.BusinessUnitWithFact>();
      var correspondentNameFacts = GetFacts(facts, "Letter", "CorrespondentName")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "CorrespondentName").Probability);
      foreach (var fact in correspondentNameFacts)
      {
        var name = GetFieldValue(fact, "CorrespondentName");
        var businessUnits = BusinessUnits.GetAll()
          .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          .Where(x => x.Name.ToLower().Contains(name));
        businessUnitsByName.AddRange(businessUnits.Select(x => Capture.Structures.Module.BusinessUnitWithFact.Create(x, fact, false)));
      }
      
      // Если факты с ИНН/КПП не найдены, то вернуть НОР по наименованию.
      var correspondentTinFacts = GetFacts(facts, "Counterparty", "TIN")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "TIN").Probability);
      if (!correspondentTinFacts.Any())
        return businessUnitsByName;
      else
      {
        // Поиск по ИНН/КПП.
        var foundByTin = new List<Capture.Structures.Module.BusinessUnitWithFact>();
        foreach (var fact in correspondentTinFacts)
        {
          var tin = GetFieldValue(fact, "TIN");
          var trrc = GetFieldValue(fact, "TRRC");
          var businessUnits = GetBusinessUnits(tin, trrc);
          foundByTin.AddRange(businessUnits.Select(x => Capture.Structures.Module.BusinessUnitWithFact.Create(x, fact, true)));
        }
        
        // Найдено по ИНН/КПП.
        if (foundByTin.Any())
          return foundByTin;
        
        // Не найдено. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
        if (businessUnitsByName.Any())
          return businessUnitsByName.Where(x => string.IsNullOrEmpty(x.BusinessUnit.TIN) && string.IsNullOrEmpty(x.BusinessUnit.TRRC)).ToList();
      }
      return businessUnitsByName;
    }
    
    /// <summary>
    /// Получить список контрагентов по ИНН/КПП.
    /// </summary>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Список контрагентов.</returns>
    public static List<ICounterparty> GetCounterparties(string tin, string trrc)
    {
      var searchByTin = !string.IsNullOrWhiteSpace(tin);
      var searchByTrrc = !string.IsNullOrWhiteSpace(trrc);
      
      if (!searchByTin && !searchByTrrc)
        return new List<ICounterparty>();

      // Отфильтровать закрытые сущности.
      var counterparties = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      // Поиск по ИНН, если ИНН передан.
      if (searchByTin)
      {
        var strongTinCounterparties = counterparties.Where(x => x.TIN == tin).ToList();
        
        // Поиск по КПП, если КПП передан.
        if (searchByTrrc)
        {
          var strongTrrcCompanies = strongTinCounterparties
            .Where(c => CompanyBases.Is(c))
            .Where(c => !string.IsNullOrWhiteSpace(CompanyBases.As(c).TRRC) && CompanyBases.As(c).TRRC == trrc)
            .ToList();
          
          if (strongTrrcCompanies.Count > 0)
            return strongTrrcCompanies;
          
          return strongTinCounterparties.Where(c => CompanyBases.Is(c) &&
                                               string.IsNullOrWhiteSpace(CompanyBases.As(c).TRRC)).ToList();
        }
        
        return strongTinCounterparties;
      }
      
      return counterparties.ToList();
    }
    
    /// <summary>
    /// Получить список НОР по ИНН/КПП.
    /// </summary>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Список НОР.</returns>
    public static List<IBusinessUnit> GetBusinessUnits(string tin, string trrc)
    {
      var searchByTin = !string.IsNullOrWhiteSpace(tin);
      var searchByTrrc = !string.IsNullOrWhiteSpace(trrc);
      
      if (!searchByTin && !searchByTrrc)
        return new List<IBusinessUnit>();

      // Отфильтровать закрытые НОР.
      var businessUnits = BusinessUnits.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      // Поиск по ИНН, если ИНН передан.
      if (searchByTin)
      {
        var strongTinBusinessUnits = businessUnits.Where(x => x.TIN == tin).ToList();
        
        // Поиск по КПП, если КПП передан.
        if (searchByTrrc)
        {
          var strongTrrcBusinessUnits = strongTinBusinessUnits
            .Where(c => !string.IsNullOrWhiteSpace(c.TRRC) && c.TRRC == trrc)
            .ToList();
          
          if (strongTrrcBusinessUnits.Count > 0)
            return strongTrrcBusinessUnits;
          
          return strongTinBusinessUnits.Where(c => string.IsNullOrWhiteSpace(c.TRRC)).ToList();
        }
        
        return strongTinBusinessUnits;
      }
      
      return new List<IBusinessUnit>();
    }
    
    #endregion
    
    #region Работа с полями/фактами
    
    /// <summary>
    /// Получить поле из факта.
    /// </summary>
    /// <param name="fact">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Поле.</returns>
    public static IFactField GetField(Structures.Module.IFact fact, string fieldName)
    {
      if (fact == null)
        return null;
      return fact.Fields.FirstOrDefault(f => f.Name == fieldName);
    }
    
    /// <summary>
    /// Получить значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Значение поля.</returns>
    public static string GetFieldValue(Structures.Module.IFact fact, string fieldName)
    {
      if (fact == null)
        return string.Empty;
      
      var field = fact.Fields.FirstOrDefault(f => f.Name == fieldName);
      if (field != null)
        return field.Value;
      
      return string.Empty;
    }

    /// <summary>
    /// Получить поле из фактов.
    /// </summary>
    /// <param name="facts"> Список фактов.</param>
    /// <param name="factName"> Имя факта, поле которого будет извлечено.</param>
    /// <returns>Поле, полученное из Ario с наибольшей вероятностью.</returns>
    public static string GetFieldValue(List<Structures.Module.IFact> facts, string factName, string fieldName)
    {
      IEnumerable<IFactField> fields = facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any())
        .SelectMany(f => f.Fields);
      var field = fields
        .OrderByDescending(f => f.Probability)
        .FirstOrDefault(f => f.Name == fieldName);
      if (field != null)
        return field.Value;
      
      return string.Empty;
    }

    /// <summary>
    /// Получить числовое значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Числовое значение поля.</returns>
    public static double? GetFieldNumericalValue(Structures.Module.IFact fact, string fieldName)
    {
      var field = GetFieldValue(fact, fieldName);
      return ConvertStringToDouble(field);
    }
    
    /// <summary>
    /// Получить числовое значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Числовое значение поля.</returns>
    public static double? GetFieldNumericalValue(List<Structures.Module.IFact> facts, string factName, string fieldName)
    {
      var field = GetFieldValue(facts, factName, fieldName);
      return ConvertStringToDouble(field);
    }
    
    /// <summary>
    /// Получить значение поля типа DateTime из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Значение поля типа DateTime.</returns>
    public static DateTime? GetFieldDateTimeValue(Structures.Module.IFact fact, string fieldName)
    {
      var recognizedDate = GetFieldValue(fact, fieldName);
      if (string.IsNullOrWhiteSpace(recognizedDate))
        return null;
      
      DateTime date;
      if (Calendar.TryParseDate(recognizedDate, out date))
        return date;
      else
        return null;
    }
    
    /// <summary>
    /// Получить список фактов с переданными именем факта и именем поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Список фактов с наибольшей вероятностью.</returns>
    /// <remarks>С учетом вероятности факта.</remarks>
    public static List<Structures.Module.IFact> GetFacts(List<Structures.Module.IFact> facts, string factName, string fieldName)
    {
      return facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any(fl => fl.Name == fieldName))
        .ToList();
    }
    
    /// <summary>
    /// Получить сортированный список фактов.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="orderFieldName">Имя поля по вероятности которого будет произведена сортировка.</param>
    /// <returns>Список фактов с наибольшей вероятностью.</returns>
    /// <remarks>С учетом вероятности факта.</remarks>
    public static List<Structures.Module.IFact> GetOrderedFacts(List<Structures.Module.IFact> facts, string factName, string orderFieldName)
    {
      return facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any(fl => fl.Name == orderFieldName))
        .OrderByDescending(f => f.Fields.First(fl => fl.Name == orderFieldName).Probability)
        .ToList();
    }
    
    /// <summary>
    /// Получить тело документа из Арио.
    /// </summary>
    /// <param name="documentGuid">Гуид документа в Арио.</param>
    /// <returns>Тело документа.</returns>
    public static System.IO.Stream GetDocumentBody(string documentGuid)
    {
      var arioUrl = Functions.Module.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      return arioConnector.GetDocumentByGuid(documentGuid);
    }
    
    /// <summary>
    /// Получить значение численного параметра из docflow_params.
    /// </summary>
    /// <param name="paramName">Наименование параметра.</param>
    /// <returns>Значение параметра.</returns>
    private static double GetDocflowParamsNumbericValue(string paramName)
    {
      double result = 0;
      var paramValue = Functions.Module.GetDocflowParamsValue(paramName);
      if (!(paramValue is DBNull) && paramValue != null)
        double.TryParse(paramValue.ToString(), out result);
      return result;
    }
    
    /// <summary>
    /// Получить наименование контрагента.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование контрагента.</param>
    /// <param name="nameFieldName">Наименование поля с наименованием контрагента.</param>
    /// <param name="legalFormFieldName">Наименование поля с организационо-правовой формой контрагента.</param>
    /// <returns>Наименование контрагента.</returns>
    private static string GetCorrespondentName(Structures.Module.IFact fact, string nameFieldName, string legalFormFieldName)
    {
      if (fact == null)
        return string.Empty;
      
      var name = GetFieldValue(fact, nameFieldName);
      var legalForm = GetFieldValue(fact, legalFormFieldName);
      return string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm);
    }
    
    /// <summary>
    /// Получить наименование ведущего документа.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование ведущего документа.</param>
    /// <returns>Наименование ведущего документа с номером и датой.</returns>
    private static string GetLeadingDocumentName(Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var documentName = GetFieldValue(fact, "DocumentBaseName");
      var date = Functions.Module.GetShortDate(GetFieldValue(fact, "DocumentBaseDate"));
      var number = GetFieldValue(fact, "DocumentBaseNumber");
      
      if (string.IsNullOrWhiteSpace(documentName))
        return string.Empty;
      
      if (!string.IsNullOrWhiteSpace(number))
        documentName = string.Format("{0} №{1}", documentName, number);
      
      if (!string.IsNullOrWhiteSpace(date))
        documentName = string.Format("{0} от {1}", documentName, date);
      
      return documentName;
    }
    
    /// <summary>
    /// Преобразовать строковое значение поля в числовое.
    /// </summary>
    /// <param name="field">Поле.</param>
    /// <returns>Число.</returns>
    private static double? ConvertStringToDouble(string field)
    {
      if (string.IsNullOrWhiteSpace(field))
        return null;

      double result;
      double.TryParse(field, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out result);
      return result;
    }
    
    /// <summary>
    /// Проложить связь между фактом и свойством документа.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Арио.</param>
    /// <param name="fact">Факт, который будет связан со свойством документа.</param>
    /// <param name="fieldName">Поле, которое будет связано со свойством документа. Если не указано, то будут связаны все поля факта.</param>
    /// <param name="propertyName">Имя свойства документа.</param>
    /// <param name="propertyValue">Значение свойства.</param>
    /// <param name="isTrusted">Признак, доверять результату извлечения из Арио или нет.</param>
    public static void LinkFactAndProperty(Structures.Module.IRecognizedDocument recognizedDocument,
                                           Structures.Module.IFact fact,
                                           string fieldName,
                                           string propertyName,
                                           object propertyValue,
                                           bool? isTrusted = null)
    {
      var propertyStringValue = GetPropertyValueAsString(propertyValue);
      if (string.IsNullOrWhiteSpace(propertyStringValue))
        return;
      
      // Если значение определилось не из фактов,
      // для подсветки заносим это свойство и результату не доверяем.
      if (fact == null)
      {
        var calculatedFact = recognizedDocument.Info.Facts.AddNew();
        calculatedFact.PropertyName = propertyName;
        calculatedFact.PropertyValue = propertyStringValue;
        calculatedFact.IsTrusted = false;
      }
      else
      {
        if (isTrusted == null)
          isTrusted = IsTrustedField(fact, fieldName);
        
        var facts = recognizedDocument.Info.Facts
          .Where(f => f.FactId == fact.Id)
          .Where(f => string.IsNullOrWhiteSpace(fieldName) || f.FieldName == fieldName);
        var factLabel = GetFactLabel(fact, propertyName);
        foreach (var recognizedFact in facts)
        {
          recognizedFact.PropertyName = propertyName;
          recognizedFact.PropertyValue = propertyStringValue;
          recognizedFact.IsTrusted = isTrusted;
          recognizedFact.FactLabel = factLabel;
        }
      }
    }
    
    /// <summary>
    /// Получить метку факта.
    /// </summary>
    /// <param name="fact">Факт из Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Метка факта.</returns>
    /// <remarks>Используется для быстрого поиска факта в результатах извлечения фактов.</remarks>
    public static string GetFactLabel(Structures.Module.IFact fact, string propertyName)
    {
    	string factInfo = fact.Name + propertyName;
    	foreach (var field in fact.Fields)
    		factInfo += field.Name + field.Value;
    	
    	var factHash = string.Empty;
    	using (MD5 md5Hash = MD5.Create())
    	{
    		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(factInfo));
    		for (int i = 0; i < data.Length; i++)
    			factHash += data[i].ToString("x2");
    	}
    	return factHash;
    }
    
    /// <summary>
    /// Получить список распознанных свойств документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="isTrusted">Точно ли распознано свойство: да/нет.</param>
    /// <returns>Список распознанных свойств документа.</returns>
    [Remote, Public]
    public static List<string> GetRecognizedDocumentProperties(Docflow.IOfficialDocument document, bool isTrusted)
    {
      var result = new List<string>();
      
      if (document == null)
        return result;
      
      // Получить результаты распознавания документа без проверки прав.
      // Сделано, чтобы не выдавать пользователям права на технический справочник.
      AccessRights.AllowRead(
        () =>
        {
          var recognitionInfo = DocumentRecognitionInfos.GetAll(x => x.DocumentId == document.Id).FirstOrDefault();
          if (recognitionInfo == null)
            return;
          
          // Взять только заполненные свойства самого документа. Свойства-коллекции записываются через точку.
          var linkedFacts = recognitionInfo.Facts
            .Where(x => !string.IsNullOrEmpty(x.PropertyName) && !x.PropertyName.Any(с => с == '.'))
            .Where(x => x.IsTrusted == isTrusted);
          
          // Взять только неизмененные пользователем свойства.
          var type = document.GetType();
          foreach (var linkedFact in linkedFacts)
          {
            var propertyName = linkedFact.PropertyName;
            var property = type.GetProperty(propertyName);
            if (property != null)
            {
              object propertyValue = property.GetValue(document);
              var propertyStringValue = GetPropertyValueAsString(propertyValue);
              if (!string.IsNullOrWhiteSpace(propertyStringValue) && Equals(propertyStringValue, linkedFact.PropertyValue))
              {
                var propertyAndPosition = string.Format("{1}{0}{2}", Constants.Module.PropertyAndPositionDelimiter,
                                                        propertyName, linkedFact.Position);
                result.Add(propertyAndPosition);
              }
            }
          }
        });
      
      return result.Distinct().ToList();
    }
    
    /// <summary>
    /// Сохранить результат верификации заполнения свойств.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static void StoreVerifiedPropertiesValues(Docflow.IOfficialDocument document)
    {
      AccessRights.AllowRead(
        () =>
        {
          var recognitionInfo = Capture.DocumentRecognitionInfos
            .GetAll(x => x.DocumentId == document.Id)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();
          if (recognitionInfo == null)
            return;
          
          // Взять только заполненные свойства самого документа. Свойства-коллекции записываются через точку.
          var linkedFacts = recognitionInfo.Facts
            .Where(x => !string.IsNullOrEmpty(x.PropertyName) && !x.PropertyName.Any(с => с == '.'));
          
          // Взять только измененные пользователем свойства.
          var type = document.GetType();
          foreach (var linkedFact in linkedFacts)
          {
            var propertyName = linkedFact.PropertyName;
            var property = type.GetProperty(propertyName);
            if (property != null)
            {
              object propertyValue = property.GetValue(document);
              var propertyStringValue = GetPropertyValueAsString(propertyValue);
              if (!string.IsNullOrWhiteSpace(propertyStringValue) && !Equals(propertyStringValue, linkedFact.PropertyValue))
                linkedFact.VerifiedValue = propertyStringValue;
            }
          }
        });
    }
    
    /// <summary>
    /// Получить строковое значение свойства.
    /// </summary>
    /// <param name="propertyValue">Значение свойства.</param>
    /// <returns></returns>
    /// <remarks>Для свойств типа сущность будет возвращена строка с Ид сущности.</remarks>
    private static string GetPropertyValueAsString(object propertyValue)
    {
      if (propertyValue == null)
        return string.Empty;
      
      var propertyStringValue = propertyValue.ToString();
      if (propertyValue is Sungero.Domain.Shared.IEntity)
        propertyStringValue = ((Sungero.Domain.Shared.IEntity)propertyValue).Id.ToString();
      return propertyStringValue;
    }
    
    /// <summary>
    /// Получить признак - доверять извлеченному полю или нет.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Признак, доверять извлеченному полю или нет.</returns>
    public static bool IsTrustedField(Structures.Module.IFact fact, string fieldName)
    {
      var field = GetField(fact, fieldName);
      if (field == null)
        return false;
      
      double trustedProbability;
      var valueReceived = Sungero.Core.Cache.TryGetValue(Constants.Module.TrustedFactProbabilityKey, out trustedProbability);
      if (!valueReceived)
      {
        trustedProbability = GetDocflowParamsNumbericValue(Constants.Module.TrustedFactProbabilityKey);
        Sungero.Core.Cache.AddOrUpdate(Constants.Module.TrustedFactProbabilityKey, trustedProbability, Calendar.Now.AddMinutes(10));
      }
      return field.Probability >= trustedProbability;
    }
    
    /// <summary>
    /// Создать тело документа.
    /// </summary>
    /// <param name="document">Документ Rx.</param>
    /// <param name="recognizedDocument">Результат обработки входящего документа в Арио.</param>
    public static void CreateVersion(IOfficialDocument document, Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var needCreatePublicBody = recognizedDocument.OriginalFile != null && recognizedDocument.OriginalFile.Data != null;
      var pdfApp = Content.AssociatedApplications.GetByExtension("pdf");
      var originalFileApp = Content.AssociatedApplications.Null;
      if (needCreatePublicBody)
        originalFileApp = GetAssociatedApplicationByFileName(recognizedDocument.OriginalFile.Path);
      
      // При создании версии Subject не должен быть пустым, иначе задваивается имя документа.
      var subjectIsEmpty = string.IsNullOrEmpty(document.Subject);
      if (subjectIsEmpty)
        document.Subject = "tmp_Subject";
      
      document.CreateVersion();
      var version = document.LastVersion;
      
      if (needCreatePublicBody)
      {
        using (var publicBody = GetDocumentBody(recognizedDocument.BodyGuid))
          version.PublicBody.Write(publicBody);
        using (var body = new MemoryStream(recognizedDocument.OriginalFile.Data))
          version.Body.Write(body);
        version.AssociatedApplication = pdfApp;
        version.BodyAssociatedApplication = originalFileApp;
      }
      else
      {
        using (var body = GetDocumentBody(recognizedDocument.BodyGuid))
          version.Body.Write(body);
        version.AssociatedApplication = pdfApp;
      }
      
      // Очистить Subject, если он был пуст до создания версии.
      if (subjectIsEmpty)
        document.Subject = string.Empty;
    }
    
    /// <summary>
    /// Получить приложение-обработчик по имени файла.
    /// </summary>
    /// <param name="fileName">Имя или путь до файла.</param>
    /// <returns>Приложение-обработчик</returns>
    private static Sungero.Content.IAssociatedApplication GetAssociatedApplicationByFileName(string fileName)
    {
      var app = Sungero.Content.AssociatedApplications.Null;
      var ext = System.IO.Path.GetExtension(fileName).TrimStart('.').ToLower();
      app = Content.AssociatedApplications.GetByExtension(ext);
      
      // Взять приложение-обработчик unknown, если не смогли подобрать по расширению.
      if (app == null)
        app = Sungero.Content.AssociatedApplications.GetAll()
          .SingleOrDefault(x => x.Sid == Sungero.Docflow.PublicConstants.Module.UnknownAppSid);
      
      return app;
    }
    
    #endregion
  }
}