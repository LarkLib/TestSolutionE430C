SET ANSI_PADDING ON

IF EXISTS (SELECT name from sys.indexes  
           WHERE name = N'IX_Detail_CodeDatetime')   
   DROP INDEX IX_Detail_CodeDatetime ON [dbo].[Detail];   
GO  
CREATE NONCLUSTERED INDEX IX_Detail_CodeDatetime ON [dbo].[Detail]
(
	[Code] ASC,
	[DateTime] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [DetailDataGroup]  
GO 