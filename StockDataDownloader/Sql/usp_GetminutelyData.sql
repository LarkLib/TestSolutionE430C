use stock001
go

IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetMinutelyData')
    DROP PROCEDURE [dbo].[usp_GetMinutelyData]
GO

CREATE PROCEDURE [usp_GetMinutelyData] @code NVARCHAR(50),@beginDate DATETIMEOFFSET, @endDate DATETIMEOFFSET, @itervalCount INT = 1
AS
BEGIN
    SELECT  CONVERT(varchar(16), DateTime, 120) AS date, MIN(Price) AS y1, 
          SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount) AS y2, 
          SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount * LOG (ABS(PriceOffset) * 100 + 1.1)) AS y3, COUNT(1) AS y4, 
          SUM(PriceOffset * Amount) AS y5, SUM(Volume) / COUNT(1) AS y6
    FROM     dbo.Detail d WITH (nolock)
    WHERE  code=@code AND (DateTime >=@beginDate ) AND (DateTime < @endDate)
    GROUP BY Code, CONVERT(varchar(16), DateTime, 120)
    ORDER BY date
END
GO

[usp_GetMinutelyData] 'sh600519','2015-01-01','2015-02-01'
GO
[usp_GetMinutelyData] 'sh600132','2000-01-01','2018-01-01'