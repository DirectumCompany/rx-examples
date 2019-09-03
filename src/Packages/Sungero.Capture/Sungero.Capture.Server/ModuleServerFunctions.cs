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
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(Contracts.Resources.IncomingInvoiceTypeName,
                                                                              Capture.Server.MockIncomingInvoice.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Incoming, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(Contracts.Resources.ContractTypeName,
                                                                              Capture.Server.MockContract.ClassTypeGuid,
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
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Contracts.Resources.IncomingInvoiceKindName,
                                                                              Contracts.Resources.IncomingInvoiceKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true, false,
                                                                              Capture.Server.MockIncomingInvoice.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockIncomingInvoiceGuid);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Contracts.Resources.ContractKindName,
                                                                              Contracts.Resources.ContractKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Incoming, true, false,
                                                                              Capture.Server.MockContract.ClassTypeGuid,
                                                                              actions, Sungero.Capture.Constants.Module.Initialize.MockContractGuid);
      
      // Добавить параметр признака активации демо-режима.
      Sungero.Docflow.PublicFunctions.Module.InsertOrUpdateDocflowParam(Sungero.Capture.Constants.Module.CaptureMockModeKey, string.Empty);
    }
    
    /// <summary>
    /// Установить основные параметры захвата.
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
    /// Обработать документы комплекта.
    /// </summary>
    /// <param name="package">Распознанные документы комплекта.</param>
    /// <param name="notRecognizedDocuments">Нераспознанные документы комплекта.</param>
    /// <param name="isNeedFillNotClassifiedDocumentNames">Признак необходимости заполнять имена всех неклассифицированных документов в комплекте .</param>
    /// <returns>Список Id созданных документов.</returns>
    [Remote]
    public virtual Structures.Module.DocumentsCreatedByRecognitionResults ProcessPackageAfterCreationDocuments(List<IOfficialDocument> package,
                                                                                                               List<IOfficialDocument> notRecognizedDocuments,
                                                                                                               bool isNeedFillNotClassifiedDocumentNames)
    {
      var result = Structures.Module.DocumentsCreatedByRecognitionResults.Create();
      if (!package.Any() && (notRecognizedDocuments == null || !notRecognizedDocuments.Any()))
        return result;
      
      // Сформировать список документов, которые не смогли пронумеровать.
      var documentsWithRegistrationFailure = package.Where(d => IsDocumentRegistrationFailed(d)).ToList();
      
      // Получить ведущий документ из распознанных документов комплекта. Если список пуст, то из нераспознанных.
      var leadingDocument = package.Any() ? GetLeadingDocument(package) : GetLeadingDocument(notRecognizedDocuments);
      LinkDocuments(leadingDocument, package, notRecognizedDocuments);
      
      // Для документов, нераспознанных Ario:
      // со сканера - заполнить имена,
      // с электронной почты - заполнять имена не надо, они будут как у исходного вложения.
      if (isNeedFillNotClassifiedDocumentNames)
        FillNotClassifiedDocumentNames(leadingDocument, package);
      
      // Добавить документы, не распознанные Ario, к документам комплекта, чтобы вложить в задачу на обработку.
      if (notRecognizedDocuments != null && notRecognizedDocuments.Any())
        package.AddRange(notRecognizedDocuments);
      
      result.LeadingDocumentId = leadingDocument.Id;
      result.RelatedDocumentIds = package.Select(x => x.Id).Where(d => d != result.LeadingDocumentId).ToList();
      result.DocumentWithRegistrationFailureIds = documentsWithRegistrationFailure.Select(x => x.Id).ToList();
      return result;
    }
    
    /// <summary>
    /// Создать документы в RX.
    /// </summary>
    /// <param name="recognitionResults">Json результаты классификации и извлечения фактов.</param>
    /// <param name="originalFile">Исходный файл, полученный с DCS.</param>
    /// <param name="responsible">Сотрудник, ответственный за проверку документов.</param>
    /// <param name="sendedByEmail">Доставлено эл.почтой.</param>
    /// <param name="fromEmail">Адрес эл.почты отправителя.</param>
    /// <returns>Ид созданных документов.</returns>
    [Remote]
    public virtual List<IOfficialDocument> CreateDocumentsByRecognitionResults(string recognitionResults, Structures.Module.IFileInfo originalFile,
                                                                               IEmployee responsible, bool sendedByEmail, string fromEmail)
    {
      var recognizedDocuments = GetRecognizedDocuments(recognitionResults, originalFile, sendedByEmail);
      var package = new List<IOfficialDocument>();
      var documentsWithRegistrationFailure = new List<IOfficialDocument>();
      
      foreach (var recognizedDocument in recognizedDocuments)
      {
        // Поиск документа по ШК.
        var document = OfficialDocuments.Null;
        using (var body = GetDocumentBody(recognizedDocument.BodyGuid))
        {
          var docIds = GetDocumentIdByBarcode(body);
          if (docIds != null && docIds.Any())
          {
            document = OfficialDocuments.GetAll().Where(x => x.Id == docIds.FirstOrDefault()).FirstOrDefault();
            if (document != null)
            {
              CreateVersion(document, recognizedDocument, Resources.VersionCreateFromBarcode);
              document.ExternalApprovalState = Docflow.OfficialDocument.ExchangeState.Signed;
              document.Save();
            }
          }
        }
        
        // Создание нового документа по фактам.
        if (document == null)
          document = CreateDocumentByRecognizedDocument(recognizedDocument, responsible, fromEmail);
        
        // Добавить Ид документа в запись справочника с результатами обработки Ario.
        recognizedDocument.Info.DocumentId = document.Id;
        recognizedDocument.Info.Save();
        
        package.Add(document);
      }
      
      return package;
    }
    
    /// <summary>
    /// Сформировать имена для всех неклассифицированных документов в комплекте.
    /// </summary>
    /// <param name="leadingDocument">Ведущий документ.</param>
    /// <param name="package">Комплект документов.</param>
    /// <remarks>
    /// Если неклассифицированных документов несколько и ведущий документ простой,
    /// то у ведущего будет номер 1, у остальных - следующие по порядку.
    /// </remarks>
    public virtual void FillNotClassifiedDocumentNames(IOfficialDocument leadingDocument, List<IOfficialDocument> package)
    {
      // Если ведущий документ SimpleDocument, то переименовываем его,
      // для того чтобы в имени содержался его порядковый номер.
      int simpleDocumentNumber = 1;
      var leadingDocumentIsSimple = SimpleDocuments.Is(leadingDocument);
      if (leadingDocumentIsSimple)
      {
        leadingDocument.Name = Resources.DocumentNameFormat(simpleDocumentNumber);
        leadingDocument.Save();
        simpleDocumentNumber++;
      }
      
      var addendums = package.Where(x => !Equals(x, leadingDocument));
      foreach (var addendum in addendums)
      {
        // У простых документов, захваченных с почты, имя не меняется.
        if (SimpleDocuments.Is(addendum))
        {
          addendum.Name = leadingDocumentIsSimple
            ? Resources.DocumentNameFormat(simpleDocumentNumber)
            : Resources.AttachmentNameFormat(simpleDocumentNumber);
          addendum.Save();
          simpleDocumentNumber++;
        }
      }
    }
    
    /// <summary>
    /// Связать документы комплекта.
    /// </summary>
    /// <param name="leadingDocument">Ведущий документ.</param>
    /// <param name="package">Распознанные документы комплекта.</param>
    /// <param name="notRecognizedDocuments">Нераспознанные документы комплекта.</param>
    /// <remarks>
    /// Для распознанных документов комплекта, если ведущий документ - простой, то тип связи - "Прочие". Иначе "Приложение".
    /// Для нераспознанных документов комплекта - тип связи "Прочие".
    /// </remarks>
    public virtual void LinkDocuments(IOfficialDocument leadingDocument, List<IOfficialDocument> package, List<IOfficialDocument> notRecognizedDocuments)
    {
      var leadingDocumentIsSimple = SimpleDocuments.Is(leadingDocument);
      
      var relation = leadingDocumentIsSimple
        ? Constants.Module.SimpleRelationRelationName
        : Docflow.PublicConstants.Module.AddendumRelationName;
      
      // Связать приложения с ведущим документом.
      var addendums = package.Where(x => !Equals(x, leadingDocument));
      foreach (var addendum in addendums)
      {
        addendum.Relations.AddFrom(relation, leadingDocument);
        addendum.Save();
      }
      
      // Связать нераспознанные документы с ведущим документом, тип связи - "Прочие".
      if (notRecognizedDocuments != null)
      {
        notRecognizedDocuments = notRecognizedDocuments.Where(x => !Equals(x, leadingDocument)).ToList();
        foreach (var notRecognizedDocument in notRecognizedDocuments)
        {
          notRecognizedDocument.Relations.AddFrom(Constants.Module.SimpleRelationRelationName, leadingDocument);
          notRecognizedDocument.Save();
        }
      }
    }
    
    /// <summary>
    /// Проверить, пронумерован ли документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True, если документ успешно пронумерован. Иначе False.</returns>
    public virtual bool IsDocumentRegistrationFailed(IOfficialDocument document)
    {
      var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
      if (documentParams.ContainsKey(Constants.Module.DocumentNumberingBySmartCaptureResultParamName))
        return true;
      
      return false;
    }
    
    /// <summary>
    /// Десериализовать результат классификации комплекта или отдельного документа в Ario.
    /// </summary>
    /// <param name="jsonClassificationResults">Json с результатами классификации и извлечения фактов.</param>
    /// <param name="originalFile">Исходный файл.</param>
    /// <param name="sendedByEmail">Файл получен из эл.почты.</param>
    /// <returns>Десериализованный результат классификации в Ario.</returns>
    public virtual List<Structures.Module.IRecognizedDocument> GetRecognizedDocuments(string jsonClassificationResults,
                                                                                      Structures.Module.IFileInfo originalFile,
                                                                                      bool sendedByEmail)
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
        recognizedDocument.SendedByEmail = sendedByEmail;
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
          var pages = packageProcessResult.ExtractionResult.DocumentPages;
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
                  .Select(p => string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                                             Constants.Module.PositionElementDelimiter,
                                             p.Page, p.Top, p.Left, p.Width, p.Height,
                                             pages.Where(x => x.Number == p.Page).Select(x => x.Width).FirstOrDefault(),
                                             pages.Where(x => x.Number == p.Page).Select(x => x.Height).FirstOrDefault()));
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
    /// Создать документ DirectumRX из результата классификации в Ario.
    /// </summary>
    /// <param name="recognizedDocument">Результат классификации в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <param name="fromEmail">Адрес эл.почты отправителя.</param>
    /// <returns>Документ, созданный на основе классификации.</returns>
    public virtual IOfficialDocument CreateDocumentByRecognizedDocument(Structures.Module.IRecognizedDocument recognizedDocument,
                                                                        IEmployee responsible, string fromEmail)
    {
      // Входящее письмо.
      var recognizedClass = recognizedDocument.PredictedClass;
      var isMockMode = GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null;
      var document = OfficialDocuments.Null;
      if (recognizedClass == Constants.Module.LetterClassName)
      {
        document = isMockMode
          ? CreateMockIncomingLetter(recognizedDocument)
          : CreateIncomingLetter(recognizedDocument, responsible);
      }
      
      // Акт выполненных работ.
      else if (recognizedClass == Constants.Module.ContractStatementClassName)
      {
        document = isMockMode
          ? CreateMockContractStatement(recognizedDocument)
          : CreateContractStatement(recognizedDocument, responsible);
      }
      
      // Товарная накладная.
      else if (recognizedClass == Constants.Module.WaybillClassName)
      {
        document = isMockMode
          ? CreateMockWaybill(recognizedDocument)
          : CreateWaybill(recognizedDocument, responsible);
      }
      
      // Счет-фактура.
      else if (recognizedClass == Constants.Module.TaxInvoiceClassName)
      {
        document = isMockMode
          ? CreateMockIncomingTaxInvoice(recognizedDocument)
          : CreateTaxInvoice(recognizedDocument, responsible, false);
      }
      
      // Корректировочный счет-фактура.
      else if (recognizedClass == Constants.Module.TaxinvoiceCorrectionClassName && !isMockMode)
      {
        document = CreateTaxInvoice(recognizedDocument, responsible, true);
      }
      
      // УПД.
      else if (recognizedClass == Constants.Module.UniversalTransferDocumentClassName && !isMockMode)
      {
        document = CreateUniversalTransferDocument(recognizedDocument, responsible, false);
      }
      
      // УКД.
      else if (recognizedClass == Constants.Module.GeneralCorrectionDocumentClassName && !isMockMode)
      {
        document = CreateUniversalTransferDocument(recognizedDocument, responsible, true);
      }
      
      // Счет на оплату.
      else if (recognizedClass == Constants.Module.IncomingInvoiceClassName)
      {
        document = isMockMode
          ? CreateMockIncomingInvoice(recognizedDocument)
          : CreateIncomingInvoice(recognizedDocument, responsible);
      }
      
      // Договор.
      else if (recognizedClass == Constants.Module.ContractClassName && isMockMode)
      {
        document = CreateMockContract(recognizedDocument);
      }
      
      // Все нераспознанные документы создать простыми.
      else
      {
        var name = !string.IsNullOrWhiteSpace(recognizedDocument.OriginalFile.Description) ? recognizedDocument.OriginalFile.Description : Resources.SimpleDocumentName;
        document = Docflow.PublicFunctions.SimpleDocument.CreateSimpleDocument(name, responsible);
      }
      
      FillDeliveryMethod(document, recognizedDocument.SendedByEmail);
      /* Статус документа задается до создания версии, чтобы корректно прописалось наименование,
         если его не из чего формировать.*/
      document.VerificationState = Docflow.OfficialDocument.VerificationState.InProcess;
      CreateVersion(document, recognizedDocument);
      
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Определить ведущий документ распознанного комплекта.
    /// </summary>
    /// <param name="package">Комплект документов.</param>
    /// <returns>Ведущий документ.</returns>
    public virtual IOfficialDocument GetLeadingDocument(List<IOfficialDocument> package)
    {
      var leadingDocument = package.FirstOrDefault();
      var isMockMode = GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null;
      
      var incLetter = isMockMode
        ? package.Where(d => MockIncomingLetters.Is(d)).FirstOrDefault()
        : package.Where(d => IncomingLetters.Is(d)).FirstOrDefault();
      if (incLetter != null)
        return incLetter;
      
      var contract = isMockMode
        ? package.Where(d => MockContracts.Is(d)).FirstOrDefault()
        : null;
      if (contract != null)
        return contract;
      
      var contractStatement = isMockMode
        ? package.Where(d => MockContractStatements.Is(d)).FirstOrDefault()
        : package.Where(d => FinancialArchive.ContractStatements.Is(d)).FirstOrDefault();
      if (contractStatement != null)
        return contractStatement;
      
      var waybill = isMockMode
        ? package.Where(d => MockWaybills.Is(d)).FirstOrDefault()
        : package.Where(d => FinancialArchive.Waybills.Is(d)).FirstOrDefault();
      if (waybill != null)
        return waybill;
      
      var incTaxInvoice = isMockMode
        ? package.Where(d => MockIncomingTaxInvoices.Is(d)).FirstOrDefault()
        : package.Where(d => FinancialArchive.IncomingTaxInvoices.Is(d)).FirstOrDefault();
      if (incTaxInvoice != null)
        return incTaxInvoice;
      
      return leadingDocument;
    }
    
    /// <summary>
    /// Выполнить задания на контроль возврата пришедшего документа. Если их нет - отправить уведомление ответственному за документ.
    /// </summary>
    /// <param name="document">Захваченный документ.</param>
    public virtual void CompleteApprovalCheckReturnAssignment(IOfficialDocument document)
    {
      // Выполнить задания на контроль возврата.
      var documentsGroupGuid = Docflow.PublicConstants.Module.TaskMainGroup.ApprovalTask;
      var approvalRegulationsAssignments = Assignments.GetAll()
        .Where(a => Sungero.Docflow.ApprovalCheckReturnAssignments.Is(a))
        .Where(a => !a.Completed.HasValue)
        .Where(a => a.AttachmentDetails.Any(g => g.GroupId == documentsGroupGuid && g.AttachmentId == document.Id))
        .GroupBy(a => a.MainTask.Id);
      
      foreach (var assignmentGroup in approvalRegulationsAssignments)
        assignmentGroup.FirstOrDefault().Complete(Sungero.Docflow.ApprovalCheckReturnAssignment.Result.Signed);
      
      // Если нет заданий на контроль возврата, то отправить уведомление ответственному за документ.
      if (approvalRegulationsAssignments.ToList().Count == 0)
      {
        var responsibleEmployee = Sungero.Docflow.PublicFunctions.OfficialDocument.GetDocumentResponsibleEmployee(document);
        if (responsibleEmployee != null && responsibleEmployee.IsSystem != true)
        {
          var notice = SimpleTasks.Create();
          notice.Subject = Resources.NoticeToAuthorSubjectFormat(document);
          notice.Attachments.Add(document);
          
          if (notice.Subject.Length > notice.Info.Properties.Subject.Length)
            notice.Subject = notice.Subject.Substring(0, notice.Info.Properties.Subject.Length);

          var routeStep = notice.RouteSteps.AddNew();
          routeStep.AssignmentType = Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
          routeStep.Performer = responsibleEmployee;
          routeStep.Deadline = null;
          notice.Start();
        }
      }
    }
    #endregion
    
    #region Фасад DirectumRX
    
    [Public, Remote]
    public static string GetCurrentTenant()
    {
      var currentTenant = Sungero.Domain.TenantRegistry.Instance.CurrentTenant;
      return currentTenant != null ? currentTenant.Id : string.Empty;
    }
    
    /// <summary>
    /// Получить значение параметра из docflow_params.
    /// </summary>
    /// <param name="paramName">Наименование параметра.</param>
    /// <returns>Значение параметра.</returns>
    public virtual object GetDocflowParamsValue(string paramName)
    {
      var command = string.Format(Queries.Module.SelectDocflowParamsValue, paramName);
      return Docflow.PublicFunctions.Module.ExecuteScalarSQLCommand(command);
    }
    
    /// <summary>
    /// Получить адрес сервиса Арио.
    /// </summary>
    /// <returns>Адрес Арио.</returns>
    [Remote]
    public virtual string GetArioUrl()
    {
      var commandExecutionResult = GetDocflowParamsValue(Constants.Module.ArioUrlKey);
      var arioUrl = string.Empty;
      if (!(commandExecutionResult is DBNull) && commandExecutionResult != null)
        arioUrl = commandExecutionResult.ToString();
      
      return arioUrl;
    }

    /// <summary>
    /// Поиск адресата письма.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Адресат.</returns>
    public virtual Structures.Module.EmployeeWithFact GetAdresseeByFact(Sungero.Capture.Structures.Module.IFact fact, string propertyName)
    {
      var result = Structures.Module.EmployeeWithFact.Create(Sungero.Company.Employees.Null, fact, false);
      if (fact == null)
        return result;
      
      var addressee = GetFieldValue(fact, "Addressee");
      var employees =  Company.PublicFunctions.Employee.Remote.GetEmployeesByName(addressee);
      result.Employee = employees.FirstOrDefault();
      result.IsTrusted = (employees.Count() == 1) ? IsTrustedField(fact, "Addressee") : false;
      return result;
    }

    /// <summary>
    /// Получить полное ФИО из частей имени содержащихся в факте.
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
      
      return GetFullNameByFact(surname, name, patronymic);
    }
    
    /// <summary>
    /// Получить полное ФИО из частей имени содержащихся в факте для договоров.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</returns>
    public static string GetFullNameByFactForContract(Sungero.Capture.Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var surname = GetFieldValue(fact, "SignatorySurname");
      var name = GetFieldValue(fact, "SignatoryName");
      var patronymic = GetFieldValue(fact, "SignatoryPatrn");
      
      return GetFullNameByFact(surname, name, patronymic);
    }
    
    /// <summary>
    /// Сформировать полное ФИО из частей имени.
    /// </summary>
    /// <param name="surnameFieldValue">Фамилия.</param>
    /// <param name="nameFieldValue">Имя.</param>
    /// <param name="patronymicFieldValue">Отчество.</param>
    /// <returns>Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</returns>
    public static string GetFullNameByFact(string surnameFieldValue, string nameFieldValue, string patronymicFieldValue)
    {
      // Собрать ФИО из фамилии, имени и отчества.
      var parts = new List<string>();
      
      if (!string.IsNullOrWhiteSpace(surnameFieldValue))
        parts.Add(surnameFieldValue);
      if (!string.IsNullOrWhiteSpace(nameFieldValue))
        parts.Add(nameFieldValue);
      if (!string.IsNullOrWhiteSpace(patronymicFieldValue))
        parts.Add(patronymicFieldValue);
      
      return string.Join(" ", parts);
    }
    
    /// <summary>
    /// Получить сокращенное ФИО из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О.".</returns>
    public virtual string GetShortNameByFact(Sungero.Capture.Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var surname = GetFieldValue(fact, "Surname");
      var name = GetFieldValue(fact, "Name");
      var patronymic = GetFieldValue(fact, "Patrn");
      return Parties.PublicFunctions.Person.GetSurnameAndInitialsInTenantCulture(name, patronymic, surname);
    }
    
    /// <summary>
    /// Получить контактные лица по данным из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент - владелец контактного лица.</param>
    /// <returns>Список контактных лиц.</returns>
    public virtual IQueryable<IContact> GetContactsByFact(Sungero.Capture.Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return new List<IContact>().AsQueryable();
      
      var fullName = GetFullNameByFact(fact);
      var shortName = GetShortNameByFact(fact);
      return Parties.PublicFunctions.Contact.GetContactsByName(fullName, shortName, counterparty);
    }
    
    /// <summary>
    /// Получить контактное лицо по данным из факта.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public virtual Structures.Module.ContactWithFact GetContactByFact(Sungero.Capture.Structures.Module.IFact fact, string propertyName, ICounterparty counterparty, string counterpartyPropertyName)
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
      
      var filteredContacts =  GetContactsByFact(fact, counterparty);
      if (!filteredContacts.Any())
        return result;
      result.Contact = filteredContacts.FirstOrDefault();
      result.IsTrusted = (filteredContacts.Count() == 1) ? IsTrustedField(fact, "Type") : false;
      return result;
    }
    
    /// <summary>
    /// Поиск контактного лица контрагента в верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public virtual Structures.Module.ContactWithFact GetContactByVerifiedData(Structures.Module.IFact fact, string propertyName, string  counterpartyPropertyValue, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContactWithFact.Create(Contacts.Null, fact, false);
      var contactField = GetFieldByVerifiedData(fact, propertyName, counterpartyPropertyValue, counterpartyPropertyName);
      if (contactField == null)
        return result;
      int contactId;
      if (!int.TryParse(contactField.VerifiedValue, out contactId))
        return result;
      
      var filteredContact = Contacts.GetAll(x => x.Id == contactId).FirstOrDefault();
      if (filteredContact != null)
      {
        result.Contact = filteredContact;
        result.IsTrusted = contactField.IsTrusted == true;
      }
      return result;
    }
    
    /// <summary>
    /// Получить ведущие документы по номеру и дате из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Список документов с подходящими номером и датой.</returns>
    public virtual IQueryable<Sungero.Contracts.IContractualDocument> GetLeadingDocuments(Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return new List<Sungero.Contracts.IContractualDocument>().AsQueryable();
      
      var docDate = GetFieldDateTimeValue(fact, "DocumentBaseDate");
      var number = GetFieldValue(fact, "DocumentBaseNumber");
      
      if (string.IsNullOrWhiteSpace(number))
        return new List<Sungero.Contracts.IContractualDocument>().AsQueryable();
      
      return Sungero.Contracts.ContractualDocuments.GetAll(x => x.RegistrationNumber == number &&
                                                           x.RegistrationDate == docDate &&
                                                           (counterparty == null || x.Counterparty.Equals(counterparty)));
    }
    
    /// <summary>
    /// Получить ведущий документ по номеру и дате из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="leadingDocPropertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyName">Имя свойства, связанного с контрагентом.</param>
    /// <returns>Структура, содержащая ведущий документ, факт и признак доверия.</returns>
    public virtual Structures.Module.ContractWithFact GetLeadingDocument(Structures.Module.IFact fact, ICounterparty counterparty, string leadingDocPropertyName, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContractWithFact.Create(Contracts.ContractualDocuments.Null, fact, false);
      if (fact == null)
        return result;
      
      if (string.IsNullOrEmpty(leadingDocPropertyName))
      {
        result = GetContractByVerifiedData(fact, leadingDocPropertyName, counterparty.Id.ToString(), counterpartyPropertyName);
        if (result.Contract != null)
          return result;
      }
      var contracts = GetLeadingDocuments(fact, counterparty);
      result.Contract = contracts.FirstOrDefault();
      result.IsTrusted = (contracts.Count() == 1) ? IsTrustedField(fact, "DocumentBaseNumber") : false;
      return result;
    }
    
    /// <summary>
    /// Поиск ведущего документ в верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя свойства, связанного с контрагентом.</param>
    /// <returns></returns>
    public virtual Structures.Module.ContractWithFact GetContractByVerifiedData(Structures.Module.IFact fact, string propertyName, string  counterpartyPropertyValue, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContractWithFact.Create(Contracts.ContractualDocuments.Null, fact, false);
      var contractField = GetFieldByVerifiedData(fact, propertyName, counterpartyPropertyValue, counterpartyPropertyName);
      if (contractField == null)
        return result;
      
      int docId;
      if (!int.TryParse(contractField.VerifiedValue, out docId))
        return result;
      
      var filteredDocument = Contracts.ContractualDocuments.GetAll(x => x.Id == docId).FirstOrDefault();
      if (filteredDocument != null)
      {
        result.Contract = filteredDocument;
        result.IsTrusted = contractField.IsTrusted == true;
      }
      return result;
    }
    
    /// <summary>
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="leadingDocument">Ведущий документ.</param>
    /// <param name="documents">Прочие документы из комплекта.</param>
    /// <param name="documentsWithRegistrationFailure">Документы, которые не удалось зарегистрировать.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="emailBody">Тело электронного письма.</param>
    /// <returns>Простая задача.</returns>
    [Public, Remote]
    public virtual void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents,
                                          List<IOfficialDocument> documentsWithRegistrationFailure, Company.IEmployee responsible, Docflow.IOfficialDocument emailBody)
    {
      if (leadingDocument == null)
        return;
      
      // Собрать пакет документов. Порядок важен, чтобы ведущий был первым.
      var package = new List<IOfficialDocument>();
      package.Add(leadingDocument);
      package.AddRange(documents);
      
      // Тема.
      var task = SimpleTasks.Create();
      task.Subject = Docflow.SimpleDocuments.Is(leadingDocument)
        ? Resources.FailedClassifyDocumentsTaskName
        : package.Count() > 1
        ? Resources.CheckPackageTaskNameFormat(leadingDocument)
        : Resources.CheckDocumentTaskNameFormat(leadingDocument);
      if (task.Subject.Length > task.Info.Properties.Subject.Length)
        task.Subject = task.Subject.Substring(0, task.Info.Properties.Subject.Length);
      
      // Вложить в задачу и выдать права на документы ответственному.
      var notClassifiedDocumentsHyperlinks = new List<string>();
      foreach (var document in package)
      {
        document.AccessRights.Grant(responsible, DefaultAccessRightsTypes.FullAccess);
        document.Save();
        task.Attachments.Add(document);
        
        // Собрать ссылки на неклассифицированные документы.
        // Не нужно считать тело письма неклассифицированным документом и писать об этом.
        if (Docflow.SimpleDocuments.Is(document) && (emailBody == null || document.Id != emailBody.Id))
          notClassifiedDocumentsHyperlinks.Add(Hyperlinks.Get(document));
      }
      
      // Собрать ссылки на документы, которые не удалось зарегистрировать.
      var documentsWithRegistrationFailureHyperlinks = new List<string>();
      documentsWithRegistrationFailureHyperlinks.AddRange(documentsWithRegistrationFailure.Select(x => Hyperlinks.Get(x)));
      
      // Текст задачи.
      task.ActiveText = Resources.CheckPackageTaskText;
      
      // Добавить в текст задачи список не классифицированных документов.
      if (notClassifiedDocumentsHyperlinks.Any())
      {
        var failedClassifyTaskText = notClassifiedDocumentsHyperlinks.Count() == 1
          ? Resources.FailedClassifyDocumentTaskText
          : Resources.FailedClassifyDocumentsTaskText;
        
        var notClassifiedDocumentsHyperlinksLabel = string.Join("\n    ", notClassifiedDocumentsHyperlinks);
        
        task.ActiveText = string.Format("{0}\n\n{1}\n    {2}", task.ActiveText, failedClassifyTaskText, notClassifiedDocumentsHyperlinksLabel);
      }
      
      // Добавить в текст задачи список документов, которые не удалось зарегистрировать.
      if (documentsWithRegistrationFailure.Any())
      {
        documentsWithRegistrationFailure = documentsWithRegistrationFailure.OrderBy(x => x.DocumentKind.Name).ToList();
        var documentsText = documentsWithRegistrationFailure.Count() == 1 ? Sungero.Capture.Resources.Document : Sungero.Capture.Resources.Documents;
        var documentKinds = documentsWithRegistrationFailure.Select(x => string.Format("\"{0}\"", x.DocumentKind.Name)).Distinct();
        var documentKindsText = documentKinds.Count() == 1 ? Sungero.Capture.Resources.Kind : Sungero.Capture.Resources.Kinds;
        var documentKindsListText = string.Join(", ", documentKinds);
        
        var documentsWithRegistrationFailureTaskText = string.Format(Sungero.Capture.Resources.DocumentsWithRegistrationFailureTaskText,
                                                                     documentsText, documentKindsText, documentKindsListText);
        
        var documentsWithRegistrationFailureHyperlinksLabel = string.Join("\n    ", documentsWithRegistrationFailureHyperlinks);
        
        task.ActiveText = string.Format("{0}\n\n{1}\n    {2}", task.ActiveText, documentsWithRegistrationFailureTaskText,
                                        documentsWithRegistrationFailureHyperlinksLabel);
      }
      
      // Маршрут.
      var step = task.RouteSteps.AddNew();
      step.AssignmentType = Workflow.SimpleTask.AssignmentType.Assignment;
      step.Performer = responsible;
      
      task.NeedsReview = false;
      task.Deadline = Calendar.Now.AddWorkingHours(4);
      task.Save();
      task.Start();
      
      // Старт фонового процесса на смену статуса верификации.
      Jobs.ChangeVerificationState.Enqueue();
    }
    
    /// <summary>
    /// Отправить документы ответственному.
    /// </summary>
    /// <param name="documentsCreatedByRecognition">Результат создания документов.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// <param name="emailBody">Тело электронного письма.</param>
    [Remote]
    public virtual void SendToResponsible(Structures.Module.DocumentsCreatedByRecognitionResults documentsCreatedByRecognition,
                                          Sungero.Company.IEmployee responsible, Docflow.IOfficialDocument emailBody)
    {
      var leadingDocument = OfficialDocuments.GetAll()
        .FirstOrDefault(x => x.Id == documentsCreatedByRecognition.LeadingDocumentId);

      var relatedDocuments = documentsCreatedByRecognition.RelatedDocumentIds != null
        ? OfficialDocuments.GetAll().Where(x => documentsCreatedByRecognition.RelatedDocumentIds.Contains(x.Id)).ToList()
        : new List<Docflow.IOfficialDocument>();
      
      if (leadingDocument == null && !relatedDocuments.Any())
        return;
      
      var allDocuments = new List<Docflow.IOfficialDocument>();
      allDocuments.Add(leadingDocument);
      allDocuments.AddRange(relatedDocuments);
      
      var documentsWithRegistrationFailure = documentsCreatedByRecognition.DocumentWithRegistrationFailureIds != null
        ? allDocuments.Where(x => documentsCreatedByRecognition.DocumentWithRegistrationFailureIds.Contains(x.Id)).ToList()
        : new List<Docflow.IOfficialDocument>();
      SendToResponsible(leadingDocument, relatedDocuments, documentsWithRegistrationFailure, responsible, emailBody);
    }
    
    #endregion
    
    #region Простой документ
    
    /// <summary>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// Создать документ из тела эл. письма.
    /// </summary>
    /// <param name="mailInfo">Информация о захваченном письме.</param>
    /// <param name="bodyInfo">Путь до тела пиьсма.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// <returns>ИД созданного документа.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromEmailBody(Structures.Module.CapturedMailInfo mailInfo,
                                                                                     Structures.Module.IFileInfo bodyInfo,
                                                                                     IEmployee responsible)
    {
      if (!System.IO.File.Exists(bodyInfo.Path))
        throw new ApplicationException(Resources.FileNotFoundFormat(bodyInfo.Path));
      
      var documentName = Resources.EmailBodyDocumentNameFormat(mailInfo.FromEmail);
      var document = Docflow.PublicFunctions.SimpleDocument.CreateSimpleDocument(documentName, responsible);      
      FillDeliveryMethod(document, true);
      
      // Наименование и содержание.
      if (!string.IsNullOrWhiteSpace(mailInfo.Subject))
      {
        var name = string.Format("{0} \"{1}\"", document.Name, mailInfo.Subject);
        document.Name = Docflow.PublicFunctions.OfficialDocument.AddClosingQuote(name, document);
        var subject = mailInfo.Subject;
        if (subject.Length > document.Info.Properties.Subject.Length)
          subject = subject.Substring(0, document.Info.Properties.Subject.Length);
        document.Subject = subject;
      }
      
      using (var body = new MemoryStream(bodyInfo.Data))
      {
        document.CreateVersion();
        var version = document.LastVersion;
        if (Path.GetExtension(bodyInfo.Path).ToLower() == ".html")
        {
          var pdfConverter = new AsposeExtensions.Converter();
          using (var pdfDocumentStream = pdfConverter.GeneratePdf(body, "html"))
          {
            if (pdfDocumentStream != null)
            {
              version.Body.Write(pdfDocumentStream);
              version.AssociatedApplication = Content.AssociatedApplications.GetByExtension("pdf");
            }
          }
        }
        
        // Если тело письма не удалось преобразовать в pdf или расширение не html, то в тело пишем исходный файл.
        if (version.Body.Size == 0)
        {
          version.Body.Write(body);
          version.AssociatedApplication = GetAssociatedApplicationByFileName(bodyInfo.Path);
        }
      }
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать простой документ из файла.
    /// </summary>
    /// <param name="File">Файл.</param>
    /// <param name="sendedByEmail">Доставлен эл.почтой.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// <returns>Простой документ.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromFile(Structures.Module.IFileInfo fileInfo,
                                                                                bool sendedByEmail,
                                                                                IEmployee responsible)
    {
      var name = Path.GetFileName(fileInfo.Description);
      var document = Docflow.PublicFunctions.SimpleDocument.CreateSimpleDocument(name, responsible);
      FillDeliveryMethod(document, sendedByEmail);
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
    public virtual Docflow.IOfficialDocument CreateIncomingLetter(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var document = Sungero.RecordManagement.IncomingLetters.Create();
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
      var addressee = GetAdresseeByFact(addresseeFact, document.Info.Properties.Addressee.Name);
      document.Addressee = addressee.Employee;
      LinkFactAndProperty(recognizedDocument, addresseeFact, "Addressee", props.Addressee.Name, document.Addressee, addressee.IsTrusted);
      
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
      
      var businessUnitWithFact = GetBusinessUnitWithFact(businessUnitsWithFacts, responsible, document.Addressee, document.Info.Properties.BusinessUnit.Name);
      document.BusinessUnit = businessUnitWithFact.BusinessUnit;
      LinkFactAndProperty(recognizedDocument, businessUnitWithFact.Fact, null, props.BusinessUnit.Name, document.BusinessUnit, businessUnitWithFact.IsTrusted);
      
      document.Department = document.Addressee != null
        ? Company.PublicFunctions.Department.GetDepartment(document.Addressee)
        : Company.PublicFunctions.Department.GetDepartment(responsible);
      
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
      
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо (демо режим).
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
      FillMockRegistrationData(document, recognizedDocument, "Document");
      
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
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, "DocumentAmount", "Currency").FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentCurrencyFact, "Currency", props.Currency.Name, document.Currency.Id);
      
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
      var facts = recognizedDocument.Facts;
      var document = FinancialArchive.ContractStatements.Create();
      var props = AccountingDocumentBases.Info.Properties;
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add("SELLER");
      counterpartyTypes.Add("BUYER");
      counterpartyTypes.Add(string.Empty);
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var sellerFact = factMatches.Where(m => m.Type == "SELLER").FirstOrDefault();
      var buyerFact = factMatches.Where(m => m.Type == "BUYER").FirstOrDefault();
      var nonTypeFacts = factMatches.Where(m => m.Type == string.Empty).ToList();
      var counterpartyAndBusinessUnitFacts = GetCounterpartyAndBusinessUnitFacts(buyerFact, sellerFact, nonTypeFacts, responsible);
      FillAccountingDocumentCounterpartyAndBusinessUnit(document, counterpartyAndBusinessUnitFacts);
      LinkAccountingDocumentCounterpartyAndBusinessUnit(recognizedDocument, counterpartyAndBusinessUnitFacts);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognizedDocument, "Document");
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      var leadingDocument = GetLeadingDocument(leadingDocFact, document.Counterparty,
                                               document.Info.Properties.LeadingDocument.Name,
                                               document.Info.Properties.Counterparty.Name);
      document.LeadingDocument = leadingDocument.Contract;
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, leadingDocument.IsTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную (демо режим).
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки накладной в Ario.</param>
    /// <returns>Накладная.</returns>
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
      FillMockRegistrationData(document, recognizedDocument, "FinancialDocument");
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, "DocumentAmount", "Currency").FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentCurrencyFact, "Currency", props.Currency.Name, document.Currency.Id);
      
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
      
      return document;
    }
    
    /// <summary>
    /// Создать накладную.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки накладной в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Накладная.</returns>
    public virtual Docflow.IOfficialDocument CreateWaybill(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var facts = recognizedDocument.Facts;
      var document = FinancialArchive.Waybills.Create();
      var props = document.Info.Properties;
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add("SUPPLIER");
      counterpartyTypes.Add("PAYER");
      counterpartyTypes.Add("SHIPPER");
      counterpartyTypes.Add("CONSIGNEE");
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var sellerFact = factMatches.Where(m => m.Type == "SUPPLIER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "SHIPPER").FirstOrDefault();
      var buyerFact = factMatches.Where(m => m.Type == "PAYER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "CONSIGNEE").FirstOrDefault();
      var counterpartyAndBusinessUnitFacts = GetCounterpartyAndBusinessUnitFacts(buyerFact, sellerFact, responsible);
      
      FillAccountingDocumentCounterpartyAndBusinessUnit(document, counterpartyAndBusinessUnitFacts);
      LinkAccountingDocumentCounterpartyAndBusinessUnit(recognizedDocument, counterpartyAndBusinessUnitFacts);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognizedDocument, "FinancialDocument");
      
      // Документ-основание.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      var contractualDocuments = GetLeadingDocuments(leadingDocFact, document.Counterparty);
      document.LeadingDocument = contractualDocuments.FirstOrDefault();
      var isTrusted = (contractualDocuments.Count() == 1) ? IsTrustedField(leadingDocFact, "DocumentBaseName") : false;
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      
      return document;
    }
    
    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру (демо режим).
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Счет-фактура.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var facts = recognizedDocument.Facts;
      var document = Sungero.Capture.MockIncomingTaxInvoices.Create();
      var props = document.Info.Properties;
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
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
      FillMockRegistrationData(document, recognizedDocument, "FinancialDocument");
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, "DocumentAmount", "Currency").FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentCurrencyFact, "Currency", props.Currency.Name, document.Currency.Id);
      
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
      
      return document;
    }
    
    /// <summary>
    /// Создать счет-фактуру.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="isAdjustment">Корректировочная.</param>
    /// <returns>Счет-фактура.</returns>
    public virtual Docflow.IOfficialDocument CreateTaxInvoice(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible, bool isAdjustment)
    {
      var facts = recognizedDocument.Facts;
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
      var document = AccountingDocumentBases.Null;
      var props = AccountingDocumentBases.Info.Properties;
      
      // Определить направление документа, НОР и КА.
      // Если НОР выступает продавцом, то создаем исходящую счет-фактуру, иначе - входящую.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add("SELLER");
      counterpartyTypes.Add("BUYER");
      counterpartyTypes.Add("SHIPPER");
      counterpartyTypes.Add("CONSIGNEE");
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var sellerFact = factMatches.Where(m => m.Type == "SELLER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "SHIPPER").FirstOrDefault();
      var buyerFact = factMatches.Where(m => m.Type == "BUYER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "CONSIGNEE").FirstOrDefault();
      
      var buyerIsBusinessUnit = buyerFact != null && buyerFact.BusinessUnit != null;
      var sellerIsBusinessUnit = sellerFact != null && sellerFact.BusinessUnit != null;
      var counterpartyAndBusinessUnitFacts = Structures.Module.BusinessUnitAndCounterpartyFacts.Create();
      if (buyerIsBusinessUnit && sellerIsBusinessUnit)
      {
        // Мультинорность. Уточнить НОР по ответственному.
        if (Equals(sellerFact.BusinessUnit, responsibleEmployeeBusinessUnit))
        {
          // Исходящий документ.
          document = FinancialArchive.OutgoingTaxInvoices.Create();
          counterpartyAndBusinessUnitFacts.CounterpartyFact = buyerFact;
          counterpartyAndBusinessUnitFacts.BusinessUnitFact = sellerFact;
        }
        else
        {
          // Входящий документ.
          document = FinancialArchive.IncomingTaxInvoices.Create();
          counterpartyAndBusinessUnitFacts.CounterpartyFact = sellerFact;
          counterpartyAndBusinessUnitFacts.BusinessUnitFact = buyerFact;
        }
      }
      else if (buyerIsBusinessUnit)
      {
        // Входящий документ.
        document = FinancialArchive.IncomingTaxInvoices.Create();
        counterpartyAndBusinessUnitFacts.CounterpartyFact = sellerFact;
        counterpartyAndBusinessUnitFacts.BusinessUnitFact = buyerFact;
      }
      else if (sellerIsBusinessUnit)
      {
        // Исходящий документ.
        document = FinancialArchive.OutgoingTaxInvoices.Create();
        counterpartyAndBusinessUnitFacts.CounterpartyFact = buyerFact;
        counterpartyAndBusinessUnitFacts.BusinessUnitFact = sellerFact;
      }
      else
      {
        // НОР не найдена по фактам - НОР будет взята по ответственному.
        if (buyerFact != null && buyerFact.Counterparty != null && (sellerFact == null || sellerFact.Counterparty == null))
        {
          // Исходящий документ, потому что buyer - контрагент, а другой информации нет.
          document = FinancialArchive.OutgoingTaxInvoices.Create();
          counterpartyAndBusinessUnitFacts.CounterpartyFact = buyerFact;
        }
        else
        {
          // Входящий документ.
          document = FinancialArchive.IncomingTaxInvoices.Create();
          counterpartyAndBusinessUnitFacts.CounterpartyFact = sellerFact;
        }
      }
      counterpartyAndBusinessUnitFacts.ResponsibleEmployeeBusinessUnit = responsibleEmployeeBusinessUnit;
      
      // Вид документа.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и КА.
      FillAccountingDocumentCounterpartyAndBusinessUnit(document, counterpartyAndBusinessUnitFacts);
      LinkAccountingDocumentCounterpartyAndBusinessUnit(recognizedDocument, counterpartyAndBusinessUnitFacts);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognizedDocument, "FinancialDocument");
      
      // Корректировочный документ.
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(facts, "FinancialDocument", "CorrectionDate").FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(facts, "FinancialDocument", "CorrectionNumber").FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, "CorrectionDate");
        var correctionNumber = GetFieldValue(correctionNumberFact, "CorrectionNumber");
        var isTrusted = false;
        if (correctionDate != null && !string.IsNullOrEmpty(correctionNumber))
        {
          if (FinancialArchive.IncomingTaxInvoices.Is(document))
          {
            var documents = FinancialArchive.IncomingTaxInvoices.GetAll()
              .Where(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
            document.Corrected = documents.FirstOrDefault();
            isTrusted = documents.Count() == 1;
          }
          else
          {
            var documents = FinancialArchive.OutgoingTaxInvoices.GetAll()
              .Where(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
            document.Corrected = documents.FirstOrDefault();
            isTrusted = documents.Count() == 1;
          }
          LinkFactAndProperty(recognizedDocument, correctionDateFact, "CorrectionDate", props.Corrected.Name, document.Corrected, isTrusted);
          LinkFactAndProperty(recognizedDocument, correctionNumberFact, "CorrectionNumber", props.Corrected.Name, document.Corrected, isTrusted);
        }
      }
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      
      return document;
    }
    
    #endregion
    
    #region УПД
    
    /// <summary>
    /// Создать УПД.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки УПД в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="isAdjustment">Корректировочная.</param>
    /// <returns>УПД.</returns>
    public virtual Docflow.IOfficialDocument CreateUniversalTransferDocument(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible, bool isAdjustment)
    {
      var facts = recognizedDocument.Facts;
      var document = Sungero.FinancialArchive.UniversalTransferDocuments.Create();
      var props = document.Info.Properties;
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add("SELLER");
      counterpartyTypes.Add("BUYER");
      counterpartyTypes.Add("SHIPPER");
      counterpartyTypes.Add("CONSIGNEE");

      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var sellerFact = factMatches.Where(m => m.Type == "SELLER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "SHIPPER").FirstOrDefault();
      var buyerFact = factMatches.Where(m => m.Type == "BUYER").FirstOrDefault() ?? factMatches.Where(m => m.Type == "CONSIGNEE").FirstOrDefault();
      var counterpartyAndBusinessUnitFacts = GetCounterpartyAndBusinessUnitFacts(buyerFact, sellerFact, responsible);
      FillAccountingDocumentCounterpartyAndBusinessUnit(document, counterpartyAndBusinessUnitFacts);
      LinkAccountingDocumentCounterpartyAndBusinessUnit(recognizedDocument, counterpartyAndBusinessUnitFacts);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognizedDocument, "FinancialDocument");
      
      // Корректировочный документ.
      FillCorrectedDocument(document, recognizedDocument, isAdjustment);
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);

      return document;
    }
    
    #endregion
    
    #region Счет на оплату
    
    /// <summary>
    /// Создать счет на оплату (демо режим).
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки счета на оплату в Ario.</param>
    /// <returns>Счет на оплату.</returns>
    public static Docflow.IOfficialDocument CreateMockIncomingInvoice(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockIncomingInvoices.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      var facts = recognizedDocument.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = IsTrustedField(leadingDocFact, "DocumentBaseName");
      LinkFactAndProperty(recognizedDocument, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
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
      
      // Могут прийти контрагенты без типа. Заполнить контрагентами без типа.
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
          
          if (string.IsNullOrWhiteSpace(document.SellerName))
          {
            document.SellerName = name;
            document.SellerTin = tin;
            document.SellerTrrc = trrc;
            LinkFactAndProperty(recognizedDocument, fact, "Name", props.SellerName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "LegalForm", props.SellerName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "TIN", props.SellerTin.Name, tin);
            LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.SellerTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BuyerName))
          {
            document.BuyerName = name;
            document.BuyerTin = tin;
            document.BuyerTrrc = trrc;
            LinkFactAndProperty(recognizedDocument, fact, "Name", props.BuyerName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "LegalForm", props.BuyerName.Name, name);
            LinkFactAndProperty(recognizedDocument, fact, "TIN", props.BuyerTin.Name, tin);
            LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.BuyerTrrc.Name, trrc);
          }
        }
      }
      
      // Дата и номер.
      var dateFact = GetOrderedFacts(facts, "FinancialDocument", "Date").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, "FinancialDocument", "Number").FirstOrDefault();
      document.Date = GetFieldDateTimeValue(dateFact, "Date");
      document.Number = GetFieldValue(numberFact, "Number");
      LinkFactAndProperty(recognizedDocument, dateFact, "Date", props.Date.Name, document.Date);
      LinkFactAndProperty(recognizedDocument, numberFact, "Number", props.Number.Name, document.Number);
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, "VatAmount");
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, "DocumentAmount", "Currency").FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentCurrencyFact, "Currency", props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    
    /// <summary>
    /// Создать счет на оплату.
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Счет на оплату.</returns>
    public virtual Docflow.IOfficialDocument CreateIncomingInvoice(Structures.Module.IRecognizedDocument recognizedDocument, IEmployee responsible)
    {
      var facts = recognizedDocument.Facts;
      var document = Contracts.IncomingInvoices.Create();
      var props = document.Info.Properties;
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add("SELLER");
      counterpartyTypes.Add("BUYER");
      counterpartyTypes.Add(string.Empty);
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var sellerFact = factMatches.Where(m => m.Type == "SELLER").FirstOrDefault();
      var buyerFact = factMatches.Where(m => m.Type == "BUYER").FirstOrDefault();
      var nonTypeFacts = factMatches.Where(m => m.Type == string.Empty).ToList();
      var counterpartyAndBusinessUnitFacts = GetCounterpartyAndBusinessUnitFacts(buyerFact, sellerFact, nonTypeFacts, responsible);
      FillAccountingDocumentCounterpartyAndBusinessUnit(document, counterpartyAndBusinessUnitFacts);
      LinkAccountingDocumentCounterpartyAndBusinessUnit(recognizedDocument, counterpartyAndBusinessUnitFacts);
      
      // Договор.
      var contractFact = GetOrderedFacts(facts, "FinancialDocument", "DocumentBaseName").FirstOrDefault();
      var contract = GetLeadingDocument(contractFact, document.Counterparty, document.Info.Properties.Contract.Name, document.Info.Properties.Counterparty.Name);
      document.Contract = contract.Contract;
      LinkFactAndProperty(recognizedDocument, contractFact, null, props.Contract.Name, document.Contract, contract.IsTrusted);
      
      // Дата.
      var dateFact = GetOrderedFacts(facts, "FinancialDocument", "Date").FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, "FinancialDocument", "Number").FirstOrDefault();
      document.Date = GetFieldDateTimeValue(dateFact, "Date");
      LinkFactAndProperty(recognizedDocument, dateFact, "Date", props.Date.Name, document.Date);
      
      // Номер.
      var number = GetFieldValue(numberFact, "Number");
      Nullable<bool> isTrustedNumber = null;
      if (number.Length > document.Info.Properties.Number.Length)
      {
        number = number.Substring(0, document.Info.Properties.Number.Length);
        isTrustedNumber = false;
      }
      document.Number = number;
      LinkFactAndProperty(recognizedDocument, numberFact, "Number", props.Number.Name, document.Number, isTrustedNumber);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmount(document, recognizedDocument);
      
      return document;
    }
    
    #endregion
    
    #region Договор
    
    /// <summary>
    /// Создать договор (демо режим).
    /// </summary>
    /// <param name="recognizedDocument">Результат обработки счета на оплату в Ario.</param>
    /// <returns>Договор.</returns>
    public static Docflow.IOfficialDocument CreateMockContract(Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var document = Sungero.Capture.MockContracts.Create();
      
      // Основные свойства.
      document.DocumentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      document.Name = document.DocumentKind.ShortName;
      var props = document.Info.Properties;
      var facts = recognizedDocument.Facts;
      
      // Дата и номер.
      FillMockRegistrationData(document, recognizedDocument, "Document");
      
      // Заполнить данные сторон.
      var partyNameFacts = GetOrderedFacts(facts, "Counterparty", "Name");
      if (partyNameFacts.Count() > 0)
      {
        var fact = partyNameFacts.First();
        document.FirstPartyName = GetCorrespondentName(fact, "Name", "LegalForm");
        document.FirstPartySignatory = GetFullNameByFactForContract(fact);
        LinkFactAndProperty(recognizedDocument, fact, "Name", props.FirstPartyName.Name, document.FirstPartyName);
        LinkFactAndProperty(recognizedDocument, fact, "SignatorySurname", props.FirstPartySignatory.Name, document.FirstPartySignatory);
        LinkFactAndProperty(recognizedDocument, fact, "SignatoryName", props.FirstPartySignatory.Name, document.FirstPartySignatory);
        LinkFactAndProperty(recognizedDocument, fact, "SignatoryPatrn", props.FirstPartySignatory.Name, document.FirstPartySignatory);
      }
      if (partyNameFacts.Count() > 1)
      {
        var fact = partyNameFacts.Last();
        document.SecondPartyName = GetCorrespondentName(fact, "Name", "LegalForm");
        document.SecondPartySignatory = GetFullNameByFactForContract(fact);
        LinkFactAndProperty(recognizedDocument, fact, "Name", props.SecondPartyName.Name, document.SecondPartyName);
        LinkFactAndProperty(recognizedDocument, fact, "SignatorySurname", props.SecondPartySignatory.Name, document.SecondPartySignatory);
        LinkFactAndProperty(recognizedDocument, fact, "SignatoryName", props.SecondPartySignatory.Name, document.SecondPartySignatory);
        LinkFactAndProperty(recognizedDocument, fact, "SignatoryPatrn", props.SecondPartySignatory.Name, document.SecondPartySignatory);
      }
      
      // Заполнить ИНН/КПП сторон.
      var tinTrrcFacts = GetOrderedFacts(facts, "Counterparty", "TIN");
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.FirstPartyTin = GetFieldValue(fact, "TIN");
        document.FirstPartyTrrc = GetFieldValue(fact, "TRRC");
        LinkFactAndProperty(recognizedDocument, fact, "TIN", props.FirstPartyTin.Name, document.FirstPartyTin);
        LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.FirstPartyTrrc.Name, document.FirstPartyTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.SecondPartyTin = GetFieldValue(fact, "TIN");
        document.SecondPartyTrrc = GetFieldValue(fact, "TRRC");
        LinkFactAndProperty(recognizedDocument, fact, "TIN", props.SecondPartyTin.Name, document.SecondPartyTin);
        LinkFactAndProperty(recognizedDocument, fact, "TRRC", props.SecondPartyTrrc.Name, document.SecondPartyTrrc);
      }
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, "DocumentAmount", "Amount").FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, "Amount");
      LinkFactAndProperty(recognizedDocument, documentAmountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      
      var documentVatAmountFact = GetOrderedFacts(facts, "DocumentAmount", "VatAmount").FirstOrDefault();
      document.VatAmount = GetFieldNumericalValue(documentVatAmountFact, "VatAmount");
      LinkFactAndProperty(recognizedDocument, documentVatAmountFact, "VatAmount", props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, "DocumentAmount", "Currency").FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, "Currency");
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognizedDocument, documentCurrencyFact, "Currency", props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    #endregion
    
    #region Заполнение свойств документа
    
    /// <summary>
    /// Заполнить НОР и контрагента в бухгалтерском документе.
    /// </summary>
    /// <param name="document">Бухгалтерский документ.</param>
    /// <param name="facts">Факты для документа с подобором НОР и контрагента.</param>
    public virtual void FillAccountingDocumentCounterpartyAndBusinessUnit(IAccountingDocumentBase document,
                                                                          Structures.Module.BusinessUnitAndCounterpartyFacts facts)
    {
      var counterpartyFact = facts.CounterpartyFact;
      var businessUnitFact = facts.BusinessUnitFact;
      var businessUnitMatched = businessUnitFact != null && businessUnitFact.BusinessUnit != null;
      
      document.Counterparty = counterpartyFact != null ? counterpartyFact.Counterparty : null;
      document.BusinessUnit = businessUnitMatched ? businessUnitFact.BusinessUnit : facts.ResponsibleEmployeeBusinessUnit;
    }
    
    /// <summary>
    /// Связать факты для НОР и контрагента с подобранными значениями.
    /// </summary>
    /// <param name="recognizedDocument">Результаты обработки бухгалтерского документа в Ario.</param>
    /// <param name="facts">Факты для документа с подбором НОР и контрагента.</param>
    public virtual void LinkAccountingDocumentCounterpartyAndBusinessUnit(Structures.Module.IRecognizedDocument recognizedDocument,
                                                                          Structures.Module.BusinessUnitAndCounterpartyFacts facts)
    {
      var counterpartyPropertyName = AccountingDocumentBases.Info.Properties.Counterparty.Name;
      var businessUnitPropertyName = AccountingDocumentBases.Info.Properties.BusinessUnit.Name;
      var counterpartyMatched = facts.CounterpartyFact != null &&
        facts.CounterpartyFact.Counterparty != null;
      var businessUnitMatched = facts.BusinessUnitFact != null &&
        facts.BusinessUnitFact.BusinessUnit != null;
      
      if (counterpartyMatched)
        LinkFactAndProperty(recognizedDocument, facts.CounterpartyFact.Fact, null,
                            counterpartyPropertyName, facts.CounterpartyFact.Counterparty, facts.CounterpartyFact.IsTrusted);

      if (businessUnitMatched)
        LinkFactAndProperty(recognizedDocument, facts.BusinessUnitFact.Fact, null,
                            businessUnitPropertyName, facts.BusinessUnitFact.BusinessUnit, facts.BusinessUnitFact.IsTrusted);
      else
        LinkFactAndProperty(recognizedDocument, null, null,
                            businessUnitPropertyName, facts.ResponsibleEmployeeBusinessUnit, false);
      
    }
    
    /// <summary>
    /// Заполнить сумму и валюту.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    public virtual void FillAmount(IAccountingDocumentBase document, Structures.Module.IRecognizedDocument recognizedDocument)
    {
      var facts = recognizedDocument.Facts;
      var props = document.Info.Properties;
      var amountFacts = GetOrderedFacts(facts, "DocumentAmount", "Amount");
      
      var amountFact = amountFacts.FirstOrDefault();
      if (amountFact != null)
      {
        document.TotalAmount = GetFieldNumericalValue(amountFact, "Amount");
        LinkFactAndProperty(recognizedDocument, amountFact, "Amount", props.TotalAmount.Name, document.TotalAmount);
      }
      
      // В факте с суммой документа может быть не указана валюта, поэтому факт с валютой ищем отдельно,
      // так как на данный момент функция используется только для обработки бухгалтерских документов,
      // а в них все расчеты ведутся в одной валюте.
      var currencyFacts = GetOrderedFacts(facts, "DocumentAmount", "Currency");
      var currencyFact = currencyFacts.FirstOrDefault();
      if (currencyFact != null)
      {
        var currencyCode = GetFieldValue(currencyFact, "Currency");
        document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
        LinkFactAndProperty(recognizedDocument, currencyFact, "Currency", props.Currency.Name, document.Currency);
      }
    }
    
    /// <summary>
    /// Пронумеровать документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public virtual void NumberDocument(IOfficialDocument document,
                                       Structures.Module.IRecognizedDocument recognizedDocument,
                                       string factName)
    {
      // Проверить конфигурацию DirectumRX на возможность нумерации документа.
      // Можем нумеровать только тогда, когда однозначно подобран журнал.
      var registers = Sungero.Docflow.PublicFunctions.OfficialDocument.GetDocumentRegistersByDocument(document, Sungero.Docflow.RegistrationSetting.SettingType.Numeration);
      
      // Присвоить номер, если вид документа - нумеруемый.
      if (document.DocumentKind == null || document.DocumentKind.NumberingType != Docflow.DocumentKind.NumberingType.Numerable)
        return;

      // Если не смогли пронумеровать, то передать параметр с результатом в задачу на обработку документа.
      if (registers.Count != 1)
      {
        ((Domain.Shared.IExtendedEntity)document).Params[Constants.Module.DocumentNumberingBySmartCaptureResultParamName] = false;
        return;
      }

      // Дата.
      var facts = recognizedDocument.Facts;
      var regDateFact = GetOrderedFacts(facts, factName, "Date").FirstOrDefault();
      var regDate = GetFieldDateTimeValue(regDateFact, "Date");
      Nullable<bool> isTrustedDate = null;
      if (regDate == null || !regDate.HasValue)
      {
        regDate = Calendar.SqlMinValue;
        isTrustedDate = false;
      }
      
      // Номер.
      var regNumberFact = GetOrderedFacts(facts, factName, "Number").FirstOrDefault();
      var regNumber = GetFieldValue(regNumberFact, "Number");
      Nullable<bool> isTrustedNumber = null;
      if (string.IsNullOrWhiteSpace(regNumber))
      {
        regNumber = Resources.UnknownNumber;
        isTrustedNumber = false;
      }
      else if (regNumber.Length > document.Info.Properties.RegistrationNumber.Length)
      {
        regNumber = regNumber.Substring(0, document.Info.Properties.RegistrationNumber.Length);
        isTrustedNumber = false;
      }
      
      // Не сохранять документ при нумерации, чтобы не потерять параметр DocumentNumberingBySmartCaptureResult.
      Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, registers.First(), regDate, regNumber, false, false);
      
      var props = document.Info.Properties;
      LinkFactAndProperty(recognizedDocument, regDateFact, "Date", props.RegistrationDate.Name, document.RegistrationDate, isTrustedDate);
      LinkFactAndProperty(recognizedDocument, regNumberFact, "Number", props.RegistrationNumber.Name, document.RegistrationNumber, isTrustedNumber);
    }
    
    /// <summary>
    /// Заполнить номер и дату для Mock-документов.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public static void FillMockRegistrationData(IOfficialDocument document,
                                                Structures.Module.IRecognizedDocument recognizedDocument,
                                                string factName)
    {
      // Дата.
      var facts = recognizedDocument.Facts;
      var regDateFact = GetOrderedFacts(facts, factName, "Date").FirstOrDefault();
      document.RegistrationDate = GetFieldDateTimeValue(regDateFact, "Date");

      // Номер.
      var regNumberFact = GetOrderedFacts(facts, factName, "Number").FirstOrDefault();
      var regNumber = GetFieldValue(regNumberFact, "Number");
      Nullable<bool> isTrustedNumber = null;
      if (regNumber.Length > document.Info.Properties.RegistrationNumber.Length)
      {
        regNumber = regNumber.Substring(0, document.Info.Properties.RegistrationNumber.Length);
        isTrustedNumber = false;
      }
      document.RegistrationNumber = regNumber;
      
      var props = document.Info.Properties;
      LinkFactAndProperty(recognizedDocument, regDateFact, "Date", props.RegistrationDate.Name, document.RegistrationDate);
      LinkFactAndProperty(recognizedDocument, regNumberFact, "Number", props.RegistrationNumber.Name, document.RegistrationNumber, isTrustedNumber);
    }
    
    /// <summary>
    /// Заполнить корректируемый документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognizedDocument">Результат обработки документа в Ario.</param>
    /// <param name="factName">Корректировочный.</param>
    public virtual void FillCorrectedDocument(IAccountingDocumentBase document,
                                              Structures.Module.IRecognizedDocument recognizedDocument,
                                              bool isAdjustment)
    {
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionDate").FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(recognizedDocument.Facts, "FinancialDocument", "CorrectionNumber").FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, "CorrectionDate");
        var correctionNumber = GetFieldValue(correctionNumberFact, "CorrectionNumber");
        
        document.Corrected = FinancialArchive.UniversalTransferDocuments.GetAll()
          .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
        var props = document.Info.Properties;
        LinkFactAndProperty(recognizedDocument, correctionDateFact, "CorrectionDate", props.Corrected.Name, document.Corrected, true);
        LinkFactAndProperty(recognizedDocument, correctionNumberFact, "CorrectionNumber", props.Corrected.Name, document.Corrected, true);
      }
    }
    
    /// <summary>
    /// Заполнить способ доставки
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="sendedByEmail">Доставлен эл.почтой.</param>
    public virtual void FillDeliveryMethod(IOfficialDocument document, bool sendedByEmail)
    {
      var methodName = sendedByEmail
        ? MailDeliveryMethods.Resources.EmailMethod
        : MailDeliveryMethods.Resources.MailMethod;
      
      document.DeliveryMethod = MailDeliveryMethods.GetAll()
        .Where(m => m.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefault();
    }
    
    #endregion
    
    #region Поиск контрагента/НОР
    
    /// <summary>
    /// Получить факты с контрагентом указанного типа из общего списка фактов.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="counterpartyType">Тип контрагента.</param>
    /// <returns></returns>
    public virtual List<IFact> GetCounterpartyFacts(List<Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetOrderedFacts(facts, "Counterparty", "Name")
        .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType);
      
      if (!counterpartyFacts.Any())
        counterpartyFacts = GetOrderedFacts(facts, "Counterparty", "TIN")
          .Where(f => GetFieldValue(f, "CounterpartyType") == counterpartyType);
      
      return counterpartyFacts.ToList();
    }
    
    /// <summary>
    /// Подобрать по факту контрагента и НОР.
    /// </summary>
    /// <param name="allFacts">Факты.</param>
    /// <param name="counterpartyTypes">Типы фактов контрагентов.</param>
    /// <returns>Наши организации и контрагенты, найденные по фактам.</returns>
    public virtual List<Structures.Module.BusinessUnitAndCounterpartyWithFact> MatchFactsWithBusinessUnitsAndCounterparties(List<Structures.Module.IFact> allFacts,
                                                                                                                            List<string> counterpartyTypes)
    {
      var counterpartyPropertyName = AccountingDocumentBases.Info.Properties.Counterparty.Name;
      var businessUnitPropertyName = AccountingDocumentBases.Info.Properties.BusinessUnit.Name;
      
      // Фильтр фактов по типам.
      var facts = new List<IFact>();
      foreach (var counterpartyType in counterpartyTypes)
        facts.AddRange(GetCounterpartyFacts(allFacts, counterpartyType));
      
      var businessUnitsAndCounterparties = new List<Structures.Module.BusinessUnitAndCounterpartyWithFact>();
      foreach (var fact in facts)
      {
        var counterparty = Counterparties.Null;
        var businessUnit = BusinessUnits.Null;
        bool isTrusted = true;
        
        // Поиск контрагента по хэшу.
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, counterpartyPropertyName);
        if (verifiedCounterparty != null)
        {
          counterparty = verifiedCounterparty.Counterparty;
          isTrusted = verifiedCounterparty.IsTrusted;
        }
        
        // Поиск НОР по хэшу.
        var verifiedBusinessUnit = GetBusinessUnitByVerifiedData(fact, businessUnitPropertyName);
        if (verifiedBusinessUnit != null)
        {
          businessUnit = verifiedBusinessUnit.BusinessUnit;
          isTrusted = verifiedBusinessUnit.IsTrusted;
        }
        
        // Поиск по инн/кпп.
        var tin = GetFieldValue(fact, "TIN");
        var trrc = GetFieldValue(fact, "TRRC");
        if (businessUnit == null)
        {
          var businessUnits = Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);
          if (businessUnits.Count > 1)
            isTrusted = false;
          businessUnit = businessUnits.FirstOrDefault();
        }
        if (counterparty == null)
        {
          var counterparties = Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, true);
          if (counterparties.Count > 1)
            isTrusted = false;
          counterparty = counterparties.FirstOrDefault();
        }
        
        if (counterparty != null || businessUnit != null)
        {
          var businessUnitAndCounterparty = Structures.Module.BusinessUnitAndCounterpartyWithFact.Create(businessUnit, counterparty, fact, GetFieldValue(fact, "CounterpartyType"), isTrusted);
          businessUnitsAndCounterparties.Add(businessUnitAndCounterparty);
          continue;
        }
        
        // Если не нашли по инн/кпп то ищем по наименованию.
        var name = GetCorrespondentName(fact, "Name", "LegalForm");
        counterparty = Counterparties.GetAll()
          .FirstOrDefault(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed && x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        businessUnit = BusinessUnits.GetAll()
          .FirstOrDefault(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed && x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (counterparty != null || businessUnit != null)
        {
          var businessUnitAndCounterparty = Structures.Module.BusinessUnitAndCounterpartyWithFact.Create(businessUnit, counterparty, fact, GetFieldValue(fact, "CounterpartyType"), false);
          businessUnitsAndCounterparties.Add(businessUnitAndCounterparty);
        }
      }
      
      return businessUnitsAndCounterparties;
    }
    
    public virtual Structures.Module.BusinessUnitAndCounterpartyFacts GetCounterpartyAndBusinessUnitFacts(Structures.Module.BusinessUnitAndCounterpartyWithFact buyerFact,
                                                                                                          Structures.Module.BusinessUnitAndCounterpartyWithFact sellerFact,
                                                                                                          List<Structures.Module.BusinessUnitAndCounterpartyWithFact> nonTypeFacts,
                                                                                                          IEmployee responsibleEmployee)
    {
      Structures.Module.BusinessUnitAndCounterpartyWithFact counterpartyFact = null;
      Structures.Module.BusinessUnitAndCounterpartyWithFact businessUnitFact = null;
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsibleEmployee);
      
      // НОР.
      var businessUnitFindedNotExactly = false;
      if (buyerFact != null)
      {
        // НОР по факту с типом BUYER.
        businessUnitFact = buyerFact;
      }
      else
      {
        // НОР по факту без типа.
        var nonTypeBusinessUnits = nonTypeFacts.Where(m => m.BusinessUnit != null);
        
        // Уточнить НОР по ответственному.
        if (nonTypeBusinessUnits.Count() > 1)
        {
          businessUnitFact = nonTypeBusinessUnits.Where(m => Equals(m.BusinessUnit, responsibleEmployeeBusinessUnit)).FirstOrDefault();
          
          // НОР не определилась по ответственному.
          if (businessUnitFact == null)
            businessUnitFindedNotExactly = true;
        }
        
        if (businessUnitFact == null)
          businessUnitFact = nonTypeBusinessUnits.FirstOrDefault();
        
        // Подсветить жёлтым, если НОР было несколько и определить по ответственному не удалось.
        if (businessUnitFindedNotExactly)
          businessUnitFact.IsTrusted = false;
      }
      
      // Контрагент.
      if (sellerFact != null && sellerFact.Counterparty != null)
      {
        // Контрагент по факту с типом SELLER.
        counterpartyFact = sellerFact;
      }
      else
      {
        // Контрагент по факту без типа. Исключить факт, по которому нашли НОР.
        var nonTypeCounterparties = nonTypeFacts
          .Where(m => m.Counterparty != null)
          .Where(m => !Equals(m, businessUnitFact));
        counterpartyFact = nonTypeCounterparties.FirstOrDefault();
        
        // Подсветить жёлтым, если контрагентов было несколько.
        if (nonTypeCounterparties.Count() > 1 || businessUnitFindedNotExactly)
          counterpartyFact.IsTrusted = false;
      }
      
      return Structures.Module.BusinessUnitAndCounterpartyFacts.Create(businessUnitFact, counterpartyFact, responsibleEmployeeBusinessUnit);
    }
    
    public virtual Structures.Module.BusinessUnitAndCounterpartyFacts GetCounterpartyAndBusinessUnitFacts(Structures.Module.BusinessUnitAndCounterpartyWithFact buyerFact,
                                                                                                          Structures.Module.BusinessUnitAndCounterpartyWithFact sellerFact,
                                                                                                          IEmployee responsibleEmployee)
    {
      Structures.Module.BusinessUnitAndCounterpartyWithFact counterpartyFact = null;
      Structures.Module.BusinessUnitAndCounterpartyWithFact businessUnitFact = null;
      
      // НОР.
      if (buyerFact != null)
        businessUnitFact = buyerFact;
      
      // Контрагент.
      if (sellerFact != null && sellerFact.Counterparty != null)
        counterpartyFact = sellerFact;
      
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsibleEmployee);
      
      return Structures.Module.BusinessUnitAndCounterpartyFacts.Create(businessUnitFact, counterpartyFact, responsibleEmployeeBusinessUnit);
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
    public virtual Structures.Module.CounterpartyWithFact GetCounterparty(List<Structures.Module.IFact> facts, string propertyName)
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
          var counterparties = Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, true);
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
    /// <returns>Связка контрагент + факт.</returns>
    public virtual Structures.Module.CounterpartyWithFact GetCounterpartyByVerifiedData(Structures.Module.IFact fact, string propertyName)
    {
      var counterpartyUnitField = GetFieldByVerifiedData(fact, propertyName);
      if (counterpartyUnitField == null)
        return null;
      int counterpartyId;
      if (!int.TryParse(counterpartyUnitField.VerifiedValue, out counterpartyId))
        return null;
      
      var filteredCounterparty = Counterparties.GetAll(x => x.Id == counterpartyId).FirstOrDefault();
      if (filteredCounterparty == null)
        return null;
      
      return Structures.Module.CounterpartyWithFact.Create(filteredCounterparty, fact, counterpartyUnitField.IsTrusted == true);
    }
    
    /// <summary>
    /// Получить нор по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связку контрагент + факт.</returns>
    public virtual Structures.Module.BusinessUnitWithFact GetBusinessUnitByVerifiedData(Structures.Module.IFact fact, string propertyName)
    {
      var businessUnitField = GetFieldByVerifiedData(fact, propertyName);
      if (businessUnitField == null)
        return null;
      int businessUnitId;
      if (!int.TryParse(businessUnitField.VerifiedValue, out businessUnitId))
        return null;
      
      var filteredBusinessUnit = BusinessUnits.GetAll(x => x.Id == businessUnitId).FirstOrDefault();
      if (filteredBusinessUnit == null)
        return null;
      
      return Structures.Module.BusinessUnitWithFact.Create(filteredBusinessUnit, fact, businessUnitField.IsTrusted == true);
    }
    
    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного и адресата.
    /// </summary>
    /// <param name="businessUnits">НОР, найденные по фактам.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <param name="addressee">Адресат.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public virtual Capture.Structures.Module.BusinessUnitWithFact GetBusinessUnitWithFact(List<Capture.Structures.Module.BusinessUnitWithFact> businessUnitsWithFacts, 
                                                                                          IEmployee responsible, IEmployee addressee, 
                                                                                          string businessUnitPropertyName)
    {
      
      // Сначала поиск по хэшам фактов.
      foreach(var record in businessUnitsWithFacts)
      {
        var result = GetBusinessUnitByVerifiedData(record.Fact, businessUnitPropertyName);
        if (result != null && result.BusinessUnit != null)
          return result;
      }
      
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
    public virtual List<Capture.Structures.Module.BusinessUnitWithFact> GetBusinessUnitsWithFacts(List<Structures.Module.IFact> facts)
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
          var businessUnits = Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);
          var isTrusted = businessUnits.Count == 1;
          foundByTin.AddRange(businessUnits.Select(x => Capture.Structures.Module.BusinessUnitWithFact.Create(x, fact, isTrusted)));
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
    /// Получить значение поля из факта.
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
    /// Получить значение поля из фактов.
    /// </summary>
    /// <param name="facts"> Список фактов.</param>
    /// <param name="factName"> Имя факта, поле которого будет извлечено.</param>
    /// <param name="fieldName">Имя поля, значение которого нужно извлечь.</param> 
    /// <returns>Значение поля, полученное из Ario с наибольшей вероятностью.</returns>
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
    /// Получить запись, которая уже сопоставлялась с переданным фактом.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя свойства документа связанное с фактом.</param>
    /// <returns>Связь факта с свойством.</returns>
    public virtual IDocumentRecognitionInfoFacts GetFieldByVerifiedData(Structures.Module.IFact fact, string propertyName)
    {
      var factLabel = GetFactLabel(fact, propertyName);
      var recognitionInfo = DocumentRecognitionInfos.GetAll()
        .Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return null;
      
      return recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
    }
    
    /// <summary>
    /// Получить запись, которая уже сопоставлялась с переданным фактом, с дополнительной фильтжрацией по контрагенту.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя свойства документа связанное с фактом.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя свойства документа, содержащее контрагента.</param>
    /// <returns>Связь факта с свойством.</returns>
    public virtual IDocumentRecognitionInfoFacts GetFieldByVerifiedData(Structures.Module.IFact fact, string propertyName, string counterpartyPropertyValue, string counterpartyPropertyName)
    {
      var factLabel = GetFactLabel(fact, propertyName);
      var recognitionInfo = DocumentRecognitionInfos.GetAll()
        .Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty) &&
               d.Facts.Any(f => f.PropertyName == counterpartyPropertyName && f.PropertyValue == counterpartyPropertyValue))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return null;
      
      return recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
    }
    
    /// <summary>
    /// Получить список фактов с переданными именем факта и именем поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Список фактов.</returns>
    /// <remarks>С учетом вероятности факта.</remarks>
    public static List<Structures.Module.IFact> GetFacts(List<Structures.Module.IFact> facts, string factName, string fieldName)
    {
      return facts
        .Where(f => f.Name == factName)
        .Where(f => f.Fields.Any(fl => fl.Name == fieldName))
        .ToList();
    }
    
    /// <summary>
    /// Получить список фактов отфильтрованный по имени факта и отсортированный по вероятности поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="orderFieldName">Имя поля по вероятности которого будет произведена сортировка.</param>
    /// <returns>Отсортированный список фактов.</returns>
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
    public virtual System.IO.Stream GetDocumentBody(string documentGuid)
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
    public static double GetDocflowParamsNumbericValue(string paramName)
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
    public static string GetCorrespondentName(Structures.Module.IFact fact, string nameFieldName, string legalFormFieldName)
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
    public static double? ConvertStringToDouble(string field)
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
    [Remote(IsPure = true), Public]
    public virtual List<string> GetRecognizedDocumentProperties(Docflow.IOfficialDocument document, bool isTrusted)
    {
      var result = new List<string>();
      
      if (document == null)
        return result;
      
      var recognitionInfo = DocumentRecognitionInfos.GetAll(x => x.DocumentId == document.Id).FirstOrDefault();
      if (recognitionInfo == null)
        return result;
      
      // Взять только заполненные свойства самого документа. Свойства-коллекции записываются через точку.
      var linkedFacts = recognitionInfo.Facts
        .Where(x => !string.IsNullOrEmpty(x.PropertyName) && !x.PropertyName.Any(с => с == '.'))
        .Where(x => x.IsTrusted == isTrusted);
      
      // Взять только неизмененные пользователем свойства.
      var type = document.GetType();
      foreach (var linkedFact in linkedFacts)
      {
        var propertyName = linkedFact.PropertyName;
        var property = type.GetProperties().Where(p => p.Name == propertyName).LastOrDefault();
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
      
      return result.Distinct().ToList();
    }
    
    /// <summary>
    /// Сохранить результат верификации заполнения свойств.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public virtual void StoreVerifiedPropertiesValues(Docflow.IOfficialDocument document)
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
            var property = type.GetProperties().Where(p => p.Name == propertyName).LastOrDefault();
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
    public static string GetPropertyValueAsString(object propertyValue)
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
    /// <param name="versionNote">Примечание к версии.</param>    
    /// <param name="recognizedDocument">Результат обработки входящего документа в Арио.</param>
    public virtual void CreateVersion(IOfficialDocument document, Structures.Module.IRecognizedDocument recognizedDocument, string versionNote = "")
    {
      var needCreatePublicBody = recognizedDocument.OriginalFile != null && recognizedDocument.OriginalFile.Data != null;
      var pdfApp = Content.AssociatedApplications.GetByExtension("pdf");
      if (pdfApp == Content.AssociatedApplications.Null)
        pdfApp = GetAssociatedApplicationByFileName(recognizedDocument.OriginalFile.Path);
      
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
        {
          version.Body.Write(body);
        }
        
        version.AssociatedApplication = pdfApp;
      }
      
      if (!string.IsNullOrEmpty(versionNote))
        version.Note = versionNote;
      
      // Очистить Subject, если он был пуст до создания версии.
      if (subjectIsEmpty)
        document.Subject = string.Empty;
    }
    
    /// <summary>
    /// Получить приложение-обработчик по имени файла.
    /// </summary>
    /// <param name="fileName">Имя или путь до файла.</param>
    /// <returns>Приложение-обработчик</returns>
    public virtual Sungero.Content.IAssociatedApplication GetAssociatedApplicationByFileName(string fileName)
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
    
    #region ШК
    
    /// <summary>
    /// Поиск ШК в документе и извлечение из него ИД документа в системе.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Ид документа или null, если ИД не найден.</returns>
    /// <remarks>
    /// Поиск ШК осуществляется только на первой странице документа.
    /// Формат ШК - Code128.
    /// </remarks>
    public virtual List<int> GetDocumentIdByBarcode(System.IO.Stream document)
    {
      var result = new List<int>();
      try
      {
        var barcodeReader = new AsposeExtensions.BarcodeReader();
        var barcodeList = barcodeReader.Extract(document);
        if (!barcodeList.Any())
          return result;
        
        var tenantId = GetCurrentTenant();
        var formattedTenantId = Docflow.PublicFunctions.Module.FormatTenantIdForBarcode(tenantId).Trim();
        var ourBarcodes = barcodeList.Where(b => b.Contains(formattedTenantId));
        foreach (var barcode in ourBarcodes)
        {
          int id;
          // Формат штрихкода "id тенанта - id документа".
          var stringId = barcode.Split(new string[] {" - ", "-"}, StringSplitOptions.None).Last();
          if (int.TryParse(stringId, out id))
            result.Add(id);
        }
      }
      catch (AsposeExtensions.BarcodeReaderException e)
      {
        Logger.Error(e.Message);
      }
      return result;
    }
    
    #endregion
  }
}