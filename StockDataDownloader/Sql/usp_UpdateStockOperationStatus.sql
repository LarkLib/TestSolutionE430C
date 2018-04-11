IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_UpdateStockOperationStatus')
    DROP PROCEDURE [dbo].[usp_UpdateStockOperationStatus]
GO

CREATE PROCEDURE [usp_UpdateStockOperationStatus] @id                  UNIQUEIDENTIFIER,
                                                  @action              INT,
                                                  @status              INT,
                                                  @content             NVARCHAR(50)     = NULL,
                                                  @message             NVARCHAR(2000)   = NULL,
                                                  @elapsedMilliseconds INT              = 0
AS
BEGIN
    UPDATE [dbo].[StockOperation]
    SET    
	   [Action] = @action, -- int
	   [Content] = @content, -- nvarchar
	   [Status] = @status, -- int
	   [ElapsedMilliseconds] = @elapsedMilliseconds, -- int
	   [Message] = @message, -- nvarchar
	   [LastUpdateTime] = SYSDATETIMEOFFSET() -- datetimeoffset 
    WHERE  [Id] = @id
END
GO

[dbo].[usp_UpdateStockOperationStatus] '8DC1B907-FAF4-E611-A069-463500000031', 2, 1,'SummarySave,1999-Q3'
GO

SELECT *
FROM   [dbo].[StockOperation]
WHERE  [id] = '8DC1B907-FAF4-E611-A069-463500000031'