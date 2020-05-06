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

    #region Входящее письмо

    /// <summary>
    /// Создать входящее письмо (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки письма в Ario.</param>
    /// <returns>Документ.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingLetter(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var document = Sungero.Capture.MockIncomingLetters.Create();
      var props = document.Info.Properties;
      var facts = arioDocument.Facts;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      
      // Заполнить номер со стороны корреспондента.
      var recognizedNumber = Docflow.PublicFunctions.Module.GetRecognizedNumber(facts, FactNames.Letter, FieldNames.Letter.Number, props.InNumber);
      if (recognizedNumber.Fact != null)
      {
        document.InNumber = recognizedNumber.Number;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, recognizedNumber.Fact, 
                                                   FieldNames.Letter.Number, props.InNumber.Name, 
                                                   document.InNumber, recognizedNumber.Probability);
      }
      
      // Заполнить дату со стороны корреспондента.
      var recognizedDate = DocflowPublicFunctions.GetRecognizedDate(facts, FactNames.Letter, FieldNames.Letter.Date);
      Sungero.Docflow.PublicFunctions.OfficialDocument.FillDocumentDate(document,
                                                                        arioDocument.RecognitionInfo,
                                                                        recognizedDate,
                                                                        FieldNames.Letter.Date,
                                                                        props.Dated.Name);
      
      // Заполнить данные корреспондента.
      var correspondentNameFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.CorrespondentName);
      if (correspondentNameFacts.Count() > 0)
      {
        var fact = correspondentNameFacts.First();
        document.Correspondent = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Letter.CorrespondentName, props.Correspondent.Name, document.Correspondent);
      }
      if (correspondentNameFacts.Count() > 1)
      {
        var fact = correspondentNameFacts.Last();
        document.Recipient = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Letter.CorrespondentName, FieldNames.Letter.CorrespondentLegalForm);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Letter.CorrespondentName, props.Recipient.Name, document.Recipient);
      }
      
      // Заполнить ИНН/КПП для КА и НОР.
      var tinTrrcFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.CorrespondentTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.CorrespondentTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.CorrespondentTin.Name, document.CorrespondentTin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.CorrespondentTrrc.Name, document.CorrespondentTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.RecipientTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.RecipientTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.RecipientTin.Name, document.RecipientTin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.RecipientTrrc.Name, document.RecipientTrrc);
      }
      
      // В ответ на.
      var responseToNumberFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToNumber).FirstOrDefault();
      var responseToNumber = DocflowPublicFunctions.GetFieldValue(responseToNumberFact, FieldNames.Letter.ResponseToNumber);
      var responseToDateFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate).FirstOrDefault();
      var responseToDate = Functions.Module.GetShortDate(DocflowPublicFunctions.GetFieldValue(facts, FactNames.Letter, FieldNames.Letter.ResponseToDate));
      document.InResponseTo = string.IsNullOrEmpty(responseToDate)
        ? responseToNumber
        : string.Format("{0} {1} {2}", responseToNumber, Sungero.Docflow.Resources.From, responseToDate);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, responseToNumberFact, FieldNames.Letter.ResponseToNumber, props.InResponseTo.Name, document.InResponseTo);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, responseToDateFact, FieldNames.Letter.ResponseToDate, props.InResponseTo.Name, document.InResponseTo);
      
      // Заполнить подписанта.
      var personFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.LetterPerson, FieldNames.LetterPerson.Surname);
      var predictedClass = arioDocument.PredictedClass;
      if (document.Signatory == null)
      {
        var signatoryFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Signatory).FirstOrDefault();
        document.Signatory = Docflow.Server.ModuleFunctions.GetFullNameByFact(predictedClass, signatoryFact);
        var probability = DocflowPublicFunctions.GetFieldProbability(signatoryFact, FieldNames.LetterPerson.Type);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, signatoryFact, null, props.Signatory.Name, document.Signatory, probability);
      }
      
      // Заполнить контакт.
      if (document.Contact == null)
      {
        var responsibleFact = personFacts.Where(x => DocflowPublicFunctions.GetFieldValue(x, FieldNames.LetterPerson.Type) == LetterPersonTypes.Responsible).FirstOrDefault();
        document.Contact = Docflow.Server.ModuleFunctions.GetFullNameByFact(predictedClass, responsibleFact);
        var probability = DocflowPublicFunctions.GetFieldProbability(responsibleFact, FieldNames.LetterPerson.Type);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, responsibleFact, null, props.Contact.Name, document.Contact, probability);
      }
      
      // Заполнить данные нашей стороны.
      var addresseeFacts = DocflowPublicFunctions.GetFacts(facts, FactNames.Letter, FieldNames.Letter.Addressee);
      foreach (var fact in addresseeFacts)
      {
        var addressee = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Letter.Addressee);
        document.Addressees = string.IsNullOrEmpty(document.Addressees) ? addressee : string.Format("{0}; {1}", document.Addressees, addressee);
      }
      foreach (var fact in addresseeFacts)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, null, props.Addressees.Name,
                                                   document.Addressees,
                                                   Docflow.Constants.Module.PropertyProbabilityLevels.Max);
      
      // Заполнить содержание перед сохранением, чтобы сформировалось наименование.
      var subjectFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Letter, FieldNames.Letter.Subject).FirstOrDefault();
      var subject = DocflowPublicFunctions.GetFieldValue(subjectFact, FieldNames.Letter.Subject);
      if (!string.IsNullOrEmpty(subject))
      {
        document.Subject = string.Format("{0}{1}", subject.Substring(0, 1).ToUpper(), subject.Remove(0, 1).ToLower());
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, subjectFact, FieldNames.Letter.Subject, props.Subject.Name, document.Subject);
      }
      
      return document;
    }
    
    #endregion
    
    #region Акт
    
    /// <summary>
    /// Создать акт выполненных работ (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки акта выполненных работ в Ario.</param>
    /// <returns>Акт выполненных работ.</returns>
    public Docflow.IOfficialDocument CreateMockContractStatement(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var document = Sungero.Capture.MockContractStatements.Create();
      var props = document.Info.Properties;
      
      // Заполнить основные свойства.
      FillDocumentKind(document);
      var facts = arioDocument.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.LeadDoc = GetLeadingDocumentName(leadingDocFact);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, leadingDocFact, null, props.LeadDoc.Name,
                                                 document.LeadDoc,
                                                 Docflow.Constants.Module.PropertyProbabilityLevels.Max);
      
      // Дата и номер.
      FillMockRegistrationData(document, arioDocument.RecognitionInfo, arioDocument.Facts, FactNames.Document);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.CounterpartyName = seller.Name;
        document.CounterpartyTin = seller.Tin;
        document.CounterpartyTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, seller.Trrc);
      }
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BusinessUnitName = buyer.Name;
        document.BusinessUnitTin = buyer.Tin;
        document.BusinessUnitTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, buyer.Trrc);
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
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.CounterpartyName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.LegalForm, props.CounterpartyName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.CounterpartyTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.CounterpartyTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BusinessUnitName))
          {
            document.BusinessUnitName = name;
            document.BusinessUnitTin = tin;
            document.BusinessUnitTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.BusinessUnitName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.LegalForm, props.BusinessUnitName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.BusinessUnitTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.BusinessUnitTrrc.Name, trrc);
          }
        }
      }
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
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
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      return document;
    }
    
    #endregion
    
    #region Накладная
    
    /// <summary>
    /// Создать накладную (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки накладной в Ario.</param>
    /// <returns>Накладная.</returns>
    public Docflow.IOfficialDocument CreateMockWaybill(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var document = Sungero.Capture.MockWaybills.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = arioDocument.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var probability = DocflowPublicFunctions.GetFieldProbability(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, leadingDocFact, null, props.Contract.Name, document.Contract, probability);
      
      // Заполнить контрагентов по типу.
      // Тип передается либо со 100% вероятностью, либо не передается ни тип, ни наименование контрагента.
      var shipper = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Shipper);
      if (shipper != null)
      {
        document.Shipper = shipper.Name;
        document.ShipperTin = shipper.Tin;
        document.ShipperTrrc = shipper.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.Name, props.Shipper.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.LegalForm, props.Shipper.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.Consignee = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.Name, props.Consignee.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.LegalForm, props.Consignee.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var supplier = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Supplier);
      if (supplier != null)
      {
        document.Supplier = supplier.Name;
        document.SupplierTin = supplier.Tin;
        document.SupplierTrrc = supplier.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, supplier.Fact, FieldNames.Counterparty.Name, props.Supplier.Name, supplier.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, supplier.Fact, FieldNames.Counterparty.LegalForm, props.Supplier.Name, supplier.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, supplier.Fact, FieldNames.Counterparty.TIN, props.SupplierTin.Name, supplier.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, supplier.Fact, FieldNames.Counterparty.TRRC, props.SupplierTrrc.Name, supplier.Trrc);
      }
      
      var payer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Payer);
      if (payer != null)
      {
        document.Payer = payer.Name;
        document.PayerTin = payer.Tin;
        document.PayerTrrc = payer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, payer.Fact, FieldNames.Counterparty.Name, props.Payer.Name, payer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, payer.Fact, FieldNames.Counterparty.LegalForm, props.Payer.Name, payer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, payer.Fact, FieldNames.Counterparty.TIN, props.PayerTin.Name, payer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, payer.Fact, FieldNames.Counterparty.TRRC, props.PayerTrrc.Name, payer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, arioDocument.RecognitionInfo, facts, FactNames.FinancialDocument);
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
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
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      
      return document;
    }

    #endregion
    
    #region Счет-фактура
    
    /// <summary>
    /// Создать счет-фактуру (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки счет-фактуры в Ario.</param>
    /// <returns>Счет-фактура.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingTaxInvoice(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var facts = arioDocument.Facts;
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
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.Name, props.ShipperName.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.LegalForm, props.ShipperName.Name, shipper.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.TIN, props.ShipperTin.Name, shipper.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, shipper.Fact, FieldNames.Counterparty.TRRC, props.ShipperTrrc.Name, shipper.Trrc);
      }
      
      var consignee = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Consignee);
      if (consignee != null)
      {
        document.ConsigneeName = consignee.Name;
        document.ConsigneeTin = consignee.Tin;
        document.ConsigneeTrrc = consignee.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.Name, props.ConsigneeName.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.LegalForm, props.ConsigneeName.Name, consignee.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.TIN, props.ConsigneeTin.Name, consignee.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, consignee.Fact, FieldNames.Counterparty.TRRC, props.ConsigneeTrrc.Name, consignee.Trrc);
      }
      
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
      }
      
      // Дата и номер.
      FillMockRegistrationData(document, arioDocument.RecognitionInfo, facts, FactNames.FinancialDocument);
      document.IsAdjustment = false;
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
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
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Name, string.Format(formatter, props.Goods.Properties.Name.Name),
                                                   good.Name, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.UnitName, string.Format(formatter, props.Goods.Properties.UnitName.Name),
                                                   good.UnitName, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Count, string.Format(formatter, props.Goods.Properties.Count.Name),
                                                   good.Count, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Price, string.Format(formatter, props.Goods.Properties.Price.Name),
                                                   good.Price, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.VatAmount, string.Format(formatter, props.Goods.Properties.VatAmount.Name),
                                                   good.VatAmount, null, good.Id);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Goods.Amount, string.Format(formatter, props.Goods.Properties.TotalAmount.Name),
                                                   good.TotalAmount, null, good.Id);
      }
      
      return document;
    }
    
    #endregion
    
    #region Счет на оплату
    
    /// <summary>
    /// Создать счет на оплату (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки счета на оплату в Ario.</param>
    /// <returns>Счет на оплату.</returns>
    public Docflow.IOfficialDocument CreateMockIncomingInvoice(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var document = Sungero.Capture.MockIncomingInvoices.Create();
      var props = document.Info.Properties;
      
      // Основные свойства.
      FillDocumentKind(document);
      var facts = arioDocument.Facts;
      
      // Договор.
      var leadingDocFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.DocumentBaseName).FirstOrDefault();
      document.Contract = GetLeadingDocumentName(leadingDocFact);
      var probability = DocflowPublicFunctions.GetFieldProbability(leadingDocFact, FieldNames.FinancialDocument.DocumentBaseName);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, leadingDocFact, null, props.Contract.Name, document.Contract, probability);
      
      // Заполнить контрагентов по типу.
      var seller = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Seller);
      if (seller != null)
      {
        document.SellerName = seller.Name;
        document.SellerTin = seller.Tin;
        document.SellerTrrc = seller.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.Name, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, seller.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, seller.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, seller.Fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, seller.Trrc);
      }
      
      var buyer = GetMostProbableMockCounterparty(facts, CounterpartyTypes.Buyer);
      if (buyer != null)
      {
        document.BuyerName = buyer.Name;
        document.BuyerTin = buyer.Tin;
        document.BuyerTrrc = buyer.Trrc;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.Name, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, buyer.Name);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, buyer.Tin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, buyer.Fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, buyer.Trrc);
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
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.SellerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.LegalForm, props.SellerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.SellerTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.SellerTrrc.Name, trrc);
          }
          // Если контрагент уже заполнен, то занести наименование, ИНН/КПП для нашей стороны.
          else if (string.IsNullOrWhiteSpace(document.BuyerName))
          {
            document.BuyerName = name;
            document.BuyerTin = tin;
            document.BuyerTrrc = trrc;
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.BuyerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.LegalForm, props.BuyerName.Name, name);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.BuyerTin.Name, tin);
            DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.BuyerTrrc.Name, trrc);
          }
        }
      }
      
      // Дата и номер.
      var recognizedDate = DocflowPublicFunctions.GetRecognizedDate(facts, FactNames.FinancialDocument, FieldNames.FinancialDocument.Date);
      Sungero.Docflow.PublicFunctions.OfficialDocument.FillDocumentDate(document,
                                                                        arioDocument.RecognitionInfo,
                                                                        recognizedDate,
                                                                        FieldNames.FinancialDocument.Date,
                                                                        props.Date.Name);
      
      // Номер.
      var recognizedNumber = Docflow.PublicFunctions.Module.GetRecognizedNumber(facts,
                                                                                FactNames.FinancialDocument,
                                                                                FieldNames.FinancialDocument.Number,
                                                                                props.Number);
      if (recognizedNumber.Fact != null)
      {
        document.Number = recognizedNumber.Number;
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, recognizedNumber.Fact, FieldNames.FinancialDocument.Number,
                                                   props.Number.Name, document.Number, recognizedNumber.Probability);
      }
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
      return document;
    }

    #endregion
    
    #region Договор
    
    /// <summary>
    /// Создать договор (демо режим).
    /// </summary>
    /// <param name="arioDocument">Результат обработки договора в Ario.</param>
    /// <returns>Договор.</returns>
    public Docflow.IOfficialDocument CreateMockContract(Sungero.SmartProcessing.Structures.Module.IArioDocument arioDocument)
    {
      var document = Sungero.Capture.MockContracts.Create();
      
      // Основные свойства.
      FillDocumentKind(document);
      document.Name = document.DocumentKind.ShortName;
      var props = document.Info.Properties;
      var facts = arioDocument.Facts;
      
      // Дата и номер.
      FillMockRegistrationData(document, arioDocument.RecognitionInfo, facts, FactNames.Document);
      
      // Заполнить данные сторон.
      var partyNameFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.Name);
      if (partyNameFacts.Count() > 0)
      {
        var fact = partyNameFacts.First();
        document.FirstPartyName = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.FirstPartySignatory = Docflow.Server.ModuleFunctions.GetFullNameByFactForContract(fact);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.FirstPartyName.Name, document.FirstPartyName);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatorySurname, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatoryName, props.FirstPartySignatory.Name, document.FirstPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatoryPatrn, props.FirstPartySignatory.Name, document.FirstPartySignatory);
      }
      if (partyNameFacts.Count() > 1)
      {
        var fact = partyNameFacts.Last();
        document.SecondPartyName = DocflowPublicFunctions.GetCounterpartyName(fact, FieldNames.Counterparty.Name, FieldNames.Counterparty.LegalForm);
        document.SecondPartySignatory = Docflow.Server.ModuleFunctions.GetFullNameByFactForContract(fact);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.Name, props.SecondPartyName.Name, document.SecondPartyName);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatorySurname, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatoryName, props.SecondPartySignatory.Name, document.SecondPartySignatory);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.SignatoryPatrn, props.SecondPartySignatory.Name, document.SecondPartySignatory);
      }
      
      // Заполнить ИНН/КПП сторон.
      var tinTrrcFacts = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.Counterparty, FieldNames.Counterparty.TIN);
      if (tinTrrcFacts.Count() > 0)
      {
        var fact = tinTrrcFacts.First();
        document.FirstPartyTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.FirstPartyTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.FirstPartyTin.Name, document.FirstPartyTin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.FirstPartyTrrc.Name, document.FirstPartyTrrc);
      }
      
      if (tinTrrcFacts.Count() > 1)
      {
        var fact = tinTrrcFacts.Last();
        document.SecondPartyTin = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TIN);
        document.SecondPartyTrrc = DocflowPublicFunctions.GetFieldValue(fact, FieldNames.Counterparty.TRRC);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TIN, props.SecondPartyTin.Name, document.SecondPartyTin);
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, fact, FieldNames.Counterparty.TRRC, props.SecondPartyTrrc.Name, document.SecondPartyTrrc);
      }
      
      // Сумма и валюта.
      var documentAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Amount).FirstOrDefault();
      document.TotalAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentAmountFact, FieldNames.DocumentAmount.Amount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentAmountFact, FieldNames.DocumentAmount.Amount, props.TotalAmount.Name, document.TotalAmount);
      
      var documentVatAmountFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.VatAmount).FirstOrDefault();
      document.VatAmount = DocflowPublicFunctions.GetFieldNumericalValue(documentVatAmountFact, FieldNames.DocumentAmount.VatAmount);
      DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentVatAmountFact, FieldNames.DocumentAmount.VatAmount, props.VatAmount.Name, document.VatAmount);
      
      var documentCurrencyFact = DocflowPublicFunctions.GetOrderedFacts(facts, FactNames.DocumentAmount, FieldNames.DocumentAmount.Currency).FirstOrDefault();
      var currencyCode = DocflowPublicFunctions.GetFieldValue(documentCurrencyFact, FieldNames.DocumentAmount.Currency);
      document.Currency = Commons.Currencies.GetAll(x => x.NumericCode == currencyCode).FirstOrDefault();
      if (document.Currency != null)
        DocflowPublicFunctions.LinkFactAndProperty(arioDocument.RecognitionInfo, documentCurrencyFact, FieldNames.DocumentAmount.Currency, props.Currency.Name, document.Currency.Id);
      
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
    /// Заполнить номер и дату для Mock-документов.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="recognitionInfo">Запись в справочнике для сохранения результатов распознавания документа.</param>
    /// <param name="facts">Извлеченные из документа факты.</param>
    /// <param name="factName">Наименование факта с датой и номером документа.</param>
    public void FillMockRegistrationData(IOfficialDocument document,
                                         Commons.IEntityRecognitionInfo recognitionInfo,
                                         List<Docflow.Structures.Module.IArioFact> facts,
                                         string factName)
    {
      var props = document.Info.Properties;
      
      // Дата.
      var recognizedDate = DocflowPublicFunctions.GetRecognizedDate(facts, factName, FieldNames.Document.Date);
      Sungero.Docflow.PublicFunctions.OfficialDocument.FillDocumentDate(document,
                                                                        recognitionInfo,
                                                                        recognizedDate,
                                                                        FieldNames.Document.Date,
                                                                        props.RegistrationDate.Name);
      
      // Номер.
      var recognizedNumber = Docflow.PublicFunctions.Module.GetRecognizedNumber(facts, factName, FieldNames.Document.Number, props.RegistrationNumber);
      if (recognizedNumber.Fact != null)
      {
        document.RegistrationNumber = recognizedNumber.Number;
        DocflowPublicFunctions.LinkFactAndProperty(recognitionInfo, recognizedNumber.Fact, FieldNames.Document.Number,
                                                   props.RegistrationNumber.Name, document.RegistrationNumber,
                                                   recognizedNumber.Probability);
      }
    }
    
    #endregion
    
    #region Поиск контрагента/НОР
    
    /// <summary>
    /// Поиск контрагента для документов в демо режиме.
    /// </summary>
    /// <param name="facts">Факты для поиска факта с контрагентом.</param>
    /// <param name="counterpartyType">Тип контрагента.</param>
    /// <returns>Контрагент.</returns>
    public static Structures.Module.MockCounterparty GetMostProbableMockCounterparty(List<Sungero.Docflow.Structures.Module.IArioFact> facts, string counterpartyType)
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
    /// Получить наименование ведущего документа.
    /// </summary>
    /// <param name="fact">Исходный факт, содержащий наименование ведущего документа.</param>
    /// <returns>Наименование ведущего документа с номером и датой.</returns>
    private static string GetLeadingDocumentName(Sungero.Docflow.Structures.Module.IArioFact fact)
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
    
    #endregion
  }
}