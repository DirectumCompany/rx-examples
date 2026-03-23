if exists (select * from information_schema.tables where table_name = '{0}')
  drop table {0}

select r.Storage, rdk.DocumentKind,
  case when r.DocDateType = 'DocumentDate' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as DocumentDate,
  case when r.DocDateType = 'Modified' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as Modified,
  case when r.DocDateType = 'LastRead' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as LastRead,
  case when r.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef' then r.[Priority] else r.[Priority] + (select coalesce(max(r2.[Priority]), 0) from Sungero_Docflow_StoragePolicy r2 where r2.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef') end as [Priority],
  case when coalesce(r.NextRetention, @now) <= @now then 1 else 0 end as NeedRetention
into {0}
  from Sungero_Docflow_StoragePolicy r
  join Sungero_Docflow_SPolicyDocKind rdk
    on rdk.StoragePolicy = r.Id
	where r.Status = 'Active';

insert into {0}
select r.Storage, rdk.Id,
  case when r.DocDateType = 'DocumentDate' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as DocumentDate,
  case when r.DocDateType = 'Modified' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as Modified,
  case when r.DocDateType = 'LastRead' then dateadd(DD, -1 * r.DaysToMove, @now) else @maxDate end as LastRead,
  case when r.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef' then r.[Priority] else r.[Priority] + (select coalesce(max(r2.[Priority]), 0) from Sungero_Docflow_StoragePolicy r2 where r2.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef') end as [Priority],
  case when coalesce(r.NextRetention, @now) <= @now then 1 else 0 end as NeedRetention
  from Sungero_Docflow_StoragePolicy r
  join Sungero_Docflow_DocumentKind rdk
    on 1=1
	where r.Status = 'Active'
	  and not exists (select 1 from Sungero_Docflow_SPolicyDocKind k where k.StoragePolicy = r.Id);

insert into {0}
select s.Id, rdk.Id, @maxDate, @maxDate, @maxDate, -1, 1
  from Sungero_Core_Storage s
  join Sungero_Docflow_DocumentKind rdk
    on 1=1
  where s.IsDefault = 1;
