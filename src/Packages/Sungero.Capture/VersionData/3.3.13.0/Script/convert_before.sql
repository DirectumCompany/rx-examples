insert into sungero_commons_entityrecognit
select 	drr.id
	    , drr.discriminator
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
select * from sungero_capture_facts