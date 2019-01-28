﻿# DirectumRXExamples

Репозиторий с примерами слоения прикладной разработки.

## Структура проекта

* src - папка со всеми исходными кодами решения
* src\Packages - прикладная разработка

## Кейсы

1. Перекрытие логики наложения отметки о ПЭП для всех документов. В отметке о ПЭП изменёны логотип и пропорции заголовка; также в отметку добавлены дата и время подписания, цвет отметки изменён на фиолетовый. **Точки входа:** серверные методы GetSignatureMarkForSignatureAsHtml и GetSignatureMarkForCertificateAsHtml модуля Docflow.