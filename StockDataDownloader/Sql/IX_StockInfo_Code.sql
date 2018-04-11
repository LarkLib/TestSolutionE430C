IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IX_StockInfo_Code]') AND name = N'IX_StockInfo_Code')
DROP INDEX [IX_StockInfo_Code] ON [dbo].[StockInfo] WITH ( ONLINE = OFF )

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_StockInfo_Code] ON [dbo].[StockInfo]
(
	[Code] ASC
)
INCLUDE ([ListingDate]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

