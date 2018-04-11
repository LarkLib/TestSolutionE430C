IF EXISTS(SELECT 1
          FROM   [sysobjects]
          WHERE  [type] = 'FN' AND [name] = 'fn_GetDataTimeOffsetQuarterFirstDay')
    DROP FUNCTION [dbo].[fn_GetDataTimeOffsetQuarterFirstDay]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](@date DATETIMEOFFSET)
RETURNS DATETIMEOFFSET
AS
BEGIN
    --RETURN CONVERT(CHAR(8), DATEPART(YEAR, @date) * 10000 + (DATEPART(QUARTER, @date) * 3 - 2) * 100 + 1)+' 00:00:00'+DATENAME([tz], @date)
    RETURN TODATETIMEOFFSET(CAST(DATEADD([qq], DATEDIFF([qq], 0, @date), 0) AS DATETIMEOFFSET), DATENAME([tz], @date))
END
GO

DECLARE @date DATETIMEOFFSET= '20121212 12:33:56 +08:00' --sysdatetimeoffset()
SELECT SWITCHOFFSET(CAST(DATEADD([qq], DATEDIFF([qq], 0, @date), 0) AS DATETIMEOFFSET), DATENAME([tz], @date))
SELECT DATEADD([m], DATEPART([q], @date) * 3 - 2 - DATEPART([m], @date), DATEADD([d], 1 - DATEPART([d], @date), @date)), DATEADD([d], 1 - DATEPART([d], @date), @date), DATEPART([q], @date) * 3 - 2 - DATEPART([m], @date)
SELECT CAST(CONVERT(CHAR(8), DATEPART(YEAR, @date) * 10000 + (DATEPART(QUARTER, @date) * 3 - 2) * 100 + 1)+' 00:00:00'+DATENAME([tz], @date) AS DATETIMEOFFSET)

SELECT [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](SYSDATETIMEOFFSET())