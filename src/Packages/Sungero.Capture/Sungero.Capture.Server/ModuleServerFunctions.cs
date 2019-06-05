﻿using System;
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
    [Remote]
    public static void SetCaptureMainSettings(string arioUrl, string minFactProbability)
    {
      // Добавить параметр адреса сервиса Ario.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.ArioUrlKey, arioUrl);
      
      // Добавить параметр минимальной вероятности для факта.
      Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Constants.Module.MinFactProbabilityKey, minFactProbability);
    }
    
    #endregion
    
    #region Общий процесс обработки захваченных документов
    
    /// <summary>
    /// Создать документы в RX и отправить задачу на проверку.
    /// </summary>
    /// <param name="sourceFileName">Имя исходного файла, полученного с DCS.</param>
    /// <param name="jsonClassificationResults">Json результатом классификации и извлечения фактов.</param>
    /// <param name="responsible">Сотрудник, ответственного за проверку документов.</param>
    [Remote]
    public virtual void ProcessSplitedPackage(string sourceFileName, string jsonClassificationResults, IEmployee responsible)
    {
      // Создать документы по распознанным данным.
      var recognizedDocuments = GetRecognizedDocuments(jsonClassificationResults);
      var package = new List<IOfficialDocument>();
      foreach (var recognizedDocument in recognizedDocuments)
      {
        var document = CreateDocumentByRecognizedDocument(recognizedDocument, sourceFileName, responsible);
        package.Add(document);
        
        recognizedDocument.Info.DocumentId = document.Id;
        recognizedDocument.Info.Save();
      }
      
      // Определить ведущий документ.
      var leadingDocument = GetLeadingDocument(package);
      
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
        if (SimpleDocuments.Is(addendum))
        {
          addendum.Name = hasLeadingDocument
            ? Resources.AttachmentNameFormat(addendumNumber)
            : Resources.DocumentNameFormat(addendumNumber);
          addendumNumber++;
        }
        
        addendum.Relations.AddFrom(relation, leadingDocument);
        addendum.Save();
      }
      
      // Отправить пакет ответственному.
      SendToResponsible(leadingDocument, addendums, responsible);
    }
    
    public virtual List<Structures.Module.RecognizedDocument> GetRecognizedDocuments(string jsonClassificationResults)
    {
      var recognizedDocuments = new List<RecognizedDocument>();
      var packageProcessResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      foreach (var packageProcessResult in packageProcessResults)
      {
        // Класс и гуид тела документа.
        var recognizedDocument = RecognizedDocument.Create();
        var clsResult = packageProcessResult.ClassificationResult;
        recognizedDocument.ClassificationResultId = clsResult.Id;
        recognizedDocument.BodyGuid = clsResult.DocumentGuid;
        recognizedDocument.PredictedClass = clsResult.PredictedClass != null ? clsResult.PredictedClass.Name : string.Empty;
        recognizedDocument.Message = packageProcessResult.Message;
        var docInfo = DocumentRecognitionInfos.Create();
        docInfo.Name = recognizedDocument.PredictedClass;
        docInfo.RecognizedClass = recognizedDocument.PredictedClass;
        if (clsResult.PredictedProbability != null)
          docInfo.ClassProbability = (double)(clsResult.PredictedProbability);
        
        // Факты и поля фактов.
        recognizedDocument.Facts = new List<Fact>();
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
              .Select(f => FactField.Create(f.Id, f.Name, f.Value, (decimal)(f.Probability)));
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
    public virtual IOfficialDocument CreateDocumentByRecognizedDocument(Structures.Module.RecognizedDocument recognizedDocument,
                                                                        string sourceFileName,
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
      if (recognizedClass == Constants.Module.WaybillClassName && isMockMode)
        return CreateMockWaybill(recognizedDocument);
      
      // Счет-фактура входящая.
      if (recognizedClass == Constants.Module.IncomingTaxInvoiceClassName && isMockMode)
        return CreateMockIncomingTaxInvoice(recognizedDocument);
      
      // УПД.
      if (recognizedClass == Constants.Module.UniversalTransferDocumentClassName && !isMockMode)
        return CreateUniversalTransferDocument(recognizedDocument, responsible);
      
      // Все нераспознанные документы создать простыми.
      return CreateSimpleDocument(sourceFileName, recognizedDocument.BodyGuid, recognizedDocument.Message);
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
    /// <param name="lastName">Фамилия.</param>
    /// <param name="firstName">Имя в формате "И." или "Имя".</param>
    /// <param name="middleName">Отчество в формате "О." или "Отчество".</param>
    /// <returns>Контактное лицо.</returns>
    public static IContact GetContactByName(string lastName, string firstName, string middleName)
    {
      var noBreakSpace = new string('\u00A0', 1);
      var space = new string('\u0020', 1);
      
      var name = Functions.Module.ConcatFullName(lastName, firstName, middleName);
      name = name.ToLower().Replace(noBreakSpace, space).Replace(". ", ".");
      
      return Contacts.GetAll()
        .Where(x => x.Name.ToLower().Replace(noBreakSpace, space).Replace(". ", ".") == name ||
               (x.Person != null
                ? x.Person.ShortName.ToLower().Replace(noBreakSpace, space).Replace(". ", ".") == name
                : false))
        .FirstOrDefault();
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
    private static Sungero.Contracts.IContractualDocument GetLeadingDocument(Fact fact)
    {
      if (fact == null)
        return Sungero.Contracts.ContractualDocuments.Null;
      
      DateTime docDate;
      var date = Functions.Module.GetShortDate(GetFieldValue(fact, "DocumentBaseDate"));
      Calendar.TryParseDate(date, out docDate);
      var number = GetFieldValue(fact, "DocumentBaseNumber");
      
      return Sungero.Contracts.ContractualDocuments.GetAll(x => x.RegistrationNumber == number &&
                                                           x.RegistrationDate == docDate).FirstOrDefault();
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
    
    #endregion
    
    #region Простой документ
    
    /// <summary>
    /// Создать документ в Rx, тело документа загружается из Арио.
    /// </summary>
    /// <param name="name">Имя документа.</param>
    /// <param name="documentGuid">Гуид тела документа.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateSimpleDocument(string name, string documentGuid, string note)
    {
      var document = SimpleDocuments.Create();
      document.Name = !string.IsNullOrWhiteSpace(name) ? name : Resources.SimpleDocumentName;
      document.Note = note;
      var documentBody = GetDocumentBody(documentGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      documentBody = null;
      document.Save();
      return document;
    }
    
    #endregion
    
    #region Входящее письмо
    
    /// <summary>
    /// Создать входящее письмо в RX.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateIncomingLetter(Structures.Module.RecognizedDocument recognizedDocument, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      var subjectFact = GetOrderedFacts(facts, "Letter", "Subject").FirstOrDefault();
      var subject = GetFieldValue(subjectFact, "Subject");
      document.Subject = !string.IsNullOrEmpty(subject) ?
        string.Format("{0}{1}", subject.Substring(0,1).ToUpper(), subject.Remove(0,1).ToLower()) : string.Empty;
      LinkFactAndProperty(recognizedDocument, subjectFact, "Subject", props.Subject.Name, document.Subject);
      
      // Адресат.
      foreach (var fact in GetFacts(facts, "Letter", "Addressee"))
      {
        var addresseeFact = GetOrderedFacts(facts, "Letter", "Addressee").FirstOrDefault();
        var addressee = GetFieldValue(addresseeFact, "Addressee");
        document.Addressee = GetEmployeeByName(addressee);
        if (document.Addressee != null)
        {
          LinkFactAndProperty(recognizedDocument, addresseeFact, "Addressee", props.Addressee.Name, document.Addressee); 
          break;
        }
      }
      
      // Заполнить данные нашей стороны.
      var businessUnits = GetBusinessUnitsByFacts(facts);
      document.BusinessUnit = GetBusinessUnit(businessUnits, responsible, document.Addressee);
      document.Department = document.Addressee != null
        ? GetDepartment(document.Addressee)
        : GetDepartment(responsible);
      
      // Заполнить данные корреспондента.
      document.Correspondent = GetCounterparty(facts);
      var dateFact = GetOrderedFacts(facts, "Letter", "Date").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, "Letter", "Number").FirstOrDefault();
      document.Dated = GetFieldDateTimeValue(dateFact, "Date");
      document.InNumber = GetFieldValue(numberFact, "Number");
      LinkFactAndProperty(recognizedDocument, dateFact, null, props.RegistrationDate.Name, document.Dated);
      LinkFactAndProperty(recognizedDocument, numberFact, null, props.RegistrationNumber.Name, document.InNumber);
      
      // Заполнить подписанта и контакт.
      foreach (var fact in GetFacts(facts, "LetterPerson", "Surname"))
      {
        var type = GetFieldValue(fact, "Type");
        var surname = GetFieldValue(fact, "Surname");
        var name = GetFieldValue(fact, "Name");
        var patrn = GetFieldValue(fact, "Patrn");
        
        if (type == "SIGNATORY" && document.SignedBy == null)
          document.SignedBy = GetContactByName(surname, name, patrn);
        else if (type == "RESPONSIBLE" && document.Contact == null)
          document.Contact = GetContactByName(surname, name, patrn);
      }
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо с текстовыми полями.
    /// </summary>
    /// <param name="letterClassificationResult">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingLetter(Structures.Module.RecognizedDocument letterClassificationResult)
    {
      var document = Sungero.Capture.MockIncomingLetters.Create();
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = letterClassificationResult.Facts;
      
      // Заполнить данные корреспондента.
      document.InNumber = GetFieldValue(facts, "Letter", "Number");
      document.Dated = Functions.Module.GetShortDate(GetFieldValue(facts, "Letter", "Date"));
      var correspondentNames = GetFacts(facts, "Letter", "CorrespondentName")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "CorrespondentName").Probability);
      
      if (correspondentNames.Count() > 0)
        document.Correspondent = GetCorrespondentName(correspondentNames.First(),
                                                      "CorrespondentName", "CorrespondentLegalForm");
      if (correspondentNames.Count() > 1)
        document.Recipient = GetCorrespondentName(correspondentNames.Last(),
                                                  "CorrespondentName", "CorrespondentLegalForm");
      
      // Заполнить ИНН/КПП для КА и НОР.
      var tinTrrcFacts = GetFacts(facts, "Counterparty", "TIN")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "TIN").Probability);
      if (tinTrrcFacts.Count() > 0)
      {
        document.CorrespondentTin = GetFieldValue(tinTrrcFacts.First(), "TIN");
        document.CorrespondentTrrc = GetFieldValue(tinTrrcFacts.First(), "TRRC");
      }
      if (tinTrrcFacts.Count() > 1)
      {
        document.RecipientTin = GetFieldValue(tinTrrcFacts.Last(), "TIN");
        document.RecipientTrrc = GetFieldValue(tinTrrcFacts.Last(), "TRRC");
      }
      
      // В ответ на.
      document.InResponseTo = GetFieldValue(facts, "Letter", "ResponseToNumber");
      var responseToDate = Functions.Module.GetShortDate(GetFieldValue(facts, "Letter", "ResponseToDate"));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? document.InResponseTo
        : string.Format("{0} {1} {2}", document.InResponseTo, Sungero.Docflow.Resources.From, responseToDate);
      
      // Подписант и контакт.
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
        var addressee = GetFieldValue(fact, "Addressee");
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? addressee : string.Format("{0}; {1}", document.Addressees, addressee);
      }
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subject = GetFieldValue(facts, "Letter", "Subject");
      document.Subject = !string.IsNullOrEmpty(subject) ?
        string.Format("{0}{1}", subject.Substring(0,1).ToUpper(), subject.Remove(0,1).ToLower()) : string.Empty;
      
      document.Save();
      
      var documentBody = GetDocumentBody(letterClassificationResult.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      return document;
    }
    
    #endregion
    
    #region Акт
    
    public static void FillRegistrationData(IOfficialDocument document, Structures.Module.RecognizedDocument recognizedDocument, string factName)
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
    
    public static void FillCounterpartyAndBusinessUnit(IAccountingDocumentBase document,
                                                       Structures.Module.RecognizedDocument recognizedDocument,
                                                       IEmployee responsible)
    {
      // В документах считаем что SELLER - Контрагент, BUYER - НОР. Но также проверяем наоборот (мультинорность).
      var facts = recognizedDocument.Facts;
      var props = document.Info.Properties;
      var buyerBusinessUnit = GetMostProbableBusinessUnit(facts, "BUYER");
      var businessUnitWithoutType = GetMostProbableBusinessUnit(facts, string.Empty);
      var sellerBusinessUnit = GetMostProbableBusinessUnit(facts, "SELLER");
      var businessUnitWithFact = buyerBusinessUnit ?? businessUnitWithoutType ?? sellerBusinessUnit;
      if (businessUnitWithFact != null)
      {
        document.BusinessUnit = businessUnitWithFact.BusinessUnit;
        LinkFactAndProperty(recognizedDocument, businessUnitWithFact.Fact, null, props.BusinessUnit.Name, businessUnitWithFact.BusinessUnit.Name);
      }
      else
      {
        document.BusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
      }
      
      var sellerCounterparty = GetMostProbableCounterparty(facts, "SELLER");
      var counterpartyWithoutType = GetMostProbableCounterparty(facts, string.Empty);
      var buyerCounterparty = GetMostProbableCounterparty(facts, "BUYER");
      var counterpartyWithFact = sellerCounterparty ?? counterpartyWithoutType ?? buyerCounterparty;
      if (counterpartyWithFact != null)
      {
        document.Counterparty = counterpartyWithFact.Counterparty;
        LinkFactAndProperty(recognizedDocument, counterpartyWithFact.Fact, null, props.Counterparty.Name, counterpartyWithFact.Counterparty.Name);
      }
    }
    
    /// <summary>
    /// Создать акт выполненных работ (демо режим).
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <returns>Акт выполненных работ.</returns>
    public static Docflow.IOfficialDocument CreateMockContractStatement(Structures.Module.RecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocNames = GetFacts(facts, "FinancialDocument", "DocumentBaseName")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "DocumentBaseName").Probability);
      document.LeadDoc = GetLeadingDocumentName(leadingDocNames.FirstOrDefault());
      
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
      
      // Заполнить сумму и валюту.
      document.TotalAmount = GetFieldNumericalValue(facts, "DocumentAmount", "Amount");
      document.VatAmount = GetFieldNumericalValue(facts, "DocumentAmount", "VatAmount");
      var currencyCode = GetFieldValue(facts, "DocumentAmount", "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      
      // Заполнить Номенклатуру.
      foreach (var fact in GetFacts(facts, "Goods", "Name"))
      {
        var good = document.Goods.AddNew();
        good.Name = GetFieldValue(fact, "Name");
        good.UnitName = GetFieldValue(fact, "UnitName");
        good.Count = GetFieldNumericalValue(fact, "Count");
        good.Price = GetFieldNumericalValue(fact, "Price");
        good.VatAmount = GetFieldNumericalValue(fact, "VatAmount");
        good.TotalAmount = GetFieldNumericalValue(fact, "Amount");
      }
      document.Save();
      
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      return document;
    }
    
    /// <summary>
    /// Создать акт выполненных работ.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <returns>Акт выполненных работ.</returns>
    public virtual Docflow.IOfficialDocument CreateContractStatement(Structures.Module.RecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = FinancialArchive.ContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.LeadingDocument = GetLeadingDocument(leadingDocFact);
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument);
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "Document");
      
      // Заполнить контрагента/НОР по типу.
      FillCounterpartyAndBusinessUnit(document, recognizedDocument, responsible);
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Заполнить сумму и валюту.
      document.TotalAmount = GetFieldNumericalValue(facts, "DocumentAmount", "Amount");
      var currencyCode = GetFieldValue(facts, "DocumentAmount", "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Регистрация.
      RegisterDocument(document);
      
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки накладной в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockWaybill(Structures.Module.RecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockWaybills.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocNames = GetFacts(facts, "FinancialDocument", "DocumentBaseName")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "DocumentBaseName").Probability);
      document.Contract = GetLeadingDocumentName(leadingDocNames.FirstOrDefault());
      
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
      document.TotalAmount = GetFieldNumericalValue(facts, "DocumentAmount", "Amount");
      document.VatAmount = GetFieldNumericalValue(facts, "DocumentAmount", "VatAmount");
      var currencyCode = GetFieldValue(facts, "DocumentAmount", "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      
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
      }
      document.Save();
      
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      return document;
    }
    
    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру с текстовыми полями.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Structures.Module.RecognizedDocument recognizedDocument)
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
      document.TotalAmount = GetFieldNumericalValue(facts, "DocumentAmount", "Amount");
      document.VatAmount = GetFieldNumericalValue(facts, "DocumentAmount", "VatAmount");
      var currencyCode = GetFieldValue(facts, "DocumentAmount", "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      
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
      }
      document.Save();
      
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
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
    public virtual Docflow.IOfficialDocument CreateUniversalTransferDocument(Structures.Module.RecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = Sungero.FinancialArchive.UniversalTransferDocuments.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и контрагент.
      FillCounterpartyAndBusinessUnit(document, recognizedDocument, responsible);
      
      // Подразделение и ответственный.
      document.Department = GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата и номер.
      FillRegistrationData(document, recognizedDocument, "FinancialDocument");
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var facts = recognizedDocument.Facts;
      document.TotalAmount = GetFieldNumericalValue(facts, "DocumentAmount", "Amount");
      var currencyCode = GetFieldValue(facts, "DocumentAmount", "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      document.Save();
      
      var documentBody = GetDocumentBody(recognizedDocument.BodyGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Регистрация.
      RegisterDocument(document);
      
      return document;
    }
    
    #endregion
    
    #region Поиск контрагента/НОР
    
    public static Structures.Module.MockCounterparty GetMostProbableMockCounterparty(List<Structures.Module.Fact> facts, string counterpartyType)
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
    
    public static Structures.Module.BusinessUnitWithFact GetMostProbableBusinessUnit(List<Structures.Module.Fact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetFacts(facts, "Counterparty", "Name")
        .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType)
        .OrderByDescending(x => x.Fields.First(f => f.Name == "Name").Probability);
      
      foreach (var fact in counterpartyFacts)
      {
        var tin = GetFieldValue(fact, "TIN");
        var trrc = GetFieldValue(fact, "TRRC");
        var bussinesUnits = GetBusinessUnits(tin, trrc);
        if (bussinesUnits.Any())
          return Structures.Module.BusinessUnitWithFact.Create(bussinesUnits.First(), fact);
      }
      
      return null;
    }
    
    public static Structures.Module.CounterpartyWithFact GetMostProbableCounterparty(List<Structures.Module.Fact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetFacts(facts, "Counterparty", "Name")
        .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType)
        .OrderByDescending(x => x.Fields.First(f => f.Name == "Name").Probability);
      
      foreach (var fact in counterpartyFacts)
      {
        var tin = GetFieldValue(fact, "TIN");
        var trrc = GetFieldValue(fact, "TRRC");
        var counterparties = GetCounterparties(tin, trrc);
        if (counterparties.Any())
          return Structures.Module.CounterpartyWithFact.Create(counterparties.First(), fact);
      }
      
      return null;
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
        var name = GetCorrespondentName(fact, "CorrespondentName", "CorrespondentLegalForm");
        correspondentNames.Add(name);
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
        {
          var tin = GetFieldValue(fact, "TIN");
          var trrc = GetFieldValue(fact, "TRRC");
          var counterparties = GetCounterparties(tin, trrc);
          foundByTin.AddRange(counterparties);
        }
        
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
    /// Поиск НОР по извлеченным фактам.
    /// </summary>
    /// <param name="businessUnits">НОР найденные по фактам.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="addressee">Адресат.</param>
    /// <returns>НОР.</returns>
    public static Sungero.Company.IBusinessUnit GetBusinessUnit(List<IBusinessUnit> businessUnits, IEmployee responsible, IEmployee addressee)
    {
      // Если факты с ИНН/КПП не найдены, и по наименованию не найдено, то вернуть НОР из адресата.
      var businessUnitByAddressee = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(addressee);
      
      // Попытаться уточнить по адресату.
      var businessUnit = businessUnits.Any() && businessUnitByAddressee != null
        ? businessUnitByAddressee
        : businessUnits.FirstOrDefault();
      
      return businessUnit ?? businessUnitByAddressee ?? Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
    }
    
    /// <summary>
    /// Поиск НОР по извлеченным фактам.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <returns>НОР.</returns>
    public static List<Sungero.Company.IBusinessUnit> GetBusinessUnitsByFacts(List<Structures.Module.Fact> facts)
    {
      // Получить ИНН/КПП и наименования/ФС корреспондентов из фактов.
      var correspondentNames = new List<string>();
      var correspondents = GetFacts(facts, "Letter", "CorrespondentName")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "CorrespondentName").Probability);
      foreach (var fact in correspondents)
      {
        var name = GetFieldValue(fact, "CorrespondentName");
        correspondentNames.Add(name.ToLower());
      }
      
      // Поиск НОР по наименованию без ФС.
      var foundByName = new List<IBusinessUnit>();
      foreach (var correspondentName in correspondentNames)
      {
        var businessUnits = BusinessUnits.GetAll()
          .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          .Where(x => x.Name.ToLower().Contains(correspondentName));
        foundByName.AddRange(businessUnits);
      }
      
      // Если факты с ИНН/КПП не найдены, то вернуть НОР по наименованию.
      var correspondentTINs = GetFacts(facts, "Counterparty", "TIN")
        .OrderByDescending(x => x.Fields.First(f => f.Name == "TIN").Probability);
      if (!correspondentTINs.Any())
        return foundByName;
      else
      {
        // Поиск по ИНН/КПП.
        var foundByTin = new List<IBusinessUnit>();
        foreach (var fact in correspondentTINs)
        {
          var tin = GetFieldValue(fact, "TIN");
          var trrc = GetFieldValue(fact, "TRRC");
          var businessUnits = GetBusinessUnits(tin, trrc);
          foundByTin.AddRange(businessUnits);
        }
        
        // Найдено по ИНН/КПП.
        if (foundByTin.Any())
          return foundByTin;
        
        // Не найдено. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
        if (foundByName.Any())
          return foundByName.Where(x => string.IsNullOrEmpty(x.TIN) && string.IsNullOrEmpty(x.TRRC)).ToList();
      }
      return foundByName;
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
    public static FactField GetField(Structures.Module.Fact fact, string fieldName)
    {
      return fact.Fields.FirstOrDefault(f => f.Name == fieldName);
    }
    
    /// <summary>
    /// Получить значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Значение поля.</returns>
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
    /// Получить числовое значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Числовое значение поля.</returns>
    public static double? GetFieldNumericalValue(Structures.Module.Fact fact, string fieldName)
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
    public static double? GetFieldNumericalValue(List<Structures.Module.Fact> facts, string factName, string fieldName)
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
    public static DateTime? GetFieldDateTimeValue(Structures.Module.Fact fact, string fieldName)
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
    public static List<Structures.Module.Fact> GetFacts(List<Structures.Module.Fact> facts, string factName, string fieldName)
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
    public static List<Structures.Module.Fact> GetOrderedFacts(List<Structures.Module.Fact> facts, string factName, string orderFieldName)
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
    
    /// <summary>
    /// Получить наименование контрагента.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование контрагента.</param>
    /// <param name="nameFieldName">Наименование поля с наименованием контрагента.</param>
    /// <param name="legalFormFieldName">Наименование поля с организационо-правовой формой контрагента.</param>
    /// <returns>Наименование контрагента.</returns>
    private static string GetCorrespondentName(Fact fact, string nameFieldName, string legalFormFieldName)
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
    private static string GetLeadingDocumentName(Fact fact)
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
    
    public static void LinkFactAndProperty(Structures.Module.RecognizedDocument recognizedDocument,
                                           Structures.Module.Fact fact, string fieldName,
                                           string propertyName, object propertyValue)
    {
      if (fact == null || propertyValue == null)
        return;
      
      var propertyStringValue = propertyValue.ToString();
      if (propertyValue is Sungero.Domain.Shared.IEntity)
        propertyStringValue = ((Sungero.Domain.Shared.IEntity)propertyValue).Id.ToString();
      
      var facts = recognizedDocument.Info.Facts
        .Where(f => f.FactId == fact.Id)
        .Where(f => string.IsNullOrWhiteSpace(fieldName) || f.FieldName == fieldName);
      foreach (var recognizedFact in facts)
      {
        recognizedFact.PropertyName = propertyName;
        recognizedFact.PropertyValue = propertyStringValue;
      }
    }
    
    #endregion
  }
}