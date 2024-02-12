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

## Верификация комплекта документов

1. Добавлен пример расширения схемы варианта процесса [Верификация комплекта документов](https://github.com/DirectumCompany/rx-examples/tree/master/env/ProcessKinds_Examples/VerificationTask_EndToEndProcesses.datx): после задания на верификацию происходит автоматическая отправка входящих писем на рассмотрение руководителем, а договоров на согласование по регламенту.

2. Добавлен блок-задача [Отправка задачи на рассмотрение](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.RecordManagement/ModuleBlockHandlers.cs#L13-L28) в перекрытии модуля RecordManagement.

3. Добавлен блок-задача [Отправка задачи на согласование по регламенту](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleBlockHandlers.cs#L13-L37) в перекрытии модуля Docflow.

### Согласование по регламенту
Добавлена роль "Сотрудники подразделения инициатора" с несколькими исполнителями. Роль можно указать в качестве исполнителя в этапах согласования с типом "Согласование".
<br>**Точки входа:** серверные методы [GetRolePerformers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ApprovalRole/ApprovalRoleServerFunctions.cs#L17-L23) и  [GetInitiatorDepartmentEmployeesRolePerformers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ApprovalRole/ApprovalRoleServerFunctions.cs#L30-L40) справочника ApprovalRole,
а также шаредный метод [GetPossibleRoles](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Shared/ApprovalStage/ApprovalStageSharedFunctions.cs#L16-L24).

### Преобразование в PDF и наложение отметки об ЭП

1. Все документы. Изменен логотип и цвет отметки на фиолетовый. В отметку добавлена дата и время подписания. 
<br>**Точки входа:** серверные методы [GetSignatureMarkForSimpleSignatureAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L22-L35) и [GetSignatureMarkForCertificateAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L48-L74) модуля Docflow.

2. Входящий счёт. Цвет отметки изменён на красный, текст отметки изменен на "ПРИНЯТ К ОПЛАТЕ". 
<br>**Точка входа:** серверный метод [GetSignatureMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L17-L29) документа IncomingInvoice. 

3. Все документы. Изменено условие для интерактивного преобразования документов. Теперь изображения в формате jpg размером < 1 Mb конвертируются интерактивно. 
<br>**Точка входа:** серверный метод [CanConvertToPdfInteractively](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L81-L88) модуля Docflow. 

4. Служебная записка. На первой странице документа, в верхнем левом углу поставить отметки о всех подписях документа. Отметки ставятся сверху вниз, с небольшим отступом друг от друга. 
<br>**Точка входа:** серверный метод [ConvertToPdfAndAddSignatureMark](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L48-L128) документа Memo. 

### Преобразование в PDF и наложение отметки о поступлении
Входящее письмо. Цвет отметки о поступлении изменён на фиолетовый, изменена толщина рамки, расположение (отметка сдвинута ближе к центру), добавлено указание подразделения.
<br>**Точки входа:** серверные методы IncomingLetter: [AddRegistrationStamp](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingLetter/IncomingLetterServerFunctions.cs#L47-L50) и [GetRegistrationStampAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingLetter/IncomingLetterServerFunctions.cs#L17-L38).

### Политики перемещения 
Добавлено событие "Обращение к документу" в критерии перемещения. Документ перемещается в новое хранилище, если прошло указанное время с даты последнего обращения к документу. 
<br>**Точки входа:** серверные методы [GetStoragePolicySettingsQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L95-L98), [GetDocumentsToTransferQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L104-L107) модуля Docflow.
 
### Интеллектуальная обработка
Дополнительный классификатор в настройках интеллектуальной обработки. 
<br>**Точки входа:** серверный метод [FillSmartAdditionalClassifiers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.SmartProcessing/ModuleInitializer.cs#L25-L42) в инициализации модуля SmartProcessing. Серверный метод [CreateIncomingLetter](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ModuleServerFunctions.cs#L22-L35) в решении Examples.
 
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