USE [StockInfo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WeightInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[IndexCode] [nvarchar](50) NOT NULL,
	[IndexNameEng] [nvarchar](50) NOT NULL,
	[ConstituentCode] [nvarchar](50) NOT NULL,
	[ConstituentNameEng] [nvarchar](50) NOT NULL,
	[Exchange] [nvarchar](50) NOT NULL,
	[Weight] [decimal](18, 3) NOT NULL,
	[TradingCurrency] [nvarchar](50) NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_WeightInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WeightInfo] ADD  CONSTRAINT [DF_WeightInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[WeightInfo] ADD  CONSTRAINT [DF_WeightInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
CREATE NONCLUSTERED INDEX [IX_WeightInfo_Code_IndexCode] ON [dbo].[WeightInfo]
(
	[IndexCode] ASC,
	[Code] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

