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

### Преобразование в PDF и наложение отметки об ЭП

1. Все документы. Изменен логотип и цвет отметки на фиолетовый. В отметку добавлена дата и время подписания. 
<br>**Точки входа:** серверные методы [GetSignatureMarkForSimpleSignatureAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L22-L35) и [GetSignatureMarkForCertificateAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L48-L74) модуля Docflow.

2. Входящий счёт. Цвет отметки изменён на красный, текст отметки изменен на "ПРИНЯТ К ОПЛАТЕ". 
<br>**Точка входа:** серверный метод [GetSignatureMarkAsHtml](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L17-L29) документа IncomingInvoice. 

3. Все документы. Изменено условие для интерактивного преобразования документов. Теперь изображения в формате jpg размером < 1 Mb конвертируются интерактивно. 
<br>**Точка входа:** серверный метод [CanConvertToPdfInteractively](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L81-L88) модуля Docflow. 

4. Служебная записка. На первой странице документа, в верхнем левом углу поставить отметки о всех подписях документа. Отметки ставятся сверху вниз, с небольшим отсутпом друг от друга. 
<br>**Точка входа:** серверный метод [ConvertToPdfAndAddSignatureMark](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Memo/MemoServerFunctions.cs#L48-L128) документа Memo. 

### Политики перемещения 
Добавлено событие "Обращение к документу" в критерии перемещения. Документ перемещается в новое хранилище, если прошло указанное время с даты последнего обращения к документу. 
<br>**Точки входа:** серверные методы [GetStoragePolicySettingsQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L95-L98), [GetDocumentsToTransferQuery](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.Docflow/ModuleServerFunctions.cs#L104-L107) модуля Docflow.
 
### Интеллектуальная обработка
Дополнительный классификатор в настройках интеллектуальной обработки. 
<br>**Точки входа:** серверный метод [FillSmartAdditionalClassifiers](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/Sungero.SmartProcessing/ModuleInitializer.cs#L25-L42) в инициализации модуля SmartProcessing. Серверный метод [CreateIncomingLetter](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.Server/ModuleServerFunctions.cs#L22-L35) в решении Examples.
 
### Валидация панели фильтрации 
Список "Входящие документы" на обложке модуля Делопроизводство. Реализовано ограничение списка "Входящие документы" при помощи валидации в панели фильтрации. Должен быть заполнен один из критериев: журнал регистрации, от кого, произвольный период. 
<br>**Точка входа:** действие [IncomingDocumentsValidateFilterPanel](https://github.com/DirectumCompany/rx-examples/blob/master/src/Packages/Sungero.Examples/Sungero.Examples.ClientBase/Sungero.RecordManagementUI/ModuleHandlers.cs#L12-L16) вычисляемой папки модуля Делопроизводство.
