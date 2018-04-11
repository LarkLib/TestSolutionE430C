IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RealTimeData]') AND name = N'IX_RealTimeData_StockcodeHosttime')
DROP INDEX [IX_RealTimeData_StockcodeHosttime] ON [dbo].[RealTimeData] WITH ( ONLINE = OFF )

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX IX_RealTimeData_StockcodeHosttime ON [dbo].[RealTimeData]
(
	[StockCode] ASC,
	[HostTime] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [RealtimeDataGroup]

