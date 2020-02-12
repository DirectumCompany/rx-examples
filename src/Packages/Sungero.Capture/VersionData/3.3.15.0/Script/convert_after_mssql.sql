--Заполнить таблицы EntityRecognitionInfo на основе таблиц DocumentRecognitionInfo.
select drr.Id
      , '32EA0857-ADF7-41C2-BC0C-188320E40786' as Discriminator
      , drr.SecureObject
      , drr.Status
      , drr.Name
      , drr.RecognClass
      , drr.Probability
      , drr.DocumentId 
      , edoc.Discriminator as entitytype
from Sungero_Capture_DocumentRecogn as drr
join Sungero_Content_Edoc as edoc
on drr.DocumentId = edoc.Id;

select Id
      , 'EA588697-56D5-4A14-9A78-39FA6517351B' as Discriminator
      , DocumentRecogn
      , FactId
      , FieldId
      , PropertyName
      , PropertyValue
      , FactName
      , FieldName
      , FieldValue
      , FieldProbabil
      , Position
      , IsTrusted
      , VerifiedValue
      , FactLabel
      , CollectionReco
from Sungero_Capture_Facts;

--Актуализировать в sungero_system_ids текущие ИД таблиц EntityRecognitionInfo.
delete from Sungero_System_Ids
where TableName = 'Sungero_Commons_EntityRecogn';

if 
  ((select count(Id) from Sungero_Commons_EntityRecogn) > 0) 
  insert into Sungero_System_Ids
  values('Sungero_Commons_EntityRecogn', (select max(Id) from Sungero_Commons_EntityRecogn));

delete from Sungero_System_Ids
where TableName = 'Sungero_Commons_Facts';

if 
  ((select count(Id) from Sungero_Commons_Facts) > 0)
  insert into Sungero_System_Ids
  values('Sungero_Commons_Facts', (select max(Id) from Sungero_Commons_Facts));