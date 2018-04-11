IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_UpdateStockSummaryStatus')
    DROP PROCEDURE [dbo].[usp_UpdateStockSummaryStatus]
GO

CREATE PROCEDURE [usp_UpdateStockSummaryStatus] @id                  UNIQUEIDENTIFIER,
                                                @status              INT              = 1,
                                                @content             NVARCHAR(50)     = NULL,
                                                @elapsedMilliseconds INT              = 0
AS
BEGIN
    UPDATE [dbo].[Summary]
    SET [Content] = COALESCE(@content, [Content]), -- nvarchar
	   [Status] = @status, -- int
	   [ElapsedMilliseconds] = @elapsedMilliseconds, -- int
	   [LastUpdateTime] = SYSDATETIMEOFFSET() -- datetimeoffset 
    WHERE  [Id] = @id
END
GO

[dbo].[usp_UpdateStockSummaryStatus] 'F587D3D1-70D2-E611-A3CA-001E101F2500', 1
GO

SELECT *
FROM   [dbo].[Summary]
WHERE  [id] = 'F587D3D1-70D2-E611-A3CA-001E101F2500'