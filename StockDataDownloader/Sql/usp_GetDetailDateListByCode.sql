IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetDetailDateListByCode')
    DROP PROCEDURE [dbo].[usp_GetDetailDateListByCode]
GO

CREATE PROCEDURE [usp_GetDetailDateListByCode] @stockCode NVARCHAR(50)
AS
BEGIN
    SELECT [Id], [DateTime] [ActionDate]
    FROM   [summary] WITH (NOLOCK)
    WHERE  [Code] = @stockCode AND [Status] = 0 AND [DateTime] >= '2005-01-01'
END
GO

[dbo].[usp_GetDetailDateListByCode] 'sh600518'