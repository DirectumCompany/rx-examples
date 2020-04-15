begin

--Полностью очистить таблицы EntityRecognitionInfo, данные в них на момент конвертации не имеют ценности.
delete from Sungero_Commons_Facts;
delete from Sungero_Commons_EntityRecogn;

--Заполнить таблицы EntityRecognitionInfo на основе таблиц DocumentRecognitionInfo.
insert into Sungero_Commons_EntityRecogn
      (Id, Discriminator, SecureObject, Status, Name, RecognClass, Probability, EntityId, EntityType)
select drr.Id
      , '32EA0857-ADF7-41C2-BC0C-188320E40786'
      , drr.SecureObject
      , drr.Status
      , drr.Name
      , drr.RecognClass
      , drr.Probability
      , drr.DocumentId 
      , edoc.Discriminator
from Sungero_Capture_DocumentRecogn as drr
join Sungero_Content_Edoc as edoc
on drr.DocumentId = edoc.Id;

insert into Sungero_Commons_Facts 
      (Id, Discriminator, EntityRecogn, FactId, FieldId, PropertyName, PropertyValue, FactName, FieldName, 
	   FieldValue, FieldProbabil, Position, VerifiedValue, FactLabel, CollectionReco)
select Id
      , 'EA588697-56D5-4A14-9A78-39FA6517351B'
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
      , VerifiedValue
      , FactLabel
      , CollectionReco
from Sungero_Capture_Facts;

--Актуализировать в sungero_system_ids текущие ИД таблиц EntityRecognitionInfo.
delete from Sungero_System_Ids
where TableName = 'Sungero_Commons_EntityRecogn';

if ((select count(Id) from Sungero_Commons_EntityRecogn) > 0) 
  insert into Sungero_System_Ids
  values('Sungero_Commons_EntityRecogn', (select max(Id) from Sungero_Commons_EntityRecogn));
else
  insert into Sungero_System_Ids
  values('Sungero_Commons_EntityRecogn', 0);	

delete from Sungero_System_Ids
where TableName = 'Sungero_Commons_Facts';

if ((select count(Id) from Sungero_Commons_Facts) > 0)
  insert into Sungero_System_Ids
  values('Sungero_Commons_Facts', (select max(Id) from Sungero_Commons_Facts));
else
  insert into Sungero_System_Ids
  values('Sungero_Commons_Facts', 0);

end