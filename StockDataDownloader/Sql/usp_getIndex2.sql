IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetDailyIndexExport')
    DROP PROCEDURE [dbo].[usp_GetDailyIndexExport]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetDailyIndexExport]    Script Date: 5/19/2017 21:14:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_GetDailyIndexExport] @beginDate DATETIMEOFFSET, @endDate DATETIMEOFFSET
AS
BEGIN
    with i as 
    (
	   select
	   m.[Datetime]							[DateTime],
	   m.[Volume]                                      ,
	   m.[Amount]                                      ,
	   m.[FlowingAmount]                               ,
	   m.[FlowingAmount100K]                           ,
	   m.[FlowingAmount500K]                           ,
	   case when COALESCE([TurnoverRate],0)>0 then m.[OpenPrice]			*d.[Volume]/[TurnoverRate]*100 else 0 end		[MarketAmountOpenPrice]		,
	   case when COALESCE([TurnoverRate],0)>0 then m.[HighestPrice]			*d.[Volume]/[TurnoverRate]*100 else 0 end		[MarketAmountHighestPrice]	,
	   case when COALESCE([TurnoverRate],0)>0 then m.[ClosePrice]			*d.[Volume]/[TurnoverRate]*100 else 0 end		[MarketAmountClosePrice]	,
	   case when COALESCE([TurnoverRate],0)>0 then m.[LowestPrice]			*d.[Volume]/[TurnoverRate]*100 else 0 end		[MarketAmountLowestPrice]
	   from VDaily m 
	   left join VDaily163 d on m.code=d.code and m.[Datetime] =d.datetime and d.datetime>=@beginDate and d.datetime<@endDate
	   left join VRightsOfferingInfo r on m.code=r.code and m.[Datetime]=r.datetime and r.datetime>=@beginDate and r.datetime<@endDate
	   where m.datetime>=@beginDate and m.datetime<@endDate
    )
    select 
    [DateTime]						[DateTime]				,
    sum(cast([Volume] as bigint)	  	)	[Volume]         			,
    sum([Amount]         			)	[Amount]         			,
    sum([FlowingAmount]  			)	[FlowingAmount]  			,
    sum([FlowingAmount100K]			)	[FlowingAmount100K]			,
    sum([FlowingAmount500K] 			)	[FlowingAmount500K] 		,
    sum([MarketAmountOpenPrice]		)	[MarketAmountOpenPrice]		,
    sum([MarketAmountHighestPrice]	)	[MarketAmountHighestPrice]	,
    sum([MarketAmountClosePrice]		)	[MarketAmountClosePrice]	,
    sum([MarketAmountLowestPrice]		)	[MarketAmountLowestPrice]	
    from i 
    group by datetime
    order by datetime
END
go
[usp_GetDailyIndexExport] '2012-03-01 00:00:00.0000000 +08:00', '2012-07-01 00:00:00.0000000 +08:00'