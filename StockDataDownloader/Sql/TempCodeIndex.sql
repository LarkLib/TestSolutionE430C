with codeindex as
(
select
mi.[Code]
,mi.[DateTime]
,[OpenPrice]		    *wi.[Weight]	[OpenPrice]	
,[HighestPrice] 	    *wi.[Weight]	[HighestPrice]
,[ClosePrice]  	    *wi.[Weight]	[ClosePrice]  
,[LowestPrice] 	    *wi.[Weight]	[LowestPrice] 
,[Volume]   
,[Amount]   
,[FlowingAmount] 
,[FlowingAmount100K]
,[FlowingAmount500K]
,[BuyPrice1]		    *wi.[Weight]	[BuyPrice1]
,[BuyPrice2]  		    *wi.[Weight]	[BuyPrice2]
,[BuyPrice3]  		    *wi.[Weight]	[BuyPrice3]
,[BuyPrice4]  		    *wi.[Weight]	[BuyPrice4]
,[BuyPrice5]  		    *wi.[Weight]	[BuyPrice5]
,[BuyVolume1]  
,[BuyVolume2]  
,[BuyVolume3]  
,[BuyVolume4]  
,[BuyVolume5]  
,[SalePrice1]		    *wi.[Weight]	[SalePrice1]
,[SalePrice2]  	    *wi.[Weight]	[SalePrice2]
,[SalePrice3]  	    *wi.[Weight]	[SalePrice3]
,[SalePrice4]  	    *wi.[Weight]	[SalePrice4]
,[SalePrice5]  	    *wi.[Weight]	[SalePrice5]
,[SaleVolume1] 
,[SaleVolume2] 
,[SaleVolume3] 
,[SaleVolume4] 
,[SaleVolume5]
,wi.[Weight] [Weight]
from [dbo].[Minute] mi
join StockInfo.dbo.WeightInfo wi on mi.code = wi.code
where mi.DateTime>='2017-01-01 00:00:00.0000000 +08:00' and mi.DateTime<'2017-01-09 00:00:00.0000000 +08:00' and wi.IndexCode='000905'
)
SELECT 
    [DateTime]									 [DateTime]
    ,sum([OpenPrice]			 )/count(1)		 [OpenPrice]
    ,sum([HighestPrice]			 )/count(1)		 [HighestPrice] 
    ,sum([ClosePrice]			 )/count(1)		 [ClosePrice]  
    ,sum([LowestPrice]			 )/count(1)		 [LowestPrice] 
    ,sum([Volume]				 )				 [Volume]   
    ,sum([Amount]				 )				 [Amount]   
    ,sum([FlowingAmount]			 )				 [FlowingAmount] 
    ,sum([FlowingAmount100K]		 )				 [FlowingAmount100K]
    ,sum([FlowingAmount500K]		 )/count(1)		 [FlowingAmount500K]
    ,sum([BuyPrice1]			 )/count(1)		 [BuyPrice1]  
    ,sum([BuyPrice2]			 )/count(1)		 [BuyPrice2]  
    ,sum([BuyPrice3]			 )/count(1)		 [BuyPrice3]  
    ,sum([BuyPrice4]			 )/count(1)		 [BuyPrice4]  
    ,sum([BuyPrice5]			 )/count(1)		 [BuyPrice5]  
    ,sum([BuyVolume1]			 )				 [BuyVolume1]  
    ,sum([BuyVolume2]			 )				 [BuyVolume2]  
    ,sum([BuyVolume3]			 )				 [BuyVolume3]  
    ,sum([BuyVolume4]			 )				 [BuyVolume4]  
    ,sum([BuyVolume5]			 )				 [BuyVolume5]  
    ,sum([SalePrice1]			 )/count(1)		 [SalePrice1]  
    ,sum([SalePrice2]			 )/count(1)		 [SalePrice2]  
    ,sum([SalePrice3]			 )/count(1)		 [SalePrice3]  
    ,sum([SalePrice4]			 )/count(1)		 [SalePrice4]  
    ,sum([SalePrice5]			 )/count(1)		 [SalePrice5]  
    ,sum([SaleVolume1]			 )				 [SaleVolume1] 
    ,sum([SaleVolume2]			 )				 [SaleVolume2] 
    ,sum([SaleVolume3]			 )				 [SaleVolume3] 
    ,sum([SaleVolume4]			 )				 [SaleVolume4] 
    ,sum([SaleVolume5]   		 )				 [SaleVolume5] 
	,sum([Weight]				 )				 [Weight]
FROM codeindex
group by DateTime
order by datetime
