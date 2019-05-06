using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Company;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Workflow;
using Sungero.Parties;

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
      var сlassificationResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      var letterсlassificationResult = сlassificationResults
        .Where(d => d.ClassificationResult.PredictedClass != null)
        .FirstOrDefault(d => d.ClassificationResult.PredictedClass.Name.Equals(Constants.Module.LetterClassName, StringComparison.InvariantCultureIgnoreCase));
      
      IOfficialDocument leadingDocument;
      if (letterсlassificationResult != null)
      {
        // Если в пакете есть документ с классом письмо, то создаем письмо и делаем его ведущим документом.
        leadingDocument = CreateIncomingLetter(letterсlassificationResult, responsible);
        сlassificationResults.Remove(letterсlassificationResult);
      }
      else
      {
        // Иначе ведущий документ - первый документ в списке.
        leadingDocument = CreateDocumentByGuid(sourceFileName, 0, сlassificationResults.First().ClassificationResult.DocumentGuid, null);
        сlassificationResults = сlassificationResults.Skip(1).ToList();
      }
      
      var documents = new List<IOfficialDocument>();
      int addendumNumber = 1;
      foreach(var сlassificationResult in сlassificationResults)
        documents.Add(CreateDocumentByGuid(string.Empty, addendumNumber++, сlassificationResult.ClassificationResult.DocumentGuid, leadingDocument));
      
      if (leadingDocument != null)
        SendToResponsible(leadingDocument, documents, responsible);
    }
    
    /// <summary>
    /// Получить адрес сервиса Арио.
    /// </summary>
    /// <returns>Адрес Арио.</returns>
    [Remote]
    public static string GetArioUrl()
    {
      var key = Constants.Module.ArioUrlKey;
      var command = string.Format(Queries.Module.SelectArioUrl, key);
      var commandExecutionResult = Docflow.PublicFunctions.Module.ExecuteScalarSQLCommand(command);
      var arioUrl = string.Empty;
      if (!(commandExecutionResult is DBNull) && commandExecutionResult != null)
        arioUrl = commandExecutionResult.ToString();
      
      return arioUrl;
    }
    
    /// <summary>
    /// Создать документ в Rx, тело документа загружается из Арио.
    /// </summary>
    /// <param name="name">Имя документа.</param>
    /// <param name="addendumNumber">Номер приложения.</param>
    /// <param name="documentGuid">Гуид тела документа.</param>
    /// <param name="firstDoc">Ведущий документ.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateDocumentByGuid(string name, int addendumNumber,string documentGuid, IOfficialDocument leadingDoc)
    {
      var documentBody = GetDocumentBody(documentGuid);
      var document = SimpleDocuments.Create();
      document.Name = string.IsNullOrWhiteSpace(name) ? Resources.DocumentNameFormat(addendumNumber) : name;
      document.CreateVersionFrom(documentBody, "pdf");
      if (leadingDoc != null)
      {
        if (IncomingDocumentBases.Is(leadingDoc))
        {
          document.Relations.AddFrom(Docflow.PublicConstants.Module.AddendumRelationName, leadingDoc);
          document.Name = string.IsNullOrWhiteSpace(name) ? Resources.AttachmentNameFormat(addendumNumber) : name;
        }
        else
        {
          document.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, leadingDoc);
        }
      }
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо в RX.
    /// </summary>
    /// <param name="letterсlassificationResult">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public static Docflow.IOfficialDocument CreateIncomingLetter(ArioExtensions.Models.PackageProcessResult letterсlassificationResult, IEmployee responsible)
    {
      // Создать версию раньше заполнения содержания, потому что при создании версии пустое содержание заполнится значением по умолчанию.
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      var documentBody = GetDocumentBody(letterсlassificationResult.ClassificationResult.DocumentGuid);
      document.CreateVersionFrom(documentBody, "pdf");
      
      // Заполнить основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      document.Name = document.DocumentKind.ShortName;
      var facts = letterсlassificationResult.ExtractionResult.Facts;
      var subject = GetField(facts, "letter", "subject");
      document.Subject = subject != null && !string.IsNullOrEmpty(subject.Value) ?
        string.Format("{0}{1}", subject.Value.Substring(0,1).ToUpper(), subject.Value.Remove(0,1).ToLower()) : string.Empty;
      
      // Заполнить данные корреспондента.
      document.Correspondent = null;
      var correspondentNumber = GetField(facts, "letter", "number");
      document.InNumber = correspondentNumber != null ? correspondentNumber.Value : string.Empty;
      var correspondentDate = GetField(facts, "letter", "date");
      if (correspondentDate != null)
        document.Dated = DateTime.Parse(correspondentDate.Value);
      document.Correspondent = GetCounterparty(facts);
      
      // Заполнить данные нашей стороны.
      document.BusinessUnit =  Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      document.Department = GetDepartment(responsible);
      
      document.Save();
      return document;
    }
    
    public static Sungero.Parties.ICounterparty GetCounterparty(List<ArioExtensions.Models.Fact> facts)
    {
      var tinList = new List<ICounterparty>();
      // Поиск корреспондента по ИНН/КПП.
      var correspondentRequisites = GetFacts(facts, "Counterparty", "TIN");
      foreach (var fact in correspondentRequisites)
      {
        var tin = GetField(fact, "TIN");
        var trrc = GetField(fact, "TRRC");
        var counterparties = GetCounterparties(tin, trrc);
        if (counterparties.Any())
          tinList.AddRange(counterparties.ToList());
      }
      
      if (tinList.Count == 1)
        return tinList.First();
      
      var nameList = new List<ICounterparty>();
      // Поиск корреспондента по наименованию, если не нашли по ИНН/КПП.
      var correspondentNames = GetFacts(facts, "Letter", "CorrespondentName");
      foreach (var fact in correspondentNames)
      {
        var name = GetField(fact, "CorrespondentName");
        var legalForm = GetField(fact, "CorrespondentLegalForm");
        name = string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm);
        var counterparties = Counterparties.GetAll().Where(x => x.Name == name &&
                                                           x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed).ToList();
        if (counterparties.Any())
        {
          nameList.AddRange(counterparties.ToList());
        }
      }
      
      if (!tinList.Any())
        return nameList.FirstOrDefault();
      
      var a = nameList.Where(t => tinList.Any(n => n == t));
      if (a.Any())
        return a.First();
      
      return Sungero.Parties.Counterparties.Null;
    }
    
    public static string GetField(ArioExtensions.Models.Fact fact, string fieldName)
    {
      var field = fact.Fields.FirstOrDefault(f => Equals(f.Name, fieldName));
      if (field != null)
        return field.Value;
      return string.Empty;
    }
    
    /// <summary>
    /// Получить поле из фактов.
    /// </summary>
    /// <param name="facts"> Список фактов.</param>
    /// <param name="factName"> Имя факта, поле которого будет извлечено.</param>
    /// <returns> Поле, полученное из Ario с наибольшей вероятностью.</returns>
    public static ArioExtensions.Models.FactField GetField(List<ArioExtensions.Models.Fact> facts, string factName, string fieldName)
    {
      var filteredFacts = facts.Where(fact => fact.Name.Equals(factName, StringComparison.InvariantCultureIgnoreCase));
      IEnumerable<ArioExtensions.Models.FactField> fields = filteredFacts.SelectMany(fact => fact.Fields);
      fields.OrderByDescending(f => f.Probability);
      return fields.FirstOrDefault(f => f.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
    }
    
    public static List<ArioExtensions.Models.Fact> GetFacts(List<ArioExtensions.Models.Fact> facts, string factName, string fieldName)
    {
      var filteredFacts = facts.Where(fact => fact.Name.Equals(factName, StringComparison.InvariantCultureIgnoreCase));
      filteredFacts = filteredFacts.Where(f => f.Fields.Any(field => Equals(field.Name, fieldName)));
      return filteredFacts.OrderByDescending(f => f.Fields.Where(field => Equals(field.Name, fieldName)).Select(field => field.Probability)).ToList();
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
    /// <param name="taskName">Тема задачи.</param>
    /// <param name="documentId">ИД вкладываемого документа.</param>
    /// <param name="responsibleId">ИД ответственного.</param>
    /// <returns>Простая задача.</returns>
    [Remote, Public]
    public static void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents, Company.IEmployee responsible)
    {
      if (leadingDocument == null)
        return;
      
      var task = SimpleTasks.Create();
      task.Subject = Resources.TaskNameFormat(leadingDocument);
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
    
    public static List<ICounterparty> GetCounterparties(string tin, string trrc)
    {
      var searchByTin = !string.IsNullOrWhiteSpace(tin);
      var searchByTrrc = !string.IsNullOrWhiteSpace(trrc);
      
      if (!searchByTin && !searchByTrrc)
        return new List<ICounterparty>();
      
      var counterparties = Counterparties.GetAll();
      var result = new List<ICounterparty>();
      
      // Отфильтровать закрытые сущности.
      counterparties = counterparties.Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      // Поиск по ИНН, если ИНН передан.
      if (searchByTin)
      {
        var counterpartiesByTin = counterparties.Where(x => x.TIN == tin);
        
        // Поиск по КПП, если КПП передан.
        if (searchByTrrc)
        {
          var companies = counterpartiesByTin.ToList().Where(c => CompanyBases.Is(c)).Select(c => CompanyBases.As(c)).ToList();
          result = companies.Where(x => x.TRRC == trrc).ToList<ICounterparty>();
          
          // Поиск по пустому КПП, если не наидено записей по точному совпадению ИНН/КПП.
          if (result.Count == 0)
            result = companies.Where(x => string.IsNullOrWhiteSpace(x.TRRC)).ToList<ICounterparty>();
        }
        else
          result = counterpartiesByTin.ToList();
      }
      
      return result;
    }
    
  }
}