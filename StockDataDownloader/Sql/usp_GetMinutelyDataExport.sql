USE [Stock001]
GO

IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetMinutelyDataExport')
    DROP PROCEDURE [dbo].[usp_GetMinutelyDataExport]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetMinutelyDataExport]    Script Date: 5/19/2017 21:14:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_GetMinutelyDataExport] @code NVARCHAR(50),@beginDate DATETIMEOFFSET, @endDate DATETIMEOFFSET, @itervalCount INT = 1
AS
BEGIN
    with d as 
    (
    select --top 1  
    code,
    dateadd(ss,-datepart(ss,datetime),datetime) datetime, 
    min(datetime) [OpenPriceDateTime],
    max(price) [HighestPrice],
    max(datetime) [ClosePriceDateTime],
    min(price) [LowestPrice],
    sum([Volume]) [Volume],
    sum([Amount]) [Amount],
    SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount) [FlowingAmount],
    SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 100000 THEN [Amount] ELSE 0 END) [FlowingAmount100K],
    SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 500000 THEN [Amount] ELSE 0 END) [FlowingAmount500K]
    from detail with(nolock)
    where code=@code and datetime >= @beginDate and datetime < @endDate
    group by code,dateadd(ss,-datepart(ss,datetime),datetime))
    select 
    d.code,
    d.datetime,
    (select top 1 price from detail where d.code=code and datetime=[OpenPriceDateTime]) * roi.factor [OpenPrice],
    d.[HighestPrice] * roi.factor [HighestPrice],
    l.[Price] * roi.factor [ClosePrice],
    d.[LowestPrice] * roi.factor [LowestPrice],
    cast(d.[Volume] / roi.factor as int) [Volume],
    d.[Amount],
    l.[BuyPrice1] * roi.factor [BuyPrice1],
    l.[BuyPrice2] * roi.factor [BuyPrice2],
    l.[BuyPrice3] * roi.factor [BuyPrice3],
    l.[BuyPrice4] * roi.factor [BuyPrice4],
    l.[BuyPrice5] * roi.factor [BuyPrice5],
    cast(l.[BuyVolume1] / roi.factor as int) [BuyVolume1],
    cast(l.[BuyVolume2] / roi.factor as int) [BuyVolume2],
    cast(l.[BuyVolume3] / roi.factor as int) [BuyVolume3],
    cast(l.[BuyVolume4] / roi.factor as int) [BuyVolume4],
    cast(l.[BuyVolume5] / roi.factor as int) [BuyVolume5],
    l.[SalePrice1] * roi.factor [SalePrice1],
    l.[SalePrice2] * roi.factor [SalePrice2],
    l.[SalePrice3] * roi.factor [SalePrice3],
    l.[SalePrice4] * roi.factor [SalePrice4],
    l.[SalePrice5] * roi.factor [SalePrice5],
    cast(l.[SaleVolume1] / roi.factor as int) [SaleVolume1],
    cast(l.[SaleVolume2] / roi.factor as int) [SaleVolume2],
    cast(l.[SaleVolume3] / roi.factor as int) [SaleVolume3],
    cast(l.[SaleVolume4] / roi.factor as int) [SaleVolume4],
    cast(l.[SaleVolume5] / roi.factor as int) [SaleVolume5],
    d.[FlowingAmount],
    d.[FlowingAmount100K],
    d.[FlowingAmount500K]
    from d with(nolock)
    join [RightsOfferingInfo] roi with(nolock) on d.code=roi.code and TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime
    join detail l with(nolock) on d.code=l.code and [ClosePriceDateTime]=l.datetime
    order by datetime
END

GO

[dbo].[usp_GetMinutelyDataExport] 'sh600020', '2012-03-01', '2012-03-02'