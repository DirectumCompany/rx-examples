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
using LetterPersonTypes = Sungero.RecordManagement.PublicConstants.IncomingLetter.LetterPersonTypes;
using CounterpartyTypes = Sungero.Docflow.Constants.Module.CounterpartyTypes;
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
        var arioUrl = Docflow.PublicFunctions.SmartProcessingSetting.GetArioUrl();
        var document = OfficialDocuments.Null;
        using (var body = Commons.PublicFunctions.Module.GetDocumentBody(arioUrl, recognitionResult.BodyGuid))
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
                Docflow.PublicFunctions.OfficialDocument.CreateVersion(document, recognitionResult, Sungero.Docflow.OfficialDocuments.Resources.VersionCreatedByCaptureService);
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
      return documentParams.ContainsKey(Sungero.Docflow.Constants.OfficialDocument.DocumentNumberingBySmartCaptureResultParamName);
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
      
      Docflow.PublicFunctions.OfficialDocument.CreateVersion(document, recognitionResult, string.Empty);
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
      Docflow.PublicFunctions.OfficialDocument.FillDeliveryMethod(document, true);
      
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
              version.AssociatedApplication = Content.AssociatedApplications.GetByExtension(Docflow.PublicConstants.OfficialDocument.PdfExtension);
            }
          }
        }
        
        // Если тело письма не удалось преобразовать в pdf или расширение не html, то в тело пишем исходный файл.
        if (version.Body.Size == 0)
        {
          version.Body.Write(body);
          version.AssociatedApplication = Docflow.PublicFunctions.Module.GetAssociatedApplicationByFileName(bodyDto.Path);
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
      Docflow.PublicFunctions.OfficialDocument.FillDeliveryMethod(document, sendedByEmail);
      document.Save();
      
      var application = Docflow.PublicFunctions.Module.GetAssociatedApplicationByFileName(file.Path);
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
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, null);
      
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
      var document = FinancialArchive.ContractStatements.Create();
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, null);
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
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Supplier);
      counterpartyTypes.Add(CounterpartyTypes.Payer);
      counterpartyTypes.Add(CounterpartyTypes.Shipper);
      counterpartyTypes.Add(CounterpartyTypes.Consignee);
      
      var factMatches = Docflow.PublicFunctions.Module.MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Supplier).FirstOrDefault() ??
        factMatches.Where(m => m.Type == CounterpartyTypes.Shipper).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Payer).FirstOrDefault() ??
        factMatches.Where(m => m.Type == CounterpartyTypes.Consignee).FirstOrDefault();
      var documentParties = Docflow.PublicFunctions.Module.GetDocumentParties(buyer, seller, responsible);
      
      Docflow.PublicFunctions.AccountingDocumentBase.FillDocumentParties(document, documentParties);
      Docflow.PublicFunctions.AccountingDocumentBase.LinkDocumentParties(document, recognitionResult, documentParties);
      
      // Дата, номер и регистрация.
      Docflow.PublicFunctions.OfficialDocument.NumberDocument(document, recognitionResult, FactNames.FinancialDocument, null);
      
      // Документ-основание.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contractualDocuments = Docflow.PublicFunctions.Module.GetLeadingDocuments(leadingDocFact, document.Counterparty);
      document.LeadingDocument = contractualDocuments.FirstOrDefault();
      var isTrusted = (contractualDocuments.Count() == 1) ? Docflow.PublicFunctions.Module.IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName) : false;
      Docflow.PublicFunctions.Module.LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, documentParties);
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
      
      var factMatches = Docflow.PublicFunctions.Module.MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
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
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, documentParties);
      
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
      var document = Sungero.FinancialArchive.UniversalTransferDocuments.Create();
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, null);
      
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
      var isDateValid = DocflowPublicFunctions.IsDateValid(date);
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
      
      // НОР и КА.
      var counterpartyTypes = new List<string>();
      counterpartyTypes.Add(CounterpartyTypes.Seller);
      counterpartyTypes.Add(CounterpartyTypes.Buyer);
      counterpartyTypes.Add(string.Empty);
      var factMatches = Docflow.PublicFunctions.Module.MatchFactsWithBusinessUnitsAndCounterparties(facts, counterpartyTypes);
      var seller = factMatches.Where(m => m.Type == CounterpartyTypes.Seller).FirstOrDefault();
      var buyer = factMatches.Where(m => m.Type == CounterpartyTypes.Buyer).FirstOrDefault();
      var nonType = factMatches.Where(m => m.Type == string.Empty).ToList();
      var documentParties = Docflow.PublicFunctions.Module.GetDocumentParties(buyer, seller, nonType, responsible);
      // TODO rassokhina: FillDocumentParties() и LinkDocumentParties() везде используються в паре подряд. Можно объединить.
      Docflow.PublicFunctions.AccountingDocumentBase.FillDocumentParties(document, documentParties);
      Docflow.PublicFunctions.AccountingDocumentBase.LinkDocumentParties(document, recognitionResult, documentParties);
      
      // Договор.
      var contractFact = Docflow.PublicFunctions.Module.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contract = Docflow.PublicFunctions.Module.GetLeadingDocument(contractFact, document.Info.Properties.Contract.Name, document.Counterparty, document.Info.Properties.Counterparty.Name);
      document.Contract = contract.Contract;
      Docflow.PublicFunctions.Module.LinkFactAndProperty(recognitionResult, contractFact, null, props.Contract.Name, document.Contract, contract.IsTrusted);
      
      // Дата.
      var dateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date).FirstOrDefault();
      var date = DocflowPublicFunctions.GetFieldDateTimeValue(dateFact, FieldNames.FinancialDocument.Date);
      var isDateValid = DocflowPublicFunctions.IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && Docflow.PublicFunctions.Module.IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Date = date;
      Docflow.PublicFunctions.Module.LinkFactAndProperty(recognitionResult, dateFact, FieldNames.FinancialDocument.Date, props.Date.Name, date, isTrustedDate);
      
      // Номер.
      var numberFact = Docflow.PublicFunctions.Module.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Number).FirstOrDefault();
      var number = Docflow.PublicFunctions.Module.GetFieldValue(numberFact, FieldNames.FinancialDocument.Number);
      Nullable<bool> isTrustedNumber = null;
      if (number.Length > document.Info.Properties.Number.Length)
      {
        number = number.Substring(0, document.Info.Properties.Number.Length);
        isTrustedNumber = false;
      }
      document.Number = number;
      Docflow.PublicFunctions.Module.LinkFactAndProperty(recognitionResult, numberFact, FieldNames.FinancialDocument.Number, props.Number.Name, document.Number, isTrustedNumber);
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, documentParties);
      
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
      
      Docflow.PublicFunctions.OfficialDocument.FillProperties(document, recognitionResult, responsible, null);
      
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
      var isDateValid = DocflowPublicFunctions.IsDateValid(date);
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
  
    #endregion
    
    #region Поиск контрагента/НОР
    
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
          
    #endregion
    
    #region Работа с полями/фактами    
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