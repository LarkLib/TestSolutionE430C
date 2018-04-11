IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetSummaryDateListByCode')
    DROP PROCEDURE [dbo].[usp_GetSummaryDateListByCode]
GO

CREATE PROCEDURE [usp_GetSummaryDateListByCode] @stockCode NVARCHAR(50)
AS
BEGIN
    DECLARE @stockOperation TABLE([Code]       NVARCHAR(50) NOT NULL,
                                  [ActionDate] DATETIMEOFFSET(7) NOT NULL)
    DECLARE @action INT
    DECLARE @tempDate DATETIMEOFFSET
    DECLARE @startDate DATETIMEOFFSET

    SET @action = 2

    UPDATE [StockOperation]
    SET    [Status] = -1
    WHERE  DATEDIFF([ss], [LastUpdateTime], SYSDATETIMEOFFSET()) > 30 AND (([Action] = @action AND [Status] = 0) OR [Action] = 1)

    SELECT @startDate = DATEADD([q], 1, MAX([ActionDate]))
    FROM   [dbo].[StockOperation] WITH (NOLOCK)
    WHERE  [Code] = @stockCode
    GROUP BY [Code]
    IF @startDate IS NULL
        SELECT @startDate = [ListingDate]
        FROM   [dbo].[StockInfo]
        WHERE  [Code] = @stockCode

    SELECT @tempdate = [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](@startDate)

    WHILE @tempDate IS NOT NULL AND @tempDate <= [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](SYSDATETIMEOFFSET())
    BEGIN
        INSERT INTO                @stockOperation([Code], [ActionDate])
        VALUES                     (@stockCode, @tempDate)
        SET @tempDate = DATEADD([q], 1, @tempDate)
    END

    MERGE INTO [dbo].[StockOperation] [sot]
    USING @stockOperation [sov]
    ON [sot].[Code] = [sov].[Code] AND [sot].[ActionDate] = [sov].[ActionDate]
        WHEN NOT MATCHED
        THEN INSERT([Code], [Action], [Content], [Status], [ElapsedMilliseconds], [ActionDate]) VALUES([sov].[code], 0, [sov].[ActionDate], 0, 0, [sov].[ActionDate]);

    SELECT [ActionDate], [Id]
    FROM   [dbo].[StockOperation] [so]
    WHERE  (([so].[Status] = 0 AND [so].[Action] = 0) OR ([so].[Status] < 0)) AND [code] = @stockCode AND [ActionDate] >= '2005-01-01'
END
GO

[usp_GetSummaryDateListByCode] 'sh600518'