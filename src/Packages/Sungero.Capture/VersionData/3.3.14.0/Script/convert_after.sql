delete from sungero_commons_facts
delete from sungero_commons_entityrecogn

insert into sungero_commons_entityrecogn
select 	drr.id
      , '32EA0857-ADF7-41C2-BC0C-188320E40786' as discriminator
      , drr.secureobject
      , drr.status
      , drr.name
      , drr.recognclass
      , drr.probability
      , drr.documentid 
      , edoc.discriminator as entitytype
from sungero_capture_documentrecogn as drr
join sungero_content_edoc as edoc
on drr.documentid = edoc.id;

insert into sungero_commons_facts
select  id
      , 'EA588697-56D5-4A14-9A78-39FA6517351B' as discriminator
      , documentrecogn
      , factid
      , fieldid
      , propertyname
      , propertyvalue
      , factname
      , fieldname
      , fieldvalue
      , fieldprobabil
      , position
      , istrusted
      , verifiedvalue
      , factlabel
      , collectionreco
from sungero_capture_facts;