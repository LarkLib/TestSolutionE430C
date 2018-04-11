IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockOperation]') AND name = N'IX_StockOperation_Code')
DROP INDEX [IX_StockOperation_Code] ON [dbo].[StockOperation] WITH ( ONLINE = OFF )

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_StockOperation_Code] ON [dbo].[StockOperation]
(
	[Code] ASC
)
INCLUDE ([ActionDate]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

