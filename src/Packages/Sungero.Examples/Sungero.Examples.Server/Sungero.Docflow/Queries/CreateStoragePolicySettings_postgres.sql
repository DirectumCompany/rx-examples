DO $$
BEGIN
  IF EXISTS (SELECT * FROM information_schema.tables WHERE table_name = LOWER('{0}')) THEN
    DROP TABLE {0};
  END IF;
END $$;

SELECT r.Storage, rdk.DocumentKind,
  CASE 
    WHEN r.DocDateType = 'DocumentDate' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS DocumentDate,
  CASE 
    WHEN r.DocDateType = 'Modified' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS Modified,
  CASE 
    WHEN r.DocDateType = 'LastRead' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS LastRead,																			  
  CASE 
    WHEN r.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef' 
    THEN r.Priority 
    ELSE r.Priority + (SELECT COALESCE(MAX(r2.Priority), 0) FROM Sungero_Docflow_StoragePolicy r2 WHERE r2.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef') 
  END AS Priority,
  CASE 
    WHEN COALESCE(r.NextRetention, '{1}') <= '{1}' 
    THEN 1 
    ELSE 0
  END AS NeedRetention
INTO {0}
  FROM Sungero_Docflow_StoragePolicy r
  JOIN Sungero_Docflow_SPolicyDocKind rdk
    ON rdk.StoragePolicy = r.Id
  WHERE r.Status = 'Active';

INSERT INTO {0}
SELECT r.Storage, rdk.Id,
  CASE 
    WHEN r.DocDateType = 'DocumentDate' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS DocumentDate,
  CASE 
    WHEN r.DocDateType = 'Modified' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS Modified,
  CASE 
    WHEN r.DocDateType = 'LastRead' 
    THEN TO_TIMESTAMP('{1}','YYYY-MM-DD HH24:MI:SS') - r.DaysToMove * INTERVAL '1 day' 
    ELSE TO_TIMESTAMP('2100-01-01 00:00:00.000','YYYY-MM-DD HH24:MI:SS') 
  END AS LastRead,
  CASE 
    WHEN r.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef' 
    THEN r.Priority 
    ELSE r.Priority + (SELECT COALESCE(MAX(r2.Priority), 0) FROM Sungero_Docflow_StoragePolicy r2 WHERE r2.Discriminator = '9fed5653-77e7-4543-b071-6586033907ef') 
  END AS Priority,
  CASE 
    WHEN COALESCE(r.NextRetention, '{1}') <= '{1}' 
    THEN 1 
    ELSE 0 
  END AS NeedRetention
  FROM Sungero_Docflow_StoragePolicy r
  JOIN Sungero_Docflow_DocumentKind rdk
    ON 1=1
    WHERE r.Status = 'Active'
      AND NOT EXISTS (SELECT 1 FROM Sungero_Docflow_SPolicyDocKind k WHERE k.StoragePolicy = r.Id);