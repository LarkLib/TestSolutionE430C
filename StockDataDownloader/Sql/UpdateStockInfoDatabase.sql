USE [StockInfo]

EXECUTE sp_rename N'dbo.StockInfo.PublicShareholders', N'CirculateStockHolder', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.StockInfo.PrincipalShareholder', N'StockHolder', 'COLUMN' 
GO

CREATE TABLE [dbo].[RightsOfferingInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[OpenPrice] [decimal](18, 3) NOT NULL,
	[HighestPrice] [decimal](18, 3) NOT NULL,
	[ClosePrice] [decimal](18, 3) NOT NULL,
	[LowestPrice] [decimal](18, 3) NOT NULL,
	[Volume] [int] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[Factor] [decimal](18, 3) NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_RightsOfferingInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
	[Code] ASC,
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

USE [Stock001]
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO

UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d
JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code

GO