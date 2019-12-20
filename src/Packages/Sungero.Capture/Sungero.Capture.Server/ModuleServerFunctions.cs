﻿using System;
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
using Sungero.Parties;
using Sungero.RecordManagement;
using Sungero.Workflow;
using FieldNames = Sungero.Capture.Constants.Module.FieldNames;
using FactNames = Sungero.Capture.Constants.Module.FactNames;
using LetterPersonTypes = Sungero.Capture.Constants.Module.LetterPersonTypes;
using CounterpartyTypes = Sungero.Capture.Constants.Module.CounterpartyTypes;
using ArioClassNames = Sungero.Capture.Constants.Module.ArioClassNames;
using MessageTypes = Sungero.Capture.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;
using SettingsValidationMessageStructure = Sungero.Capture.Structures.SmartProcessingSetting.SettingsValidationMessage;

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
    /// <param name="lowerConfidenceLimit">Нижняя граница доверия извлеченным фактам.</param>
    /// <param name="upperConfidenceLimit">Верхняя граница доверия извлеченным фактам.</param>
    /// <param name="firstPageClassifierName">Имя классификатора первых страниц.</param>
    /// <param name="typeClassifierName">Имя классификатора по типам документов.</param>
    /// <returns>Ошибка, если заполнить настройки не удалось.</returns>
    [Remote]
    public static Structures.SmartProcessingSetting.SettingsValidationMessage SetCaptureMainSettings(string arioUrl,
                                                                                                     string lowerConfidenceLimit,
                                                                                                     string upperConfidenceLimit,
                                                                                                     string firstPageClassifierName,
                                                                                                     string typeClassifierName)
    {
      var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
      
      // Адрес.
      smartProcessingSettings.ArioUrl = arioUrl;
      var arioUrlValidationMessage = Functions.SmartProcessingSetting.ValidateArioUrl(smartProcessingSettings);
      if (arioUrlValidationMessage != null)
        return arioUrlValidationMessage;
      
      // Границы.
      int lowerLimit;
      int upperLimit;
      smartProcessingSettings.LowerConfidenceLimit = int.TryParse(lowerConfidenceLimit, out lowerLimit) ? lowerLimit : -1;
      smartProcessingSettings.UpperConfidenceLimit = int.TryParse(upperConfidenceLimit, out upperLimit) ? upperLimit : -1;
      var confidenceLimitsValidationMessage = Functions.SmartProcessingSetting.ValidateConfidenceLimits(smartProcessingSettings);
      if (confidenceLimitsValidationMessage != null)
        return confidenceLimitsValidationMessage;
      
      // Классификаторы.
      var classifiers = Functions.SmartProcessingSetting.GetArioClassifiers(smartProcessingSettings);
      var firstPageClassifier = classifiers.Where(a => a.Name == firstPageClassifierName).FirstOrDefault();
      var typeClassifier = classifiers.Where(a => a.Name == typeClassifierName).FirstOrDefault();
      
      if (firstPageClassifier == null || typeClassifier == null)
        return SettingsValidationMessageStructure.Create(MessageTypes.Error,
                                                         SmartProcessingSettings.Resources.SetCorrectClassifiers);
      
      smartProcessingSettings.FirstPageClassifierName = firstPageClassifier.Name;
      smartProcessingSettings.FirstPageClassifierId = firstPageClassifier.Id;
      smartProcessingSettings.TypeClassifierName = typeClassifier.Name;
      smartProcessingSettings.TypeClassifierId = typeClassifier.Id;
      
      smartProcessingSettings.Save();
      return null;
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
    public virtual List<IOfficialDocument> CreateDocumentsByRecognitionResults(string recognitionResultsJson, Structures.Module.IFileDto originalFile,
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
        recognitionResult.Info.DocumentId = document.Id;
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
      return documentParams.ContainsKey(Constants.Module.DocumentNumberingBySmartCaptureResultParamName);
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
    public virtual List<Structures.Module.IRecognitionResult> GetRecognitionResults(string jsonClassificationResults,
                                                                                    Structures.Module.IFileDto file,
                                                                                    bool sendedByEmail)
    {
      var recognitionResults = new List<IRecognitionResult>();
      if (string.IsNullOrWhiteSpace(jsonClassificationResults))
        return recognitionResults;
      
      var packageProcessResults = ArioExtensions.ArioConnector.DeserializeClassifyAndExtractFactsResultString(jsonClassificationResults);
      foreach (var packageProcessResult in packageProcessResults)
      {
        // Класс и гуид тела документа.
        var recognitionResult = RecognitionResult.Create();
        var clsResult = packageProcessResult.ClassificationResult;
        recognitionResult.ClassificationResultId = clsResult.Id;
        recognitionResult.BodyGuid = packageProcessResult.Guid;
        recognitionResult.PredictedClass = clsResult.PredictedClass != null ? clsResult.PredictedClass.Name : string.Empty;
        recognitionResult.Message = packageProcessResult.Message;
        recognitionResult.File = file;
        recognitionResult.SendedByEmail = sendedByEmail;
        var docInfo = DocumentRecognitionInfos.Create();
        docInfo.Name = recognitionResult.PredictedClass;
        docInfo.RecognizedClass = recognitionResult.PredictedClass;
        if (clsResult.PredictedProbability != null)
          docInfo.ClassProbability = (double)(clsResult.PredictedProbability);
        
        // Факты и поля фактов.
        recognitionResult.Facts = new List<IFact>();
        var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
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
              .Select(f => FactField.Create(f.Id, f.Name, f.Value, f.Probability));
            recognitionResult.Facts.Add(Fact.Create(fact.Id, fact.Name, fields.ToList()));
            
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
    public virtual IOfficialDocument CreateDocumentByRecognitionResult(Structures.Module.IRecognitionResult recognitionResult,
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
      else if (predictedClass == ArioClassNames.SupAgreement)
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
    /// Определить ведущий документ распознанного комплекта.
    /// </summary>
    /// <param name="package">Комплект документов.</param>
    /// <returns>Ведущий документ.</returns>
    public virtual IOfficialDocument GetLeadingDocument(List<IOfficialDocument> package)
    {
      var leadingDocument = package.FirstOrDefault();
      var isMockMode = Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.CaptureMockModeKey) != null;
      
      var incLetter = isMockMode
        ? package.Where(d => MockIncomingLetters.Is(d)).FirstOrDefault()
        : package.Where(d => IncomingLetters.Is(d)).FirstOrDefault();
      if (incLetter != null)
        return incLetter;
      
      var contract = isMockMode
        ? package.Where(d => MockContracts.Is(d)).FirstOrDefault()
        : package.Where(d => Contracts.Contracts.Is(d)).FirstOrDefault();
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
                                                                                     Structures.Module.IFileDto bodyDto,
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
    public virtual Sungero.Docflow.ISimpleDocument CreateSimpleDocumentFromFile(Structures.Module.IFileDto file,
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
    public virtual Docflow.IOfficialDocument CreateIncomingLetter(Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
    {
      var document = Sungero.RecordManagement.IncomingLetters.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      var subjectFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Subject).FirstOrDefault();
      var subject = GetFieldValue(subjectFact, FieldNames.Letter.Subject);
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        LinkFactAndProperty(recognitionResult, subjectFact, FieldNames.Letter.Subject, props.Subject.Name, document.Subject);
      }
      
      // Адресат.
      var addresseeFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Addressee).FirstOrDefault();
      var addressee = GetAdresseeByFact(addresseeFact);
      document.Addressee = addressee.Employee;
      LinkFactAndProperty(recognitionResult, addresseeFact, FieldNames.Letter.Addressee, props.Addressee.Name, document.Addressee, addressee.IsTrusted);
      
      // Заполнить данные корреспондента.
      var correspondent = GetCounterparty(recognitionResult, props.Correspondent.Name);
      if (correspondent != null)
      {
        document.Correspondent = correspondent.Counterparty;
        LinkFactAndProperty(recognitionResult, correspondent.Fact, null, props.Correspondent.Name, document.Correspondent, correspondent.IsTrusted);
      }
      
      // Дата, номер.
      var dateFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Date).FirstOrDefault();
      var date = GetFieldDateTimeValue(dateFact, FieldNames.Document.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Dated = date;
      LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Letter.Date, props.Dated.Name, date, isTrustedDate);
      
      var numberFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Number).FirstOrDefault();
      document.InNumber = GetFieldValue(numberFact, FieldNames.Letter.Number);
      LinkFactAndProperty(recognitionResult, numberFact, FieldNames.Letter.Number, props.InNumber.Name, document.InNumber);
      
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
      LinkFactAndProperty(recognitionResult, businessUnitWithFact.Fact, null, props.BusinessUnit.Name,
                          document.BusinessUnit, businessUnitWithFact.IsTrusted);
      
      document.Department = document.Addressee != null
        ? Company.PublicFunctions.Department.GetDepartment(document.Addressee)
        : Company.PublicFunctions.Department.GetDepartment(responsible);
      
      // Заполнить подписанта.
      var personFacts = GetOrderedFacts(facts, FactNames.LetterPerson, FieldNames.LetterPerson.Surname);
      var signatoryFact = personFacts.Where(x => GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Signatory).FirstOrDefault();
      var signedBy = GetContactByFact(signatoryFact, props.SignedBy.Name,
                                      document.Correspondent, props.Correspondent.Name, recognitionResult.PredictedClass);
      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && signedBy.Contact != null)
      {
        // Если контрагент определился из подписанта, то результату не доверяем.
        LinkFactAndProperty(recognitionResult, null, null, props.Correspondent.Name, signedBy.Contact.Company, false);
      }
      document.SignedBy = signedBy.Contact;
      var isTrustedSignatory = signedBy.IsTrusted ? IsTrustedField(signatoryFact, FieldNames.LetterPerson.Type) : signedBy.IsTrusted;
      LinkFactAndProperty(recognitionResult, signatoryFact, null, props.SignedBy.Name, document.SignedBy, isTrustedSignatory);
      
      // Заполнить контакт.
      var responsibleFact = personFacts.Where(x => GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Responsible).FirstOrDefault();
      var contact = GetContactByFact(responsibleFact, props.Contact.Name,
                                     document.Correspondent, props.Correspondent.Name, recognitionResult.PredictedClass);
      
      // При заполнении полей подписал и контакт, если контрагент не заполнен, он подставляется из подписанта/контакта.
      if (document.Correspondent == null && contact.Contact != null)
      {
        // Если контрагент определился из контакта, то результату не доверяем.
        LinkFactAndProperty(recognitionResult, null, null, props.Correspondent.Name, contact.Contact.Company, false);
      }
      document.Contact = contact.Contact;
      var isTrustedContact = contact.IsTrusted && IsTrustedField(responsibleFact, FieldNames.LetterPerson.Type);
      LinkFactAndProperty(recognitionResult, responsibleFact, null, props.Contact.Name, document.Contact, isTrustedContact);
      
      return document;
    }
    
    /// <summary>
    /// Создать входящее письмо (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingLetter(Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockIncomingLetters.Create();
      var props = document.Info.Properties;
      var facts = recognitionResult.Facts;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      
      // Заполнить дату и номер письма со стороны корреспондента.
      var dateFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Date).FirstOrDefault();
      var numberFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Number).FirstOrDefault();
      document.InNumber = GetFieldValue(numberFact, FieldNames.Letter.Number);
      document.Dated = Functions.Module.GetShortDate(GetFieldValue(dateFact, FieldNames.Letter.Date));
      LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Letter.Date, props.Dated.Name, document.Dated);
      LinkFactAndProperty(recognitionResult, numberFact, FieldNames.Letter.Number, props.InNumber.Name, document.InNumber);
      
      // Заполнить данные корреспондента.
      var correspondentNameFacts = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.CorrespondentName);
      if (correspondentNameFacts.Count() > 0)
      {
        var fact = correspondentNameFacts.First();
        document.Correspondent = GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Letter.CorrespondentName, props.Correspondent.Name, document.Correspondent);
      }
      if (correspondentNameFacts.Count() > 1)
      {
        var fact = correspondentNameFacts.Last();
        document.Recipient = GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Letter.CorrespondentName, props.Recipient.Name, document.Recipient);
      }
      
      // Заполнить ИНН/КПП для КА и НОР.
      var tinTrrcFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.CorrespondentTin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.CorrespondentTrrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.CorrespondentTin.Name, document.CorrespondentTin);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.CorrespondentTrrc.Name, document.CorrespondentTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.RecipientTin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.RecipientTrrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.RecipientTin.Name, document.RecipientTin);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.RecipientTrrc.Name, document.RecipientTrrc);
      }
      
      // В ответ на.
      var responseToNumberFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToNumber).FirstOrDefault();
      var responseToNumber = GetFieldValue(responseToNumberFact, FieldNames.Letter.ResponseToNumber);
      var responseToDateFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate).FirstOrDefault();
      var responseToDate = Functions.Module.GetShortDate(GetFieldValue(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? responseToNumber
        : string.Format("{0} {1} {2}", responseToNumber, Sungero.Docflow.Resources.From, responseToDate);
      LinkFactAndProperty(recognitionResult, responseToNumberFact, FieldNames.Letter.ResponseToNumber, props.InResponseTo.Name, document.InResponseTo);
      LinkFactAndProperty(recognitionResult, responseToDateFact, FieldNames.Letter.ResponseToDate, props.InResponseTo.Name, document.InResponseTo);
      
      // Заполнить подписанта.
      var personFacts = GetOrderedFacts(facts, FactNames.LetterPerson, FieldNames.LetterPerson.Surname);
      var predictedClass = recognitionResult.PredictedClass;
      if (document.Signatory == null)
      {
        var signatoryFact = personFacts.Where(x => GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Signatory).FirstOrDefault();
        document.Signatory = GetFullNameByFact(predictedClass, signatoryFact);
        var isTrusted = IsTrustedField(signatoryFact, FieldNames.LetterPerson.Type);
        LinkFactAndProperty(recognitionResult, signatoryFact, null, props.Signatory.Name, document.Signatory, isTrusted);
      }
      
      // Заполнить контакт.
      if (document.Contact == null)
      {
        var responsibleFact = personFacts.Where(x => GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Responsible).FirstOrDefault();
        document.Contact = GetFullNameByFact(predictedClass, responsibleFact);
        var isTrusted = IsTrustedField(responsibleFact, FieldNames.LetterPerson.Type);
        LinkFactAndProperty(recognitionResult, responsibleFact, null, props.Contact.Name, document.Contact, isTrusted);
      }
      
      // Заполнить данные нашей стороны.
      var addresseeFacts = GetFacts(facts, FactNames.Letter, FieldNames.Letter.Addressee);
      foreach (var fact in addresseeFacts)
      {
        var addressee = GetFieldValue(fact, FieldNames.Letter.Addressee);
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? addressee : string.Format("{0}; {1}", document.Addressees, addressee);
      }
      foreach (var fact in addresseeFacts)
        LinkFactAndProperty(recognitionResult, fact, null, props.Addressees.Name, document.Addressees, true);
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subjectFact = GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Subject).FirstOrDefault();
      var subject = GetFieldValue(subjectFact, FieldNames.Letter.Subject);
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        LinkFactAndProperty(recognitionResult, subjectFact, FieldNames.Letter.Subject, props.Subject.Name, document.Subject);
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
    public Docflow.IOfficialDocument CreateMockContractStatement(Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.LeadDoc = GetLeadingDocumentName(leadingDocFact);
      LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadDoc.Name, document.LeadDoc, true);
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.Document);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.CounterpartyName = seller.Name;
        document.CounterpartyTin = seller.Tin;
        document.CounterpartyTrrc = seller.Trrc;
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, seller.Tin);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, seller.Trrc);
      }
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BusinessUnitName = buyer.Name;
        document.BusinessUnitTin = buyer.Tin;
        document.BusinessUnitTrrc = buyer.Trrc;
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, buyer.Tin);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, buyer.Trrc);
      }
      
      // В актах могут прийти контрагенты без типа. Заполнить контрагентами без типа.
      if (seller == null || buyer == null)
      {
        var withoutTypeFacts = GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
          .Where(f => string.IsNullOrWhiteSpace(GetFieldValue(f, FieldNames.Counterparty.CounterpartyType)))
          .OrderByDescending(x => x.Fields.First(f => f.Name == FieldNames.Counterparty.Name).Probability);
        foreach (var fact in withoutTypeFacts)
        {
          var name = GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
          
          var tin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
          var trrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
          var type = GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType);
          
          if (string.IsNullOrWhiteSpace(document.CounterpartyName))
          {
            document.CounterpartyName = name;
            document.CounterpartyTin = tin;
            document.CounterpartyTrrc = trrc;
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, tin);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BusinessUnitName))
          {
            document.BusinessUnitName = name;
            document.BusinessUnitTin = tin;
            document.BusinessUnitTrrc = trrc;
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, tin);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, trrc);
          }
        }
      }
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName = GetFieldValue(fact, FieldNames.Goods.UnitName);
        good.Count = GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                            good.Name, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                            good.UnitName, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                            good.Count, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                            good.Price, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                            good.VatAmount, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                            good.TotalAmount, collectionRecordId : good.Id);
      }
      return document;
    }
    
    /// <summary>
    /// Создать акт выполненных работ.
    /// </summary>
    /// <param name="сlassificationResult">Результат обработки акта выполненных работ в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Акт выполненных работ.</returns>
    public virtual Docflow.IOfficialDocument CreateContractStatement(Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
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
      FillAccountingDocumentParties(document, documentParties);
      LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognitionResult, FactNames.Document);
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var leadingDocument = GetLeadingDocument(leadingDocFact,
                                               document.Info.Properties.LeadingDocument.Name,
                                               document.Counterparty, document.Info.Properties.Counterparty.Name);
      document.LeadingDocument = leadingDocument.Contract;
      LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, leadingDocument.IsTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки накладной в Ario.</param>
    /// <returns>Накладная.</returns>
    public Docflow.IOfficialDocument CreateMockWaybill(Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockWaybills.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var shipper = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Shipper);
      if (shipper != null)
      {
        document.Shipper = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.Name, props.Shipper.Name, shipper.Name);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.LegalForm, props.Shipper.Name, shipper.Name);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.Consignee = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.Name, props.Consignee.Name, consignee.Name);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.LegalForm, props.Consignee.Name, consignee.Name);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var supplier = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Supplier);
      if (supplier != null)
      {
        document.Supplier = supplier.Name;
        document.SupplierTin = supplier.Tin;
        document.SupplierTrrc = supplier.Trrc;
        LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.Name, props.Supplier.Name, supplier.Name);
        LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.LegalForm, props.Supplier.Name, supplier.Name);
        LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.TIN, props.SupplierTin.Name, supplier.Tin);
        LinkFactAndProperty(recognitionResult, supplier.Fact, FieldNames.Counterparty.TRRC, props.SupplierTrrc.Name, supplier.Trrc);
      }
      
      var payer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Payer);
      if (payer != null)
      {
        document.Payer = payer.Name;
        document.PayerTin = payer.Tin;
        document.PayerTrrc = payer.Trrc;
        LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.Name, props.Payer.Name, payer.Name);
        LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.LegalForm, props.Payer.Name, payer.Name);
        LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.TIN, props.PayerTin.Name, payer.Tin);
        LinkFactAndProperty(recognitionResult, payer.Fact, FieldNames.Counterparty.TRRC, props.PayerTrrc.Name, payer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.FinancialDocument);
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName = GetFieldValue(fact, FieldNames.Goods.UnitName);
        
        good.Count = GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                            good.Name, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                            good.UnitName, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                            good.Count, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                            good.Price, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                            good.VatAmount, collectionRecordId : good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                            good.TotalAmount, collectionRecordId : good.Id);
      }
      
      return document;
    }
    
    /// <summary>
    /// Создать накладную.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки накладной в Ario.</param>
    /// <param name="responsible">Ответственный сотрудник.</param>
    /// <returns>Накладная.</returns>
    public virtual Docflow.IOfficialDocument CreateWaybill(Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
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
      
      FillAccountingDocumentParties(document, documentParties);
      LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognitionResult, FactNames.FinancialDocument);
      
      // Документ-основание.
      var leadingDocFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contractualDocuments = GetLeadingDocuments(leadingDocFact, document.Counterparty);
      document.LeadingDocument = contractualDocuments.FirstOrDefault();
      var isTrusted = (contractualDocuments.Count() == 1) ? IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName) : false;
      LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.LeadingDocument.Name, document.LeadingDocument, isTrusted);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Счет-фактура.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Structures.Module.IRecognitionResult recognitionResult)
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
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.Name, props.ShipperName.Name, shipper.Name);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.LegalForm, props.ShipperName.Name, shipper.Name);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        LinkFactAndProperty(recognitionResult, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.ConsigneeName = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.Name, props.ConsigneeName.Name, consignee.Name);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.LegalForm, props.ConsigneeName.Name, consignee.Name);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        LinkFactAndProperty(recognitionResult, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, recognitionResult, FactNames.FinancialDocument);
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      // Номенклатура.
      foreach (var fact in GetFacts(facts, FactNames.Goods, FieldNames.Goods.Name))
      {
        var good = document.Goods.AddNew();
        
        // Обрезать наименование под размер поля.
        var goodName = GetFieldValue(fact, FieldNames.Goods.Name);
        if (goodName.Length > document.Info.Properties.Goods.Properties.Name.Length)
          goodName = goodName.Substring(0, document.Info.Properties.Goods.Properties.Name.Length);
        good.Name = goodName;
        
        good.UnitName =  GetFieldValue(fact, FieldNames.Goods.UnitName);
        good.Count = GetFieldNumericalValue(fact, FieldNames.Goods.Count);
        good.Price = GetFieldNumericalValue(fact, FieldNames.Goods.Price);
        good.VatAmount = GetFieldNumericalValue(fact, FieldNames.Goods.VatAmount);
        good.TotalAmount = GetFieldNumericalValue(fact, FieldNames.Goods.Amount);
        
        var formatter = string.Format("{0}.{1}", props.Goods.Name, "{0}");
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                            good.Name, collectionRecordId: good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                            good.UnitName, collectionRecordId: good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                            good.Count, collectionRecordId: good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                            good.Price, collectionRecordId: good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                            good.VatAmount, collectionRecordId: good.Id);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                            good.TotalAmount, collectionRecordId: good.Id);
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
    public virtual Docflow.IOfficialDocument CreateTaxInvoice(Structures.Module.IRecognitionResult recognitionResult, bool isAdjustment, IEmployee responsible)
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
      var documentParties = Structures.Module.DocumentParties.Create();
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
      
      // НОР и КА.
      FillAccountingDocumentParties(document, documentParties);
      LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Вид документа.
      FillDocumentKind(document);
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognitionResult, FactNames.FinancialDocument);
      
      // Корректировочный документ.
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.CorrectionDate).FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.CorrectionNumber).FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, FieldNames.FinancialDocument.CorrectionDate);
        var correctionNumber = GetFieldValue(correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber);
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
          LinkFactAndProperty(recognitionResult, correctionDateFact, FieldNames.FinancialDocument.CorrectionDate, props.Corrected.Name, document.Corrected, isTrusted);
          LinkFactAndProperty(recognitionResult, correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber, props.Corrected.Name, document.Corrected, isTrusted);
        }
      }
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
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
    public virtual Docflow.IOfficialDocument CreateUniversalTransferDocument(Structures.Module.IRecognitionResult recognitionResult, bool isAdjustment, IEmployee responsible)
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
      FillAccountingDocumentParties(document, documentParties);
      LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Дата, номер и регистрация.
      NumberDocument(document, recognitionResult, FactNames.FinancialDocument);
      
      // Корректировочный документ.
      FillCorrectedDocument(document, recognitionResult, isAdjustment);
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);

      return document;
    }
    
    #endregion
    
    #region Счет на оплату
    
    /// <summary>
    /// Создать счет на оплату (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки счета на оплату в Ario.</param>
    /// <returns>Счет на оплату.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingInvoice(Structures.Module.IRecognitionResult recognitionResult)
    {
      var document = Sungero.Capture.MockIncomingInvoices.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = recognitionResult.Facts;
      
      // Договор.
      var leadingDocFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var isTrusted = IsTrustedField(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      LinkFactAndProperty(recognitionResult, leadingDocFact, null, props.Contract.Name, document.Contract, isTrusted);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        LinkFactAndProperty(recognitionResult, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        LinkFactAndProperty(recognitionResult, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Могут прийти контрагенты без типа. Заполнить контрагентами без типа.
      if (seller == null || buyer == null)
      {
        var withoutTypeFacts = GetFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
          .Where(f => string.IsNullOrWhiteSpace(GetFieldValue(f, FieldNames.Counterparty.CounterpartyType)))
          .OrderByDescending(x => x.Fields.First(f => f.Name == FieldNames.Counterparty.Name).Probability);
        foreach (var fact in withoutTypeFacts)
        {
          var name = GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
          
          var tin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
          var trrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
          var type = GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType);
          
          if (string.IsNullOrWhiteSpace(document.SellerName))
          {
            document.SellerName = name;
            document.SellerTin = tin;
            document.SellerTrrc = trrc;
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.SellerName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, tin);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BuyerName))
          {
            document.BuyerName = name;
            document.BuyerTin = tin;
            document.BuyerTrrc = trrc;
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.BuyerName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, name);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, tin);
            LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, trrc);
          }
        }
      }
      
      // Дата и номер.
      var dateFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date).FirstOrDefault();
      var date = GetFieldDateTimeValue(dateFact, FieldNames.FinancialDocument.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Date = date;
      LinkFactAndProperty(recognitionResult, dateFact, FieldNames.FinancialDocument.Date, props.Date.Name, date, isTrustedDate);
      
      var numberFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Number).FirstOrDefault();
      document.Number = GetFieldValue(numberFact, FieldNames.FinancialDocument.Number);
      LinkFactAndProperty(recognitionResult, numberFact, FieldNames.FinancialDocument.Number, props.Number.Name, document.Number);
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    
    /// <summary>
    /// Создать счет на оплату.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Арио.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Счет на оплату.</returns>
    public virtual Docflow.IOfficialDocument CreateIncomingInvoice(Structures.Module.IRecognitionResult recognitionResult, IEmployee responsible)
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
      FillAccountingDocumentParties(document, documentParties);
      LinkAccountingDocumentParties(recognitionResult, documentParties);
      
      // Договор.
      var contractFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      var contract = GetLeadingDocument(contractFact, document.Info.Properties.Contract.Name, document.Counterparty, document.Info.Properties.Counterparty.Name);
      document.Contract = contract.Contract;
      LinkFactAndProperty(recognitionResult, contractFact, null, props.Contract.Name, document.Contract, contract.IsTrusted);
      
      // Дата.
      var dateFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date).FirstOrDefault();
      var date = GetFieldDateTimeValue(dateFact, FieldNames.FinancialDocument.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && IsTrustedField(dateFact, FieldNames.Document.Date);
      document.Date = date;
      LinkFactAndProperty(recognitionResult, dateFact, FieldNames.FinancialDocument.Date, props.Date.Name, date, isTrustedDate);
      
      // Номер.
      var numberFact = GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Number).FirstOrDefault();
      var number = GetFieldValue(numberFact, FieldNames.FinancialDocument.Number);
      Nullable<bool> isTrustedNumber = null;
      if (number.Length > document.Info.Properties.Number.Length)
      {
        number = number.Substring(0, document.Info.Properties.Number.Length);
        isTrustedNumber = false;
      }
      document.Number = number;
      LinkFactAndProperty(recognitionResult, numberFact, FieldNames.FinancialDocument.Number, props.Number.Name, document.Number, isTrustedNumber);
      
      // Подразделение и ответственный.
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
      return document;
    }
    
    #endregion
    
    #region Договор
    
    /// <summary>
    /// Создать договор (демо режим).
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <returns>Договор.</returns>
    public Docflow.IOfficialDocument CreateMockContract(Structures.Module.IRecognitionResult recognitionResult)
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
      var partyNameFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name);
      if (partyNameFacts.Count() > 0)
      {
        var fact = partyNameFacts.First();
        document.FirstPartyName = GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.FirstPartySignatory = GetFullNameByFactForContract(fact);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.FirstPartyName.Name, document.FirstPartyName);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatorySurname, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryName, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryPatrn, props.FirstPartySignatory.Name, document.FirstPartySignatory);
      }
      if (partyNameFacts.Count() > 1)
      {
        var fact = partyNameFacts.Last();
        document.SecondPartyName = GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.SecondPartySignatory = GetFullNameByFactForContract(fact);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.Name, props.SecondPartyName.Name, document.SecondPartyName);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatorySurname, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryName, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.SignatoryPatrn, props.SecondPartySignatory.Name, document.SecondPartySignatory);
      }
      
      // Заполнить ИНН/КПП сторон.
      var tinTrrcFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.FirstPartyTin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.FirstPartyTrrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.FirstPartyTin.Name, document.FirstPartyTin);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.FirstPartyTrrc.Name, document.FirstPartyTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.SecondPartyTin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.SecondPartyTrrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TIN, props.SecondPartyTin.Name, document.SecondPartyTin);
        LinkFactAndProperty(recognitionResult, fact, FieldNames.Counterparty.TRRC, props.SecondPartyTrrc.Name, document.SecondPartyTrrc);
      }
      
      // Сумма и валюта.
      var documentAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      LinkFactAndProperty(recognitionResult, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      
      var documentVatAmountFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.VatAmount).FirstOrDefault();
      document.VatAmount = GetFieldNumericalValue(documentVatAmountFact, FieldNames.DocumentAmount.VatAmount);
      LinkFactAndProperty(recognitionResult, documentVatAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        LinkFactAndProperty(recognitionResult, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      return document;
    }
    
    /// <summary>
    /// Создать договор.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>Договор.</returns>
    public Docflow.IOfficialDocument CreateContract(Structures.Module.IRecognitionResult recognitionResult,
                                                    Sungero.Company.IEmployee responsible)
    {
      var document = Sungero.Contracts.Contracts.Create();
      
      // Вид документа.
      FillDocumentKind(document);
      
      // Заполнить данные нашей стороны и корреспондента.
      FillContractualDocumentParties(recognitionResult, responsible, document);
      
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;
      
      // Сумма и валюта.
      FillAmountAndCurrency(document, recognitionResult);
      
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
    public Docflow.IOfficialDocument CreateSupAgreement(Structures.Module.IRecognitionResult recognitionResult, Sungero.Company.IEmployee responsible)
    {
      var document = Sungero.Contracts.SupAgreements.Create();
      
      // Вид документа.
      FillDocumentKind(document);

      // Заполнить данные нашей стороны и корреспондента.
      FillContractualDocumentParties(recognitionResult, responsible, document);
      
      document.Department = Company.PublicFunctions.Department.GetDepartment(responsible);
      document.ResponsibleEmployee = responsible;

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
    /// Заполнить НОР и контрагента в бухгалтерском документе.
    /// </summary>
    /// <param name="document">Бухгалтерский документ.</param>
    /// <param name="documentParties">НОР и контрагент.</param>
    public virtual void FillAccountingDocumentParties(IAccountingDocumentBase document,
                                                      Structures.Module.IDocumentParties documentParties)
    {
      var counterparty = documentParties.Counterparty;
      var businessUnit = documentParties.BusinessUnit;
      var businessUnitMatched = businessUnit != null && businessUnit.BusinessUnit != null;
      
      document.Counterparty = counterparty != null ? counterparty.Counterparty : null;
      document.BusinessUnit = businessUnitMatched ? businessUnit.BusinessUnit : documentParties.ResponsibleEmployeeBusinessUnit;
    }
    
    /// <summary>
    /// Связать факты для НОР и контрагента с подобранными значениями.
    /// </summary>
    /// <param name="recognitionResult">Результаты обработки бухгалтерского документа в Ario.</param>
    /// <param name="documentParties">НОР и контрагент.</param>
    public virtual void LinkAccountingDocumentParties(Structures.Module.IRecognitionResult recognitionResult,
                                                      Structures.Module.IDocumentParties documentParties)
    {
      var counterpartyPropertyName = AccountingDocumentBases.Info.Properties.Counterparty.Name;
      var businessUnitPropertyName = AccountingDocumentBases.Info.Properties.BusinessUnit.Name;
      
      var counterpartyMatched = documentParties.Counterparty != null &&
        documentParties.Counterparty.Counterparty != null;
      var businessUnitMatched = documentParties.BusinessUnit != null &&
        documentParties.BusinessUnit.BusinessUnit != null;
      
      if (counterpartyMatched)
        LinkFactAndProperty(recognitionResult, documentParties.Counterparty.Fact, null,
                            counterpartyPropertyName, documentParties.Counterparty.Counterparty, documentParties.Counterparty.IsTrusted);

      if (businessUnitMatched)
        LinkFactAndProperty(recognitionResult, documentParties.BusinessUnit.Fact, null,
                            businessUnitPropertyName, documentParties.BusinessUnit.BusinessUnit, documentParties.BusinessUnit.IsTrusted);
      else
        LinkFactAndProperty(recognitionResult, null, null,
                            businessUnitPropertyName, documentParties.ResponsibleEmployeeBusinessUnit, false);
    }
    
    /// <summary>
    /// Заполнить сумму и валюту.
    /// </summary>
    /// <param name="document">Бухгалтерский документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    public virtual void FillAmountAndCurrency(IAccountingDocumentBase document, Structures.Module.IRecognitionResult recognitionResult)
    {
      if (!AccountingDocumentBases.Is(document))
        return;
      
      var recognizedAmount = GetRecognizedAmount(recognitionResult);
      if (recognizedAmount.HasValue)
      {
        var amount = recognizedAmount.Amount;
        document.TotalAmount = amount;
        LinkFactAndProperty(recognitionResult, recognizedAmount.Fact, FieldNames.DocumentAmount.Amount, document.Info.Properties.TotalAmount.Name, amount, recognizedAmount.IsTrusted);
      }

      // В факте с суммой документа может быть не указана валюта, поэтому факт с валютой ищем отдельно,
      // так как на данный момент функция используется для обработки бухгалтерских и договорных документов,
      // а в них все расчеты ведутся в одной валюте.
      var recognizedCurrency = GetRecognizedCurrency(recognitionResult);
      if (recognizedCurrency.HasValue)
      {
        var currency = recognizedCurrency.Currency;
        document.Currency = currency;
        LinkFactAndProperty(recognitionResult, recognizedCurrency.Fact, FieldNames.DocumentAmount.Currency, document.Info.Properties.Currency.Name, currency, recognizedCurrency.IsTrusted);
      }
    }
    
    /// <summary>
    /// Заполнить сумму и валюту.
    /// </summary>
    /// <param name="document">Договорной документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    public virtual void FillAmountAndCurrency(IContractualDocumentBase document, Structures.Module.IRecognitionResult recognitionResult)
    {
      if (!ContractualDocumentBases.Is(document))
        return;
      
      var recognizedAmount = GetRecognizedAmount(recognitionResult);
      if (recognizedAmount.HasValue)
      {
        var amount = recognizedAmount.Amount;
        document.TotalAmount = amount;
        LinkFactAndProperty(recognitionResult, recognizedAmount.Fact, FieldNames.DocumentAmount.Amount, document.Info.Properties.TotalAmount.Name, amount, recognizedAmount.IsTrusted);
      }
      
      // В факте с суммой документа может быть не указана валюта, поэтому факт с валютой ищем отдельно,
      // так как на данный момент функция используется для обработки бухгалтерских и договорных документов,
      // а в них все расчеты ведутся в одной валюте.
      var recognizedCurrency = GetRecognizedCurrency(recognitionResult);
      if (recognizedCurrency.HasValue)
      {
        var currency = recognizedCurrency.Currency;
        document.Currency = currency;
        LinkFactAndProperty(recognitionResult, recognizedCurrency.Fact, FieldNames.DocumentAmount.Currency, document.Info.Properties.Currency.Name, currency, recognizedCurrency.IsTrusted);
      }
    }
    
    /// <summary>
    /// Пронумеровать документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public virtual void NumberDocument(IOfficialDocument document,
                                       Structures.Module.IRecognitionResult recognitionResult,
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
      var facts = recognitionResult.Facts;
      var regDateFact = GetOrderedFacts(facts, factName, FieldNames.Document.Date).FirstOrDefault();
      var regDate = GetFieldDateTimeValue(regDateFact, FieldNames.Document.Date);
      Nullable<bool> isTrustedDate = null;
      if (regDate == null || !regDate.HasValue || regDate < Calendar.SqlMinValue)
      {
        regDate = Calendar.SqlMinValue;
        isTrustedDate = false;
      }
      
      // Номер.
      var regNumberFact = GetOrderedFacts(facts, factName, FieldNames.Document.Number).FirstOrDefault();
      var regNumber = GetFieldValue(regNumberFact, FieldNames.Document.Number);
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
      LinkFactAndProperty(recognitionResult, regDateFact, FieldNames.Document.Date, props.RegistrationDate.Name, document.RegistrationDate, isTrustedDate);
      LinkFactAndProperty(recognitionResult, regNumberFact, FieldNames.Document.Number, props.RegistrationNumber.Name,
                          document.RegistrationNumber, isTrustedNumber);
    }
    
    /// <summary>
    /// Заполнить номер и дату для Mock-документов.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public void FillMockRegistrationData(IOfficialDocument document,
                                         Structures.Module.IRecognitionResult recognitionResult,
                                         string factName)
    {
      // Дата.
      var facts = recognitionResult.Facts;
      var dateFact = GetOrderedFacts(facts, factName, FieldNames.Document.Date).FirstOrDefault();
      var date = GetFieldDateTimeValue(dateFact, FieldNames.Document.Date);
      var isDateValid = IsDateValid(date);
      if (!isDateValid)
        date = Calendar.SqlMinValue;
      var isTrustedDate = isDateValid && IsTrustedField(dateFact, FieldNames.Document.Date);
      document.RegistrationDate = date;

      // Номер.
      var regNumberFact = GetOrderedFacts(facts, factName, FieldNames.Document.Number).FirstOrDefault();
      var regNumber = GetFieldValue(regNumberFact, FieldNames.Document.Number);
      Nullable<bool> isTrustedNumber = null;
      if (regNumber.Length > document.Info.Properties.RegistrationNumber.Length)
      {
        regNumber = regNumber.Substring(0, document.Info.Properties.RegistrationNumber.Length);
        isTrustedNumber = false;
      }
      document.RegistrationNumber = regNumber;
      
      var props = document.Info.Properties;
      LinkFactAndProperty(recognitionResult, dateFact, FieldNames.Document.Date, props.RegistrationDate.Name, date, isTrustedDate);
      LinkFactAndProperty(recognitionResult, regNumberFact, FieldNames.Document.Number, props.RegistrationNumber.Name,
                          document.RegistrationNumber, isTrustedNumber);
    }
    
    /// <summary>
    /// Заполнить корректируемый документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="isAdjustment">Корректировочный.</param>
    public virtual void FillCorrectedDocument(IAccountingDocumentBase document,
                                              Structures.Module.IRecognitionResult recognitionResult,
                                              bool isAdjustment)
    {
      if (isAdjustment)
      {
        document.IsAdjustment = true;
        var correctionDateFact = GetOrderedFacts(recognitionResult.Facts, FactNames.FinancialDocument,
                                                 FieldNames.FinancialDocument.CorrectionDate).FirstOrDefault();
        var correctionNumberFact = GetOrderedFacts(recognitionResult.Facts, FactNames.FinancialDocument,
                                                   FieldNames.FinancialDocument.CorrectionNumber).FirstOrDefault();
        var correctionDate = GetFieldDateTimeValue(correctionDateFact, FieldNames.FinancialDocument.CorrectionDate);
        var correctionNumber = GetFieldValue(correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber);
        
        document.Corrected = FinancialArchive.UniversalTransferDocuments.GetAll()
          .FirstOrDefault(d => d.RegistrationNumber.Equals(correctionNumber, StringComparison.InvariantCultureIgnoreCase) && d.RegistrationDate == correctionDate);
        var props = document.Info.Properties;
        LinkFactAndProperty(recognitionResult, correctionDateFact, FieldNames.FinancialDocument.CorrectionDate,
                            props.Corrected.Name, document.Corrected, true);
        LinkFactAndProperty(recognitionResult, correctionNumberFact, FieldNames.FinancialDocument.CorrectionNumber,
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

    /// <summary>
    /// Распознать сумму.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <returns>Результаты распознавания суммы.</returns>
    public virtual Structures.Module.IRecognizedAmount GetRecognizedAmount(Structures.Module.IRecognitionResult recognitionResult)
    {
      var recognizedAmount = Structures.Module.RecognizedAmount.Create();
      var facts = recognitionResult.Facts;
      var amountFacts = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount);
      
      var amountFact = amountFacts.FirstOrDefault();
      if (amountFact == null)
        return recognizedAmount;
      
      recognizedAmount.Fact = amountFact;
      
      var totalAmount = GetFieldNumericalValue(amountFact, FieldNames.DocumentAmount.Amount);
      if (!totalAmount.HasValue || totalAmount.Value <= 0)
        return recognizedAmount;
      
      recognizedAmount.HasValue = true;
      recognizedAmount.Amount = totalAmount.Value;
      recognizedAmount.IsTrusted = IsTrustedField(amountFact, FieldNames.DocumentAmount.Amount);
      
      // Если в сумме больше 2 знаков после запятой,
      // то обрезать лишние разряды,
      // иначе пометить, что результату извлечения можно доверять
      var roundedAmount = Math.Round(totalAmount.Value, 2);
      var amountClipping = roundedAmount - totalAmount.Value != 0.0;
      if (amountClipping)
      {
        recognizedAmount.Amount = roundedAmount;
        recognizedAmount.IsTrusted = false;
      }
      
      return recognizedAmount;
    }

    /// <summary>
    /// Распознать валюту.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <returns>Результаты распознавания валюты.</returns>
    public virtual Structures.Module.IRecognizedCurrency GetRecognizedCurrency(Structures.Module.IRecognitionResult recognitionResult)
    {
      var recognizedCurrency = Structures.Module.RecognizedCurrency.Create();
      
      var facts = recognitionResult.Facts;
      var currencyFacts = GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency);
      var currencyFact = currencyFacts.FirstOrDefault();
      if (currencyFact == null)
        return recognizedCurrency;
      
      recognizedCurrency.Fact = currencyFact;
      
      var defaultCurrency = Commons.Currencies.GetAll(x => x.IsDefault == true).FirstOrDefault();
      recognizedCurrency.Currency = defaultCurrency;
      recognizedCurrency.HasValue = true;
      
      var currencyCode = GetFieldValue(currencyFact, FieldNames.DocumentAmount.Currency);
      int resCurrencyCode;
      if (!Int32.TryParse(currencyCode, out resCurrencyCode))
        return recognizedCurrency;
      
      var currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).OrderBy(x => x.Status).FirstOrDefault();
      if (currency == null)
        return recognizedCurrency;
      
      recognizedCurrency.IsTrusted = true;
      var currencyStatus = currency.Status;
      if (currencyStatus == CoreEntities.DatabookEntry.Status.Closed)
      {
        currency = defaultCurrency;
        recognizedCurrency.IsTrusted = false;
      }
      
      if (currency != null)
        recognizedCurrency.Currency = currency;
      
      return recognizedCurrency;
    }
    
    /// <summary>
    ////Проверка даты на валидность.
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
    public virtual List<IFact> GetCounterpartyFacts(List<Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name)
        .Where(f => GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType);
      
      if (!counterpartyFacts.Any())
        counterpartyFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN)
          .Where(f => GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType);
      
      return counterpartyFacts.ToList();
    }
    
    /// <summary>
    /// Подобрать по факту контрагента и НОР.
    /// </summary>
    /// <param name="allFacts">Факты.</param>
    /// <param name="counterpartyTypes">Типы фактов контрагентов.</param>
    /// <returns>Наши организации и контрагенты, найденные по фактам.</returns>
    public virtual List<Structures.Module.ICounterpartyFactMatching> MatchFactsWithBusinessUnitsAndCounterparties(List<Structures.Module.IFact> allFacts,
                                                                                                                  List<string> counterpartyTypes)
    {
      var counterpartyPropertyName = AccountingDocumentBases.Info.Properties.Counterparty.Name;
      var businessUnitPropertyName = AccountingDocumentBases.Info.Properties.BusinessUnit.Name;
      
      // Фильтр фактов по типам.
      var facts = new List<IFact>();
      foreach (var counterpartyType in counterpartyTypes)
        facts.AddRange(GetCounterpartyFacts(allFacts, counterpartyType));
      
      var matchings = new List<Structures.Module.ICounterpartyFactMatching>();
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
        var tin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
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
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create(businessUnit, counterparty, fact,
                                                                                           GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType),
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
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create(businessUnit, counterparty, fact,
                                                                                           GetFieldValue(fact, FieldNames.Counterparty.CounterpartyType),
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
    public virtual Structures.Module.IDocumentParties GetDocumentParties(Structures.Module.ICounterpartyFactMatching buyer,
                                                                         Structures.Module.ICounterpartyFactMatching seller,
                                                                         List<Structures.Module.ICounterpartyFactMatching> nonType,
                                                                         IEmployee responsibleEmployee)
    {
      Structures.Module.ICounterpartyFactMatching counterparty = null;
      Structures.Module.ICounterpartyFactMatching businessUnit = null;
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
        ? Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeePersonalSettingsBusinessUnit)
        : Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeeBusinessUnit);
    }
    
    
    /// <summary>
    /// Подобрать участников сделки (НОР и контрагент).
    /// </summary>
    /// <param name="buyer">Список фактов с данными о контрагенте. Тип контрагента - покупатель.</param>
    /// <param name="seller">Список фактов с данными о контрагенте. Тип контрагента - продавец.</param>
    /// <param name="responsibleEmployee">Ответственный сотрудник.</param>
    /// <returns>НОР и контрагент.</returns>
    public virtual Structures.Module.IDocumentParties GetDocumentParties(Structures.Module.ICounterpartyFactMatching buyer,
                                                                         Structures.Module.ICounterpartyFactMatching seller,
                                                                         IEmployee responsibleEmployee)
    {
      Structures.Module.ICounterpartyFactMatching counterparty = null;
      Structures.Module.ICounterpartyFactMatching businessUnit = null;
      
      // НОР.
      if (buyer != null)
        businessUnit = buyer;
      
      // Контрагент.
      if (seller != null && seller.Counterparty != null)
        counterparty = seller;
      
      var responsibleEmployeeBusinessUnit = Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(responsibleEmployee);
      
      return Structures.Module.DocumentParties.Create(businessUnit, counterparty, responsibleEmployeeBusinessUnit);
    }
    
    /// <summary>
    /// Поиск контрагента для документов в демо режиме.
    /// </summary>
    /// <param name="facts">Факты для поиска факта с контрагентом.</param>
    /// <param name="counterpartyType">Тип контрагента.</param>
    /// <returns>Контрагент.</returns>
    public static Structures.Module.MockCounterparty GetMostProbableMockCounterparty(List<Structures.Module.IFact> facts, string counterpartyType)
    {
      var counterpartyFacts = GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name);
      var mostProbabilityFact = counterpartyFacts.Where(f =>  GetFieldValue(f, FieldNames.Counterparty.CounterpartyType) == counterpartyType).FirstOrDefault();
      if (mostProbabilityFact == null)
        return null;

      var counterparty = Structures.Module.MockCounterparty.Create();
      counterparty.Name = GetCounterpartyName(mostProbabilityFact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
      counterparty.Tin = GetFieldValue(mostProbabilityFact, FieldNames.Counterparty.TIN);
      counterparty.Trrc = GetFieldValue(mostProbabilityFact, FieldNames.Counterparty.TRRC);
      counterparty.Fact = mostProbabilityFact;
      return counterparty;
    }
    
    /// <summary>
    /// Получить название факта контрагента с названиями полей наименования и ОПФ.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Наименование факта, наименования полей с наименованием и ОПФ организации.</returns>
    public virtual ICounterpartyFactNames GetCounterpartyFactNames(string predictedClass)
    {
      var counterpartyFactNames = CounterpartyFactNames.Create();
      
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
    
    /// <summary>
    /// Поиск корреспондента по извлеченным фактам.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Корреспондент.</returns>
    public virtual Structures.Module.ICounterpartyFactMatching GetCounterparty(Structures.Module.IRecognitionResult recognitionResult, string propertyName)
    {
      var actualCounterparties = Counterparties.GetAll()
        .Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      var foundByName = new List<Structures.Module.ICounterpartyFactMatching>();
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
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create();
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
      var foundByTin = new List<Structures.Module.ICounterpartyFactMatching>();
      foreach (var fact in correspondentTINs)
      {
        // Если для свойства propertyName по факту существует верифицированное ранее значение, то вернуть его.
        var verifiedCounterparty = GetCounterpartyByVerifiedData(fact, propertyName);
        if (verifiedCounterparty != null)
          return verifiedCounterparty;
        
        var tin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        var counterparties = Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, true);
        foreach (var counterparty in counterparties)
        {
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create();
          counterpartyFactMatching.Counterparty = counterparty;
          counterpartyFactMatching.Fact = fact;
          counterpartyFactMatching.IsTrusted = true;
          foundByTin.Add(counterpartyFactMatching);
        }
      }
      
      Structures.Module.ICounterpartyFactMatching resultCounterparty = null;
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
    
    /// <summary>
    /// Получить контрагента по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связка контрагент + факт.</returns>
    public virtual Structures.Module.ICounterpartyFactMatching GetCounterpartyByVerifiedData(Structures.Module.IFact fact, string propertyName)
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
      
      var result = Structures.Module.CounterpartyFactMatching.Create();
      result.Counterparty = filteredCounterparty;
      result.Fact = fact;
      result.IsTrusted = counterpartyUnitField.IsTrusted == true;
      return result;
    }
    
    /// <summary>
    /// Получить нор по результатам верификации пользователя.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <returns>Связку контрагент + факт.</returns>
    public virtual Structures.Module.ICounterpartyFactMatching GetBusinessUnitByVerifiedData(Structures.Module.IFact fact, string propertyName)
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
      
      var result = Structures.Module.CounterpartyFactMatching.Create();
      result.BusinessUnit = filteredBusinessUnit;
      result.Fact = fact;
      result.IsTrusted = businessUnitField.IsTrusted == true;
      return result;
    }

    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного и адресата.
    /// </summary>
    /// <param name="businessUnitsWithFacts">НОР, найденные по фактам.</param>
    /// <param name="businessUnitPropertyName">Имя связанного свойства.</param>
    /// <param name="addressee">Адресат.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public virtual ICounterpartyFactMatching GetMostProbableBusinessUnitMatching(List<ICounterpartyFactMatching> businessUnitsWithFacts,
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
      var businessUnitByAddresseeFactMatching = Capture.Structures.Module.CounterpartyFactMatching.Create();
      businessUnitByAddresseeFactMatching.BusinessUnit = businessUnitByAddressee;
      businessUnitByAddresseeFactMatching.Fact = null;
      businessUnitByAddresseeFactMatching.IsTrusted = false;
      
      // Попытаться уточнить по адресату.
      // TODO Dmitirev_IA: Скорее всего стоит уточнять по адресату, если фактов нет или их несколько.
      var hasAnyBusinessUnitFacts = businessUnitsWithFacts.Any();
      var hasBusinessUnitByAddressee = businessUnitByAddressee != null;
      Structures.Module.ICounterpartyFactMatching businessUnitWithFact = null;
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
      var businessUnitByResponsibleFactMatching = Capture.Structures.Module.CounterpartyFactMatching.Create();
      businessUnitByResponsibleFactMatching.BusinessUnit = businessUnitByResponsible;
      businessUnitByResponsibleFactMatching.Fact = null;
      businessUnitByResponsibleFactMatching.IsTrusted = false;
      return businessUnitByResponsibleFactMatching;
    }
    
    /// <summary>
    /// Поиск НОР, наиболее подходящей для ответственного.
    /// </summary>
    /// <param name="businessUnitsWithFacts">НОР, найденные по фактам.</param>
    /// <param name="businessUnitPropertyName">Имя связанного свойства.</param>
    /// <param name="responsible">Ответственный.</param>
    /// <returns>НОР и соответствующий ей факт.</returns>
    public virtual ICounterpartyFactMatching GetMostProbableBusinessUnitMatching(List<ICounterpartyFactMatching> businessUnitsWithFacts,
                                                                                 string businessUnitPropertyName,
                                                                                 IEmployee responsible)
    {
      var businessUnit = Capture.Structures.Module.CounterpartyFactMatching.Create();
      
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
        // Если в персональных настройках ответственного указана НОР.
        if (responsibleEmployeePersonalSettingsBusinessUnit != null)
          businessUnit.BusinessUnit = responsibleEmployeePersonalSettingsBusinessUnit;
        else
          businessUnit.BusinessUnit = responsibleEmployeeBusinessUnit;
        
        businessUnit.Fact = null;
        businessUnit.IsTrusted = false;
        return businessUnit;
      }
      
      businessUnit = businessUnitsWithFacts.FirstOrDefault();
      return businessUnit;
    }
    
    /// <summary>
    /// Получение списка НОР по извлеченным фактам.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <returns>Список НОР и соответствующих им фактов.</returns>
    public virtual List<Capture.Structures.Module.ICounterpartyFactMatching> GetBusinessUnitsWithFacts(Structures.Module.IRecognitionResult recognitionResult)
    {
      var facts = recognitionResult.Facts;
      
      var counterpartyFactNames = GetCounterpartyFactNames(recognitionResult.PredictedClass);
      
      // Получить факты с наименованиями организаций.
      var businessUnitsByName = new List<Capture.Structures.Module.ICounterpartyFactMatching>();
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
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create();
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
      var foundByTin = new List<Capture.Structures.Module.ICounterpartyFactMatching>();
      foreach (var fact in correspondentTinFacts)
      {
        var tin = GetFieldValue(fact, FieldNames.Counterparty.TIN);
        var trrc = GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        var businessUnits = Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);
        var isTrusted = businessUnits.Count == 1;
        
        foreach (var businessUnit in businessUnits)
        {
          var counterpartyFactMatching = Structures.Module.CounterpartyFactMatching.Create();
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
    
    /// <summary>
    /// Заполнить стороны договорного документа.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки документа в Ario.</param>
    /// <param name="responsible">Ответственный за верификацию.</param>
    /// <param name="document">Договорной документ.</param>
    public virtual void FillContractualDocumentParties(Structures.Module.IRecognitionResult recognitionResult,
                                                       Sungero.Company.IEmployee responsible,
                                                       Sungero.Contracts.IContractualDocument document)
    {
      var props = document.Info.Properties;
      var businessUnitPropertyName = props.BusinessUnit.Name;
      var counterpartyPropertyName = props.Counterparty.Name;
      
      // Заполнить данные нашей стороны.
      var businessUnitsWithFacts = GetBusinessUnitsWithFacts(recognitionResult);
      
      var businessUnitWithFact = GetMostProbableBusinessUnitMatching(businessUnitsWithFacts,
                                                                     businessUnitPropertyName,
                                                                     responsible);
      document.BusinessUnit = businessUnitWithFact.BusinessUnit;
      LinkFactAndProperty(recognitionResult, businessUnitWithFact.Fact, null, businessUnitPropertyName,
                          document.BusinessUnit, businessUnitWithFact.IsTrusted);
      // Заполнить подписанта.
      var ourSignatory = GetContractualDocumentSignatory(recognitionResult, document, businessUnitWithFact);
      document.OurSignatory = ourSignatory.Employee;
      
      var isTrustedOurSignatory = ourSignatory.IsTrusted && IsTrustedField(ourSignatory.Fact,
                                                                           FieldNames.Counterparty.SignatorySurname);
      
      LinkFactAndProperty(recognitionResult, ourSignatory.Fact, null, props.OurSignatory.Name,
                          document.OurSignatory, isTrustedOurSignatory);
      
      // Заполнить данные корреспондента.
      // Убираем уже использованный факт для подбора НОР, чтобы организация не искалась по тем же реквизитам что и НОР.
      if (document.BusinessUnit != null)
        recognitionResult.Facts.Remove(businessUnitWithFact.Fact);
      var сounterparty = GetCounterparty(recognitionResult, counterpartyPropertyName);
      if (сounterparty != null)
      {
        document.Counterparty = сounterparty.Counterparty;
        LinkFactAndProperty(recognitionResult, сounterparty.Fact, null, counterpartyPropertyName,
                            document.Counterparty, сounterparty.IsTrusted);
      }
      
      // Заполнить подписанта.
      var signedBy = GetContractualDocumentSignatory(recognitionResult, document, сounterparty);
      
      // Если контрагент не заполнен, он подставляется из подписанта и результату не доверяем.
      if (document.Counterparty == null && signedBy.Contact != null)
        LinkFactAndProperty(recognitionResult, null, null, props.Counterparty.Name, signedBy.Contact.Company, false);
      
      document.CounterpartySignatory = signedBy.Contact;
      var isTrustedSignatory = signedBy.IsTrusted && IsTrustedField(signedBy.Fact, FieldNames.Counterparty.SignatorySurname);
      LinkFactAndProperty(recognitionResult, signedBy.Fact, null, props.CounterpartySignatory.Name,
                          document.CounterpartySignatory, isTrustedSignatory);
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
    /// Получить список полей из факта.
    /// </summary>
    /// <param name="fact">Имя факта.</param>
    /// <param name="fieldName">Список имен поля.</param>
    /// <returns>Список полей.</returns>
    public static IQueryable<IFactField> GetFields(Structures.Module.IFact fact, List<string> fieldNames)
    {
      if (fact == null)
        return null;
      return fact.Fields.Where(f => fieldNames.Contains(f.Name)).AsQueryable();
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
    /// Получить запись, которая уже сопоставлялась с переданным фактом, с дополнительной фильтрацией по контрагенту.
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
    /// Получить список фактов с переданным именем факта.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <returns>Список фактов.</returns>
    public static List<Structures.Module.IFact> GetFacts(List<Structures.Module.IFact> facts, string factName)
    {
      return facts
        .Where(f => f.Name == factName)
        .ToList();
    }
    
    /// <summary>
    /// Получить список фактов с переданными именем факта и именем поля.
    /// </summary>
    /// <param name="facts">Список фактов.</param>
    /// <param name="factName">Имя факта.</param>
    /// <param name="fieldName">Имя поля.</param>
    /// <returns>Список фактов.</returns>
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
    /// Получить адрес сервиса Арио.
    /// </summary>
    /// <returns>Адрес Арио.</returns>
    [Remote]
    public virtual string GetArioUrl()
    {
      var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
      var arioUrl = smartProcessingSettings.ArioUrl;
      return arioUrl;
    }
    
    /// <summary>
    /// Получить значение численного параметра из docflow_params.
    /// </summary>
    /// <param name="paramName">Наименование параметра.</param>
    /// <returns>Значение параметра.</returns>
    public static double GetDocflowParamsNumbericValue(string paramName)
    {
      double result = 0;
      var paramValue = Docflow.PublicFunctions.Module.GetDocflowParamsValue(paramName);
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
    public static string GetCounterpartyName(Structures.Module.IFact fact, string nameFieldName, string legalFormFieldName)
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
      
      var documentName = GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseName);
      var date = Functions.Module.GetShortDate(GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseDate));
      var number = GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseNumber);
      
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
    /// <param name="recognitionResult">Результат обработки документа в Арио.</param>
    /// <param name="fact">Факт, который будет связан со свойством документа.</param>
    /// <param name="fieldName">Поле, которое будет связано со свойством документа. Если не указано, то будут связаны все поля факта.</param>
    /// <param name="propertyName">Имя свойства документа.</param>
    /// <param name="propertyValue">Значение свойства.</param>
    /// <param name="isTrusted">Признак, доверять результату извлечения из Арио или нет.</param>
    /// <param name="collectionRecordId">ИД записи свойства-коллекции.</param>
    public static void LinkFactAndProperty(Structures.Module.IRecognitionResult recognitionResult,
                                           Structures.Module.IFact fact,
                                           string fieldName,
                                           string propertyName,
                                           object propertyValue,
                                           bool? isTrusted = null,
                                           int? collectionRecordId = null)
    {
      var propertyStringValue = Functions.Module.GetPropertyValueAsString(propertyValue);
      if (string.IsNullOrWhiteSpace(propertyStringValue))
        return;
      
      // Если значение определилось не из фактов,
      // для подсветки заносим это свойство и результату не доверяем.
      if (fact == null)
      {
        var calculatedFact = recognitionResult.Info.Facts.AddNew();
        calculatedFact.PropertyName = propertyName;
        calculatedFact.PropertyValue = propertyStringValue;
        calculatedFact.IsTrusted = false;
      }
      else
      {
        if (isTrusted == null)
          isTrusted = IsTrustedField(fact, fieldName);
        
        var facts = recognitionResult.Info.Facts
          .Where(f => f.FactId == fact.Id)
          .Where(f => string.IsNullOrWhiteSpace(fieldName) || f.FieldName == fieldName);
        var factLabel = GetFactLabel(fact, propertyName);
        foreach (var recognizedFact in facts)
        {
          recognizedFact.PropertyName = propertyName;
          recognizedFact.PropertyValue = propertyStringValue;
          recognizedFact.IsTrusted = isTrusted;
          recognizedFact.FactLabel = factLabel;
          recognizedFact.CollectionRecordId = collectionRecordId;
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
          var propertyStringValue = Functions.Module.GetPropertyValueAsString(propertyValue);
          if (!string.IsNullOrWhiteSpace(propertyStringValue) && !Equals(propertyStringValue, linkedFact.PropertyValue))
            linkedFact.VerifiedValue = propertyStringValue;
        }
      }
      documentParams.Remove(Docflow.PublicConstants.OfficialDocument.NeedStoreVerifiedPropertiesValuesParamName);
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
      
      var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
      return field.Probability >= smartProcessingSettings.UpperConfidenceLimit;
    }
    
    /// <summary>
    /// Создать тело документа.
    /// </summary>
    /// <param name="document">Документ Rx.</param>
    /// <param name="recognitionResult">Результат обработки входящего документа в Арио.</param>
    /// <param name="versionNote">Примечание к версии.</param>
    public virtual void CreateVersion(IOfficialDocument document, Structures.Module.IRecognitionResult recognitionResult, string versionNote = "")
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
    public virtual Structures.Module.ISignatoryFactMatching GetAdresseeByFact(Sungero.Capture.Structures.Module.IFact fact)
    {
      var result = Structures.Module.SignatoryFactMatching.Create(Sungero.Company.Employees.Null, null, fact, false);
      
      if (fact == null)
        return result;
      
      var addressee = GetFieldValue(fact, FieldNames.Letter.Addressee);
      var employees =  Company.PublicFunctions.Employee.Remote.GetEmployeesByName(addressee);
      result.Employee = employees.FirstOrDefault();
      result.IsTrusted = (employees.Count() == 1) ? IsTrustedField(fact, FieldNames.Letter.Addressee) : false;
      return result;
    }
    
    /// <summary>
    /// Поиск сотрудника по ФИО из фактов и НОР.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="predictedClass">НОР.</param>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Сотрудник.</returns>
    public virtual List<IEmployee> GetOurEmployeeByFact(Sungero.Capture.Structures.Module.IFact fact,
                                                        IBusinessUnit businessUnit,
                                                        string predictedClass)
    {
      if (fact == null)
        return new List<IEmployee>();
      
      var fullName = GetFullNameByFact(predictedClass, fact);
      
      var businessUnitEmployees = Company.PublicFunctions.Employee.Remote.GetEmployeesByName(fullName);
      var employees = businessUnitEmployees.Where(e => e.Department.BusinessUnit.Equals(businessUnit)).ToList();
      return employees;
    }

    /// <summary>
    /// Получить полное ФИО из частей имени содержащихся в факте.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О." или "Фамилия Имя Отчество".</returns>
    public static string GetFullNameByFact(string predictedClass, Sungero.Capture.Structures.Module.IFact fact)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return string.Empty;
      
      var сontactFactNames = GetContactFactNames(predictedClass);
      
      var surname = GetFieldValue(fact, сontactFactNames.SurnameField);
      var name = GetFieldValue(fact, сontactFactNames.NameField);
      var patronymic = GetFieldValue(fact, сontactFactNames.PatronymicField);
      
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
      
      var surname = GetFieldValue(fact, FieldNames.Counterparty.SignatorySurname);
      var name = GetFieldValue(fact, FieldNames.Counterparty.SignatoryName);
      var patronymic = GetFieldValue(fact, FieldNames.Counterparty.SignatoryPatrn);
      
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
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <returns>Имя в формате "Фамилия И.О.".</returns>
    public virtual string GetShortNameByFact(string predictedClass, Sungero.Capture.Structures.Module.IFact fact)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return string.Empty;
      
      var ContactFactNames = GetContactFactNames(predictedClass);
      
      var surname = GetFieldValue(fact, ContactFactNames.SurnameField);
      var name = GetFieldValue(fact, ContactFactNames.NameField);
      var patronymic = GetFieldValue(fact, ContactFactNames.PatronymicField);
      return Parties.PublicFunctions.Person.GetSurnameAndInitialsInTenantCulture(name, patronymic, surname);
    }
    
    /// <summary>
    /// Получить контактные лица по данным из факта.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <param name="fact">Факт.</param>
    /// <param name="counterparty">Контрагент - владелец контактного лица.</param>
    /// <returns>Список контактных лиц.</returns>
    public virtual IQueryable<IContact> GetContactsByFact(string predictedClass, Sungero.Capture.Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if ((fact == null) || (predictedClass == string.Empty))
        return new List<IContact>().AsQueryable();
      
      var fullName = GetFullNameByFact(predictedClass, fact);
      var shortName = GetShortNameByFact(predictedClass, fact);
      
      return Parties.PublicFunctions.Contact.GetContactsByName(fullName, shortName, counterparty);
    }
    
    /// <summary>
    /// Получить контактное лицо по данным из факта и контрагента.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Структура, содержащая факт, контактное лицо и признак доверия.</returns>
    public virtual Structures.Module.ISignatoryFactMatching GetContactByFact(Sungero.Capture.Structures.Module.IFact fact,
                                                                             string propertyName,
                                                                             ICounterparty counterparty,
                                                                             string counterpartyPropertyName,
                                                                             string predictedClass)
    {
      var signedBy = Structures.Module.SignatoryFactMatching.Create(null, Sungero.Parties.Contacts.Null, fact, false);
      
      if ((fact == null) || (predictedClass == string.Empty))
        return signedBy;

      if (counterparty != null)
      {
        // Если для свойства propertyName по факту существует верифицированное ранее значение, то вернуть его.
        signedBy = GetContactByVerifiedData(fact, propertyName, counterparty.Id.ToString(), counterpartyPropertyName);
        if (signedBy.Contact != null)
          return signedBy;
      }
      
      var filteredContacts = GetContactsByFact(predictedClass, fact, counterparty);
      if (!filteredContacts.Any())
        return signedBy;
      
      signedBy.Contact = filteredContacts.FirstOrDefault();
      signedBy.IsTrusted = filteredContacts.Count() == 1;
      
      return signedBy;
    }
    
    /// <summary>
    /// Получить контактное лицо контрагента из верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид контрагента.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства контрагента.</param>
    /// <returns>Контактное лицо.</returns>
    public virtual Structures.Module.ISignatoryFactMatching GetContactByVerifiedData(Structures.Module.IFact fact,
                                                                                     string propertyName,
                                                                                     string counterpartyPropertyValue,
                                                                                     string counterpartyPropertyName)
    {
      var result = Structures.Module.SignatoryFactMatching.Create(null, Contacts.Null, fact, false);
      
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
    /// Получить название факта подписанта с названиями полей ФИО.
    /// </summary>
    /// <param name="predictedClass">Распознанный класс документа.</param>
    /// <returns>Наименование факта, наименования полей с ФИО подписанта.</returns>
    public static IContactFactNames GetContactFactNames(string predictedClass)
    {
      var contactFactNames = ContactFactNames.Create();
      
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
    
    /// <summary>
    /// Получить подписанта нашей стороны из фактов и НОР для договорного документа.
    /// </summary>
    /// <param name="recognitionResult">Результат обработки договора в Ario.</param>
    /// <param name="document">Договорной документ.</param>
    /// <param name="сounterparty">Найденная НОР.</param>
    /// <returns>Сотрудник.</returns>
    public virtual Structures.Module.ISignatoryFactMatching GetContractualDocumentSignatory(IRecognitionResult recognitionResult,
                                                                                            IContractualDocumentBase document,
                                                                                            ICounterpartyFactMatching organizationFactMatching)
    {
      var props = document.Info.Properties;
      var signatoryFacts = GetFacts(recognitionResult.Facts, FactNames.Counterparty);
      var signedBy = Structures.Module.SignatoryFactMatching.Create(null, null, null, false);
      var signatoryFieldNames = new List<string> {FieldNames.Counterparty.SignatorySurname,
        FieldNames.Counterparty.SignatoryName,
        FieldNames.Counterparty.SignatoryPatrn};
      
      if (organizationFactMatching == null)
        return signedBy;
      
      var isBusinessUnit = organizationFactMatching.BusinessUnit != null;
      
      var organizationFact = organizationFactMatching.Fact;
      if (organizationFact != null)
      {
        signedBy.Fact = organizationFact;
        var isOrganizationFactWithSignatory = GetFields(organizationFact, signatoryFieldNames).Any();
        
        if (isOrganizationFactWithSignatory)
          return isBusinessUnit
            ? GetContractualDocumentOurSignatoryByFact(organizationFact, document, recognitionResult.PredictedClass)
            : GetContactByFact(organizationFact, props.CounterpartySignatory.Name, document.Counterparty,
                               props.Counterparty.Name, recognitionResult.PredictedClass);
      }
      
      var organizationName = isBusinessUnit ? organizationFactMatching.BusinessUnit.Name : organizationFactMatching.Counterparty.Name;
      
      // Ожидаемое наименование НОР в формате {Название}, {ОПФ}.
      var organizationNameAndLegalForm = organizationName.Split(new string[] { ", " }, StringSplitOptions.None);

      signatoryFacts = signatoryFacts
        .Where(f => f.Fields.Any(fl => fl.Name == FieldNames.Counterparty.Name &&
                                 fl.Value.Equals(organizationNameAndLegalForm[0], StringComparison.InvariantCultureIgnoreCase)))
        .Where(f => f.Fields.Any(fl => fl.Name == FieldNames.Counterparty.LegalForm &&
                                 fl.Value.Equals(organizationNameAndLegalForm[1], StringComparison.InvariantCultureIgnoreCase))).ToList();

      signatoryFacts = signatoryFacts
        .Where(f => GetFields(f, signatoryFieldNames).Any()).ToList();
      
      var organizationSignatories = new List<ISignatoryFactMatching>();
      foreach (var fact in signatoryFacts)
      {
        Capture.Structures.Module.ISignatoryFactMatching signatory = null;
        if (isBusinessUnit)
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
    
    /// <summary>
    /// Получить подписанта нашей стороны из верифицированных данных.
    /// </summary>
    /// <param name="fact">Факт Арио.</param>
    /// <param name="propertyName">Имя связанного свойства.</param>
    /// <param name="counterpartyPropertyValue">Ид НОР.</param>
    /// <param name="counterpartyPropertyName">Имя связанного свойства НОР.</param>
    /// <returns>Сотрудник.</returns>
    public virtual Structures.Module.ISignatoryFactMatching GetEmployeeByVerifiedData(Structures.Module.IFact fact,
                                                                                      string propertyName,
                                                                                      string businessUnitPropertyValue,
                                                                                      string businessUnitPropertyName)
    {
      var result = Structures.Module.SignatoryFactMatching.Create(null, null, fact, false);
      var employeeField = GetFieldByVerifiedData(fact, propertyName, businessUnitPropertyValue, businessUnitPropertyName);
      if (employeeField == null)
        return result;
      int employeeId;
      if (!int.TryParse(employeeField.VerifiedValue, out employeeId))
        return result;
      
      var filteredEmloyee = Employees.GetAll(x => x.Id == employeeId).FirstOrDefault();
      if (filteredEmloyee != null)
      {
        result.Employee = filteredEmloyee;
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
    public virtual IQueryable<Sungero.Contracts.IContractualDocument> GetLeadingDocuments(Structures.Module.IFact fact, ICounterparty counterparty)
    {
      if (fact == null)
        return new List<Sungero.Contracts.IContractualDocument>().AsQueryable();
      
      var docDate = GetFieldDateTimeValue(fact, FieldNames.FinancialDocument.DocumentBaseDate);
      var number = GetFieldValue(fact, FieldNames.FinancialDocument.DocumentBaseNumber);
      
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
    public virtual IContractFactMatching GetLeadingDocument(IFact fact, string leadingDocPropertyName,
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
      result.IsTrusted = (contracts.Count() == 1) ? IsTrustedField(fact, FieldNames.FinancialDocument.DocumentBaseNumber) : false;
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
    public virtual Structures.Module.IContractFactMatching GetContractByVerifiedData(Structures.Module.IFact fact,
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
    
    /// <summary>
    /// Получить подписанта нашей стороны для договорного документа по факту.
    /// </summary>
    /// <param name="fact">Факт.</param>
    /// <param name="document">Документ.</param>
    /// <param name="predictedClass">Результат обработки договора в Ario.</param>
    /// <returns>Структура, содержащая сотрудника, факт и признак доверия.</returns>
    public virtual ISignatoryFactMatching GetContractualDocumentOurSignatoryByFact(Structures.Module.IFact fact,
                                                                                   IContractualDocumentBase document,
                                                                                   string predictedClass)
    {
      var signedBy = Structures.Module.SignatoryFactMatching.Create(null, null, fact, false);
      var businessUnit = document.BusinessUnit;
      
      if (fact == null || string.IsNullOrWhiteSpace(predictedClass))
        return signedBy;

      if (businessUnit != null)
      {
        // Если для свойства Подписал по факту существует верифицированное ранее значение, то вернуть его.
        signedBy = GetEmployeeByVerifiedData(fact,
                                             document.Info.Properties.OurSignatory.Name,
                                             businessUnit.Id.ToString(),
                                             document.Info.Properties.BusinessUnit.Name);
        if (signedBy.Employee != null)
          return signedBy;
      }
      
      var filteredEmloyees = GetOurEmployeeByFact(fact, businessUnit, predictedClass);
      if (!filteredEmloyees.Any())
        return signedBy;
      
      signedBy.Employee = filteredEmloyees.FirstOrDefault();
      signedBy.IsTrusted = (filteredEmloyees.Count() == 1);
      
      return signedBy;
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