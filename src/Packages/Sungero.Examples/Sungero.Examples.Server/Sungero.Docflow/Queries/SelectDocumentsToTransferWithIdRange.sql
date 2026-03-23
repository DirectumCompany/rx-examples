with RankedDocs as (
  select doc.Id,
    doc.Storage_Docflow_Sungero as CurrentStorage,
    t.Storage as TargetStorage,
    t.NeedRetention,
    row_number() over (
      partition by doc.Id
      order by t.Priority desc
    ) as rn
  from Sungero_Content_EDoc doc
  join (select EntityId, max(HistoryDate) as HD
          from Sungero_Content_DocHistory
          where Action in ('Read', 'Create', 'Update')
            and VersionNumber is not null
            and Operation <> 'DataTransfer'
          group by EntityId) as hld
    on hld.EntityId = doc.Id
  join {0} t
    on t.DocumentKind = doc.DocumentKind_Docflow_Sungero
    and doc.DocumentDate_Docflow_Sungero <= t.DocumentDate
    and doc.Modified <= t.Modified
    and hld.HD <= t.LastRead
  where t.NeedRetention = 1
    and doc.Storage_Docflow_Sungero is not null
    and t.Storage != doc.Storage_Docflow_Sungero
    and doc.Id > {1}
    and doc.Id <= {2}
)
select Id, TargetStorage
from RankedDocs
where rn = 1
