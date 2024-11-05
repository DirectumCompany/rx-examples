# rx-examples

Репозиторий с примерами перекрытия прикладной разработки.

## Порядок установки

1. Для работы требуется установленный Directum RX соответствующей версии.
2. Склонировать репозиторий rx-examples в папку.
3. Указать в _ConfigSettings.xml DDS:
```xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" />
  <repository folderName="RX" solutionType="Base" url="<адрес локального репозитория>" />
  <repository folderName="<Папка из п.2>" solutionType="Work" 
    url="https://github.com/DirectumCompany/rx-examples" />
</block>
```

## Кейсы

### Согласование по регламенту
Добавлена роль "Сотрудники подразделения инициатора" с несколькими исполнителями. Роль можно указать в качестве исполнителя в этапах согласования с типом "Согласование".
<br>**Точки входа:** серверные методы [GetRolePerformers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ApprovalRole/ApprovalRoleServerFunctions.cs#L17-L23), [GetInitiatorDepartmentEmployeesRolePerformers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ApprovalRole/ApprovalRoleServerFunctions.cs#L30-L40) и [GetStageRecipients](https://github.com/DirectumCompany/rx-examples/blob/e4d9f7f59195ad58abfc05e4146c6534c426e674/src/Packages/Sungero.Examples/Sungero.Examples.Server/ApprovalStage/ApprovalStageServerFunctions.cs#L12-L28) справочника ApprovalRole, серверный метод [GetMultipleMembersRoles](https://github.com/DirectumCompany/rx-examples/blob/e4d9f7f59195ad58abfc05e4146c6534c426e674/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L123-L129) модуля Docflow, а также шаредный метод [GetPossibleRoles](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Shared/ApprovalStage/ApprovalStageSharedFunctions.cs#L16-L24).

### Преобразование в PDF и наложение отметки об ЭП

1. Все документы. Изменен логотип и цвет отметки на фиолетовый. В отметку добавлена дата и время подписания. 
<br>**Точки входа:** серверные методы [GetSignatureMarkForSimpleSignatureAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L22-L35) и [GetSignatureMarkForCertificateAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L48-L74) модуля Docflow.

2. Входящий счёт. Цвет отметки изменён на красный, текст отметки изменен на "ПРИНЯТО К ОПЛАТЕ".
<br>**Точка входа:** серверный метод [GetSignatureMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L17-L29) документа IncomingInvoice. 

3. Все документы. Изменено условие для интерактивного преобразования документов. Теперь изображения в формате jpg размером < 1 Mb конвертируются интерактивно. 
<br>**Точка входа:** серверный метод [CanConvertToPdfInteractively](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L81-L88) модуля Docflow. 

4. Служебная записка. На первой странице документа, в верхнем левом углу поставить отметки о всех подписях документа. Отметки ставятся сверху вниз, с небольшим отступом друг от друга. 
<br>**Точка входа:** серверный метод [ConvertToPdfAndAddSignatureMark](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L48-L128) документа Memo. 

5. Договор и дополнительное соглашение. Постраничная отметка об ЭП с поворотом. Редактор отметок скрыт, т.к. данный вид отметки не поддерживается.
<br>**Точки входа:** серверные методы [ConvertToPdfWithMarks](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/ContractualDocument/ContractualDocumentServerFunctions.cs#L17-L24), [UpdateContractPaginalApproveMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/ContractualDocument/ContractualDocumentServerFunctions.cs#L30-L48), [GetContractualApprovedMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/ContractualDocument/ContractualDocumentServerFunctions.cs#L56-L68) документа ContractualDocument, серверный метод [GetApprovedMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Contract/ContractServerFunctions.cs#L17-L20) документа Contract, серверный метод [GetApprovedMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/SupAgreement/SupAgreementServerFunctions.cs#L17-L20) документа SupAgreement
<br>**Инициализация:** [CreateCustomMarkKinds](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleInitializer.cs#L21-L43) модуля Docflow.
<br>**Константы:** [PaginalApproveMarkKindSid](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Contract/ContractConstants.cs#L10), [PaginalApproveMarkKindClass](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Contract/ContractConstants.cs#L14), [PaginalApproveMarkKindMethod](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Contract/ContractConstants.cs#L18) документа Contract, [PaginalApproveMarkKindSid](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/SupAgreement/SupAgreementConstants.cs#L10), [PaginalApproveMarkKindClass](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/SupAgreement/SupAgreementConstants.cs#L14), [PaginalApproveMarkKindMethod](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/SupAgreement/SupAgreementConstants.cs#L18) документа SupAgreement.

6. Служебная записка. Простановка на первой странице отметок о всех подписях документа. Редактор отметок скрыт, т.к. данный вид отметки не поддерживается.
<br>**Точки входа:** серверные методы [ConvertToPdfWithMarks](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L19-L25), [UpdateMemoSignMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L31-L44), [GetOrCreateSignatureBasedMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L52-L69), [GetOrCreateSignatureMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L76-L80), [GetMemoSignMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L87-L91), [GetDocumentSignatures](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L98-L105) документы Memo, серверные методы [GetContent](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Mark/MarkServerFunctions.cs#L17-L36), [GetContentWithSignerInfo](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Mark/MarkServerFunctions.cs#L44-L50) справочника Mark.
<br>**Инициализация:** [CreateCustomMarkKinds](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleInitializer.cs#L21-L43) модуля Docflow.
<br>**Константы:** [SignMarkKindSid](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Memo/MemoConstants.cs#L10), [SignMarkKindClass](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Memo/MemoConstants.cs#L14), [SignMarkKindMethod](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Memo/MemoConstants.cs#L18), [MarkSignatureIdKey](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Memo/MemoConstants.cs#L22) документа Memo.

7. Протокол. Простановка на пустой странице отметки со всеми подписями документа. Редактор отметок скрыт, т.к. данный вид отметки не поддерживается.
<br>**Точки входа:** серверные методы [GetOrCreateSignatureMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Minutes/MinutesServerFunctions.cs#L16-L23), [DeleteVersionMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Minutes/MinutesServerFunctions.cs#L31-L38), [GetMinutesMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Minutes/MinutesServerFunctions.cs#L45-L66) документа Minute.
<br>**Инициализация:** [CreateCustomMarkKinds](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleInitializer.cs#L21-L43) модуля Docflow.
<br>**Константы:** [MinutesMarkKindGuid](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Minutes/MinutesConstants.cs#L9), [MinutesMarkKindClass](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Minutes/MinutesConstants.cs#L12), [MinutesMarkKindMethod](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/Minutes/MinutesConstants.cs#L15) документа Minute

### Преобразование в PDF и наложение отметки об оплате

1. Входящий счет. Простановка отметки об оплате.
<br>**Точки входа:** серверный метод [ConvertToPdfWithMarks](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L18-L25), [UpdateInvoicePaymentMark](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L33-L48), [GetPaymentMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L54-L57) документа IncomingInvoice.
<br>**Инициализация:** [CreateCustomMarkKinds](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleInitializer.cs#L21-L43) модуля Docflow.
<br>**Константы:** [PaymentMarkKindSid](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/IncomingInvoice/IncomingInvoiceConstants.cs#L10), [PaymentMarkKindClass](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/IncomingInvoice/IncomingInvoiceConstants.cs#L14), [PaymentMarkKindMethod](https://github.com/DirectumCompany/rx-examples/blob/4.11/src/Packages/Sungero.Examples/Sungero.Examples.Shared/IncomingInvoice/IncomingInvoiceConstants.cs#L18) документа IncomingInvoice.

### Преобразование в PDF и наложение отметки о поступлении
Входящее письмо. Цвет отметки о поступлении изменён на фиолетовый, изменена толщина рамки, расположение (отметка сдвинута ближе к центру), добавлено указание подразделения.
<br>**Точки входа:** серверные методы IncomingLetter: [AddRegistrationStamp](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingLetter/IncomingLetterServerFunctions.cs#L47-L50) и [GetRegistrationStampAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingLetter/IncomingLetterServerFunctions.cs#L17-L38).

### Политики перемещения 
Добавлено событие "Обращение к документу" в критерии перемещения. Документ перемещается в новое хранилище, если прошло указанное время с даты последнего обращения к документу. 
<br>**Точки входа:** серверные методы [GetStoragePolicySettingsQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L95-L98), [GetDocumentsToTransferQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L104-L107) модуля Docflow.
 
### Интеллектуальная обработка
Дополнительный классификатор в настройках интеллектуальной обработки. 
<br>**Точки входа:** серверный метод [FillSmartAdditionalClassifiers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.SmartProcessing/ModuleInitializer.cs#L25-L42) в инициализации модуля SmartProcessing. Серверный метод [CreateIncomingLetter](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ModuleServerFunctions.cs#L22-L35) в решении Examples.

### Верификация комплекта документов

1. Добавлен пример расширения схемы варианта процесса [Верификация комплекта документов](https://github.com/DirectumCompany/rx-examples/tree/master/env/ProcessKinds_Examples/VerificationTask_EndToEndProcesses.datx): после задания на верификацию происходит автоматическая отправка входящих писем на рассмотрение руководителем, а договоров на согласование по регламенту.

2. Добавлен блок-задача [Отправка задачи на рассмотрение](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.RecordManagement/ModuleBlockHandlers.cs#L13-L28) в перекрытии модуля RecordManagement.

3. Добавлен блок-задача [Отправка задачи на согласование по регламенту](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleBlockHandlers.cs#L13-L37) в перекрытии модуля Docflow.
 
### Валидация панели фильтрации 
Список "Входящие документы" на обложке модуля Делопроизводство. Реализовано ограничение списка "Входящие документы" при помощи валидации в панели фильтрации. Должен быть заполнен один из критериев: журнал регистрации, от кого, произвольный период. 
<br>**Точка входа:** действие [IncomingDocumentsValidateFilterPanel](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.ClientBase/Sungero.RecordManagementUI/ModuleHandlers.cs#L12-L16) вычисляемой папки модуля Делопроизводство.

### Интеграция с 1С

1. Открытие договора 1С по кнопке из карточки договора Directum RX (пример открытия записи, связанной через ExternalEntityLink).
<br>**Точка входа:** действие [OpenEntity1CSungero](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.ClientBase/Contract/ContractActions.cs#L12-L25) документа Contract.

2. Открытие входящего счёта 1С по кнопке из карточки входящего счета Directum RX (пример открытия записи с поиском по реквизитам). 
<br>**Точка входа:** действие [OpenEntity1CSungero](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.ClientBase/IncomingInvoice/IncomingInvoiceActions.cs#L12-L25) документа IncomingInvoice. 

3. Новый этап регламента "Создание входящего счета в 1С". В рамках данного этапа создается входящий счет в 1С на основе данных входящего счета Directum RX.
<br>**Точка входа:** серверный метод [Execute](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.ContractsExample/Sungero.ContractsExample.Server/ApprovalCreateIncInvoice1CStage/ApprovalCreateIncInvoice1CStageServerFunctions.cs#L18-L61) справочника ApprovalCreateIncInvoice1CStage. 