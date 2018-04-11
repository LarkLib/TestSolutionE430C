/*
declare @code nvarchar(50)
declare @datetime datetimeoffset
declare @factor decimal(18,3)
declare RightsOfferingInfoCursor cursor for        
select top 10 code,datetime,factor from RightsOfferingInfo
open RightsOfferingInfoCursor                      
fetch next from RightsOfferingInfoCursor into @code,@datetime,@factor	           
while @@fetch_status=0						
begin
select @code,@datetime,@factor	
fetch next from RightsOfferingInfoCursor into @code,@datetime,@factor						
end
close RightsOfferingInfoCursor
deallocate RightsOfferingInfoCursor
*/
select top 10 * from detail;
--select count(1) from  RightsOfferingInfo ci with(nolock) 
with d as 
(
select --top 1  
code,
dateadd(ss,-datepart(ss,datetime),datetime) datetime, 
min(id) [OpenPriceId],
max(price) [HighestPrice],
min(id) [ClosePriceId],
min(price) [LowestPrice],
sum([Volume]) [Volume],
sum([Amount]) [Amount],
SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount) [FlowingAmount],
SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 100000 THEN [Amount] ELSE 0 END) [FlowingAmount100K],
SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 500000 THEN [Amount] ELSE 0 END) [FlowingAmount500K]
from detail 
where code='sh600020' and datetime >= '2012-03-01 09:32:00.0000000 +08:00' and datetime < '2012-04-05 09:32:00.0000000 +08:00'
group by code,dateadd(ss,-datepart(ss,datetime),datetime))
select 
d.code,
d.datetime date,
(select price from detail where id=[OpenPriceId]) * roi.factor [OpenPrice],
d.[HighestPrice] * roi.factor [HighestPrice],
l.[Price] * roi.factor [ClosePrice],
d.[LowestPrice] * roi.factor [LowestPrice],
cast(d.[Volume] / roi.factor as int) [Volume],
d.[Amount],
l.[BuyPrice1] * roi.factor [BuyPrice1],
l.[BuyPrice2] * roi.factor [BuyPrice2],
l.[BuyPrice3] * roi.factor [BuyPrice3],
l.[BuyPrice4] * roi.factor [BuyPrice4],
l.[BuyPrice5] * roi.factor [BuyPrice5],
cast(l.[BuyVolume1] / roi.factor as int) [BuyVolume1],
cast(l.[BuyVolume2] / roi.factor as int) [BuyVolume2],
cast(l.[BuyVolume3] / roi.factor as int) [BuyVolume3],
cast(l.[BuyVolume4] / roi.factor as int) [BuyVolume4],
cast(l.[BuyVolume5] / roi.factor as int) [BuyVolume5],
l.[SalePrice1] * roi.factor [SalePrice1],
l.[SalePrice2] * roi.factor [SalePrice2],
l.[SalePrice3] * roi.factor [SalePrice3],
l.[SalePrice4] * roi.factor [SalePrice4],
l.[SalePrice5] * roi.factor [SalePrice5],
cast(l.[SaleVolume1] / roi.factor as int) [SaleVolume1],
cast(l.[SaleVolume2] / roi.factor as int) [SaleVolume2],
cast(l.[SaleVolume3] / roi.factor as int) [SaleVolume3],
cast(l.[SaleVolume4] / roi.factor as int) [SaleVolume4],
cast(l.[SaleVolume5] / roi.factor as int) [SaleVolume5],
d.[FlowingAmount],
d.[FlowingAmount100K],
d.[FlowingAmount500K]
from d
join [RightsOfferingInfo] roi on d.code=roi.code and TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime
join detail l on [ClosePriceId]=l.id
