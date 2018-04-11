IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_SaveStockContent')
    DROP PROCEDURE [dbo].[usp_SaveStockContent]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_SaveStockContent]
    @stockContentXml XML
AS
/*
<StockInfos>
  <StockInfo>
    <StockCode>sz399001</StockCode>
    <StockName>深证成指</StockName>
    <OpenPrice>10207.428</OpenPrice>
    <ClosePrice>10215.477</ClosePrice>
    <LastPrice>10224.017</LastPrice>
    <LowestPrice>10194.095</LowestPrice>
    <HighestPrice>10236.093</HighestPrice>
    <BuyPrice>0.000</BuyPrice>
    <SalePrice>0.000</SalePrice>
    <Volume>0</Volume>
    <Turnover>92460631442.535</Turnover>
    <HostTime>2017-01-12T11:09:15</HostTime>
    <BuyPrice1>0.000</BuyPrice1>
    <BuyPrice2>0.000</BuyPrice2>
    <BuyPrice3>0.000</BuyPrice3>
    <BuyPrice4>0.000</BuyPrice4>
    <BuyPrice5>0.000</BuyPrice5>
    <BuyVolume1>0</BuyVolume1>
    <BuyVolume2>0</BuyVolume2>
    <BuyVolume3>0</BuyVolume3>
    <BuyVolume4>0</BuyVolume4>
    <BuyVolume5>0</BuyVolume5>
    <SalePrice1>0.000</SalePrice1>
    <SalePrice2>0.000</SalePrice2>
    <SalePrice3>0.000</SalePrice3>
    <SalePrice4>0.000</SalePrice4>
    <SalePrice5>0.000</SalePrice5>
    <SaleVolume1>0</SaleVolume1>
    <SaleVolume2>0</SaleVolume2>
    <SaleVolume3>0</SaleVolume3>
    <SaleVolume4>0</SaleVolume4>
    <SaleVolume5>0</SaleVolume5>
  </StockInfo>
  <StockInfo>
    <StockCode>sz399101</StockCode>
    <StockName>中小板综</StockName>
    <OpenPrice>11471.972</OpenPrice>
    <ClosePrice>11484.106</ClosePrice>
    <LastPrice>11478.021</LastPrice>
    <LowestPrice>11452.197</LowestPrice>
    <HighestPrice>11505.275</HighestPrice>
    <BuyPrice>0.000</BuyPrice>
    <SalePrice>0.000</SalePrice>
    <Volume>0</Volume>
    <Turnover>37087784525.930</Turnover>
    <HostTime>2017-01-12T11:09:15</HostTime>
    <BuyPrice1>0.000</BuyPrice1>
    <BuyPrice2>0.000</BuyPrice2>
    <BuyPrice3>0.000</BuyPrice3>
    <BuyPrice4>0.000</BuyPrice4>
    <BuyPrice5>0.000</BuyPrice5>
    <BuyVolume1>0</BuyVolume1>
    <BuyVolume2>0</BuyVolume2>
    <BuyVolume3>0</BuyVolume3>
    <BuyVolume4>0</BuyVolume4>
    <BuyVolume5>0</BuyVolume5>
    <SalePrice1>0.000</SalePrice1>
    <SalePrice2>0.000</SalePrice2>
    <SalePrice3>0.000</SalePrice3>
    <SalePrice4>0.000</SalePrice4>
    <SalePrice5>0.000</SalePrice5>
    <SaleVolume1>0</SaleVolume1>
    <SaleVolume2>0</SaleVolume2>
    <SaleVolume3>0</SaleVolume3>
    <SaleVolume4>0</SaleVolume4>
    <SaleVolume5>0</SaleVolume5>
  </StockInfo>
  <StockInfo>
    <StockCode>sz399102</StockCode>
    <StockName>创业板综</StockName>
    <OpenPrice>2571.377</OpenPrice>
    <ClosePrice>2573.661</ClosePrice>
    <LastPrice>2580.089</LastPrice>
    <LowestPrice>2571.377</LowestPrice>
    <HighestPrice>2583.408</HighestPrice>
    <BuyPrice>0.000</BuyPrice>
    <SalePrice>0.000</SalePrice>
    <Volume>1067230131</Volume>
    <Turnover>23844892314.890</Turnover>
    <HostTime>2017-01-12T11:09:15</HostTime>
    <BuyPrice1>0.000</BuyPrice1>
    <BuyPrice2>0.000</BuyPrice2>
    <BuyPrice3>0.000</BuyPrice3>
    <BuyPrice4>0.000</BuyPrice4>
    <BuyPrice5>0.000</BuyPrice5>
    <BuyVolume1>0</BuyVolume1>
    <BuyVolume2>0</BuyVolume2>
    <BuyVolume3>0</BuyVolume3>
    <BuyVolume4>0</BuyVolume4>
    <BuyVolume5>0</BuyVolume5>
    <SalePrice1>0.000</SalePrice1>
    <SalePrice2>0.000</SalePrice2>
    <SalePrice3>0.000</SalePrice3>
    <SalePrice4>0.000</SalePrice4>
    <SalePrice5>0.000</SalePrice5>
    <SaleVolume1>0</SaleVolume1>
    <SaleVolume2>0</SaleVolume2>
    <SaleVolume3>0</SaleVolume3>
    <SaleVolume4>0</SaleVolume4>
    <SaleVolume5>0</SaleVolume5>
  </StockInfo>
  <StockInfo>
    <StockCode>sh000001</StockCode>
    <StockName>上证指数</StockName>
    <OpenPrice>3133.6015</OpenPrice>
    <ClosePrice>3136.7535</ClosePrice>
    <LastPrice>3143.8976</LastPrice>
    <LowestPrice>3132.7004</LowestPrice>
    <HighestPrice>3144.7485</HighestPrice>
    <BuyPrice>0</BuyPrice>
    <SalePrice>0</SalePrice>
    <Volume>66975977</Volume>
    <Turnover>73216631849</Turnover>
    <HostTime>2017-01-12T11:09:08</HostTime>
    <BuyPrice1>0</BuyPrice1>
    <BuyPrice2>0</BuyPrice2>
    <BuyPrice3>0</BuyPrice3>
    <BuyPrice4>0</BuyPrice4>
    <BuyPrice5>0</BuyPrice5>
    <BuyVolume1>0</BuyVolume1>
    <BuyVolume2>0</BuyVolume2>
    <BuyVolume3>0</BuyVolume3>
    <BuyVolume4>0</BuyVolume4>
    <BuyVolume5>0</BuyVolume5>
    <SalePrice1>0</SalePrice1>
    <SalePrice2>0</SalePrice2>
    <SalePrice3>0</SalePrice3>
    <SalePrice4>0</SalePrice4>
    <SalePrice5>0</SalePrice5>
    <SaleVolume1>0</SaleVolume1>
    <SaleVolume2>0</SaleVolume2>
    <SaleVolume3>0</SaleVolume3>
    <SaleVolume4>0</SaleVolume4>
    <SaleVolume5>0</SaleVolume5>
  </StockInfo>
</StockInfos>
*/
BEGIN
    SET NOCOUNT ON

    DECLARE @transCount INT
    SET @transCount = @@TranCount

    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
    IF (@transCount = 0)
        BEGIN TRAN

    DECLARE @stockContentTable TABLE(
		[StockCode] [nvarchar](15) NOT NULL,
		[StockName] [nvarchar](15) NOT NULL,
		[OpenPrice] [decimal](18, 3) NOT NULL,
		[ClosePrice] [decimal](18, 3) NOT NULL,
		[LastPrice] [decimal](18, 3) NOT NULL,
		[LowestPrice] [decimal](18, 3) NOT NULL,
		[HighestPrice] [decimal](18, 3) NOT NULL,
		[BuyPrice] [decimal](18, 3) NOT NULL,
		[SalePrice] [decimal](18, 3) NOT NULL,
		[Volume] [int] NOT NULL,
		[Turnover] [decimal](18, 3) NOT NULL,
		[HostTime] [datetime] NOT NULL,
		[BuyPrice1] [decimal](18, 3) NOT NULL,
		[BuyPrice2] [decimal](18, 3) NOT NULL,
		[BuyPrice3] [decimal](18, 3) NOT NULL,
		[BuyPrice4] [decimal](18, 3) NOT NULL,
		[BuyPrice5] [decimal](18, 3) NOT NULL,
		[BuyVolume1] [int] NOT NULL,
		[BuyVolume2] [int] NOT NULL,
		[BuyVolume3] [int] NOT NULL,
		[BuyVolume4] [int] NOT NULL,
		[BuyVolume5] [int] NOT NULL,
		[SalePrice1] [decimal](18, 3) NOT NULL,
		[SalePrice2] [decimal](18, 3) NOT NULL,
		[SalePrice3] [decimal](18, 3) NOT NULL,
		[SalePrice4] [decimal](18, 3) NOT NULL,
		[SalePrice5] [decimal](18, 3) NOT NULL,
		[SaleVolume1] [int] NOT NULL,
		[SaleVolume2] [int] NOT NULL,
		[SaleVolume3] [int] NOT NULL,
		[SaleVolume4] [int] NOT NULL,
		[SaleVolume5] [int] NOT NULL)
    
    INSERT @stockContentTable ([StockCode],[StockName],[OpenPrice],[ClosePrice],[LastPrice],[LowestPrice],[HighestPrice],[BuyPrice],[SalePrice],[Volume],[Turnover],[HostTime],[BuyPrice1],[BuyPrice2],[BuyPrice3],[BuyPrice4],[BuyPrice5],[BuyVolume1],[BuyVolume2],[BuyVolume3],[BuyVolume4],[BuyVolume5],[SalePrice1],[SalePrice2],[SalePrice3],[SalePrice4],[SalePrice5],[SaleVolume1],[SaleVolume2],[SaleVolume3],[SaleVolume4],[SaleVolume5])
        SELECT	StockInfos.StockInfo.value('StockCode[1]', '[nvarchar](15)') AS [StockCode],
				StockInfos.StockInfo.value('StockName[1]', '[nvarchar](15)') AS [StockName],
				StockInfos.StockInfo.value('OpenPrice[1]', '[decimal](18,3)') AS [OpenPrice],
				StockInfos.StockInfo.value('ClosePrice[1]', '[decimal](18,3)') AS [ClosePrice],
				StockInfos.StockInfo.value('LastPrice[1]', '[decimal](18,3)') AS [LastPrice],
				StockInfos.StockInfo.value('LowestPrice[1]', '[decimal](18,3)') AS [LowestPrice],
				StockInfos.StockInfo.value('HighestPrice[1]', '[decimal](18,3)') AS [HighestPrice],
				StockInfos.StockInfo.value('BuyPrice[1]', '[decimal](18,3)') AS [BuyPrice],
				StockInfos.StockInfo.value('SalePrice[1]', '[decimal](18,3)') AS [SalePrice],
				StockInfos.StockInfo.value('Volume[1]', '[int]') AS [Volume],
				StockInfos.StockInfo.value('Turnover[1]', '[decimal](18,3)') AS [Turnover],
				StockInfos.StockInfo.value('HostTime[1]', '[datetime]') AS [HostTime],
				StockInfos.StockInfo.value('BuyPrice1[1]', '[decimal](18,3)') AS [BuyPrice1],
				StockInfos.StockInfo.value('BuyPrice2[1]', '[decimal](18,3)') AS [BuyPrice2],
				StockInfos.StockInfo.value('BuyPrice3[1]', '[decimal](18,3)') AS [BuyPrice3],
				StockInfos.StockInfo.value('BuyPrice4[1]', '[decimal](18,3)') AS [BuyPrice4],
				StockInfos.StockInfo.value('BuyPrice5[1]', '[decimal](18,3)') AS [BuyPrice5],
				StockInfos.StockInfo.value('BuyVolume1[1]', '[int]') AS [BuyVolume1],
				StockInfos.StockInfo.value('BuyVolume2[1]', '[int]') AS [BuyVolume2],
				StockInfos.StockInfo.value('BuyVolume3[1]', '[int]') AS [BuyVolume3],
				StockInfos.StockInfo.value('BuyVolume4[1]', '[int]') AS [BuyVolume4],
				StockInfos.StockInfo.value('BuyVolume5[1]', '[int]') AS [BuyVolume5],
				StockInfos.StockInfo.value('SalePrice1[1]', '[decimal](18,3)') AS [SalePrice1],
				StockInfos.StockInfo.value('SalePrice2[1]', '[decimal](18,3)') AS [SalePrice2],
				StockInfos.StockInfo.value('SalePrice3[1]', '[decimal](18,3)') AS [SalePrice3],
				StockInfos.StockInfo.value('SalePrice4[1]', '[decimal](18,3)') AS [SalePrice4],
				StockInfos.StockInfo.value('SalePrice5[1]', '[decimal](18,3)') AS [SalePrice5],
				StockInfos.StockInfo.value('SaleVolume1[1]', '[int]') AS [SaleVolume1],
				StockInfos.StockInfo.value('SaleVolume2[1]', '[int]') AS [SaleVolume2],
				StockInfos.StockInfo.value('SaleVolume3[1]', '[int]') AS [SaleVolume3],
				StockInfos.StockInfo.value('SaleVolume4[1]', '[int]') AS [SaleVolume4],
				StockInfos.StockInfo.value('SaleVolume5[1]', '[int]') AS [SaleVolume5]
        FROM @stockContentXml.nodes('StockInfos/StockInfo') AS StockInfos(StockInfo)

		MERGE INTO  dbo.RealTimeData rtd
        USING @stockContentTable t ON rtd.[HostTime] = t.[HostTime] AND  rtd.[StockCode] = t.[StockCode]
        WHEN NOT MATCHED THEN 
			INSERT ([StockCode],[StockName],[OpenPrice],[ClosePrice],[LastPrice],[LowestPrice],[HighestPrice],[BuyPrice],[SalePrice],[Volume],[Turnover],[HostTime],[BuyPrice1],[BuyPrice2],[BuyPrice3],[BuyPrice4],[BuyPrice5],[BuyVolume1],[BuyVolume2],[BuyVolume3],[BuyVolume4],[BuyVolume5],[SalePrice1],[SalePrice2],[SalePrice3],[SalePrice4],[SalePrice5],[SaleVolume1],[SaleVolume2],[SaleVolume3],[SaleVolume4],[SaleVolume5])
			VALUES( t.[StockCode],t.[StockName],t.[OpenPrice],t.[ClosePrice],t.[LastPrice],t.[LowestPrice],t.[HighestPrice],t.[BuyPrice],t.[SalePrice],t.[Volume],t.[Turnover],t.[HostTime],t.[BuyPrice1],t.[BuyPrice2],t.[BuyPrice3],t.[BuyPrice4],t.[BuyPrice5],t.[BuyVolume1],t.[BuyVolume2],t.[BuyVolume3],t.[BuyVolume4],t.[BuyVolume5],t.[SalePrice1],t.[SalePrice2],t.[SalePrice3],t.[SalePrice4],t.[SalePrice5],t.[SaleVolume1],t.[SaleVolume2],t.[SaleVolume3],t.[SaleVolume4],t.[SaleVolume5] );


    IF (@transCount = 0 AND @@TranCount > 0)
        COMMIT TRAN

    SET NOCOUNT OFF
END

GO


