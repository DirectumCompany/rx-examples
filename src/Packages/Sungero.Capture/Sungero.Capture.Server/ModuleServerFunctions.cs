using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sungero.Company;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Docflow;
using Sungero.Capture.Structures.Module;
using Sungero.Metadata;
using Sungero.Parties;
using Sungero.RecordManagement;
using Sungero.Workflow;
using FieldNames = Sungero.Docflow.Constants.Module.FieldNames;
using FactNames = Sungero.Docflow.Constants.Module.FactNames;
using LetterPersonTypes = Sungero.Capture.Constants.Module.LetterPersonTypes;
using CounterpartyTypes = Sungero.Capture.Constants.Module.CounterpartyTypes;
using ArioClassNames = Sungero.Docflow.PublicConstants.Module.ArioClassNames;
using HighlightActivationStyleParamNames = Sungero.Capture.Constants.Module.HighlightActivationStyleParamNames;
using DocflowPublicFunctions = Sungero.Docflow.PublicFunctions.Module;

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
    
    #endregion
    
    #region Общий процесс обработки захваченных документов
    
    /// <summary>
    /// Обработать документы комплекта.
    /// </summary>
    /// <param name="package">Распознанные документы комплекта.</param>
    /// <param name="notRecognizedDocuments">Нераспознанные документы комплекта.</param>
    /// <param name="isNeedRenameNotClassifiedDocumentNames">Признак необходимости переименовать неклассифицированные документы в комплекте.</param>
    /// <returns>Список Id созданных документов.</returns>
    [Remote]
    public virtual Structures.Module.IDocumentsCreatedByRecognitionResults ProcessPackageAfterCreationDocuments(List<IOfficialDocument> package,
                                                                                                                List<IOfficialDocument> notRecognizedDocuments,
                                                                                                                bool isNeedRenameNotClassifiedDocumentNames)
    {
      var result = Structures.Module.DocumentsCreatedByRecognitionResults.Create();
      if (!package.Any() && (notRecognizedDocuments == null || !notRecognizedDocuments.Any()))
        return result;
      
      // Сформировать список документов, которые не смогли пронумеровать.
      var documentsWithRegistrationFailure = package.Where(d => IsDocumentRegistrationFailed(d)).ToList();
      
      // Сформировать список документов, которые найдены по штрихкоду.
      var documentsFoundByBarcode = package.Where(d => IsDocumentFoundByBarcode(d));
      documentsFoundByBarcode = documentsFoundByBarcode.Select(d => RemoveFoundByBarcodeParameter(d)).ToList();
      
      // Сформировать список заблокированных документов.
      var lockedDocuments = package.Where(d => IsDocumentLocked(d)).ToList();
      
      // Получить ведущий документ из распознанных документов комплекта. Если список пуст, то из нераспознанных.
      var leadingDocument = package.Any() ? GetLeadingDocument(package) : GetLeadingDocument(notRecognizedDocuments);
      LinkDocuments(leadingDocument, package, notRecognizedDocuments);
      
      // Для документов, нераспознанных Ario:
      // со сканера - заполнить имена,
      // с электронной почты - заполнять имена не надо, они будут как у исходного вложения.
      if (isNeedRenameNotClassifiedDocumentNames)
        RenameNotClassifiedDocuments(leadingDocument, package);
      
      // Добавить документы, не распознанные Ario, к документам комплекта, чтобы вложить в задачу на обработку.
      if (notRecognizedDocuments != null && notRecognizedDocuments.Any())
        package.AddRange(notRecognizedDocuments);
      
      result.LeadingDocumentId = leadingDocument.Id;
      result.RelatedDocumentIds = package.Select(x => x.Id).Where(d => d != result.LeadingDocumentId).ToList();
      result.DocumentWithRegistrationFailureIds = documentsWithRegistrationFailure.Select(x => x.Id).ToList();
      result.DocumentFoundByBarcodeIds = documentsFoundByBarcode.Select(x => x.Id).ToList();
      result.LockedDocumentIds = lockedDocuments.Select(x => x.Id).ToList();
      return result;
    }
    
    /// <summary>
    /// Создать документы в RX.
    /// </summary>
    /// <param name="recognitionResultsJson">Json результаты классификации и извлечения фактов.</param>
    /// <param name="originalFile">Исходный файл, полученный с DCS.</param>
    /// <param name="responsible">Сотрудник, ответственный за проверку документов.</param>
    /// <param name="sendedByEmail">Доставлено эл.почтой.</param>
    /// <param name="fromEmail">Адрес эл.почты отправителя.(Не используется, добавлен для перекрытия)</param>
    /// <returns>Ид созданных документов.</returns>
    [Remote]
    public virtual List<IOfficialDocument> CreateDocumentsByRecognitionResults(string recognitionResultsJson, Sungero.Docflow.Structures.Module.IFileDto originalFile,
                                                                               IEmployee responsible, bool sendedByEmail, string fromEmail)
    {
      var recognitionResults = GetRecognitionResults(recognitionResultsJson, originalFile, sendedByEmail);
      var package = new List<IOfficialDocument>();
      var documentsWithRegistrationFailure = new List<IOfficialDocument>();
      
      foreach (var recognitionResult in recognitionResults)
      {
        var document = OfficialDocuments.Null;
        using (var body = GetDocumentBody(recognitionResult.BodyGuid))
        {
          var docId = Functions.Module.SearchDocumentBarcodeIds(body).FirstOrDefault();
          // FOD на пустом List<int> вернет 0.
          if (docId != 0)
          {
            document = OfficialDocuments.GetAll().FirstOrDefault(x => x.Id == docId);
            if (document != null)
            {
              var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
              var documentLockInfo = Locks.GetLockInfo(document);
              if (documentLockInfo.IsLocked)
                documentParams[Constants.Module.DocumentIsLockedParamName] = true;
              else
              {
                documentParams[Docflow.PublicConstants.OfficialDocument.FindByBarcodeParamName] = true;
                CreateVersion(document, recognitionResult, Sungero.Docflow.OfficialDocuments.Resources.VersionCreatedByCaptureService);
                document.Save();
              }
            }
          }
        }
        
        // Создание нового документа по фактам.
        if (document == null)
          document = CreateDocumentByRecognitionResult(recognitionResult, responsible);
        
        // Добавить ИД документа в запись справочника с результатами обработки Ario.
        recognitionResult.Info.EntityId = document.Id;
        // Заполнить поле Тип сущности guid'ом конечного типа сущности.
        recognitionResult.Info.EntityType = document.GetEntityMetadata().GetOriginal().NameGuid.ToString();
        recognitionResult.Info.Save();
        
        package.Add(document);
      }
      
      return package;
    }
    
    /// <summary>
    /// Переименовать неклассифицированные документы в комплекте.
    /// </summary>
    /// <param name="leadingDocument">Ведущий документ.</param>
    /// <param name="package">Комплект документов.</param>
    /// <remarks>
    /// Если неклассифицированных документов несколько и ведущий документ простой,
    /// то у ведущего будет номер 1, у остальных - следующие по порядку.
    /// </remarks>
    public virtual void RenameNotClassifiedDocuments(IOfficialDocument leadingDocument, List<IOfficialDocument> package)
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
      return documentParams.ContainsKey(Sungero.Docflow.Constants.Module.DocumentNumberingBySmartCaptureResultParamName);
    }
    
    /// <summary>
    /// Удалить параметр о нахождении документа по штрихкоду.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Документ с удалённым параметром.</returns>
    public virtual IOfficialDocument RemoveFoundByBarcodeParameter(IOfficialDocument document)
    {
      var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
      documentParams.Remove(Docflow.PublicConstants.OfficialDocument.FindByBarcodeParamName);
      return document;
    }
    
    /// <summary>
    /// Проверить, не найден ли уже существующий документ в Rx по штрихкоду.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True, если документ найден в Rx по штрихкоду. Иначе False.</returns>
    public virtual bool IsDocumentFoundByBarcode(IOfficialDocument document)
    {
      var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
      return documentParams.ContainsKey(Docflow.PublicConstants.OfficialDocument.FindByBarcodeParamName);
    }
    
    /// <summary>
    /// Проверить, заблокирован ли документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True, если документ заблокирован. Иначе False.</returns>
    public virtual bool IsDocumentLocked(IOfficialDocument document)
    {
      var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
      return documentParams.ContainsKey(Constants.Module.DocumentIsLockedParamName);
    }
    
    /// <summary>
    /// Десериализовать результат классификации комплекта или отдельного документа в Ario.
    /// </summary>
    /// <param name="jsonClassificationResults">Json с результатами классификации и извлечения фактов.</param>
    /// <param name="file">Исходный файл.</param>
    /// <param name="sendedByEmail">Файл получен из эл.почты.</param>
    /// <returns>Десериализованный результат классификации в Ario.</returns>
    public virtual List<Sungero.Docflow.Structures.Module.IRecognitionResult> GetRecognitionResults(string jsonClassificationResults,
                                                                                                    Sungero.Docflow.Structures.Module.IFileDto file,
                                                                                                    bool sendedByEmail)
    {
      var recognitionResults = new List<Sungero.Docflow.Structures.Module.IRecognitionResult>();
      if (string.IsNullOrWhiteSpace(jsonClassificationResults))
        return recognitionResults;
      
      var packageProcessResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      foreach (var packageProcessResult in packageProcessResults)
      {
        // Класс и гуид тела документа.
        var recognitionResult = Sungero.Docflow.Structures.Module.RecognitionResult.Create();
        var clsResult = packageProcessResult.ClassificationResult;
        recognitionResult.ClassificationResultId = clsResult.Id;
        recognitionResult.BodyGuid = packageProcessResult.Guid;
        recognitionResult.PredictedClass = clsResult.PredictedClass != null ? clsResult.PredictedClass.Name : string.Empty;
        recognitionResult.Message = packageProcessResult.Message;
        recognitionResult.File = file;
        recognitionResult.SendedByEmail = sendedByEmail;
        var docInfo = Commons.EntityRecognitionInfos.Create();
        docInfo.Name = recognitionResult.PredictedClass;
        docInfo.RecognizedClass = recognitionResult.PredictedClass;
        if (clsResult.PredictedProbability != null)
          docInfo.ClassProbability = (double)(clsResult.PredictedProbability);
        
        // Факты и поля фактов.
        recognitionResult.Facts = new List<Sungero.Docflow.Structures.Module.IFact>();
        var smartProcessingSettings = Docflow.PublicFunctions.SmartProcessingSetting.GetSettings();
        //var minFactProbability = smartProcessingSettings.LowerConfidenceLimit;
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
              .Where(f => f.Probability >= smartProcessingSettings.LowerConfidenceLimit)
              .Select(f => Sungero.Docflow.Structures.Module.FactField.Create(f.Id, f.Name, f.Value, f.Probability));
            recognitionResult.Facts.Add(Sungero.Docflow.Structures.Module.Fact.Create(fact.Id, fact.Name, fields.ToList()));
            
            foreach (var factField in fact.Fields)
            {
              var fieldInfo = docInfo.Facts.AddNew();
              fieldInfo.FactId = fact.Id;
              fieldInfo.FieldId = factField.Id;
              fieldInfo.FactName = fact.Name;
              fieldInfo.FieldName = factField.Name;
              fieldInfo.FieldProbability = factField.Probability;
              var fieldValue = factField.Value;
              if (fieldValue != null && fieldValue.Length > 1000)
              {
                fieldValue = fieldValue.Substring(0, 1000);
                Logger.DebugFormat("WARN. Value truncated. Length is over 1000 characters. GetRecognitionResults. FactID({0}). FieldID({1}).",
                                   fact.Id,
                                   factField.Id);
              }
              fieldInfo.FieldValue = fieldValue;
              
              
              // Позиция подсветки фактов в теле документа.
              if (factField.Positions != null)
              {
                var positions = factField.Positions
                  .Where(p => p != null)
                  .Select(p => string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                                             Constants.Module.PositionElementDelimiter,
                                             p.Page,
                                             (int)Math.Round(p.Top),
                                             (int)Math.Round(p.Left),
                                             (int)Math.Round(p.Width),
                                             (int)Math.Round(p.Height),
                                             (int)Math.Round(pages.Where(x => x.Number == p.Page).Select(x => x.Width).FirstOrDefault()),
                                             (int)Math.Round(pages.Where(x => x.Number == p.Page).Select(x => x.Height).FirstOrDefault())));
                fieldInfo.Position = string.Join(Constants.Module.PositionsDelimiter.ToString(), positions);
              }
            }
          }
        }
        docInfo.Save();
        recognitionResult.Info = docInfo;
        recognitionResults.Add(recognitionResult);
      }
      return recognitionResults;
    }
    
    /// <summary>
    /// Создать документ DirectumRX из результата классификации в Ario.
    /// </summary>
    /// <param name="recognitionResult">Результат классификации в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Документ, созданный на основе классификации.</returns>
    public virtual IOfficialDocument CreateDocumentByRecognitionResult(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                                       IEmployee responsible)
    {
      // Входящее письмо.
      var predictedClass = recognitionResult.PredictedClass;
      var isMockMode = Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null;
      var document = OfficialDocuments.Null;
      if (predictedClass == ArioClassNames.Letter)
      {
        document = isMockMode
          ? CreateMockIncomingLetter(recognitionResult)
          : CreateIncomingLetter(recognitionResult, responsible);
      }
      
      // Акт выполненных работ.
      else if (predictedClass == ArioClassNames.ContractStatement)
      {
        document = isMockMode
          ? CreateMockContractStatement(recognitionResult)
          : CreateContractStatement(recognitionResult, responsible);
      }
      
      // Товарная накладная.
      else if (predictedClass == ArioClassNames.Waybill)
      {
        document = isMockMode
          ? CreateMockWaybill(recognitionResult)
          : CreateWaybill(recognitionResult, responsible);
      }
      
      // Счет-фактура.
      else if (predictedClass == ArioClassNames.TaxInvoice)
      {
        document = isMockMode
          ? CreateMockIncomingTaxInvoice(recognitionResult)
          : CreateTaxInvoice(recognitionResult, false, responsible);
      }
      
      // Корректировочный счет-фактура.
      else if (predictedClass == ArioClassNames.TaxinvoiceCorrection && !isMockMode)
      {
        document = CreateTaxInvoice(recognitionResult, true, responsible);
      }
      
      // УПД.
      else if (predictedClass == ArioClassNames.UniversalTransferDocument && !isMockMode)
      {
        document = CreateUniversalTransferDocument(recognitionResult, false, responsible);
      }
      
      // УКД.
      else if (predictedClass == ArioClassNames.UniversalTransferCorrectionDocument && !isMockMode)
      {
        document = CreateUniversalTransferDocument(recognitionResult, true, responsible);
      }
      
      // Счет на оплату.
      else if (predictedClass == ArioClassNames.IncomingInvoice)
      {
        document = isMockMode
          ? CreateMockIncomingInvoice(recognitionResult)
          : CreateIncomingInvoice(recognitionResult, responsible);
      }
      
      // Договор.
      else if (predictedClass == ArioClassNames.Contract)
      {
        document = isMockMode
          ? CreateMockContract(recognitionResult)
          : CreateContract(recognitionResult, responsible);
      }
      
      // Доп.соглашение.
      else if (predictedClass == ArioClassNames.SupAgreement && !isMockMode)
      {
        document = CreateSupAgreement(recognitionResult, responsible);
      }
      
      // Все нераспознанные документы создать простыми.
      else
      {
        var name = !string.IsNullOrWhiteSpace(recognitionResult.File.Description) ? recognitionResult.File.Description : Resources.SimpleDocumentName;
        document = CreateSimpleDocument(name, responsible);
      }
      
      FillDeliveryMethod(document, recognitionResult.SendedByEmail);
      /* Статус документа задается до создания версии, чтобы корректно прописалось наименование,
         если его не из чего формировать.*/
      document.VerificationState = Docflow.OfficialDocument.VerificationState.InProcess;
      CreateVersion(document, recognitionResult);
      // Принудительно заполняем имя документа, для случаев когда имя не автоформируемое, чтобы не падало при сохранении.
      Docflow.PublicFunctions.OfficialDocument.FillName(document);
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Получить приоритеты типов документов для определения ведущего документа в комплекте.
    /// </summary>
    /// <returns>Словарь с приоритетами типов.</returns>
    public virtual System.Collections.Generic.IDictionary<System.Type, int> GetPackageDocumentTypePriorities()
    {
      var leadingDocumentPriority = new Dictionary<System.Type, int>();
      
      if (Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null)
      {
        leadingDocumentPriority.Add(MockIncomingLetters.Info.GetType().GetFinalType(), 6);
        leadingDocumentPriority.Add(MockContracts.Info.GetType().GetFinalType(), 5);
        leadingDocumentPriority.Add(MockContractStatements.Info.GetType().GetFinalType(), 4);
        leadingDocumentPriority.Add(MockWaybills.Info.GetType().GetFinalType(), 3);
        leadingDocumentPriority.Add(MockIncomingTaxInvoices.Info.GetType().GetFinalType(), 2);
        leadingDocumentPriority.Add(MockIncomingInvoices.Info.GetType().GetFinalType(), 1);
        leadingDocumentPriority.Add(Sungero.SmartCapture.SimpleDocuments.Info.GetType().GetFinalType(), 0);
      }
      else
      {
        leadingDocumentPriority.Add(Sungero.SmartCapture.IncomingLetters.Info.GetType().GetFinalType(), 7);
        leadingDocumentPriority.Add(Sungero.SmartCapture.Contracts.Info.GetType().GetFinalType(), 6);
        leadingDocumentPriority.Add(Sungero.SmartCapture.SupAgreements.Info.GetType().GetFinalType(), 5);
        leadingDocumentPriority.Add(Sungero.SmartCapture.ContractStatements.Info.GetType().GetFinalType(), 4);
        leadingDocumentPriority.Add(Sungero.SmartCapture.Waybills.Info.GetType().GetFinalType(), 3);
        leadingDocumentPriority.Add(Sungero.SmartCapture.IncomingTaxInvoices.Info.GetType().GetFinalType(), 2);
        leadingDocumentPriority.Add(Sungero.SmartCapture.IncomingInvoices.Info.GetType().GetFinalType(), 1);
        leadingDocumentPriority.Add(Sungero.SmartCapture.SimpleDocuments.Info.GetType().GetFinalType(), 0);
      }
      return leadingDocumentPriority;
    }
    
    /// <summary>
    /// Определить ведущий документ распознанного комплекта.
    /// </summary>
    /// <param name="package">Комплект документов.</param>
    /// <returns>Ведущий документ.</returns>
    public virtual IOfficialDocument GetLeadingDocument(List<IOfficialDocument> package)
    {
      var packagePriority = new Dictionary<IOfficialDocument, int>();
      var leadingDocumentPriority = GetPackageDocumentTypePriorities();
      int priority;
      foreach (var document in package)
      {
        leadingDocumentPriority.TryGetValue(document.Info.GetType().GetFinalType(), out priority);
        packagePriority.Add(document, priority);
      }
      
      var leadingDocument = packagePriority.OrderByDescending(p => p.Value).FirstOrDefault().Key;
      return leadingDocument;
    }
    
    /// <summary>
    /// Отправить задачу на проверку документов.
    /// </summary>
    /// <param name="leadingDocument">Ведущий документ.</param>
    /// <param name="documents">Прочие документы из комплекта.</param>
    /// <param name="documentsWithRegistrationFailure">Документы, которые не удалось зарегистрировать.</param>
    /// <param name="documentsFoundByBarcode">Документы, найденные по штрихкоду.</param>
    /// <param name="lockedDocuments">Документы, которые были заблокированы.</param>
    /// <param name="emailBody">Тело электронного письма.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Простая задача.</returns>
    [Public, Remote]
    public virtual void SendToResponsible(IOfficialDocument leadingDocument, List<IOfficialDocument> documents,
                                          List<IOfficialDocument> documentsWithRegistrationFailure,
                                          List<IOfficialDocument> documentsFoundByBarcode,
                                          List<IOfficialDocument> lockedDocuments,
                                          Docflow.IOfficialDocument emailBody, Company.IEmployee responsible)
    {
      if (leadingDocument == null)
        return;
      
      // Собрать пакет документов. Порядок важен, чтобы ведущий был первым.
      var package = new List<IOfficialDocument>();
      package.Add(leadingDocument);
      package.AddRange(documents);
      
      // Тема.
      var task = SimpleTasks.Create();
      task.Subject = package.Count() > 1
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
      
      // Собрать ссылки на документы, которые найдены по штрихкоду.
      var documentsFoundByBarcodeHyperlinks = new List<string>();
      documentsFoundByBarcodeHyperlinks.AddRange(documentsFoundByBarcode.Select(x => Hyperlinks.Get(x)));

      // Текст задачи.
      task.ActiveText = Resources.CheckPackageTaskText;
      
      // Добавить в текст задачи список документов, которые найдены по штрихкоду.
      if (documentsFoundByBarcode.Any())
      {
        var documentsFoundBarcodeTaskText = Sungero.Capture.Resources.DocumentsFoundByBarcodeTaskText;
        
        var documentsFoundByBarcodeHyperlinksLabel = string.Join("\n    ", documentsFoundByBarcodeHyperlinks);
        
        task.ActiveText = string.Format("{0}\n\n{1}\n    {2}", task.ActiveText, documentsFoundBarcodeTaskText,
                                        documentsFoundByBarcodeHyperlinksLabel);
      }
      
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
      
      // Добавить в текст задачи список документов, которые были заблокированы при занесении новой версии.
      if (lockedDocuments.Any())
      {
        var failedCreateVersionTaskText = lockedDocuments.Count() == 1
          ? Sungero.Capture.Resources.FailedCreateVersionTaskText
          : Sungero.Capture.Resources.FailedCreateVersionsTaskText;

        var lockedDocumentsHyperlinksLabels = new List<string>();
        foreach (var lockedDocument in lockedDocuments)
        {
          var loginId = Locks.GetLockInfo(lockedDocument).LoginId;
          var employee = Employees.GetAll(x => x.Login.Id == loginId).FirstOrDefault();
          // Текстовка на случай, когда блокировка снята в момент создания задачи.
          var employeeLabel = Sungero.Capture.Resources.DocumentWasLockedTaskText.ToString();
          if (employee != null)
            employeeLabel = string.Format(Sungero.Capture.Resources.DocumentLockedByEmployeeTaskText,
                                          Hyperlinks.Get(employee));
          var documentHyperlink = Hyperlinks.Get(lockedDocument);
          lockedDocumentsHyperlinksLabels.Add(string.Format("{0} {1}", documentHyperlink, employeeLabel));
        }
        
        var lockedDocumentsHyperlinksLabel = string.Join("\n    ", lockedDocumentsHyperlinksLabels);
        task.ActiveText = string.Format("{0}\n\n{1}\n    {2}", task.ActiveText, failedCreateVersionTaskText, lockedDocumentsHyperlinksLabel);
      }
      
      // Маршрут.
      var step = task.RouteSteps.AddNew();
      step.AssignmentType = Workflow.SimpleTask.AssignmentType.Assignment;
      step.Performer = responsible;
      
      // Добавить наблюдателями ответственных за документы, которые вернулись по ШК.
      var responsibleEmployees = GetDocumentsResponsibleEmployees(documentsFoundByBarcode);
      responsibleEmployees = responsibleEmployees.Where(r => !Equals(r, responsible)).ToList();
      foreach (var responsibleEmployee in responsibleEmployees)
      {
        var observer = task.Observers.AddNew();
        observer.Observer = responsibleEmployee;
      }
      
      task.NeedsReview = false;
      task.Deadline = Calendar.Now.AddWorkingHours(4);
      task.Save();
      task.Start();
      
      // Старт фонового процесса на смену статуса верификации.
      Jobs.ChangeVerificationState.Enqueue();
    }
    
    /// <summary>
    /// Получить список ответственных за документы.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <returns>Список ответственных.</returns>
    /// <remarks>Ответственных искать только у документов, тип которых: договорной документ, акт, накладная, УПД.</remarks>
    public virtual List<IEmployee> GetDocumentsResponsibleEmployees(List<IOfficialDocument> documents)
    {
      var responsibleEmployees = new List<IEmployee>();
      var withResponsibleDocuments = documents.Where(d => Contracts.ContractualDocuments.Is(d) ||
                                                     FinancialArchive.ContractStatements.Is(d) ||
                                                     FinancialArchive.Waybills.Is(d) ||
                                                     FinancialArchive.UniversalTransferDocuments.Is(d));
      foreach (var document in withResponsibleDocuments)
      {
        var responsibleEmployee = Employees.Null;
        responsibleEmployee = Sungero.Docflow.PublicFunctions.OfficialDocument.GetDocumentResponsibleEmployee(document);
        
        if (responsibleEmployee != Employees.Null && responsibleEmployee.IsSystem != true)
          responsibleEmployees.Add(responsibleEmployee);
      }
      
      return responsibleEmployees.Distinct().ToList();
    }
    
    /// <summary>
    /// Отправить документы ответственному.
    /// </summary>
    /// <param name="documentsCreatedByRecognition">Результат создания документов.</param>
    /// <param name="emailBody">Тело электронного письма.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    [Remote]
    public virtual void SendToResponsible(Structures.Module.IDocumentsCreatedByRecognitionResults documentsCreatedByRecognition,
                                          Docflow.IOfficialDocument emailBody, Sungero.Company.IEmployee responsible)
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
      
      var documentsFoundByBarcode = documentsCreatedByRecognition.DocumentFoundByBarcodeIds != null
        ? allDocuments.Where(x => documentsCreatedByRecognition.DocumentFoundByBarcodeIds.Contains(x.Id)).ToList()
        : new List<Docflow.IOfficialDocument>();
      
      var lockedDocuments = documentsCreatedByRecognition.LockedDocumentIds != null
        ? allDocuments.Where(x => documentsCreatedByRecognition.LockedDocumentIds.Contains(x.Id)).ToList()
        : new List<Docflow.IOfficialDocument>();
      
      SendToResponsible(leadingDocument, relatedDocuments, documentsWithRegistrationFailure, documentsFoundByBarcode, lockedDocuments, emailBody, responsible);
    }
    
    #endregion
    
    #region Простой документ
    
    /// <summary>
    /// Создать простой документ.
    /// </summary>
    /// <param name="name">Наименование документа.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns></returns>
    [Public]
    public static ISimpleDocument CreateSimpleDocument(string name, Company.IEmployee responsible)
    {
      var document = Docflow.PublicFunctions.SimpleDocument.CreateSimpleDocument(name, responsible);
      document.VerificationState = Docflow.SimpleDocument.VerificationState.InProcess;
      return document;
    }
    
    /// <summary>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// Создать документ из тела эл. письма.
    /// </summary>
    /// <param name="mailInfo">Информация о захваченном письме.</param>
    /// <param name="bodyDto">Тело письма.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// <returns>ИД созданного документа.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromEmailBody(Structures.Module.ICapturedMailInfo mailInfo,
                                                                                     Sungero.Docflow.Structures.Module.IFileDto bodyDto,
                                                                                     IEmployee responsible)
    {
      if (!System.IO.File.Exists(bodyDto.Path))
        throw new ApplicationException(Resources.FileNotFoundFormat(bodyDto.Path));
      
      var documentName = Resources.EmailBodyDocumentNameFormat(mailInfo.FromEmail);
      var document = CreateSimpleDocument(documentName, responsible);
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
      
      using (var body = new MemoryStream(bodyDto.Data))
      {
        document.CreateVersion();
        var version = document.LastVersion;
        if (Path.GetExtension(bodyDto.Path).ToLower() == Constants.Module.HtmlExtension.WithDot)
        {
          var pdfConverter = new AsposeExtensions.Converter();
          using (var pdfDocumentStream = pdfConverter.GeneratePdf(body, Constants.Module.HtmlExtension.WithoutDot))
          {
            if (pdfDocumentStream != null)
            {
              version.Body.Write(pdfDocumentStream);
              version.AssociatedApplication = Content.AssociatedApplications.GetByExtension(Constants.Module.PdfExtension);
            }
          }
        }
        
        // Если тело письма не удалось преобразовать в pdf или расширение не html, то в тело пишем исходный файл.
        if (version.Body.Size == 0)
        {
          version.Body.Write(body);
          version.AssociatedApplication = GetAssociatedApplicationByFileName(bodyDto.Path);
        }
      }
      document.Save();
      return document;
    }
    
    /// <summary>
    /// Создать простой документ из файла.
    /// </summary>
    /// <param name="file">Файл.</param>
    /// <param name="sendedByEmail">Доставлен эл.почтой.</param>
    /// <param name="responsible">Сотрудник, ответственный за обработку документов.</param>
    /// <returns>Простой документ.</returns>
    [Remote]
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromFile(Sungero.Docflow.Structures.Module.IFileDto file,
                                                                                bool sendedByEmail,
                                                                                IEmployee responsible)
    {
      var name = Path.GetFileName(file.Description);
      var document = CreateSimpleDocument(name, responsible);
      FillDeliveryMethod(document, sendedByEmail);
      document.Save();
      
      var application = GetAssociatedApplicationByFileName(file.Path);
      using (var body = new MemoryStream(file.Data))
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
    /// <param name="recognitionResult">Результат обработки письма в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Документ.</returns>
    public virtual Docflow.IOfficialDocument CreateIncomingLetter(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
    {
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      var subjectFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Subject).FirstOrDefault();
      var subject = DocflowPublicFunctions.GetFieldValue(subjectFact, FieldNames.Letter.Subject);
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, subjectFact, FieldNames.Letter.Subject, props.Subject.Name, document.Subject);
      }
      
      // Адресат.
      var addresseeFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Addressee).FirstOrDefault();
      var addressee = GetAdresseeByFact(addresseeFact);
      document.Addressee = addressee.Employee;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, addresseeFact, FieldNames.Letter.Addressee, props.Addressee.Name, document.Addressee, addressee.IsTrusted);
      
      // Заполнить данные корреспондента.
      var correspondent = GetCounterparty(recognitionResult, props.Correspondent.Name);
      if (correspondent != null)
      {
        document.Correspondent = correspondent.Counterparty;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, correspondent.Fact, null, props.Correspondent.Name, document.Correspondent, correspondent.IsTrusted);
      }
      
      // Дата, номер.
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Date).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(dateFact, FieldNames.Document.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && DocflowPublicFunctions.IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Dated = date;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Letter.Date, props.Dated.Name, date, isTrustedDate);
      
      var numberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Number).FirstOrDefault();
      document.InNumber = DocflowPublicFunctions.GetFieldValue(numberFact, FieldNames.Letter.Number);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, numberFact, FieldNames.Letter.Number, props.InNumber.Name, document.InNumber);
      
      // Заполнить данные нашей стороны.
      // Убираем уже использованный факт для подбора контрагента, чтобы организация не искалась по тем же реквизитам что и контрагент.
      if (correspondent != null)
        facts.Remove(correspondent.Fact);
      var businessUnitsWithFacts = GetBusinessUnitsWithFacts(recognitionResult);
      
      var businessUnitWithFact = GetMostProbableBusinessUnitMatching(businessUnitsWithFacts,
                                                                     props.BusinessUnit.Name,
                                                                     document.Addressee,
                                                                     responsible);
      document.BusinessUnit = businessUnitWithFact.BusinessUnit;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, businessUnitWithFact.Fact, null, props.BusinessUnit.Name,
                                                 document.BusinessUnit, businessUnitWithFact.IsTrusted);
      
      document.Department = document.Addressee != null
        ? Company.PublicFunctions.Department.GetDepartment(document.Addressee)
        : Company.PublicFunctions.Department.GetDepartment(responsible);
      
      // Заполнить подписанта.
      var personFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.LetterPerson, FieldNames.LetterPerson.Surname);
      var signatoryFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Signatory).FirstOrDefault();
      var signedBy = GetContactByFact(signatoryFact, props.SignedBy.Name,
                                      document.Correspondent, props.Correspondent.Name, recognitionResult.PredictedClass);
      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && signedBy.Contact != null)
      {
        // Если контрагент определился из подписанта, то результату не доверяем.
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, null, null, props.Correspondent.Name, signedBy.Contact.Company, false);
      }
      document.SignedBy = signedBy.Contact;
      var isTrustedSignatory = signedBy.IsTrusted ? DocflowPublicFunctions.IsTrustedField(signatoryFact, FieldNames.LetterPerson.Type) : signedBy.IsTrusted;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, signatoryFact, null, props.SignedBy.Name, document.SignedBy, isTrustedSignatory);
      
      // Заполнить контакт.
      var responsibleFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Responsible).FirstOrDefault();
      var contact = GetContactByFact(responsibleFact, props.Contact.Name,
                                     document.Correspondent, props.Correspondent.Name, recognitionResult.PredictedClass);
      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && contact.Contact != null)
      {
        // Если контрагент определился из контакта, то результату не доверяем.
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, null, null, props.Correspondent.Name, contact.Contact.Company, false);
      }
      document.Contact = contact.Contact;
      var isTrustedContact = contact.IsTrusted && DocflowPublicFunctions.IsTrustedField(responsibleFact, FieldNames.LetterPerson.Type);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, responsibleFact, null, props.Contact.Name, document.Contact, isTrustedContact);
      
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingLetter(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockIncomingLetters.Create();
      var props = document.Info.Properties;
      var facts = recognitionResult.Facts;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      
      // Заполнить дату и номер письма со стороны корреспондента.
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Date).FirstOrDefault();
      var numberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Number).FirstOrDefault();
      document.InNumber = DocflowPublicFunctions.GetFieldValue(numberFact, FieldNames.Letter.Number);
      document.Dated = Functions.Module.GetShortDate(DocflowPublicFunctions.GetFieldValue(dateFact, FieldNames.Letter.Date));
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Letter.Date, props.Dated.Name, document.Dated);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, numberFact, FieldNames.Letter.Number, props.InNumber.Name, document.InNumber);
      
      // Заполнить данные корреспондента.
      var correspondentNameFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.CorrespondentName);
      if (correspondentNameFacts.Count() > 0)
      {
        var fact = correspondentNameFacts.First();
        document.Correspondent = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Letter.CorrespondentName, props.Correspondent.Name, document.Correspondent);
      }
      if (correspondentNameFacts.Count() > 1)
      {
        var fact = correspondentNameFacts.Last();
        document.Recipient = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Letter.CorrespondentName, props.Recipient.Name, document.Recipient);
      }
      
      // Заполнить ИНН/КПП для КА и НОР.
      var tinTrrcFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.CorrespondentTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.CorrespondentTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.CorrespondentTin.Name, document.CorrespondentTin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.CorrespondentTrrc.Name, document.CorrespondentTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.RecipientTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.RecipientTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.RecipientTin.Name, document.RecipientTin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.RecipientTrrc.Name, document.RecipientTrrc);
      }
      
      // В ответ на.
      var responseToNumberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToNumber).FirstOrDefault();
      var responseToNumber = DocflowPublicFunctions.GetFieldValue(responseToNumberFact, FieldNames.Letter.ResponseToNumber);
      var responseToDateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate).FirstOrDefault();
      var responseToDate = Functions.Module.GetShortDate(DocflowPublicFunctions.GetFieldValue(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? responseToNumber
        : string.Format("{0} {1} {2}", responseToNumber, Sungero.Docflow.Resources.From, responseToDate);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, responseToNumberFact, FieldNames.Letter.ResponseToNumber, props.InResponseTo.Name, document.InResponseTo);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, responseToDateFact, FieldNames.Letter.ResponseToDate, props.InResponseTo.Name, document.InResponseTo);
      
      // Заполнить подписанта.
      var personFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.LetterPerson, FieldNames.LetterPerson.Surname);
      var predictedClass = recognitionResult.PredictedClass;
      if (document.Signatory == null)
      {
        var signatoryFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Signatory).FirstOrDefault();
        document.Signatory = Docflow.Server.ModuleFunctions.GetFullNameByFact(predictedClass, signatoryFact);
        var isTrusted = DocflowPublicFunctions.IsTrustedField(signatoryFact, FieldNames.LetterPerson.Type);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, signatoryFact, null, props.Signatory.Name, document.Signatory, isTrusted);
      }
      
      // Заполнить контакт.
      if (document.Contact == null)
      {
        var responsibleFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Responsible).FirstOrDefault();
        document.Contact = Docflow.Server.ModuleFunctions.GetFullNameByFact(predictedClass, responsibleFact);
        var isTrusted = DocflowPublicFunctions.IsTrustedField(responsibleFact, FieldNames.LetterPerson.Type);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, responsibleFact, null, props.Contact.Name, document.Contact, isTrusted);
      }
      
      // Заполнить данные нашей стороны.
      var addresseeFacts = DocflowPublicFunctions.GetFacts(facts, FactNames.Letter, FieldNames.Letter.Addressee);
      foreach (var fact in addresseeFacts)
      {
        var addressee = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Letter.Addressee);
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? addressee : string.Format("{0}; {1}", document.Addressees, addressee);
      }
      foreach (var fact in addresseeFacts)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, null, props.Addressees.Name, document.Addressees, true);
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subjectFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Subject).FirstOrDefault();
      var subject = DocflowPublicFunctions.GetFieldValue(subjectFact, FieldNames.Letter.Subject);
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, subjectFact, FieldNames.Letter.Subject, props.Subject.Name, document.Subject);
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
    public Docflow.IOfficialDocument CreateMockContractStatement(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.LeadDoc = GetLeadingDocumentName(leadingDocFact);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadDoc.Name, document.LeadDoc, true);
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.Document);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.CounterpartyName = seller.Name;
        document.CounterpartyTin = seller.Tin;
        document.CounterpartyTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, seller.Trrc);
      }
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BusinessUnitName = buyer.Name;
        document.BusinessUnitTin = buyer.Tin;
        document.BusinessUnitTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, buyer.Trrc);
      }
      
      // В актах могут прийти контрагенты без типа. Заполнить контрагентами без типа.
      if (seller == null || buyer == null)
      {
        var withoutTypeFacts = DocflowPublicFunctions.GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
          .Where(f => string.IsNullOrWhiteSpace(DocflowPublicFunctions.GetFieldValue(f, FieldNames.Counterparty.CounterpartyType)))
          .OrderByDescending(x => x.Fields.First(f => f.Name == FieldNames.Counterparty.Name).Probability);
        foreach (var fact in withoutTypeFacts)
        {
          var name = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
          
          var tin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
          var trrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
          var type = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType);
          
          if (string.IsNullOrWhiteSpace(document.CounterpartyName))
          {
            document.CounterpartyName = name;
            document.CounterpartyTin = tin;
            document.CounterpartyTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BusinessUnitName))
          {
            document.BusinessUnitName = name;
            document.BusinessUnitTin = tin;
            document.BusinessUnitTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, trrc);
          }
        }
      }
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in DocflowPublicFunctions.GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.UnitName);
        good.Count = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      return document;
    }
    
    /// <summary>
    /// Создать акт выполненных работ.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Акт выполненных работ.</returns>
    public virtual Docflow.IOfficialDocument CreateContractStatement(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
    {
      var facts = recognitionResult.Facts;
      var document = FinancialArchive.ContractStatements.Create();
      var props = AccountingDocumentBases.Info.Properties;
      FillDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Seller);
      counterpartyTypes.Add(CounterpartyTypes.Buyer);
      counterpartyTypes.Add(string.Empty);
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Seller).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Buyer).FirstOrDefault();
      var nonType = factMatches.Where(m => m.Type == string.Empty).ToList();
      var documentParties = GetDocumentParties(buyer, seller, nonType, responsible);
      DocflowPublicFunctions.FillAccountingDocumentParties(document, documentParties);
      DocflowPublicFunctions.LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Дата, номер и регистрация.
      DocflowPublicFunctions.NumberDocument(document, recognitionResult, FactNames.Document, null);
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var leadingDocument = GetLeadingDocument(leadingDocFact,
                                               document.Info.Properties.LeadingDocument.Name,
                                               document.Counterparty, document.Info.Properties.Counterparty.Name);
      document.LeadingDocument = leadingDocument.Contract;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, leadingDocument.IsTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      DocflowPublicFunctions.FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки накладной в Ario.</param>
    /// <returns>Накладная.</returns>
    public Docflow.IOfficialDocument CreateMockWaybill(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockWaybills.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = DocflowPublicFunctions.IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var shipper = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Shipper);
      if (shipper != null)
      {
        document.Shipper = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.Name, props.Shipper.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.LegalForm, props.Shipper.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.Consignee = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.Name, props.Consignee.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.LegalForm, props.Consignee.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var supplier = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Supplier);
      if (supplier != null)
      {
        document.Supplier = supplier.Name;
        document.SupplierTin = supplier.Tin;
        document.SupplierTrrc = supplier.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.Name, props.Supplier.Name, supplier.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.LegalForm, props.Supplier.Name, supplier.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.TIN, props.SupplierTin.Name, supplier.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.TRRC, props.SupplierTrrc.Name, supplier.Trrc);
      }
      
      var payer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Payer);
      if (payer != null)
      {
        document.Payer = payer.Name;
        document.PayerTin = payer.Tin;
        document.PayerTrrc = payer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.Name, props.Payer.Name, payer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.LegalForm, props.Payer.Name, payer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.TIN, props.PayerTin.Name, payer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.TRRC, props.PayerTrrc.Name, payer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.FinancialDocument);
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in DocflowPublicFunctions.GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.UnitName);
        
        good.Count = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      
      return document;
    }
    
    /// <summary>
    /// Создать накладную.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки накладной в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Накладная.</returns>
    public virtual Docflow.IOfficialDocument CreateWaybill(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
    {
      var facts = recognitionResult.Facts;
      var document = FinancialArchive.Waybills.Create();
      var props = document.Info.Properties;
      FillDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Supplier);
      counterpartyTypes.Add(CounterpartyTypes.Payer);
      counterpartyTypes.Add(CounterpartyTypes.Shipper);
      counterpartyTypes.Add(CounterpartyTypes.Consignee);
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Supplier).FirstOrDefault() ??
        factMatches.Where(m => m.Type == CounterpartyTypes.Shipper).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Payer).FirstOrDefault() ??
        factMatches.Where(m => m.Type == CounterpartyTypes.Consignee).FirstOrDefault();
      var documentParties = GetDocumentParties(buyer, seller, responsible);
      
      DocflowPublicFunctions.FillAccountingDocumentParties(document, documentParties);
      DocflowPublicFunctions.LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Дата, номер и регистрация.
      DocflowPublicFunctions.NumberDocument(document, recognitionResult, FactNames.FinancialDocument, null);
      
      // Документ-основание.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contractualDocuments = GetLeadingDocuments(leadingDocFact, document.Counterparty);
      document.LeadingDocument = contractualDocuments.FirstOrDefault();
      var isTrusted = (contractualDocuments.Count() == 1) ? DocflowPublicFunctions.IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName) : false;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      DocflowPublicFunctions.FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Счет-фактура.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var facts = recognitionResult.Facts;
      var document = Sungero.Capture.MockIncomingTaxInvoices.Create();
      var props = document.Info.Properties;
      FillDocumentKind(document);
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var shipper = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Shipper);
      if (shipper != null)
      {
        document.ShipperName = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.Name, props.ShipperName.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.LegalForm, props.ShipperName.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.ConsigneeName = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.Name, props.ConsigneeName.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.LegalForm, props.ConsigneeName.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.FinancialDocument);
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in DocflowPublicFunctions.GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Goods.UnitName);
        good.Count = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      
      return document;
    }
    
    /// <summary>
    /// Создать счет-фактуру.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Арио.</param>
    /// <param name="isAdjustment">Корректировочная.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Счет-фактура.</returns>
    public virtual Docflow.IOfficialDocument CreateTaxInvoice(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, bool isAdjustment, IEmployee responsible)
    {
      var facts = recognitionResult.Facts;
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
      var responsibleEmployeePersonalSettings = Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(responsible);
      var responsibleEmployeePersonalSettingsBusinessUnit = responsibleEmployeePersonalSettings != null
        ? responsibleEmployeePersonalSettings.BusinessUnit
        : Company.BusinessUnits.Null;
      var document = AccountingDocumentBases.Null;
      var props = AccountingDocumentBases.Info.Properties;
      
      // Определить направление документа, НОР и КА.
      // Если НОР выступает продавцом, то создаем исходящую счет-фактуру, иначе - входящую.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Seller);
      counterpartyTypes.Add(CounterpartyTypes.Buyer);
      counterpartyTypes.Add(CounterpartyTypes.Shipper);
      counterpartyTypes.Add(CounterpartyTypes.Consignee);
      
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Seller).FirstOrDefault() ?? factMatches.Where(m => m.Type == CounterpartyTypes.Shipper).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Buyer).FirstOrDefault() ?? factMatches.Where(m => m.Type == CounterpartyTypes.Consignee).FirstOrDefault();
      
      var buyerIsBusinessUnit = buyer != null && buyer.BusinessUnit != null;
      var sellerIsBusinessUnit = seller != null && seller.BusinessUnit != null;
      var documentParties = Sungero.Docflow.Structures.Module.DocumentParties.Create();
      if (buyerIsBusinessUnit && sellerIsBusinessUnit)
      {
        // Мультинорность. Уточнить НОР по ответственному.
        if (Equals(seller.BusinessUnit, responsibleEmployeePersonalSettingsBusinessUnit) ||
            Equals(seller.BusinessUnit, responsibleEmployeeBusinessUnit))
        {
          // Исходящий документ.
          document = FinancialArchive.OutgoingTaxInvoices.Create();
          documentParties.Counterparty = buyer;
          documentParties.BusinessUnit = seller;
        }
        else
        {
          // Входящий документ.
          document = FinancialArchive.IncomingTaxInvoices.Create();
          documentParties.Counterparty = seller;
          documentParties.BusinessUnit = buyer;
        }
      }
      else if (buyerIsBusinessUnit)
      {
        // Входящий документ.
        document = FinancialArchive.IncomingTaxInvoices.Create();
        documentParties.Counterparty = seller;
        documentParties.BusinessUnit = buyer;
      }
      else if (sellerIsBusinessUnit)
      {
        // Исходящий документ.
        document = FinancialArchive.OutgoingTaxInvoices.Create();
        documentParties.Counterparty = buyer;
        documentParties.BusinessUnit = seller;
      }
      else
      {
        // НОР не найдена по фактам - НОР будет взята по ответственному.
        if (buyer != null && buyer.Counterparty != null && (seller == null || seller.Counterparty == null))
        {
          // Исходящий документ, потому что buyer - контрагент, а другой информации нет.
          document = FinancialArchive.OutgoingTaxInvoices.Create();
          documentParties.Counterparty = buyer;
        }
        else
        {
          // Входящий документ.
          document = FinancialArchive.IncomingTaxInvoices.Create();
          documentParties.Counterparty = seller;
        }
      }

      documentParties.ResponsibleEmployeeBusinessUnit = Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      
      if (FinancialArchive.IncomingTaxInvoices.Is(document))
      {
        Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, documentParties);
      }
      else
      {
        // НОР и КА.
        DocflowPublicFunctions.FillAccountingDocumentParties(document, documentParties);
        DocflowPublicFunctions.LinkAccountingDocumentParties(recognitionResult, documentParties);
        
        // Вид документа.
        FillDocumentKind(document);
        
        // Дата, номер и регистрация.
        DocflowPublicFunctions.NumberDocument(document, recognitionResult, FactNames.FinancialDocument, null);
        
        // Корректировочный документ.
        if (isAdjustment)
        {
          document.IsAdjustment = true;
          var correctionDateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.CorrectionDate).FirstOrDefault();
          var correctionNumberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.CorrectionNumber).FirstOrDefault();
          var correctionDate = DocflowPublicFunctions.GetFieldDateTimeValue(correctionDateFact, FieldNames.FinancialDocument.CorrectionDate);
          var correctionNumber = DocflowPublicFunctions.GetFieldValue(correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber);
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
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, correctionDateFact, FieldNames.FinancialDocument.CorrectionDate, props.Corrected.Name, document.Corrected, isTrusted);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber, props.Corrected.Name, document.Corrected, isTrusted);
          }
        }
        
        // Подразделение и ответственный.
        document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
        document.ResponsibleEmployee = responsible;
        
        // Сумма и валюта.
        DocflowPublicFunctions.FillAmountAndCurrency(document, recognitionResult);
      }
      return document;
    }
    
    #endregion
    
    #region УПД
    
    /// <summary>
    /// Создать УПД.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки УПД в Ario.</param>
    /// <param name="isAdjustment">Корректировочная.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>УПД.</returns>
    public virtual Docflow.IOfficialDocument CreateUniversalTransferDocument(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, bool isAdjustment, IEmployee responsible)
    {
      var facts = recognitionResult.Facts;
      var document = Sungero.FinancialArchive.UniversalTransferDocuments.Create();
      var props = document.Info.Properties;
      FillDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Seller);
      counterpartyTypes.Add(CounterpartyTypes.Buyer);
      counterpartyTypes.Add(CounterpartyTypes.Shipper);
      counterpartyTypes.Add(CounterpartyTypes.Consignee);

      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Seller).FirstOrDefault() ?? factMatches.Where(m => m.Type == CounterpartyTypes.Shipper).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Buyer).FirstOrDefault() ?? factMatches.Where(m => m.Type == CounterpartyTypes.Consignee).FirstOrDefault();
      var documentParties = GetDocumentParties(buyer, seller, responsible);
      DocflowPublicFunctions.FillAccountingDocumentParties(document, documentParties);
      DocflowPublicFunctions.LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата, номер и регистрация.
      DocflowPublicFunctions.NumberDocument(document, recognitionResult, FactNames.FinancialDocument, null);
      
      // Корректировочный документ.
      FillCorrectedDocument(document, recognitionResult, isAdjustment);
      
      // Сумма и валюта.
      DocflowPublicFunctions.FillAmountAndCurrency(document, recognitionResult);

      return document;
    }
    
    #endregion
    
    #region Счет на оплату
    
    /// <summary>
    /// Создать счет на оплату (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки счета на оплату в Ario.</param>
    /// <returns>Счет на оплату.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingInvoice(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockIncomingInvoices.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = DocflowPublicFunctions.IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Могут прийти контрагенты без типа. Заполнить контрагентами без типа.
      if (seller == null || buyer == null)
      {
        var withoutTypeFacts = DocflowPublicFunctions.GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
          .Where(f => string.IsNullOrWhiteSpace(DocflowPublicFunctions.GetFieldValue(f, FieldNames.Counterparty.CounterpartyType)))
          .OrderByDescending(x => x.Fields.First(f => f.Name == FieldNames.Counterparty.Name).Probability);
        foreach (var fact in withoutTypeFacts)
        {
          var name = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
          
          var tin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
          var trrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
          var type = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType);
          
          if (string.IsNullOrWhiteSpace(document.SellerName))
          {
            document.SellerName = name;
            document.SellerTin = tin;
            document.SellerTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.SellerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BuyerName))
          {
            document.BuyerName = name;
            document.BuyerTin = tin;
            document.BuyerTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.BuyerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, trrc);
          }
        }
      }
      
      // Дата и номер.
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(dateFact, FieldNames.FinancialDocument.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && DocflowPublicFunctions.IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Date = date;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.FinancialDocument.Date, props.Date.Name, date, isTrustedDate);
      
      var numberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Number).FirstOrDefault();
      document.Number = DocflowPublicFunctions.GetFieldValue(numberFact, FieldNames.FinancialDocument.Number);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, numberFact, FieldNames.FinancialDocument.Number, props.Number.Name, document.Number);
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    
    /// <summary>
    /// Создать счет на оплату.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Счет на оплату.</returns>
    public virtual Docflow.IOfficialDocument CreateIncomingInvoice(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
    {
      var facts = recognitionResult.Facts;
      var document = Contracts.IncomingInvoices.Create();
      var props = document.Info.Properties;
      FillDocumentKind(document);
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Seller);
      counterpartyTypes.Add(CounterpartyTypes.Buyer);
      counterpartyTypes.Add(string.Empty);
      var factMatches = MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Seller).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Buyer).FirstOrDefault();
      var nonType = factMatches.Where(m => m.Type == string.Empty).ToList();
      var documentParties = GetDocumentParties(buyer, seller, nonType, responsible);
      DocflowPublicFunctions.FillAccountingDocumentParties(document, documentParties);
      DocflowPublicFunctions.LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Договор.
      var contractFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contract = GetLeadingDocument(contractFact, document.Info.Properties.Contract.Name, document.Counterparty, document.Info.Properties.Counterparty.Name);
      document.Contract = contract.Contract;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, contractFact, null, props.Contract.Name, document.Contract, contract.IsTrusted);
      
      // Дата.
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(dateFact, FieldNames.FinancialDocument.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && DocflowPublicFunctions.IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Date = date;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.FinancialDocument.Date, props.Date.Name, date, isTrustedDate);
      
      // Номер.
      var numberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Number).FirstOrDefault();
      var number = DocflowPublicFunctions.GetFieldValue(numberFact, FieldNames.FinancialDocument.Number);
      Nullable<bool> isTrustedNumber = null;
      if (number.Length > document.Info.Properties.Number.Length)
      {
        number = number.Substring(0, document.Info.Properties.Number.Length);
        isTrustedNumber = false;
      }
      document.Number = number;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, numberFact, FieldNames.FinancialDocument.Number, props.Number.Name, document.Number, isTrustedNumber);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      DocflowPublicFunctions.FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Договор
    
    /// <summary>
    /// Создать договор (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <returns>Договор.</returns>
    public Docflow.IOfficialDocument CreateMockContract(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockContracts.Create();
      
      // Основные свойства.
      FillDocumentKind(document);
      document.Name = document.DocumentKind.ShortName;
      var props = document.Info.Properties;
      var facts = recognitionResult.Facts;
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.Document);
      
      // Заполнить данные сторон.
      var partyNameFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name);
      if (partyNameFacts.Count() > 0)
      {
        var fact = partyNameFacts.First();
        document.FirstPartyName = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.FirstPartySignatory = Docflow.Server.ModuleFunctions.GetFullNameByFactForContract(fact);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.FirstPartyName.Name, document.FirstPartyName);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatorySurname, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryName, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryPatrn, props.FirstPartySignatory.Name, document.FirstPartySignatory);
      }
      if (partyNameFacts.Count() > 1)
      {
        var fact = partyNameFacts.Last();
        document.SecondPartyName = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.SecondPartySignatory = Docflow.Server.ModuleFunctions.GetFullNameByFactForContract(fact);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.SecondPartyName.Name, document.SecondPartyName);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatorySurname, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryName, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryPatrn, props.SecondPartySignatory.Name, document.SecondPartySignatory);
      }
      
      // Заполнить ИНН/КПП сторон.
      var tinTrrcFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.FirstPartyTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.FirstPartyTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.FirstPartyTin.Name, document.FirstPartyTin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.FirstPartyTrrc.Name, document.FirstPartyTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.SecondPartyTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.SecondPartyTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.SecondPartyTin.Name, document.SecondPartyTin);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.SecondPartyTrrc.Name, document.SecondPartyTrrc);
      }
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      
      var documentVatAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.VatAmount).FirstOrDefault();
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentVatAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentVatAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    
    /// <summary>
    /// Создать договор.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Договор.</returns>
    public Docflow.IOfficialDocument CreateContract(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                    Sungero.Company.IEmployee responsible)
    {
      var document = Sungero.Contracts.Contracts.Create();

      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, null);
      
      return document;
    }
    
    #endregion
    
    #region Доп.соглашение
    
    /// <summary>
    /// Создать доп.соглашение.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки доп.соглашения в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Доп.соглашение.</returns>
    public Docflow.IOfficialDocument CreateSupAgreement(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, Sungero.Company.IEmployee responsible)
    {
      var document = Sungero.Contracts.SupAgreements.Create();
      
      // Вид документа.
      FillDocumentKind(document);

      // Заполнить данные нашей стороны и корреспондента.
      FillContractualDocumentParties(recognitionResult, responsible, document);
      
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата и номер.
      this.FillNumberAndDate(document, recognitionResult, FactNames.SupAgreement);

      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    #endregion
    
    #region Заполнение свойств документа
    
    /// <summary>
    /// Заполнить вид документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <remarks>Заполняется видом документа по умолчанию.
    ///  Если видом документа по умолчанию не указан, то формируется список всех доступных видов документа
    ///  и берется первый элемент из этого списка.</remarks>
    public virtual void FillDocumentKind(IOfficialDocument document)
    {
      var documentKind = Docflow.PublicFunctions.OfficialDocument.GetDefaultDocumentKind(document);
      if (documentKind == null)
      {
        documentKind = Docflow.PublicFunctions.DocumentKind.GetAvailableDocumentKinds(document).FirstOrDefault();
        if (documentKind == null)
        {
          Logger.Error(string.Format("Can not fill document kind for document type {0}.", GetTypeName(document)));
          return;
        }
        Logger.Debug(string.Format("Can not find default documend kind for document type {0}", GetTypeName(document)));
      }
      document.DocumentKind = documentKind;
    }
    
    /// <summary>
    /// Получить имя конечного типа сущности.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <returns>Имя конечного типа сущности.</returns>
    public string GetTypeName(Sungero.Domain.Shared.IEntity entity)
    {
      var entityFinalType = entity.GetType().GetFinalType();
      var entityTypeMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(entityFinalType);
      return entityTypeMetadata.GetDisplayName();
    }
    
    /// <summary>
    /// Заполнить сумму и валюту.
    /// </summary>
    /// <param name="document">Договорной документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    public virtual void FillAmountAndCurrency(IContractualDocumentBase document, Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      if (!ContractualDocumentBases.Is(document))
        return;
      
      var recognizedAmount = DocflowPublicFunctions.GetRecognizedAmount(recognitionResult);
      if (recognizedAmount.HasValue)
      {
        var amount = recognizedAmount.Amount;
        document.TotalAmount = amount;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, recognizedAmount.Fact, FieldNames.DocumentAmount.Amount, document.Info.Properties.TotalAmount.Name, amount, recognizedAmount.IsTrusted);
      }
      
      // В факте с суммой документа может быть не указана валюта, поэтому факт с валютой ищем отдельно,
      // так как на данный момент функция используется для обработки бухгалтерских и договорных документов,
      // а в них все расчеты ведутся в одной валюте.
      var recognizedCurrency = DocflowPublicFunctions.GetRecognizedCurrency(recognitionResult);
      if (recognizedCurrency.HasValue)
      {
        var currency = recognizedCurrency.Currency;
        document.Currency = currency;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, recognizedCurrency.Fact, FieldNames.DocumentAmount.Currency, document.Info.Properties.Currency.Name, currency, recognizedCurrency.IsTrusted);
      }
    }
    
    /// TODO Suleymanov: Удалить после переноса договора и допника в коробку
    /// <summary>
    /// Пронумеровать договорной документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public virtual void FillNumberAndDate(IContractualDocumentBase document,
                                          Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                          string factName)
    {
      // Дата.
      var recognizedDate = GetRecognizedDate(recognitionResult, factName, FieldNames.Document.Date);
      
      // Номер.
      var recognizedNumber = GetRecognizedNumber(recognitionResult, factName, FieldNames.Document.Number);
      if (recognizedNumber.Number == null)
        recognizedNumber.Number = string.Empty;
      
      // Список аналогичен синхронизации с 1С.
      var emptyNumberSymbols = new List<string> { "б/н", "бн", "б-н", string.Empty };
      
      if (string.IsNullOrWhiteSpace(recognizedNumber.Number) || emptyNumberSymbols.Contains(recognizedNumber.Number.ToLower()))
        recognizedNumber.Number = Sungero.Capture.Resources.DocumentWithoutNumber;
      
      if (recognizedNumber.Number.Length > document.Info.Properties.RegistrationNumber.Length)
      {
        recognizedNumber.Number = recognizedNumber.Number.Substring(0, document.Info.Properties.RegistrationNumber.Length);
        recognizedNumber.IsTrusted = false;
      }
      
      document.RegistrationDate = recognizedDate.Date;
      document.RegistrationNumber = recognizedNumber.Number;
      
      var props = document.Info.Properties;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, recognizedDate.Fact, FieldNames.Document.Date, props.RegistrationDate.Name, document.RegistrationDate, recognizedDate.IsTrusted);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, recognizedNumber.Fact, FieldNames.Document.Number, props.RegistrationNumber.Name,
                                                 document.RegistrationNumber, recognizedNumber.IsTrusted);
    }
    
    /// <summary>
    /// Заполнить номер и дату для Mock-документов.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public void FillMockRegistrationData(IOfficialDocument document,
                                         Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                         string factName)
    {
      // Дата.
      var facts = recognitionResult.Facts;
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, factName, FieldNames.Document.Date).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(dateFact, FieldNames.Document.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && DocflowPublicFunctions.IsTrustedField(dateFact, FieldNames.Document.Date);
      document.RegistrationDate = date;

      // Номер.
      var regNumberFact = DocflowPublicFunctions.GetOrderedFacts(facts, factName, FieldNames.Document.Number).FirstOrDefault();
      var regNumber = DocflowPublicFunctions.GetFieldValue(regNumberFact, FieldNames.Document.Number);
      Nullable<bool> isTrustedNumber = null;
      if (regNumber.Length > document.Info.Properties.RegistrationNumber.Length)
      {
        regNumber = regNumber.Substring(0, document.Info.Properties.RegistrationNumber.Length);
        isTrustedNumber = false;
      }
      document.RegistrationNumber = regNumber;
      
      var props = document.Info.Properties;
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Document.Date, props.RegistrationDate.Name, date, isTrustedDate);
      DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, regNumberFact, FieldNames.Document.Number, props.RegistrationNumber.Name,
                                                 document.RegistrationNumber, isTrustedNumber);
    }
    
    /// <summary>
    /// Заполнить корректируемый документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="isAdjustment">Корректировочный.</param>
    public virtual void FillCorrectedDocument(IAccountingDocumentBase document,
                                              Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                              bool isAdjustment)
    {
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = DocflowPublicFunctions.GetOrderedFacts(recognitionResult.Facts, FactNames.FinancialDocument,
                                                                        FieldNames.FinancialDocument.CorrectionDate).FirstOrDefault();
        var correctionNumberFact = DocflowPublicFunctions.GetOrderedFacts(recognitionResult.Facts, FactNames.FinancialDocument,
                                                                          FieldNames.FinancialDocument.CorrectionNumber).FirstOrDefault();
        var correctionDate = DocflowPublicFunctions.GetFieldDateTimeValue(correctionDateFact, FieldNames.FinancialDocument.CorrectionDate);
        var correctionNumber = DocflowPublicFunctions.GetFieldValue(correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber);
        
        document.Corrected = FinancialArchive.UniversalTransferDocuments.GetAll()
          .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
        var props = document.Info.Properties;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, correctionDateFact, FieldNames.FinancialDocument.CorrectionDate,
                                                   props.Corrected.Name, document.Corrected, true);
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber,
                                                   props.Corrected.Name, document.Corrected, true);
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
    
    /// TODO Suleymanov: Удалить после переноса договора и допника в коробку
    /// <summary>
    /// Получить распознанный номер.
    /// </summary>
    /// <param name="recognitionResult">Результаты распознавания.</param>
    /// <param name="numberFactName">Наименование факта, содержащего номер.</param>
    /// <param name="numberFieldName">Наименование поля, содержащего номер.</param>
    /// <returns>Распознанный номер с фактом.</returns>
    public virtual Docflow.Structures.Module.IRecognizedDocumentNumber GetRecognizedNumber(Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                                                                   string numberFactName,
                                                                                                   string numberFieldName)
    {
      var fact = DocflowPublicFunctions.GetOrderedFacts(recognitionResult.Facts, numberFactName, numberFieldName).FirstOrDefault();
      var number = DocflowPublicFunctions.GetFieldValue(fact, numberFieldName);
      var isTrusted = !string.IsNullOrWhiteSpace(number) && DocflowPublicFunctions.IsTrustedField(fact, numberFieldName);
      
      return Docflow.Structures.Module.RecognizedDocumentNumber.Create(number, isTrusted, fact);
    }
    
    /// TODO Suleymanov: Удалить после переноса договора и допника в коробку
    /// <summary>
    /// Получить распознанную дату.
    /// </summary>
    /// <param name="recognitionResult">Результаты распознавания.</param>
    /// <param name="dateFactName">Наименование факта, содержащего дату.</param>
    /// <param name="dateFieldName">Наименование поля, содержащего дату.</param>
    /// <returns>Распознанная дата с фактом.</returns>
    public virtual Docflow.Structures.Module.IRecognizedDocumentDate GetRecognizedDate(Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                                                               string dateFactName,
                                                                                               string dateFieldName)
    {
      var fact = DocflowPublicFunctions.GetOrderedFacts(recognitionResult.Facts, dateFactName, dateFieldName).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(fact, dateFieldName);
      var isTrusted = date != null && DocflowPublicFunctions.IsTrustedField(fact, dateFieldName);
      if (date == null || !date.HasValue || date < Calendar.SqlMinValue)
      {
        date = Calendar.SqlMinValue;
        isTrusted = false;
      }
      
      return Docflow.Structures.Module.RecognizedDocumentDate.Create(date, isTrusted, fact);
    }
    
    /// <summary>
    /// Проверка даты на валидность.
    /// </summary>
    /// <param name="date">проверяемая дата.</param>
    /// <returns>Признак - дата валидна/невалидна.</returns>
    public virtual bool IsDateValid(Nullable<DateTime> date)
    {
      return date == null ||
        date != null && date.HasValue && date >= Calendar.SqlMinValue ;
    }
    
    #endregion
    
    #region Поиск контрагента/НОР
    
    /// <summary>
    /// Получить факты с контрагентом указанного типа из общего списка фактов.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="counterpartyType">Тип контрагента.</param>
    /// <returns></returns>
    public virtual List<Sungero.Docflow.Structures.Module.IFact> GetCounterpartyFacts(List<Sungero.Docflow.Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
        .Where(f => DocflowPublicFunctions.GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType);
      
      if (!counterpartyFacts.Any())
        counterpartyFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN)
          .Where(f => DocflowPublicFunctions.GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType);
      
      return counterpartyFacts.ToList();
    }
    
    /// <summary>
    /// Подобрать по факту контрагента и НОР.
    /// </summary>
    /// <param name="allFacts">Факты.</param>
    /// <param name="counterpartyTypes">Типы фактов контрагентов.</param>
    /// <returns>Наши организации и контрагенты, найденные по фактам.</returns>
    public virtual List<Docflow.Structures.Module.ICounterpartyFactMatching> MatchFactsWithBusinessUnitsAndCounterparties(List<Sungero.Docflow.Structures.Module.IFact> allFacts,
                                                                                                                                  List<string> counterpartyTypes)
    {
      var counterpartyPropertyName = AccountingDocumentBases.Info.Properties.Counterparty.Name;
      var businessUnitPropertyName = AccountingDocumentBases.Info.Properties.BusinessUnit.Name;
      
      // Фильтр фактов по типам.
      var facts = new List<Sungero.Docflow.Structures.Module.IFact>();
      foreach (var counterpartyType in counterpartyTypes)
        facts.AddRange(GetCounterpartyFacts(allFacts, counterpartyType));
      
      var matchings = new List<Docflow.Structures.Module.ICounterpartyFactMatching>();
      foreach (var fact in facts)
      {
        var counterparty = Counterparties.Null;
        var businessUnit = BusinessUnits.Null;
        bool isTrusted = true;
        
        // Если для свойства counterpartyPropertyName по факту существует верифицированное ранее значение, то вернуть его.
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, counterpartyPropertyName);
        if (verifiedCounterparty != null)
        {
          counterparty = verifiedCounterparty.Counterparty;
          isTrusted = verifiedCounterparty.IsTrusted;
        }
        
        // Если для свойства businessUnitPropertyName по факту существует верифицированное ранее значение, то вернуть его.
        var verifiedBusinessUnit = GetBusinessUnitByVerifiedData(fact, businessUnitPropertyName);
        if (verifiedBusinessUnit != null)
        {
          businessUnit = verifiedBusinessUnit.BusinessUnit;
          isTrusted = verifiedBusinessUnit.IsTrusted;
        }
        
        // Поиск по ИНН/КПП.
        var tin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
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
          
          // Получить запись по точному совпадению по ИНН/КПП.
          if (!string.IsNullOrWhiteSpace(trrc))
            counterparty = counterparties.FirstOrDefault(x => CompanyBases.Is(x) && CompanyBases.As(x).TRRC == trrc);
          // Получить запись с совпадением по ИНН, если не найдено по точному совпадению ИНН/КПП.
          if (counterparty == null)
            counterparty = counterparties.FirstOrDefault();
        }
        
        if (counterparty != null || businessUnit != null)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create(businessUnit, counterparty, fact,
                                                                                                           DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType),
                                                                                                           isTrusted);
          matchings.Add(counterpartyFactMatching);
          continue;
        }
        
        // Если не нашли по ИНН/КПП то ищем по наименованию.
        var name = GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        counterparty = Counterparties.GetAll().FirstOrDefault(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed &&
                                                              x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        businessUnit = BusinessUnits.GetAll().FirstOrDefault(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed &&
                                                             x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (counterparty != null || businessUnit != null)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create(businessUnit, counterparty, fact,
                                                                                                           DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType),
                                                                                                           false);
          matchings.Add(counterpartyFactMatching);
        }
      }
      
      return matchings;
    }
    
    /// <summary>
    /// Подобрать участников сделки (НОР и контрагент).
    /// </summary>
    /// <param name="buyer">Список фактов с данными о контрагенте. Тип контрагента - покупатель.</param>
    /// <param name="seller">Список фактов с данными о контрагенте. Тип контрагента - продавец.</param>
    /// <param name="nonType">Список фактов с данными о контрагенте. Тип контрагента не заполнен.</param>
    /// <param name="responsibleEmployee">Ответственный сотрудник.</param>
    /// <returns>НОР и контрагент.</returns>
    public virtual Docflow.Structures.Module.IDocumentParties GetDocumentParties(Docflow.Structures.Module.ICounterpartyFactMatching buyer,
                                                                                         Docflow.Structures.Module.ICounterpartyFactMatching seller,
                                                                                         List<Docflow.Structures.Module.ICounterpartyFactMatching> nonType,
                                                                                         IEmployee responsibleEmployee)
    {
      Sungero.Docflow.Structures.Module.ICounterpartyFactMatching counterparty = null;
      Sungero.Docflow.Structures.Module.ICounterpartyFactMatching businessUnit = null;
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsibleEmployee);
      var responsibleEmployeePersonalSettings = Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(responsibleEmployee);
      var responsibleEmployeePersonalSettingsBusinessUnit = responsibleEmployeePersonalSettings != null
        ? responsibleEmployeePersonalSettings.BusinessUnit
        : Company.BusinessUnits.Null;
      
      // НОР.
      var businessUnitFindedNotExactly = false;
      if (buyer != null)
      {
        // НОР по факту с типом BUYER.
        businessUnit = buyer;
      }
      else
      {
        // НОР по факту без типа.
        var nonTypeBusinessUnits = nonType.Where(m => m.BusinessUnit != null);
        
        // Уточнить НОР по ответственному.
        if (nonTypeBusinessUnits.Count() > 1)
        {
          // Если в персональных настройках ответственного указана НОР.
          if (responsibleEmployeePersonalSettingsBusinessUnit != null)
            businessUnit = nonTypeBusinessUnits.Where(m => Equals(m.BusinessUnit, responsibleEmployeePersonalSettingsBusinessUnit)).FirstOrDefault();
          
          // НОР не определилась по персональным настройкам ответственного.
          if (businessUnit == null)
            businessUnit = nonTypeBusinessUnits.Where(m => Equals(m.BusinessUnit, responsibleEmployeeBusinessUnit)).FirstOrDefault();
          
          // НОР не определилась по ответственному.
          if (businessUnit == null)
            businessUnitFindedNotExactly = true;
        }
        
        if (businessUnit == null)
          businessUnit = nonTypeBusinessUnits.FirstOrDefault();
        
        // Подсветить жёлтым, если НОР было несколько и определить по ответственному не удалось.
        if (businessUnitFindedNotExactly)
          businessUnit.IsTrusted = false;
      }
      
      // Контрагент.
      if (seller != null && seller.Counterparty != null)
      {
        // Контрагент по факту с типом SELLER.
        counterparty = seller;
      }
      else
      {
        // Контрагент по факту без типа. Исключить факт, по которому нашли НОР.
        var nonTypeCounterparties = nonType
          .Where(m => m.Counterparty != null)
          .Where(m => !Equals(m, businessUnit));
        counterparty = nonTypeCounterparties.FirstOrDefault();
        
        // Подсветить жёлтым, если контрагентов было несколько.
        if (nonTypeCounterparties.Count() > 1 || businessUnitFindedNotExactly)
          counterparty.IsTrusted = false;
      }
      
      // В качестве НОР ответственного вернуть НОР из персональных настроек, если она указана.
      return responsibleEmployeePersonalSettingsBusinessUnit != null
        ? Docflow.Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeePersonalSettingsBusinessUnit)
        : Docflow.Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeeBusinessUnit);
    }
    
    
    /// <summary>
    /// Подобрать участников сделки (НОР и контрагент).
    /// </summary>
    /// <param name="buyer">Список фактов с данными о контрагенте. Тип контрагента - покупатель.</param>
    /// <param name="seller">Список фактов с данными о контрагенте. Тип контрагента - продавец.</param>
    /// <param name="responsibleEmployee">Ответственный сотрудник.</param>
    /// <returns>НОР и контрагент.</returns>
    public virtual Sungero.Docflow.Structures.Module.IDocumentParties GetDocumentParties(Docflow.Structures.Module.ICounterpartyFactMatching buyer,
                                                                                         Docflow.Structures.Module.ICounterpartyFactMatching seller,
                                                                                         IEmployee responsibleEmployee)
    {
      Docflow.Structures.Module.ICounterpartyFactMatching counterparty = null;
      Docflow.Structures.Module.ICounterpartyFactMatching businessUnit = null;
      
      // НОР.
      if (buyer != null)
        businessUnit = buyer;
      
      // Контрагент.
      if (seller != null && seller.Counterparty != null)
        counterparty = seller;
      
      var responsibleEmployeeBusinessUnit = Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsibleEmployee);
      
      return Docflow.Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeeBusinessUnit);
    }
    
    /// <summary>
    /// Поиск контрагента для документов в демо режиме.
    /// </summary>
    /// <param name="facts">Факты для поиска факта с контрагентом.</param>
    /// <param name="counterpartyType">Тип контрагента.</param>
    /// <returns>Контрагент.</returns>
    public static Structures.Module.MockCounterparty GetMostProbableMockCounterparty(List<Sungero.Docflow.Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name);
      var mostProbabilityFact = counterpartyFacts.Where(f => DocflowPublicFunctions.GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType).FirstOrDefault();
      if (mostProbabilityFact == null)
        return null;

      var counterparty = Structures.Module.MockCounterparty.Create();
      counterparty.Name = DocflowPublicFunctions.GetCounterpartyName(mostProbabilityFact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
      counterparty.Tin = DocflowPublicFunctions.GetFieldValue(mostProbabilityFact, FieldNames.Counterparty.TIN);
      counterparty.Trrc = DocflowPublicFunctions.GetFieldValue(mostProbabilityFact, FieldNames.Counterparty.TRRC);
      counterparty.Fact = mostProbabilityFact;
      return counterparty;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить название факта контрагента с названиями полей наименования и ОПФ.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Наименование факта, наименования полей с наименованием и ОПФ организации.</returns>
    public virtual Sungero.Docflow.Structures.Module.ICounterpartyFactNames GetCounterpartyFactNames(string predictedClass)
    {
      var counterpartyFactNames = Sungero.Docflow.Structures.Module.CounterpartyFactNames.Create();
      
      // Если пришло входящее письмо
      if (predictedClass == ArioClassNames.Letter)
      {
        counterpartyFactNames.Fact = FactNames.Letter;
        counterpartyFactNames.NameField = FieldNames.Letter.CorrespondentName;
        counterpartyFactNames.LegalFormField = FieldNames.Letter.CorrespondentLegalForm;
      }
      
      // Если пришел договорной документ
      if ((predictedClass == ArioClassNames.Contract) ||
          (predictedClass == ArioClassNames.SupAgreement))
      {
        counterpartyFactNames.Fact = FactNames.Counterparty;
        counterpartyFactNames.NameField = FieldNames.Counterparty.Name;
        counterpartyFactNames.LegalFormField = FieldNames.Counterparty.LegalForm;
      }
      return counterpartyFactNames;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Поиск корреспондента по извлеченным фактам.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Корреспондент.</returns>
    public virtual Docflow.Structures.Module.ICounterpartyFactMatching GetCounterparty(Docflow.Structures.Module.IRecognitionResult recognitionResult, string propertyName)
    {
      var actualCounterparties = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      var foundByName = new List<Docflow.Structures.Module.ICounterpartyFactMatching>();
      var correspondentNames = new List<string>();
      
      var facts = recognitionResult.Facts;
      
      var counterpartyFactNames = GetCounterpartyFactNames(recognitionResult.PredictedClass);
      
      // Подобрать контрагентов подходящих по имени для переданных фактов.
      foreach (var fact in GetFacts(facts, counterpartyFactNames.Fact, counterpartyFactNames.NameField))
      {
        // Если для свойства propertyName по факту существует верифицированное ранее значение, то вернуть его.
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, propertyName);
        if (verifiedCounterparty != null)
          return verifiedCounterparty;

        var name = GetCounterpartyName(fact, counterpartyFactNames.NameField, counterpartyFactNames.LegalFormField);
        correspondentNames.Add(name);
        
        var counterparties = actualCounterparties.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        foreach (var counterparty in counterparties)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
          counterpartyFactMatching.Counterparty = counterparty;
          counterpartyFactMatching.Fact = fact;
          counterpartyFactMatching.IsTrusted = false;
          foundByName.Add(counterpartyFactMatching);
        }
      }
      
      // Если нет фактов содержащих поле ИНН, то вернуть первого корреспондента по наименованию.
      var correspondentTINs = GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (!correspondentTINs.Any())
        return foundByName.FirstOrDefault();
      
      // Поиск по ИНН/КПП.
      var foundByTin = new List<Docflow.Structures.Module.ICounterpartyFactMatching>();
      foreach (var fact in correspondentTINs)
      {
        // Если для свойства propertyName по факту существует верифицированное ранее значение, то вернуть его.
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, propertyName);
        if (verifiedCounterparty != null)
          return verifiedCounterparty;
        
        var tin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        var counterparties = Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, true);
        foreach (var counterparty in counterparties)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
          counterpartyFactMatching.Counterparty = counterparty;
          counterpartyFactMatching.Fact = fact;
          counterpartyFactMatching.IsTrusted = true;
          foundByTin.Add(counterpartyFactMatching);
        }
      }
      
      Docflow.Structures.Module.ICounterpartyFactMatching resultCounterparty = null;
      // Найдено 0. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
      if (foundByTin.Count == 0)
        resultCounterparty = foundByName.Where(x => string.IsNullOrEmpty(x.Counterparty.TIN))
          .Where(x => !CompanyBases.Is(x.Counterparty) || string.IsNullOrEmpty(CompanyBases.As(x.Counterparty).TRRC))
          .FirstOrDefault();
      // Найден 1.
      if (foundByTin.Count == 1)
        resultCounterparty = foundByTin.First();
      
      // Найдено несколько. Уточнить поиск по наименованию.
      if (foundByTin.Count > 1)
      {
        var specifiedByName = foundByTin.Where(x => correspondentNames.Any(name => x.Counterparty.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))).ToList();
        if (specifiedByName.Count == 0)
        {
          resultCounterparty = foundByTin.FirstOrDefault();
          resultCounterparty.IsTrusted = false;
        }
        if (specifiedByName.Count == 1)
          resultCounterparty = specifiedByName.First();
        if (specifiedByName.Count > 1)
        {
          resultCounterparty = specifiedByName.FirstOrDefault();
          resultCounterparty.IsTrusted = false;
        }
      }
      
      return resultCounterparty;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма, акта, накладной, СФ, счета на оплату, УПД в коробку
    /// <summary>
    /// Получить контрагента по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связка контрагент + факт.</returns>
    public virtual Docflow.Structures.Module.ICounterpartyFactMatching GetCounterpartyByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact, string propertyName)
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
      
      var result = Docflow.Structures.Module.CounterpartyFactMatching.Create();
      result.Counterparty = filteredCounterparty;
      result.Fact = fact;
      result.IsTrusted = counterpartyUnitField.IsTrusted == true;
      return result;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вх. письма, накладной, акта, УПД, СФ в коробку
    /// <summary>
    /// Получить нор по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связку контрагент + факт.</returns>
    public virtual Docflow.Structures.Module.ICounterpartyFactMatching GetBusinessUnitByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact, string propertyName)
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
      
      var result = Docflow.Structures.Module.CounterpartyFactMatching.Create();
      result.BusinessUnit = filteredBusinessUnit;
      result.Fact = fact;
      result.IsTrusted = businessUnitField.IsTrusted == true;
      return result;
    }

    /// TODO Suleymanov: Удалить после переноса договора, допника и вход. письма в коробку
    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного и адресата.
    /// </summary>
    /// <param name="businessUnitsWithFacts">НОР, найденные по фактам.</param>
    /// <param name="businessUnitPropertyName">Имя связанного свойства.</param>
    /// <param name="addressee">Адресат.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public virtual Docflow.Structures.Module.ICounterpartyFactMatching GetMostProbableBusinessUnitMatching(List<Docflow.Structures.Module.ICounterpartyFactMatching> businessUnitsWithFacts,
                                                                                                                   string businessUnitPropertyName, IEmployee addressee,
                                                                                                                   IEmployee responsible)
    {
      // Если для свойства businessUnitPropertyName по факту существует верифицированное ранее значение, то вернуть его.
      foreach(var record in businessUnitsWithFacts)
      {
        var result = GetBusinessUnitByVerifiedData(record.Fact, businessUnitPropertyName);
        if (result != null && result.BusinessUnit != null)
          return result;
      }
      
      var businessUnitByAddressee = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(addressee);
      var businessUnitByAddresseeFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
      businessUnitByAddresseeFactMatching.BusinessUnit = businessUnitByAddressee;
      businessUnitByAddresseeFactMatching.Fact = null;
      businessUnitByAddresseeFactMatching.IsTrusted = false;
      
      // Попытаться уточнить по адресату.
      // TODO Dmitirev_IA: Скорее всего стоит уточнять по адресату, если фактов нет или их несколько.
      var hasAnyBusinessUnitFacts = businessUnitsWithFacts.Any();
      var hasBusinessUnitByAddressee = businessUnitByAddressee != null;
      Docflow.Structures.Module.ICounterpartyFactMatching businessUnitWithFact = null;
      if (hasAnyBusinessUnitFacts && hasBusinessUnitByAddressee)
        businessUnitWithFact = businessUnitByAddresseeFactMatching;
      else
      {
        businessUnitWithFact = businessUnitsWithFacts.FirstOrDefault();
        if (businessUnitsWithFacts.Count > 1)
          businessUnitWithFact.IsTrusted = false;
      }
      if (businessUnitWithFact != null)
        return businessUnitWithFact;
      
      // Если факты с ИНН/КПП не найдены, и по наименованию не найдено, то вернуть НОР из адресата.
      if (businessUnitByAddresseeFactMatching.BusinessUnit != null)
        return businessUnitByAddresseeFactMatching;
      
      // Если и по адресату не найдено, то вернуть НОР из ответственного.
      var businessUnitByResponsible = Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsible);
      var businessUnitByResponsibleFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
      businessUnitByResponsibleFactMatching.BusinessUnit = businessUnitByResponsible;
      businessUnitByResponsibleFactMatching.Fact = null;
      businessUnitByResponsibleFactMatching.IsTrusted = false;
      return businessUnitByResponsibleFactMatching;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного в договорных документах.
    /// </summary>
    /// <param name="businessUnitsWithFacts">НОР, найденные по фактам.</param>
    /// <param name="businessUnitPropertyName">Имя связанного свойства.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public virtual Docflow.Structures.Module.ICounterpartyFactMatching GetMostProbableBusinessUnitMatching(List<Docflow.Structures.Module.ICounterpartyFactMatching> businessUnitsWithFacts,
                                                                                                                   string businessUnitPropertyName,
                                                                                                                   IEmployee responsible)
    {
      var businessUnit = Docflow.Structures.Module.CounterpartyFactMatching.Create();
      
      // Если для свойства businessUnitPropertyName по факту существует верифицированное ранее значение, то вернуть его.
      foreach(var record in businessUnitsWithFacts)
      {
        businessUnit = GetBusinessUnitByVerifiedData(record.Fact, businessUnitPropertyName);
        if (businessUnit != null && businessUnit.BusinessUnit != null)
          return businessUnit;
      }
      
      // Уточнить НОР по ответственному
      var businessUnitFindedNotExactly = false;
      var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
      var responsibleEmployeePersonalSettings = Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(responsible);
      var responsibleEmployeePersonalSettingsBusinessUnit = responsibleEmployeePersonalSettings != null
        ? responsibleEmployeePersonalSettings.BusinessUnit
        : Company.BusinessUnits.Null;
      
      if (businessUnitsWithFacts.Count() > 1)
      {
        // Если в персональных настройках ответственного указана НОР.
        if (responsibleEmployeePersonalSettingsBusinessUnit != null)
          businessUnit = businessUnitsWithFacts.Where(x => Equals(x.BusinessUnit, responsibleEmployeePersonalSettingsBusinessUnit)).FirstOrDefault();
        
        // НОР не определилась по персональным настройкам ответственного.
        if (businessUnit == null)
          businessUnit = businessUnitsWithFacts.Where(x => Equals(x.BusinessUnit, responsibleEmployeeBusinessUnit)).FirstOrDefault();
        
        // НОР не определилась по ответственному.
        if (businessUnit == null)
        {
          businessUnitFindedNotExactly = true;
          businessUnit = businessUnitsWithFacts.FirstOrDefault();
        }
        
        // Подсветить жёлтым, если НОР было несколько и определить по ответственному не удалось.
        if (businessUnitFindedNotExactly)
          businessUnit.IsTrusted = false;
        
        return businessUnit;
      }
      
      if (businessUnitsWithFacts.Count() == 0)
      {
        businessUnit.Fact = null;
        businessUnit.IsTrusted = false;
        return businessUnit;
      }
      
      businessUnit = businessUnitsWithFacts.FirstOrDefault();
      return businessUnit;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получение списка НОР по извлеченным фактам.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <returns>Список НОР и соответствующих им фактов.</returns>
    public virtual List<Docflow.Structures.Module.ICounterpartyFactMatching> GetBusinessUnitsWithFacts(Docflow.Structures.Module.IRecognitionResult recognitionResult)
    {
      var facts = recognitionResult.Facts;
      
      var counterpartyFactNames = GetCounterpartyFactNames(recognitionResult.PredictedClass);
      
      // Получить факты с наименованиями организаций.
      var businessUnitsByName = new List<Sungero.Docflow.Structures.Module.ICounterpartyFactMatching>();
      var correspondentNameFacts = GetFacts(facts, counterpartyFactNames.Fact, counterpartyFactNames.NameField)
        .OrderByDescending(x => x.Fields.First(f => f.Name == counterpartyFactNames.NameField).Probability);
      
      foreach (var fact in correspondentNameFacts)
      {
        var name = GetCounterpartyName(fact, counterpartyFactNames.NameField, counterpartyFactNames.LegalFormField);
        
        var businessUnits = BusinessUnits.GetAll()
          .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          .Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        
        foreach (var businessUnit in businessUnits)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
          counterpartyFactMatching.BusinessUnit = businessUnit;
          counterpartyFactMatching.Fact = fact;
          counterpartyFactMatching.IsTrusted = false;
          
          businessUnitsByName.Add(counterpartyFactMatching);
        }
      }
      
      // Если факты с ИНН/КПП не найдены, то вернуть факты с наименованиями организаций.
      var correspondentTinFacts = GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN)
        .OrderByDescending(x => x.Fields.First(f => f.Name == FieldNames.Counterparty.TIN).Probability);
      if (!correspondentTinFacts.Any())
        return businessUnitsByName;

      // Поиск по ИНН/КПП.
      var foundByTin = new List<Docflow.Structures.Module.ICounterpartyFactMatching>();
      foreach (var fact in correspondentTinFacts)
      {
        var tin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        var businessUnits = Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);
        var isTrusted = businessUnits.Count == 1;
        
        foreach (var businessUnit in businessUnits)
        {
          var counterpartyFactMatching = Docflow.Structures.Module.CounterpartyFactMatching.Create();
          counterpartyFactMatching.BusinessUnit = businessUnit;
          counterpartyFactMatching.Fact = fact;
          counterpartyFactMatching.IsTrusted = isTrusted;
          
          foundByTin.Add(counterpartyFactMatching);
        }
        
      }
      
      // Найдено по ИНН/КПП.
      if (foundByTin.Any())
        return foundByTin;
      
      // Не найдено. Искать по наименованию в корреспондентах с пустыми ИНН/КПП.
      if (businessUnitsByName.Any())
        return businessUnitsByName.Where(x => string.IsNullOrEmpty(x.BusinessUnit.TIN) && string.IsNullOrEmpty(x.BusinessUnit.TRRC)).ToList();
      
      return businessUnitsByName;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Заполнить стороны договорного документа.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="responsible">Ответственный за верификацию.</param>
    /// <param name="document">Договорной документ.</param>
    public virtual void FillContractualDocumentParties(Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                       Sungero.Company.IEmployee responsible,
                                                       Sungero.Contracts.IContractualDocument document)
    {
      var props = document.Info.Properties;
      var businessUnitPropertyName = props.BusinessUnit.Name;
      var counterpartyPropertyName = props.Counterparty.Name;
      var signatoryFieldNames = this.GetSignatoryFieldNames();
      var counterpartyFieldNames = this.GetCounterpartyFieldNames();
      Sungero.Docflow.Structures.Module.IFact businessUnitFact = null;
      Sungero.Docflow.Structures.Module.IFact ourSignatoryFact = null;
      
      // Заполнить данные нашей стороны.
      var businessUnitsWithFacts = GetBusinessUnitsWithFacts(recognitionResult);
      
      var businessUnitWithFact = GetMostProbableBusinessUnitMatching(businessUnitsWithFacts,
                                                                     businessUnitPropertyName,
                                                                     responsible);
      if (businessUnitWithFact.BusinessUnit != null)
      {
        document.BusinessUnit = businessUnitWithFact.BusinessUnit;
        businessUnitFact = businessUnitWithFact.Fact;
        DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, businessUnitFact, counterpartyFieldNames,
                                                         businessUnitPropertyName, document.BusinessUnit, businessUnitWithFact.IsTrusted, null);
      }
      
      // Заполнить подписанта.
      var ourSignatory = GetContractualDocumentSignatory(recognitionResult, document, businessUnitWithFact, true);
      
      // Если НОР не заполнена, она подставляется из подписанта и результату не доверяем.
      if (document.BusinessUnit == null && ourSignatory.Employee != null)
        DocflowPublicFunctions.LinkFactAndProperty(recognitionResult, null, null, props.BusinessUnit.Name, ourSignatory.Employee.Department.BusinessUnit, false);
      
      document.OurSignatory = ourSignatory.Employee;
      ourSignatoryFact = ourSignatory.Fact;
      var isTrustedOurSignatory = businessUnitFact != null &&
        ourSignatory.IsTrusted &&
        DocflowPublicFunctions.IsTrustedField(ourSignatoryFact, FieldNames.Counterparty.SignatorySurname);
      DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, ourSignatoryFact, signatoryFieldNames,
                                                       props.OurSignatory.Name, document.OurSignatory, isTrustedOurSignatory, null);
      
      // Если НОР по фактам не нашли, то взять ее из персональных настроек, или от ответственного.
      if (document.BusinessUnit == null)
      {
        var responsibleEmployeeBusinessUnit = Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnit(responsible);
        var responsibleEmployeePersonalSettings = Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(responsible);
        var responsibleEmployeePersonalSettingsBusinessUnit = responsibleEmployeePersonalSettings != null
          ? responsibleEmployeePersonalSettings.BusinessUnit
          : Company.BusinessUnits.Null;
        
        // Если в персональных настройках ответственного указана НОР.
        if (responsibleEmployeePersonalSettingsBusinessUnit != null)
          document.BusinessUnit = responsibleEmployeePersonalSettingsBusinessUnit;
        else
          document.BusinessUnit = responsibleEmployeeBusinessUnit;
        
        DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, null, null, props.BusinessUnit.Name, document.BusinessUnit, false, null);
      }
      
      // Убрать использованные факты подбора НОР и подписывающего с нашей стороны.
      if (businessUnitFact != null)
        recognitionResult.Facts.Remove(businessUnitFact);
      if (ourSignatoryFact != null &&
          (businessUnitFact == null || ourSignatoryFact.Id != businessUnitFact.Id))
        recognitionResult.Facts.Remove(ourSignatoryFact);
      
      // Заполнить данные контрагента.
      var сounterparty = GetCounterparty(recognitionResult, counterpartyPropertyName);
      if (сounterparty != null)
      {
        document.Counterparty = сounterparty.Counterparty;
        DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, сounterparty.Fact, counterpartyFieldNames,
                                                         counterpartyPropertyName, document.Counterparty, сounterparty.IsTrusted, null);
      }
      
      // Заполнить подписанта.
      var signedBy = GetContractualDocumentSignatory(recognitionResult, document, сounterparty);
      
      // Если контрагент не заполнен, он подставляется из подписанта и результату не доверяем.
      if (document.Counterparty == null && signedBy.Contact != null)
        DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, null, null, props.Counterparty.Name, signedBy.Contact.Company, false, null);
      
      document.CounterpartySignatory = signedBy.Contact;
      var isTrustedSignatory = signedBy.IsTrusted && DocflowPublicFunctions.IsTrustedField(signedBy.Fact, FieldNames.Counterparty.SignatorySurname);
      DocflowPublicFunctions.LinkFactFieldsAndProperty(recognitionResult, signedBy.Fact, signatoryFieldNames,
                                                       props.CounterpartySignatory.Name, document.CounterpartySignatory, isTrustedSignatory, null);
    }
    
    #endregion
    
    #region Работа с полями/фактами

    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Получить список полей из факта.
    /// </summary>
    /// <param name="fact">Имя факта.</param>
    /// <param name="fieldName">Список имен поля.</param>
    /// <returns>Список полей.</returns>
    public static IQueryable<Sungero.Docflow.Structures.Module.IFactField> GetFields(Sungero.Docflow.Structures.Module.IFact fact, List<string> fieldNames)
    {
      if (fact == null)
        return null;
      return fact.Fields.Where(f => fieldNames.Contains(f.Name)).AsQueryable();
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма, акта, накладной, СФ, счета на оплату, УПД в коробку
    /// <summary>
    /// Получить запись, которая уже сопоставлялась с переданным фактом.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя свойства документа связанное с фактом.</param>
    /// <returns>Связь факта с свойством.</returns>
    public virtual Sungero.Commons.IEntityRecognitionInfoFacts GetFieldByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact, string propertyName)
    {
      var factLabel = DocflowPublicFunctions.GetFactLabel(fact, propertyName);
      var recognitionInfo = Commons.EntityRecognitionInfos.GetAll()
        .Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return null;
      
      return recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма, акта, накладной, СФ, счета на оплату, УПД в коробку
    /// <summary>
    /// Получить запись, которая уже сопоставлялась с переданным фактом, с дополнительной фильтрацией по контрагенту.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя свойства документа связанное с фактом.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя свойства документа, содержащее контрагента.</param>
    /// <returns>Связь факта с свойством.</returns>
    public virtual Sungero.Commons.IEntityRecognitionInfoFacts GetFieldByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact, string propertyName, string counterpartyPropertyValue, string counterpartyPropertyName)
    {
      var factLabel = DocflowPublicFunctions.GetFactLabel(fact, propertyName);
      var recognitionInfo = Commons.EntityRecognitionInfos.GetAll()
        .Where(d => d.Facts.Any(f => f.FactLabel == factLabel && f.VerifiedValue != null && f.VerifiedValue != string.Empty) &&
               d.Facts.Any(f => f.PropertyName == counterpartyPropertyName && f.PropertyValue == counterpartyPropertyValue))
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
      if (recognitionInfo == null)
        return null;
      
      return recognitionInfo.Facts
        .Where(f => f.FactLabel == factLabel && !string.IsNullOrWhiteSpace(f.VerifiedValue)).First();
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить список фактов с переданным именем факта.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <returns>Список фактов.</returns>
    public static List<Sungero.Docflow.Structures.Module.IFact> GetFacts(List<Sungero.Docflow.Structures.Module.IFact> facts, string factName)
    {
      return facts
        .Where(f => f.Name == factName)
        .ToList();
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить список фактов с переданными именем факта и именем поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Список фактов.</returns>
    public static List<Sungero.Docflow.Structures.Module.IFact> GetFacts(List<Sungero.Docflow.Structures.Module.IFact> facts, string factName, string fieldName)
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
    public virtual System.IO.Stream GetDocumentBody(string documentGuid)
    {
      var arioUrl = Docflow.PublicFunctions.SmartProcessingSetting.GetArioUrl();
      var arioConnector = new ArioExtensions.ArioConnector(arioUrl);
      return arioConnector.GetDocumentByGuid(documentGuid);
    }
    
    /// <summary>
    /// Получить значение численного параметра из docflow_params.
    /// </summary>
    /// <param name="paramName">Наименование параметра.</param>
    /// <returns>Значение параметра.</returns>
    [Public, Remote(IsPure = true)]
    public static double GetDocflowParamsNumbericValue(string paramName)
    {
      return Docflow.PublicFunctions.Module.Remote.GetDocflowParamsNumbericValue(paramName);
    }
    
    /// <summary>
    /// Получить параметры отображения фокусировки подсветки.
    /// </summary>
    /// <returns>Параметры.</returns>
    [Remote]
    public virtual Structures.Module.IHighlightActivationStyle GetHighlightActivationStyle()
    {
      var highlightActivationStyle = Structures.Module.HighlightActivationStyle.Create();
      highlightActivationStyle.UseBorder = Docflow.PublicFunctions.Module.Remote.GetDocflowParamsStringValue(HighlightActivationStyleParamNames.UseBorder);
      highlightActivationStyle.BorderColor = Docflow.PublicFunctions.Module.Remote.GetDocflowParamsStringValue(HighlightActivationStyleParamNames.BorderColor);
      highlightActivationStyle.BorderWidth = GetDocflowParamsNumbericValue(HighlightActivationStyleParamNames.BorderWidth);
      highlightActivationStyle.UseFilling = Docflow.PublicFunctions.Module.Remote.GetDocflowParamsStringValue(HighlightActivationStyleParamNames.UseFilling);
      highlightActivationStyle.FillingColor = Docflow.PublicFunctions.Module.Remote.GetDocflowParamsStringValue(HighlightActivationStyleParamNames.FillingColor);
      return highlightActivationStyle;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить наименование контрагента.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование контрагента.</param>
    /// <param name="nameFieldName">Наименование поля с наименованием контрагента.</param>
    /// <param name="legalFormFieldName">Наименование поля с организационо-правовой формой контрагента.</param>
    /// <returns>Наименование контрагента.</returns>
    public static string GetCounterpartyName(Sungero.Docflow.Structures.Module.IFact fact, string nameFieldName, string legalFormFieldName)
    {
      if (fact == null)
        return string.Empty;
      
      var name = DocflowPublicFunctions.GetFieldValue(fact, nameFieldName);
      var legalForm = DocflowPublicFunctions.GetFieldValue(fact, legalFormFieldName);
      return string.IsNullOrEmpty(legalForm) ? name : string.Format("{0}, {1}", name, legalForm);
    }
    
    /// <summary>
    /// Получить наименование ведущего документа.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование ведущего документа.</param>
    /// <returns>Наименование ведущего документа с номером и датой.</returns>
    private static string GetLeadingDocumentName(Sungero.Docflow.Structures.Module.IFact fact)
    {
      if (fact == null)
        return string.Empty;
      
      var documentName = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseName);
      var date = Functions.Module.GetShortDate(DocflowPublicFunctions.GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseDate));
      var number = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseNumber);
      
      if (string.IsNullOrWhiteSpace(documentName))
        return string.Empty;
      
      if (!string.IsNullOrWhiteSpace(number))
        documentName = string.Format("{0} №{1}", documentName, number);
      
      if (!string.IsNullOrWhiteSpace(date))
        documentName = string.Format("{0} от {1}", documentName, date);
      
      return documentName;
    }
    
    /// <summary>
    /// Сохранить результат верификации заполнения свойств.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public virtual void StoreVerifiedPropertiesValues(Docflow.IOfficialDocument document)
    {
      // Сохранять только в том случае, когда статус верификации меняется на "Завершено".
      var documentParams = ((Domain.Shared.IExtendedEntity)document).Params;
      if (!documentParams.ContainsKey(Docflow.PublicConstants.OfficialDocument.NeedStoreVerifiedPropertiesValuesParamName))
        return;
      
      var recognitionInfo = Commons.PublicFunctions.EntityRecognitionInfo.Remote.GetEntityRecognitionInfo(document);
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
          var propertyStringValue = Docflow.PublicFunctions.Module.GetPropertyValueAsString(propertyValue);
          if (!string.IsNullOrWhiteSpace(propertyStringValue) && !Equals(propertyStringValue, linkedFact.PropertyValue))
            linkedFact.VerifiedValue = propertyStringValue;
        }
      }
      documentParams.Remove(Docflow.PublicConstants.OfficialDocument.NeedStoreVerifiedPropertiesValuesParamName);
    }
    
    
    /// <summary>
    /// Создать тело документа.
    /// </summary>
    /// <param name="document">Документ Rx.</param>
    /// <param name="recognitionResult">Результат обработки входящего документа в Арио.</param>
    /// <param name="versionNote">Примечание к версии.</param>
    public virtual void CreateVersion(IOfficialDocument document, Sungero.Docflow.Structures.Module.IRecognitionResult recognitionResult, string versionNote = "")
    {
      var needCreatePublicBody = recognitionResult.File != null && recognitionResult.File.Data != null;
      var pdfApp = Content.AssociatedApplications.GetByExtension(Constants.Module.PdfExtension);
      if (pdfApp == Content.AssociatedApplications.Null)
        pdfApp = GetAssociatedApplicationByFileName(recognitionResult.File.Path);
      
      var originalFileApp = Content.AssociatedApplications.Null;
      if (needCreatePublicBody)
        originalFileApp = GetAssociatedApplicationByFileName(recognitionResult.File.Path);
      
      // При создании версии Subject не должен быть пустым, иначе задваивается имя документа.
      var subjectIsEmpty = string.IsNullOrEmpty(document.Subject);
      if (subjectIsEmpty)
        document.Subject = "tmp_Subject";
      
      document.CreateVersion();
      var version = document.LastVersion;
      
      if (needCreatePublicBody)
      {
        using (var publicBody = GetDocumentBody(recognitionResult.BodyGuid))
          version.PublicBody.Write(publicBody);
        using (var body = new MemoryStream(recognitionResult.File.Data))
          version.Body.Write(body);
        version.AssociatedApplication = pdfApp;
        version.BodyAssociatedApplication = originalFileApp;
      }
      else
      {
        using (var body = GetDocumentBody(recognitionResult.BodyGuid))
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
      
      // Заполнить статус верификации для документов, в которых поддерживается режим верификации.
      if (Docflow.PublicFunctions.OfficialDocument.IsVerificationModeSupported(document))
        document.VerificationState = Docflow.OfficialDocument.VerificationState.InProcess;
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
    
    /// <summary>
    /// Поиск адресата письма.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <returns>Адресат.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetAdresseeByFact(Sungero.Docflow.Structures.Module.IFact fact)
    {
      var result = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(Sungero.Company.Employees.Null, null, fact, false);
      
      if (fact == null)
        return result;
      
      var addressee = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Letter.Addressee);
      var employees =  Company.PublicFunctions.Employee.Remote.GetEmployeesByName(addressee);
      result.Employee = employees.FirstOrDefault();
      result.IsTrusted = (employees.Count() == 1) ? DocflowPublicFunctions.IsTrustedField(fact, FieldNames.Letter.Addressee) : false;
      return result;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Поиск сотрудника по ФИО из фактов и НОР.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="predictedClass">НОР.</param>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Сотрудник.</returns>
    public virtual List<IEmployee> GetOurEmployeeByFact(Sungero.Docflow.Structures.Module.IFact fact,
                                                        IBusinessUnit businessUnit,
                                                        string predictedClass)
    {
      if (fact == null)
        return new List<IEmployee>();
      
      var fullName = GetFullNameByFact(predictedClass, fact);
      var employees = Company.PublicFunctions.Employee.Remote.GetEmployeesByName(fullName);
      
      if (businessUnit != null)
        employees = employees.Where(e => e.Department.BusinessUnit.Equals(businessUnit)).ToList();
      
      return employees;
    }

    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить полное ФИО из частей имени содержащихся в факте.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</returns>
    public static string GetFullNameByFact(string predictedClass, Sungero.Docflow.Structures.Module.IFact fact)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return string.Empty;
      
      var сontactFactNames = GetContactFactNames(predictedClass);
      
      var surname = DocflowPublicFunctions.GetFieldValue(fact, сontactFactNames.SurnameField);
      var name = DocflowPublicFunctions.GetFieldValue(fact, сontactFactNames.NameField);
      var patronymic = DocflowPublicFunctions.GetFieldValue(fact, сontactFactNames.PatronymicField);
      
      return GetFullNameByFact(surname, name, patronymic);
    }    
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
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

    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку    
    /// <summary>
    /// Получить сокращенное ФИО из факта.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О.".</returns>
    public virtual string GetShortNameByFact(string predictedClass, Sungero.Docflow.Structures.Module.IFact fact)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return string.Empty;
      
      var ContactFactNames = GetContactFactNames(predictedClass);
      
      var surname = DocflowPublicFunctions.GetFieldValue(fact, ContactFactNames.SurnameField);
      var name = DocflowPublicFunctions.GetFieldValue(fact, ContactFactNames.NameField);
      var patronymic = DocflowPublicFunctions.GetFieldValue(fact, ContactFactNames.PatronymicField);
      
      // Если 2 из 3 полей пустые, то скорее всего сервис Ario вернул Фамилию И.О. в третье поле.
      if (string.IsNullOrWhiteSpace(surname) && string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(patronymic))
        return patronymic;
      
      if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(patronymic) && !string.IsNullOrWhiteSpace(surname))
        return surname;
      
      if (string.IsNullOrWhiteSpace(surname) && string.IsNullOrWhiteSpace(patronymic) && !string.IsNullOrWhiteSpace(name))
        return name;
      
      return Parties.PublicFunctions.Person.GetSurnameAndInitialsInTenantCulture(name, patronymic, surname);
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить контактные лица по данным из факта.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент - владелец контактного лица.</param>
    /// <returns>Список контактных лиц.</returns>
    public virtual IQueryable<IContact> GetContactsByFact(string predictedClass, Sungero.Docflow.Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return new List<IContact>().AsQueryable();
      
      var fullName = GetFullNameByFact(predictedClass, fact);
      var shortName = GetShortNameByFact(predictedClass, fact);
      
      var nonBreakingSpace = new string('\u00A0', 1);
      var space = new string('\u0020', 1);
      
      // Короткое имя персоны содержит неразрывный пробел.
      shortName = shortName.Replace(". ", ".").Replace(space, nonBreakingSpace);
      
      var contacts = Parties.PublicFunctions.Contact.GetContactsByName(fullName, shortName, counterparty);
      
      if (!contacts.Any())
        contacts = Parties.PublicFunctions.Contact.GetContactsByName(shortName, shortName, counterparty);
      
      contacts = contacts.Where(x => x.Status == CoreEntities.DatabookEntry.Status.Active);
      
      return contacts;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить контактное лицо по данным из факта и контрагента.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Структура, содержащая факт, контактное лицо и признак доверия.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetContactByFact(Docflow.Structures.Module.IFact fact,
                                                                             string propertyName,
                                                                             ICounterparty counterparty,
                                                                             string counterpartyPropertyName,
                                                                             string predictedClass)
    {
      var signedBy = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(null, Sungero.Parties.Contacts.Null, fact, false);
      
      if ((fact == null) || (predictedClass == string.Empty))
        return signedBy;

      // Если для свойства propertyName по факту существует верифицированное ранее значение, то вернуть его.
      signedBy = GetContactByVerifiedData(fact, propertyName, counterparty, counterpartyPropertyName);
      if (signedBy.Contact != null)
        return signedBy;
      
      var filteredContacts = GetContactsByFact(predictedClass, fact, counterparty);
      if (!filteredContacts.Any())
        return signedBy;
      
      var contactFullName = GetFullNameByFact(predictedClass, fact);
      signedBy.Contact = filteredContacts.FirstOrDefault();
      
      // Если контакт подобран по короткому имени персоны, то значение заведомо недоверенное.
      signedBy.IsTrusted = filteredContacts.Count() == 1 &&
        string.Equals(signedBy.Contact.Name, contactFullName, StringComparison.InvariantCultureIgnoreCase);
      
      return signedBy;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить контактное лицо контрагента из верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetContactByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact,
                                                                                     string propertyName,
                                                                                     ICounterparty counterparty,
                                                                                     string counterpartyPropertyName)
    {
      var result = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(null, Contacts.Null, fact, false);
      
      var contactField = counterparty != null ?
        GetFieldByVerifiedData(fact, propertyName, counterparty.Id.ToString(), counterpartyPropertyName) :
        GetFieldByVerifiedData(fact, propertyName);
      if (contactField == null)
        return result;
      int contactId;
      if (!int.TryParse(contactField.VerifiedValue, out contactId))
        return result;
      
      var filteredContact = Contacts.GetAll(x => x.Id == contactId)
        .Where(x => x.Status == CoreEntities.DatabookEntry.Status.Active).FirstOrDefault();;
      if (filteredContact != null)
      {
        result.Contact = filteredContact;
        result.IsTrusted = contactField.IsTrusted == true;
      }
      return result;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника, вход. письма в коробку
    /// <summary>
    /// Получить название факта подписанта с названиями полей ФИО.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Наименование факта, наименования полей с ФИО подписанта.</returns>
    public static Docflow.Structures.Module.IContactFactNames GetContactFactNames(string predictedClass)
    {
      var contactFactNames = Sungero.Docflow.Structures.Module.ContactFactNames.Create();
      
      // Если пришло входящее письмо
      if (predictedClass == ArioClassNames.Letter)
      {
        contactFactNames.Fact = FactNames.LetterPerson;
        contactFactNames.SurnameField = FieldNames.LetterPerson.Surname;
        contactFactNames.NameField = FieldNames.LetterPerson.Name;
        contactFactNames.PatronymicField = FieldNames.LetterPerson.Patrn;
      }
      
      // Если пришел договорной документ
      if ((predictedClass == ArioClassNames.Contract) ||
          (predictedClass == ArioClassNames.SupAgreement))
      {
        contactFactNames.Fact = FactNames.Counterparty;
        contactFactNames.SurnameField = FieldNames.Counterparty.SignatorySurname;
        contactFactNames.NameField = FieldNames.Counterparty.SignatoryName;
        contactFactNames.PatronymicField = FieldNames.Counterparty.SignatoryPatrn;
      }
      return contactFactNames;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Получить подписанта нашей стороны из фактов и НОР для договорного документа.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <param name="document">Договорной документ.</param>
    /// <param name="сounterparty">Структура с НОР, КА, фактом и признаком доверия.</param>
    /// <param name="isOurSignatory">Признак поиска нашего подписанта (true) или подписанта КА (false).</param>
    /// <returns>Сотрудник.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetContractualDocumentSignatory(Docflow.Structures.Module.IRecognitionResult recognitionResult,
                                                                                            IContractualDocumentBase document,
                                                                                            Docflow.Structures.Module.ICounterpartyFactMatching organizationFactMatching,
                                                                                            bool isOurSignatory = false)
    {
      var props = document.Info.Properties;
      var signatoryFacts = GetFacts(recognitionResult.Facts, FactNames.Counterparty);
      var signedBy = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(null, null, null, false);
      var signatoryFieldNames = this.GetSignatoryFieldNames();
      
      if (!signatoryFacts.Any())
        return signedBy;
      
      if (organizationFactMatching != null)
      {
        var organizationFact = organizationFactMatching.Fact;
        if (organizationFact != null)
        {
          signedBy.Fact = organizationFact;
          var isOrganizationFactWithSignatory = GetFields(organizationFact, signatoryFieldNames).Any();
          
          if (isOrganizationFactWithSignatory)
            return isOurSignatory
              ? GetContractualDocumentOurSignatoryByFact(organizationFact, document, recognitionResult.PredictedClass)
              : GetContactByFact(organizationFact, props.CounterpartySignatory.Name, document.Counterparty,
                                 props.Counterparty.Name, recognitionResult.PredictedClass);
        }
        
        if (organizationFactMatching.BusinessUnit != null || organizationFactMatching.Counterparty != null)
        {
          var organizationName = isOurSignatory ? organizationFactMatching.BusinessUnit.Name : organizationFactMatching.Counterparty.Name;
          
          // Ожидаемое наименование НОР в формате {Название}, {ОПФ}.
          var organizationNameAndLegalForm = organizationName.Split(new string[] { ", " }, StringSplitOptions.None);

          signatoryFacts = signatoryFacts
            .Where(f => f.Fields.Any(fl => fl.Name == FieldNames.Counterparty.Name &&
                                     fl.Value.Equals(organizationNameAndLegalForm[0], StringComparison.InvariantCultureIgnoreCase)))
            .Where(f => f.Fields.Any(fl => fl.Name == FieldNames.Counterparty.LegalForm &&
                                     fl.Value.Equals(organizationNameAndLegalForm[1], StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
      }
      
      signatoryFacts = signatoryFacts
        .Where(f => GetFields(f, signatoryFieldNames).Any()).ToList();
      
      var organizationSignatories = new List<Sungero.Docflow.Structures.Module.ISignatoryFactMatching>();
      foreach (var fact in signatoryFacts)
      {
        Sungero.Docflow.Structures.Module.ISignatoryFactMatching signatory = null;
        if (isOurSignatory)
        {
          signatory = GetContractualDocumentOurSignatoryByFact(fact, document, recognitionResult.PredictedClass);
          if (signatory.Employee != null)
            organizationSignatories.Add(signatory);
        }
        else
        {
          signatory = GetContactByFact(fact, props.CounterpartySignatory.Name, document.Counterparty,
                                       props.Counterparty.Name, recognitionResult.PredictedClass);
          if (signatory.Contact != null)
            organizationSignatories.Add(signatory);
        }
      }
      if (organizationSignatories.Count > 0)
        signedBy = organizationSignatories.FirstOrDefault();
      
      return signedBy;
    }
    
    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Получить подписанта нашей стороны из верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">НОР.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства НОР.</param>
    /// <returns>Сотрудник.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetEmployeeByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact,
                                                                                      string propertyName,
                                                                                      IBusinessUnit businessUnit,
                                                                                      string businessUnitPropertyName)
    {
      var result = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(null, null, fact, false);
      
      var employeeField = businessUnit != null
        ? GetFieldByVerifiedData(fact, propertyName, businessUnit.Id.ToString(), businessUnitPropertyName)
        : GetFieldByVerifiedData(fact, propertyName);
      
      if (employeeField == null)
        return result;
      int employeeId;
      if (!int.TryParse(employeeField.VerifiedValue, out employeeId))
        return result;
      
      var filteredEmployee = Employees.GetAll(x => x.Id == employeeId)
        .Where(x => x.Status == CoreEntities.DatabookEntry.Status.Active).FirstOrDefault();
      if (filteredEmployee != null)
      {
        result.Employee = filteredEmployee;
        result.IsTrusted = employeeField.IsTrusted == true;
      }
      return result;
    }
    
    /// <summary>
    /// Получить ведущие документы по номеру и дате из факта.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Список документов с подходящими номером и датой.</returns>
    public virtual IQueryable<Sungero.Contracts.IContractualDocument> GetLeadingDocuments(Sungero.Docflow.Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return new List<Sungero.Contracts.IContractualDocument>().AsQueryable();
      
      var docDate = DocflowPublicFunctions.GetFieldDateTimeValue(fact, FieldNames.FinancialDocument.DocumentBaseDate);
      var number = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseNumber);
      
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
    /// <param name="leadingDocPropertyName">Имя связанного свойства.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="counterpartyPropertyName">Имя свойства, связанного с контрагентом.</param>
    /// <returns>Структура, содержащая ведущий документ, факт и признак доверия.</returns>
    public virtual IContractFactMatching GetLeadingDocument(Sungero.Docflow.Structures.Module.IFact fact, string leadingDocPropertyName,
                                                            ICounterparty counterparty, string counterpartyPropertyName)
    {
      var result = Structures.Module.ContractFactMatching.Create(Contracts.ContractualDocuments.Null, fact, false);
      if (fact == null)
        return result;
      
      if (!string.IsNullOrEmpty(leadingDocPropertyName) && counterparty != null)
      {
        result = GetContractByVerifiedData(fact, leadingDocPropertyName, counterparty.Id.ToString(), counterpartyPropertyName);
        if (result.Contract != null)
          return result;
      }
      var contracts = GetLeadingDocuments(fact, counterparty);
      result.Contract = contracts.FirstOrDefault();
      result.IsTrusted = (contracts.Count() == 1) ? DocflowPublicFunctions.IsTrustedField(fact, FieldNames.FinancialDocument.DocumentBaseNumber) : false;
      return result;
    }
    
    /// <summary>
    /// Получить ведущий документ из верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя свойства, связанного с контрагентом.</param>
    /// <returns></returns>
    public virtual Structures.Module.IContractFactMatching GetContractByVerifiedData(Sungero.Docflow.Structures.Module.IFact fact,
                                                                                     string propertyName,
                                                                                     string counterpartyPropertyValue,
                                                                                     string counterpartyPropertyName)
    {
      var result = Structures.Module.ContractFactMatching.Create(Contracts.ContractualDocuments.Null, fact, false);
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
    
    /// TODO Suleymanov: Удалить после переноса договора, допника в коробку
    /// <summary>
    /// Получить подписанта нашей стороны для договорного документа по факту.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="document">Документ.</param>
    /// <param name="predictedClass">Результат обработки договора в Ario.</param>
    /// <returns>Структура, содержащая сотрудника, факт и признак доверия.</returns>
    public virtual Docflow.Structures.Module.ISignatoryFactMatching GetContractualDocumentOurSignatoryByFact(Sungero.Docflow.Structures.Module.IFact fact,
                                                                                   IContractualDocumentBase document,
                                                                                   string predictedClass)
    {
      var signedBy = Sungero.Docflow.Structures.Module.SignatoryFactMatching.Create(null, null, fact, false);
      var businessUnit = document.BusinessUnit;
      
      if (fact == null || string.IsNullOrWhiteSpace(predictedClass))
        return signedBy;

      // Если для свойства Подписал по факту существует верифицированное ранее значение, то вернуть его.
      signedBy = GetEmployeeByVerifiedData(fact,
                                           document.Info.Properties.OurSignatory.Name,
                                           businessUnit,
                                           document.Info.Properties.BusinessUnit.Name);
      if (signedBy.Employee != null)
        return signedBy;
      
      var filteredEmloyees = GetOurEmployeeByFact(fact, businessUnit, predictedClass);
      if (!filteredEmloyees.Any())
        return signedBy;
      
      var fullName = GetFullNameByFact(predictedClass, fact);
      signedBy.Employee = filteredEmloyees.FirstOrDefault();
      
      // Если сотрудник подобран по короткому имени персоны, то значение заведомо недоверенное.
      signedBy.IsTrusted = filteredEmloyees.Count() == 1 &&
        string.Equals(signedBy.Employee.Name, fullName, StringComparison.InvariantCultureIgnoreCase);
      
      return signedBy;
    }

    /// TODO Suleymanov: Удалить после переноса договора и допника в коробку
    /// <summary>
    /// Получить наименования полей для подписывающего.
    /// </summary>
    /// <returns>Наимнования полей для подписывающего.</returns>
    public virtual List<string> GetSignatoryFieldNames()
    {
      return new List<string>
      {
        FieldNames.Counterparty.SignatorySurname,
        FieldNames.Counterparty.SignatoryName,
        FieldNames.Counterparty.SignatoryPatrn
      };
    }
    
    /// TODO Suleymanov: Удалить после переноса договора и допника в коробку
    /// <summary>
    /// Получить наименования полей для контрагента.
    /// </summary>
    /// <returns>Наименования полей для контрагента.</returns>
    public virtual List<string> GetCounterpartyFieldNames()
    {
      return new List<string>
      {
        FieldNames.Counterparty.Name,
        FieldNames.Counterparty.LegalForm,
        FieldNames.Counterparty.CounterpartyType,
        FieldNames.Counterparty.TIN,
        FieldNames.Counterparty.TinIsValid,
        FieldNames.Counterparty.TRRC
      };
    }
    
    #endregion
    
    #region Штрихкоды
    
    /// <summary>
    /// Поиск Id документа по штрихкодам.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Список распознанных Id документа.</returns>
    /// <remarks>
    /// Поиск штрихкодов осуществляется только на первой странице документа.
    /// Формат штрихкода: Code128.
    /// </remarks>
    public virtual List<int> SearchDocumentBarcodeIds(System.IO.Stream document)
    {
      var result = new List<int>();
      try
      {
        var barcodeReader = new AsposeExtensions.BarcodeReader();
        var barcodeList = barcodeReader.Extract(document, Aspose.BarCode.BarCodeRecognition.DecodeType.Code128);
        if (!barcodeList.Any())
          return result;
        
        var tenantId = Docflow.PublicFunctions.Module.Remote.GetCurrentTenantId();
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