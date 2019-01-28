﻿# DirectumRXExamples

Репозиторий с примерами слоения прикладной разработки.

## Структура проекта

* src - папка со всеми исходными кодами решения
* src\Packages - прикладная разработка

## Кейсы

1. Перекрытие логики наложения отметки об ЭП для всех документов. В отметке об ЭП изменёны логотип и пропорции заголовка; также в отметку добавлены дата и время подписания, цвет отметки изменён на фиолетовый. **Точки входа:** серверные методы GetSignatureMarkForSignatureAsHtml и GetSignatureMarkForCertificateAsHtml модуля Docflow.

2. Перекрытие логики наложения отметки об ЭП для входящих счетов. В отметке об ЭП изменёны логотип и пропорции заголовка; также в отметку добавлены дата и время подписания, цвет отметки изменён на красный, текст отметки изменен на "ПРИНЯТ К ОПЛАТЕ". **Точка входа:** серверный метод GetSignatureMarkAsHtml документа IncomingInvoice. 

3. Перекрытие определения интерактивно будет конвертиться документ или нет. Изображения jpg < 1 Mb должны конвертироваться интерактивно.