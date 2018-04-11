USE [StockInfo]

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'StockInfo' AND COLUMN_NAME = N'PublicShareholders')
BEGIN
    EXECUTE sp_rename N'dbo.StockInfo.PublicShareholders', N'CirculateStockHolder', 'COLUMN' 
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'StockInfo' AND COLUMN_NAME = N'PrincipalShareholder')
BEGIN
    EXECUTE sp_rename N'dbo.StockInfo.PrincipalShareholder', N'StockHolder', 'COLUMN' 
END
GO

USE [Stock001]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock002]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock003]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock004]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock005]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock006]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock007]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock008]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock009]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO

USE [Stock010]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DetailDataGroup]
) ON [DetailDataGroup]

GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[RightsOfferingInfo] ADD  CONSTRAINT [DF_RightsOfferingInfo_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RightsOfferingInfo_CodeDatatime] ON [dbo].[RightsOfferingInfo]
(
    [Code] ASC,
    [DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  ON [DetailDataGroup] 

GO