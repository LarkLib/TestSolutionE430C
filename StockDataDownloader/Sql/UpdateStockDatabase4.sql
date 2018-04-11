
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

CREATE TABLE [dbo].[Daily163](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[ClosePrice] [decimal](18, 3) NOT NULL,
	[HighestPrice] [decimal](18, 3) NOT NULL,
	[LowestPrice] [decimal](18, 3) NOT NULL,
	[OpenPrice] [decimal](18, 3) NOT NULL,
	[PrevClose] [decimal](18, 3) NOT NULL,
	[Change] [decimal](18, 3) NOT NULL,
	[ChangePercent] [decimal](18, 2) NOT NULL,
	[TurnoverRate] [decimal](18, 3) NOT NULL,
	[Volume] [bigint] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[TotalCapitalization] [decimal](18, 3) NOT NULL,
	[MarketCapitalization] [decimal](18, 3) NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL
 CONSTRAINT [PK_Daily163] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DailyDataGroup]
) ON [DailyDataGroup];


ALTER TABLE [dbo].[Daily163] ADD  CONSTRAINT [DF_Daily163_Id]  DEFAULT (newsequentialid()) FOR [Id];

ALTER TABLE [dbo].[Daily163] ADD  CONSTRAINT [DF_Daily163_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime];

CREATE NONCLUSTERED INDEX [IX_Daily163_CodeDatetime] ON [dbo].[Daily163]
(
	[Code] ASC,
	[DateTime] ASC
)WITH (DATA_COMPRESSION = PAGE, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [DailyDataGroup]
;

