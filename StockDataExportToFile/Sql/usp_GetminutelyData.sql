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
    SELECT [DateTime]           date
        ,[OpenPrice]          y1
        ,[HighestPrice]       y2
        ,[ClosePrice]         y3
        ,[LowestPrice]        y4
        ,[Volume]             y5
        ,[Amount]             y6
        ,[FlowingAmount]      y7
        ,[FlowingAmount100K]  y8
        ,[FlowingAmount500K]  y9
        ,[BuyPrice1]          y10
        ,[BuyPrice2]          y11
        ,[BuyPrice3]          y12
        ,[BuyPrice4]          y13
        ,[BuyPrice5]          y14
        ,[BuyVolume1]         y15
        ,[BuyVolume2]         y16
        ,[BuyVolume3]         y17
        ,[BuyVolume4]         y18
        ,[BuyVolume5]         y19
        ,[SalePrice1]         y20
        ,[SalePrice2]         y21
        ,[SalePrice3]         y22
        ,[SalePrice4]         y23
        ,[SalePrice5]         y24
        ,[SaleVolume1]        y25
        ,[SaleVolume2]        y26
        ,[SaleVolume3]        y27
        ,[SaleVolume4]        y28
        ,[SaleVolume5]        y29
    FROM [Minute] WITH (nolock)
    WHERE  code=@code AND (DateTime >=@beginDate ) AND (DateTime < @endDate)
    ORDER BY date
END
GO

[usp_GetMinutelyData] 'sh600020','2015-01-01','2015-02-01'
GO
[usp_GetMinutelyData] 'sh600132','2000-01-01','2018-01-01'