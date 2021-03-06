USE [master]
GO
ALTER DATABASE [Stock001] ADD FILEGROUP [DailyDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Daily_001', FILENAME = N'D:\DataBase\Data\Stock001\Daily_001.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [DailyDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Daily_002', FILENAME = N'D:\DataBase\Data\Stock001\Daily_002.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [DailyDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Daily_003', FILENAME = N'D:\DataBase\Data\Stock001\Daily_003.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [DailyDataGroup]
GO

------------------------------------------------------------------------------------------------------------------------------------------------------------------------
USE [Stock001]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Daily](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[OpenPrice] [decimal](18, 3) NOT NULL,
	[HighestPrice] [decimal](18, 3) NOT NULL,
	[ClosePrice] [decimal](18, 3) NOT NULL,
	[LowestPrice] [decimal](18, 3) NOT NULL,
	[Volume] [int] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[FlowingAmount] [decimal](18, 3) NOT NULL,
	[FlowingAmount100K] [decimal](18, 3) NOT NULL,
	[FlowingAmount500K] [decimal](18, 3) NOT NULL,
	[BuyPrice1] [decimal](18, 3) NOT NULL,
	[BuyPrice2] [decimal](18, 3) NOT NULL,
	[BuyPrice3] [decimal](18, 3) NOT NULL,
	[BuyPrice4] [decimal](18, 3) NOT NULL,
	[BuyPrice5] [decimal](18, 3) NOT NULL,
	[BuyVolume1] [int] NOT NULL,
	[BuyVolume2] [int] NOT NULL,
	[BuyVolume3] [int] NOT NULL,
	[BuyVolume4] [int] NOT NULL,
	[BuyVolume5] [int] NOT NULL,
	[SalePrice1] [decimal](18, 3) NOT NULL,
	[SalePrice2] [decimal](18, 3) NOT NULL,
	[SalePrice3] [decimal](18, 3) NOT NULL,
	[SalePrice4] [decimal](18, 3) NOT NULL,
	[SalePrice5] [decimal](18, 3) NOT NULL,
	[SaleVolume1] [int] NOT NULL,
	[SaleVolume2] [int] NOT NULL,
	[SaleVolume3] [int] NOT NULL,
	[SaleVolume4] [int] NOT NULL,
	[SaleVolume5] [int] NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Daily] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DailyDataGroup]
) ON [DailyDataGroup]

GO

ALTER TABLE [dbo].[Daily] ADD  CONSTRAINT [DF_Daily_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[Daily] ADD  CONSTRAINT [DF_Daily_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
CREATE NONCLUSTERED INDEX [IX_Daily_CodeDatetime] ON [dbo].[Daily]
(
	[Code] ASC,
	[DateTime] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DailyDataGroup]
GO

IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetDailyDataExport')
    DROP PROCEDURE [dbo].[usp_GetDailyDataExport]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetDailyDataExport]    Script Date: 5/19/2017 21:14:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_GetDailyDataExport] @code NVARCHAR(50),@beginDate DATETIMEOFFSET, @endDate DATETIMEOFFSET, @itervalCount INT = 1
AS
BEGIN
    with d as 
    (
    select --top 1  
    code,
    TODATETIMEOFFSET(CAST([Datetime] AS DATE), DATENAME([tz], [Datetime])) datetime, 
    min(DateTime) [OpenPriceDateTime],
    max([HighestPrice]) [HighestPrice],
    max(DateTime) [ClosePriceDateTime],
    min([LowestPrice]) [LowestPrice],
    sum([Volume]) [Volume],
    sum([Amount]) [Amount],
    SUM([FlowingAmount]) [FlowingAmount],
    SUM([FlowingAmount100K]) [FlowingAmount100K],
    SUM([FlowingAmount500K]) [FlowingAmount500K]
    from minute with(nolock)
    where code=@code and datetime >= @beginDate and datetime < @endDate
    group by code,TODATETIMEOFFSET(CAST([Datetime] AS DATE), DATENAME([tz], [Datetime])))
    select 
    d.code,
    d.datetime,
    (select top 1 [OpenPrice] from minute where d.code=code and DateTime=[OpenPriceDateTime]) [OpenPrice],
    d.[HighestPrice] [HighestPrice],
    l.[ClosePrice] [ClosePrice],
    d.[LowestPrice] [LowestPrice],
    d.[Volume] [Volume],
    d.[Amount],
    l.[BuyPrice1] [BuyPrice1],
    l.[BuyPrice2] [BuyPrice2],
    l.[BuyPrice3] [BuyPrice3],
    l.[BuyPrice4] [BuyPrice4],
    l.[BuyPrice5] [BuyPrice5],
    l.[BuyVolume1] [BuyVolume1],
    l.[BuyVolume2] [BuyVolume2],
    l.[BuyVolume3] [BuyVolume3],
    l.[BuyVolume4] [BuyVolume4],
    l.[BuyVolume5] [BuyVolume5],
    l.[SalePrice1] [SalePrice1],
    l.[SalePrice2] [SalePrice2],
    l.[SalePrice3] [SalePrice3],
    l.[SalePrice4] [SalePrice4],
    l.[SalePrice5] [SalePrice5],
    l.[SaleVolume1] [SaleVolume1],
    l.[SaleVolume2] [SaleVolume2],
    l.[SaleVolume3] [SaleVolume3],
    l.[SaleVolume4] [SaleVolume4],
    l.[SaleVolume5] [SaleVolume5],
    d.[FlowingAmount],
    d.[FlowingAmount100K],
    d.[FlowingAmount500K]
    from d with(nolock)
    join minute l with(nolock) on d.code=l.code and [ClosePriceDateTime]=l.DateTime
    order by datetime
END

GO

--[dbo].[usp_GetDailyDataExport] 'sh600020', '2012-04-18', '2012-04-19'
