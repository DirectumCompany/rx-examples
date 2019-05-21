using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// <summary>
    /// Создать документы в RX и отправить задачу на проверку.
    /// </summary>
    /// <param name="sourceFileName">Имя исходного файла, полученного с DCS.</param>
    /// <param name="jsonClassificationResults">Json результатом классификации и извлечения фактов.</param>
    /// <param name="responsible">Сотрудник, ответственного за проверку документов.</param>
    [Remote]
    public static void ProcessSplitedPackage(string sourceFileName, string jsonClassificationResults, IEmployee responsible)
    {
      // Создать документы по распознанным данным.
      var recognitedDocuments = GetRecognitedDocuments(jsonClassificationResults);
      var package = new List<IOfficialDocument>();
      foreach (var recognitedDocument in recognitedDocuments)
      {
        var document = CreateDocumentByRecognitedDocument(recognitedDocument, sourceFileName, responsible);
        package.Add(document);
      }
      
      // Определить ведущий документ.
      var incLetter = package.Where(d => IncomingLetters.Is(d)).FirstOrDefault();
      var leadingDocument = incLetter != null
        ? incLetter
        : package.FirstOrDefault();
      
      // Связать приложения с ведущим документом.
      var addendums = package.ToList();
      addendums.Remove(leadingDocument);
      int addendumNumber = 1;
      var hasLeadingDocument = !SimpleAssignments.Is(leadingDocument);
      var relation = hasLeadingDocument
        ? Docflow.PublicConstants.Module.AddendumRelationName
        : Constants.Module.SimpleRelationRelationName;
      foreach (var addendum in addendums)
      {
        addendum.Name = hasLeadingDocument
          ? Resources.AttachmentNameFormat(addendumNumber)
          : Resources.DocumentNameFormat(addendumNumber);
        addendum.Relations.AddFrom(relation, leadingDocument);
        addendum.Save();
        addendumNumber++;
      }
      
      // Отправить пакет ответственному.
      SendToResponsible(leadingDocument, addendums, responsible);
    }
    
    public static List<Structures.Module.RecognitedDocument> GetRecognitedDocuments(string jsonClassificationResults)
    {
      var recognitedDocuments = new List<RecognitedDocument>();
      var packageProcessResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      foreach (var packageProcessResult in packageProcessResults)
      {
        // Класс и гуид тела документа.
        var recognitedDocument = RecognitedDocument.Create();
        var clsResult = packageProcessResult.ClassificationResult;
        recognitedDocument.ClassificationResultId = clsResult.Id;
        recognitedDocument.BodyGuid = clsResult.DocumentGuid;
        recognitedDocument.PredictedClass = clsResult.PredictedClass != null ? clsResult.PredictedClass.Name : string.Empty;
        
        // Факты и поля фактов.
        recognitedDocument.Facts = new List<Fact>();
        var minFactProbability = GetMinFactProbability();
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
                                    .Select(f => FactField.Create(f.Name, f.Value, (decimal)(f.Probability)));
            recognitedDocument.Facts.Add(Fact.Create(fact.Name, fields.ToList()));
          }
        }
        
        recognitedDocuments.Add(recognitedDocument);
      }
      return recognitedDocuments;
    }
    
    public static IOfficialDocument CreateDocumentByRecognitedDocument(Structures.Module.RecognitedDocument recognitedDocument,
                                                                       string sourceFileName,
                                                                       IEmployee responsible)
    {
      // Входящее письмо.
      var recognitedClass = recognitedDocument.PredictedClass;
      var CaptureMockMode = GetDocflowParamsValue(Constants.Module.CaptureMockModeKey);
      if (recognitedClass == Constants.Module.LetterClassName)
      {
        return CaptureMockMode != null
          ? CreateMokeIncomingLetter(recognitedDocument, responsible)
          : CreateIncomingLetter(recognitedDocument, responsible);
      }
      // Акт выполненных работ.
      else if (recognitedClass == Constants.Module.ContractStatementClassName && CaptureMockMode != null)
      {
        return CreateMokeContractStatement(recognitedDocument, responsible);
      }
      // Все нераспознанные документы создать простыми.
      return CreateSimpleDocument(sourceFileName, recognitedDocument.BodyGuid);
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
    /// Создать документ в Rx, тело документа загружается из Арио.
    /// </summary>
    /// <param name="name">Имя документа.</param>
    /// <param name="documentGuid">Гуид тела документа.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateSimpleDocument(string name, string documentGuid)
    {
      var document = SimpleDocuments.Create();
      document.Name = !string.IsNullOrWhiteSpace(name) ? name : Resources.SimpleDocumentName;
      var documentBody = GetDocumentBody(documentGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      documentBody = null;
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо в RX.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateIncomingLetter(Structures.Module.RecognitedDocument letterсlassificationResult, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      var documentBody = GetDocumentBody(letterсlassificationResult.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = letterсlassificationResult.Facts;
      var subject = GetFieldValue(facts, "Letter", "Subject");
      document.Subject = !string.IsNullOrEmpty(subject) ?
        string.Format("{0}{1}", subject.Substring(0,1).ToUpper(), subject.Remove(0,1).ToLower()) : string.Empty;
      
      // Заполнить данные корреспондента.
      document.Correspondent = GetCounterparty(facts);
      document.InNumber = GetFieldValue(facts, "Letter", "Number");
      var correspondentDate = GetFieldValue(facts, "Letter", "Date");
      if (!string.IsNullOrEmpty(correspondentDate))
        document.Dated = DateTime.Parse(correspondentDate);
      
      // Заполнить данные нашей стороны.
      document.BusinessUnit =  Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      document.Department = GetDepartment(responsible);
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMokeIncomingLetter(Structures.Module.RecognitedDocument letterсlassificationResult, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.Capture.MockIncommingLetters.Create();
      var documentBody = GetDocumentBody(letterсlassificationResult.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = letterсlassificationResult.Facts;
      
      // Заполнить данные корреспондента.
      document.InNumber = GetFieldValue(facts, "Letter", "Number");
      document.Dated = Functions.Module.GetShortDate(GetFieldValue(facts, "Letter", "Date"));
          
      foreach (var fact in GetFacts(facts, "Letter", "CorrespondentName"))
      {
        var name = GetFieldValue(fact, "CorrespondentName");
        var legalForm = GetFieldValue(fact, "CorrespondentLegalForm");
        name = string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm);
        
        if (string.IsNullOrWhiteSpace(document.Correspondent))
          document.Correspondent = name;
        // Если коррекспондент уже заполнен, то занести наименование в получателя.
        else
          document.Recipient = name;
      }
      
      foreach (var fact in GetFacts(facts, "Counterparty", "TIN"))
      {
        var tin = GetFieldValue(fact, "TIN");
        var trrc = GetFieldValue(fact, "TRRC");
        if (string.IsNullOrWhiteSpace(document.CorrespondentTin))
        {
          document.CorrespondentTin = tin;
          document.CorrespondentTrrc = trrc;
        }
        // Если ИНН/КПП корреспондента уже заполнены, то занести в ИНН/КПП получателя.
        else
        {
          document.RecipientTin = tin;
          document.RecipientTrrc = trrc;
        }
      }
      
      document.InResponseTo = GetFieldValue(facts, "Letter", "ResponseToNumber");
      var responseToDate = Functions.Module.GetShortDate(GetFieldValue(facts, "Letter", "ResponseToDate"));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? document.InResponseTo
        : string.Format("{0} {1} {2}", document.InResponseTo, Sungero.Docflow.Resources.From, responseToDate);
      
      foreach (var fact in GetFacts(facts, "LetterPerson", "Surname"))
      {
        var type = GetFieldValue(fact, "Type");
        var surname = GetFieldValue(fact, "Surname");
        var name = GetFieldValue(fact, "Name");
        var patrn = GetFieldValue(fact, "Patrn");
        
        if (type == "SIGNATORY")
          document.Signatory = Functions.Module.ConcatFullName(surname, name, patrn);
        else
          document.Contact = Functions.Module.ConcatFullName(surname, name, patrn);
      }
      
      // Заполнить данные нашей стороны.
      foreach (var fact in GetFacts(facts, "Letter", "Addressee"))
      {
        var adressee = GetFieldValue(fact, "Addressee");
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? adressee : string.Format("{0}, {1}", document.Addressees, adressee);
      }
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subject = GetFieldValue(facts, "Letter", "Subject");
      document.Subject = !string.IsNullOrEmpty(subject) ?
        string.Format("{0}{1}", subject.Substring(0,1).ToUpper(), subject.Remove(0,1).ToLower()) : string.Empty;
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать акт выполненных работ с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMokeContractStatement(Structures.Module.RecognitedDocument сlassificationResult, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.Capture.MockContractStatements.Create();
      var documentBody = GetDocumentBody(сlassificationResult.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = сlassificationResult.Facts;
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Поиск корреспондента по извлеченным фактам.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <returns>Корреспондент.</returns>
    public static Sungero.Parties.ICounterparty GetCounterparty(List<Structures.Module.Fact> facts)
    {
      // Получить ИНН/КПП и наименования/ФС контрагентов из фактов.
      var correspondentNames = new List<string>();
      foreach (var fact in GetFacts(facts, "Letter", "CorrespondentName"))
      {
        var name = GetFieldValue(fact, "CorrespondentName");
        var legalForm = GetFieldValue(fact, "CorrespondentLegalForm");
        correspondentNames.Add(string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm));
      }
      
      // Поиск корреспондентов по наименованию.
      var foundByName = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
        .Where(x => x.Note == null || !x.Note.Equals(BusinessUnits.Resources.BusinessUnitComment))
        .Where(x => correspondentNames.Contains(x.Name))
        .ToList();
      
      // Если факты с ИНН/КПП не найдены, то вернуть корреспондента по наименованию.
      var correspondentTINs = GetFacts(facts, "Counterparty", "TIN");
      if (!correspondentTINs.Any())
        return foundByName.FirstOrDefault();
      else
      {
        // Поиск по ИНН/КПП.
        var foundByTin = new List<ICounterparty>();
        foreach (var fact in correspondentTINs)
          foundByTin.AddRange(GetCounterparties(GetFieldValue(fact, "TIN"), GetFieldValue(fact, "TRRC")));
        
        // Найден ровно 1.
        if (foundByTin.Count == 1)
          return foundByTin.First();
        
        // Найдено 0. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
        if (!foundByTin.Any())
          return foundByName
            .Where(x => string.IsNullOrEmpty(x.TIN))
            .Where(x => !CompanyBases.Is(x) || string.IsNullOrEmpty(CompanyBases.As(x).TRRC))
            .FirstOrDefault();

        // Найдено несколько. Уточнить поиск по наименованию.
        foundByName = foundByTin.Where(x => correspondentNames.Any(n => n == x.Name)).ToList();
        if (foundByName.Any())
          return foundByName.FirstOrDefault();
        else
          return foundByTin.FirstOrDefault();
      }
    }

    /// <summary>
    /// Получить значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Зачение поля.</returns>
    public static string GetFieldValue(Structures.Module.Fact fact, string fieldName)
    {
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
    public static string GetFieldValue(List<Structures.Module.Fact> facts, string factName, string fieldName)
    {
      IEnumerable<FactField> fields = facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any())
        .SelectMany(f => f.Fields);
      var field = fields
        .OrderByDescending(f => f.Probability)
        .FirstOrDefault(f => string.Equals(f.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
      if (field != null)
        return field.Value;
      
      return string.Empty;
    }

    /// <summary>
    /// Получить список фактов с переданными именем факта и именем поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Список фактов с наибольшей вероятностью.</returns>
    public static List<Structures.Module.Fact> GetFacts(List<Structures.Module.Fact> facts, string factName, string fieldName)
    {
      return facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any(fl => fl.Name == fieldName))
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
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="leadingDocument">Основной документ.</param>
    /// <param name="documents">Прочие документы.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Простая задача.</returns>
    [Remote, Public]
    public static void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents, Company.IEmployee responsible)
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

    [Remote, Public]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
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

      // Отфильтровать закрытые сущности и копии НОР.
      var counterparties = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
        .Where(x => x.Note == null || !x.Note.Equals(BusinessUnits.Resources.BusinessUnitComment));
      
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
    /// Инициализация демо-режима.
    /// </summary>
    [Remote]
    public static void InitCaptureMockMode()
    {
      // Создать типы документов.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(RecordManagement.Resources.IncomingLetterKindName,
                                                                              Capture.Server.MockIncommingLetter.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(FinancialArchive.Resources.ContractStatementKindName,
                                                                              Capture.Server.MockContractStatement.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Contracts, true);
      
      // Создать виды документов.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(RecordManagement.Resources.IncomingLetterKindName,
                                                                              RecordManagement.Resources.IncomingLetterKindShortName,
                                                                              Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true, false,
                                                                              Sungero.Capture.Server.MockIncommingLetter.ClassTypeGuid, null,
                                                                              Sungero.Capture.Constants.Module.Initialize.MockIncommingLetterKind);

      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(FinancialArchive.Resources.ContractStatementKindName,
                                                                              FinancialArchive.Resources.ContractStatementKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Contracts, true, false,
                                                                              Capture.Server.MockContractStatement.ClassTypeGuid, null, true, false,
                                                                              Sungero.Capture.Constants.Module.Initialize.MockContractStatementKind, true);
      
      // Добавить параметр признака активации демо-режима.
      Sungero.Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Sungero.Capture.Constants.Module.CaptureMockModeKey, string.Empty);
    }
    
    /// <summary>
    /// Задать основные параметры захвата.
    /// </summary>
    /// <param name="arioUrl">Адрес Арио.</param>
    /// <param name="minFactProbability">Минимальная вероятность для факта.</param>
    [Remote]
    public static void SetCaptureMainSettings(string arioUrl, string minFactProbability)
    {
      // Добавить параметр адреса сервиса Ario.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.ArioUrlKey, arioUrl);
      
      // Добавить параметр минимальной вероятности для факта.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.MinFactProbabilityKey, minFactProbability);
    }
    
    /// <summary>
    /// Получить значение минимальной вероятности доверия факту.
    /// </summary>
    /// <returns>Минимальная вероятность доверия факту.</returns>
    private static float GetMinFactProbability()
    {
      float minProbability = 0;
      var paramValue = Functions.Module.GetDocflowParamsValue(Constants.Module.MinFactProbabilityKey);
      if (!(paramValue is DBNull) && paramValue != null)
        float.TryParse(paramValue.ToString(), out minProbability);
      return minProbability;
    }
  }
}