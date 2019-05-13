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
      var facts = letterсlassificationResult.ExtractionResult.Facts;
      var subject = GetField(facts, "letter", "subject");
      document.Subject = !string.IsNullOrEmpty(subject) ?
        string.Format("{0}{1}", subject.Substring(0,1).ToUpper(), subject.Remove(0,1).ToLower()) : string.Empty;
      
      // Заполнить данные корреспондента.
      document.Correspondent = GetCounterparty(facts);
      document.InNumber = GetField(facts, "letter", "number");
      var correspondentDate = GetField(facts, "letter", "date");
      if (!string.IsNullOrEmpty(correspondentDate))
        document.Dated = DateTime.Parse(correspondentDate);
      
      // Заполнить данные нашей стороны.
      document.BusinessUnit =  Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      document.Department = GetDepartment(responsible);
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Поиск корреспондента по извлеченным фактам.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <returns>Корреспондент.</returns>
    public static Sungero.Parties.ICounterparty GetCounterparty(List<ArioExtensions.Models.Fact> facts)
    {
      // Получить ИНН/КПП и наименования/ФС контрагентов из фактов.
      var correspondentTINs = GetFacts(facts, "Counterparty", "TIN");
      var correspondentNames = new List<string>();
      foreach (var fact in GetFacts(facts, "Letter", "CorrespondentName"))
      {
        var name = GetField(fact, "CorrespondentName");
        var legalForm = GetField(fact, "CorrespondentLegalForm");
        correspondentNames.Add(string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm));
      }
      
      // Поиск корреспондентов по наименованию.
      var foundByName = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
        .Where(x => x.Note == null || !x.Note.Equals(BusinessUnits.Resources.BusinessUnitComment))
        .Where(x => correspondentNames.Contains(x.Name))
        .ToList();
      
      // Если факты с ИНН/КПП не найдены, то вернуть корреспондента по наименованию.      
      if (!correspondentTINs.Any())
        return foundByName.FirstOrDefault();
      else
      {
        var foundByTin = new List<ICounterparty>();
        foreach (var fact in correspondentTINs)
          foundByTin.AddRange(GetCounterparties(GetField(fact, "TIN"), GetField(fact, "TRRC")));
        
        if (foundByTin.Count == 1)
          return foundByTin.First();
        else
        {
          // Если по ИНН/КПП не нашлось корреспондента, то искать по наименованию в корреспондентах с пустыми ИНН/КПП.
          if (!foundByTin.Any())
          {
            foundByName = foundByName.Where(x => string.IsNullOrEmpty(x.TIN) &&
                                                 (!CompanyBases.Is(x) || string.IsNullOrEmpty(CompanyBases.As(x).TRRC)))
                                     .ToList();
            return foundByName.FirstOrDefault();
          }
          else
          {
            // Если по ИНН/КПП найдено несколько корреспондентов, то уточнить поиск по наименованию.
            foundByName = foundByTin.Where(x => correspondentNames.Any(n => n == x.Name)).ToList();
            if (foundByName.Any())
              return foundByName.FirstOrDefault();
            else
              return foundByTin.FirstOrDefault();
          }
        }
      }
    }
    
    /// <summary>
    /// Получить значение поля из фактов.
    /// </summary>
    /// <param name="fact">Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param>
    /// <returns>Зачение поля.</returns>
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
    public static string GetField(List<ArioExtensions.Models.Fact> facts, string factName, string fieldName)
    {
      var filteredFacts = facts.Where(f => !string.IsNullOrWhiteSpace(f.Name))
                               .Where(f => string.Equals(f.Name, factName, StringComparison.InvariantCultureIgnoreCase))
                               .Where(f => f.Fields.Any());
      IEnumerable<ArioExtensions.Models.FactField> fields = filteredFacts.SelectMany(f => f.Fields);
      var query = fields.OrderByDescending(f => f.Probability);
      var field = query.FirstOrDefault(f => string.Equals(f.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));      
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
    public static List<ArioExtensions.Models.Fact> GetFacts(List<ArioExtensions.Models.Fact> facts, string factName, string fieldName)
    {
      var filteredFacts = facts.Where(fact => fact.Name.Equals(factName, StringComparison.InvariantCultureIgnoreCase));      
      filteredFacts = filteredFacts.Where(f => f.Fields.Any(field => Equals(field.Name, fieldName)));      
      return filteredFacts.OrderByDescending(f => f.Fields.FirstOrDefault(field => Equals(field.Name, fieldName)).Probability).ToList();
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
          
          return strongTinCounterparties.Where(c => CompanyBases.Is(c) && string.IsNullOrWhiteSpace(CompanyBases.As(c).TRRC)).ToList();
        }
        
        return strongTinCounterparties;
      }
      
      return counterparties.ToList();
    }
  }
}