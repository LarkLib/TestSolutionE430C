SET STATISTICS IO ON 
SET STATISTICS TIME ON 
--SELECT * FROM [dbo].[StockInfo] WHERE [Code]='sh600518' --2001-03-19 00:00:00.0000000 +08:00
--SELECT * FROM [dbo].[StockOperation] WHERE [Code]='sh600518' ORDER BY [ActionDate]SELECT * FROM [dbo].[StockOperation] WHERE [Code]='sh600518' and( status=-1 or (Action=0 and status=0)) -- ORDER BY [ActionDate]
--DELETE [dbo].[Summary] WHERE [Code]='sh600518' and [DateTime]<'2001-04-01 08:00:00.0000000 +08:00'
--SELECT * FROM [dbo].[Summary] WHERE [Code]='sh600518' ORDER BY [DateTime]
--UPDATE [dbo].[StockOperation] SET [Action] = 0, [Status] = 0 WHERE [Code]='sh600518'
--IF @@TRANCOUNT>0 COMMIT
--SELECT DISTINCT [Code] FROM [dbo].[Summary] 
--SELECT * FROM [dbo].[Detail] WHERE [Code] = 'sh600104'
--SELECT count(id) FROM [dbo].[Detail] WHERE [Code] = 'sh600518'
--SELECT * FROM [dbo].[Summary] WHERE [Code]='sh600518' and status=1
--dbo.usp_GetDetailDateListByCode 'sh600518'
--SELECT count(1) FROM [dbo].[Summary] WHERE [Code]='sh600518' or [Code] = 'sh600104'
--SELECT count(1) FROM [dbo].[Summary] WHERE [Code]='sh600518' or [Code] = 'sh600104'
--SELECT count(1) FROM [dbo].Detail WHERE [Code]='sh600518' or [Code] = 'sh600104'
--SELECT count(1) FROM [dbo].StockOperation WHERE [Code]='sh600518' or [Code] = 'sh600104'
--SELECT count(distinct datetime) FROM [dbo].Detail WHERE [Code]='sh000802'
--select convert(varchar(8),DateTime,112),count(1) from detail with(nolock) where Code='sh000802' and datetime>'20070101' group by convert(varchar(8),DateTime,112) order by convert(varchar(8),DateTime,112) 
--select count(*) from detail where Code='sh000802' and convert(varchar(8),DateTime,112)='20161213'
--select code,min(datetime),max(datetime) from detail group by code
--update summary set status=0,content='removed' where Code='sh000802'
--truncate table StockOperation
--select count(1) from Summary
--select code,status,count(1) from Summary with(nolock) group by status,Code order by code
--select code,datetime,count(1) srows from Summary with(nolock) group by datetime,Code order by srows desc
--select * from StockOperation where status=0
--select * from StockInfo where code='sz000802'
--select convert(varchar,DateTime,112),count(1) from detail with(nolock) where Code='sz000802' and datetime>'20070101' group by convert(varchar,DateTime,112) order by convert(varchar,DateTime,112) 
--select top 10 convert(varchar,DateTime,112) from detail
--select code, count(1),min(LastUpdateTime),max(LastUpdateTime),datediff(mi,min(LastUpdateTime),max(LastUpdateTime)) from detail with(nolock) group by code
--SELECT [code], datetime, COUNT(1) [srows]  FROM   [detail] GROUP BY [code], datetime ORDER BY [srows] DESC
SELECT [code], COUNT(1) [srows],min(datetime)  FROM   [detail] GROUP BY [code]
--delete detail where 'sh600519'=code
--select * from detail where code='sh600519' and datetime='9/24/2007 15:00:04 +08:00' --149888
--SELECT [StockCode],hosttime, count(1) rrows FROM [dbo].[RealTimeData] GROUP BY [StockCode],[HostTime] ORDER BY rrows DESC
SELECT  * FROM  [dbo].[Summary]  WHERE [Code]='sh600519' -- IS NOT NULL ORDER BY duration DESC
SELECT  * FROM [dbo].[StockOperation]   WHERE [Code]='sh600519'
SELECT  count(*) FROM [dbo].[Detail]    WHERE [Code]='sh600519'
--SELECT [Code], COUNT(1) [Count], min([DateTime]) [From], max([DateTime]) [To] FROM [dbo].[Detail] GROUP BY [Code]
--SELECT [Code], COUNT(1) [Count], min([DateTime]) [From], max([DateTime]) [To] FROM [dbo].[Summary]  GROUP BY [Code]



SELECT TOP 111
[Code] cc,
[perDayClosePrice],
[LowestPrice],
[HighestPrice],
([LowestPrice] - [perDayClosePrice])/[perDayClosePrice]*100 as down,
([HighestPrice] - [perDayClosePrice])/[perDayClosePrice]*100 as up,
NTILE(100) OVER (PARTITION BY [Code] ORDER BY [DateTime]) AS ntileNumber,
DENSE_RANK() OVER(PARTITION BY [Code] ORDER BY [DateTime]) AS DenseRank ,
ROW_NUMBER() OVER (PARTITION BY [Code] ORDER BY [DateTime]) AS RowNumber,
COUNT(1) OVER (PARTITION BY [Code] ORDER BY [DateTime]) AS countNumber,
CEILING(ROW_NUMBER() OVER (PARTITION BY [Code] ORDER BY [DateTime])/5.0) AS GroupNumber,
*
FROM 
(
SELECT
LAG([ClosePrice]) OVER(PARTITION BY [Code] ORDER BY[Code], [DateTime]) perDayClosePrice,
LAG([Code]) OVER(PARTITION BY [Code] ORDER BY [Code],[DateTime]) perDayCode,
*
FROM [dbo].[Summary]
) AS cp
WHERE ([LowestPrice] - [perDayClosePrice])/[perDayClosePrice]*100 <=-7
ORDER BY [Code],[DateTime]
