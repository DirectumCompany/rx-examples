select doc.Id, t.Storage
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
	where t.Priority = (select max(tt.Priority) 
	                      from {0} tt 
						            where tt.DocumentKind = doc.DocumentKind_Docflow_Sungero
	                        and doc.DocumentDate_Docflow_Sungero <= tt.DocumentDate
	                        and doc.Modified <= tt.Modified
							and hld.HD <= t.LastRead)
	and t.Storage != doc.Storage_Docflow_Sungero
	and doc.Storage_Docflow_Sungero is not null
	and t.NeedRetention = 1