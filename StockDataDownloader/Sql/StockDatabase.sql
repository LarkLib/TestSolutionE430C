USE [master]
GO
/****** Object:  Database [Stock]    Script Date: 2/20/2017 12:48:19 ******/
CREATE DATABASE [Stock]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Stock', FILENAME = N'D:\GitDB\Stock.mdf' , SIZE = 430080KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Stock_log', FILENAME = N'D:\GitDB\Stock_log.ldf' , SIZE = 18240KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Stock] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Stock].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Stock] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Stock] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Stock] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Stock] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Stock] SET ARITHABORT OFF 
GO
ALTER DATABASE [Stock] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Stock] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Stock] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Stock] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Stock] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Stock] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Stock] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Stock] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Stock] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Stock] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Stock] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Stock] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Stock] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Stock] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Stock] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Stock] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Stock] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Stock] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Stock] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Stock] SET  MULTI_USER 
GO
ALTER DATABASE [Stock] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Stock] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Stock] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Stock] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Stock', N'ON'
GO
USE [Stock]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDetailDateList]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetDetailDateList] @idListXml XML
AS
BEGIN
    DECLARE @startDate TABLE([StockCode] NVARCHAR(15) NOT NULL)

    INSERT INTO           @startDate([StockCode])
    SELECT DISTINCT
           [ids].[item].value('@Id[1]', 'VARCHAR(10)')
    FROM   @idListXml.nodes('IdList/Item') [ids]([item])

    SELECT [Code],[DateTime] [ActionDate],[Id]
    FROM   [dbo].[Summary]  [so]
    JOIN @startDate [st] ON [st].[StockCode] = [so].[Code]
    WHERE  [so].[Status] = 0
END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetDetailDateListByCode]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetDetailDateListByCode] @stockCode NVARCHAR(50)
AS
BEGIN
    SELECT [Id], [DateTime] [ActionDate]
    FROM   [summary]
    WHERE  [Code] = @stockCode AND [Status] = 0
END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetSummaryDateList]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetSummaryDateList] @idListXml XML
AS
BEGIN
    DECLARE @startDate TABLE([StockCode] NVARCHAR(15) NOT NULL,
                             [StartDate] DATETIMEOFFSET(7),
                             [Status]    INT NOT NULL DEFAULT 1)
    DECLARE @stockOperation TABLE([Code]       NVARCHAR(50) NOT NULL,
                                  [ActionDate] DATETIMEOFFSET(7) NOT NULL)
    DECLARE @action INT
    DECLARE @tempDate DATETIMEOFFSET
    DECLARE @stockCode NVARCHAR(15)

    SET @action = 2

    UPDATE [StockOperation]
    SET    [Status] = -1
    WHERE  DATEDIFF([ss], [LastUpdateTime], SYSDATETIMEOFFSET()) > 30 AND (([Action] = @action AND [Status] = 0) OR [Action] = 1)

    INSERT INTO           @startDate([StockCode])
    SELECT DISTINCT
           [ids].[item].value('@Id[1]', 'VARCHAR(10)')
    FROM   @idListXml.nodes('IdList/Item') [ids]([item])

    UPDATE [sd]
    SET    [StartDate] = [si].[ListingDate]
    FROM @startDate [sd]
    JOIN [StockInfo] [si] ON [sd].[StockCode] = [si].[Code]

    UPDATE [sd]
    SET    [StartDate] = [so].[ActionDate]
    FROM @startDate [sd]
    JOIN(SELECT [Code], DATEADD([q], 1, MAX([ActionDate])) [ActionDate]
         FROM   [StockOperation]
         GROUP BY [Code]) [so] ON [sd].[StockCode] = [so].[Code]

    UPDATE @startDate
    SET    [Status] = 2
    WHERE  [StartDate] > [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](SYSDATETIMEOFFSET())

    SELECT TOP 1 @tempdate = [dbo].[fn_GetDataTimeOffsetQuarterFirstDay]([StartDate]), @stockCode = [StockCode]
    FROM         @startDate
    WHERE        [status] = 1 AND [startdate] IS NOT NULL

    WHILE @tempDate IS NOT NULL
    BEGIN
        WHILE @tempDate IS NOT NULL AND @tempDate <= [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](SYSDATETIMEOFFSET())
        BEGIN
            INSERT INTO                @stockOperation([Code], [ActionDate])
            VALUES                     (@stockCode, @tempDate)
            SET @tempDate = DATEADD([q], 1, @tempDate)
        END

        UPDATE @startDate
        SET    [StockCode] = @stockCode, [Status] = 2
        WHERE  [StockCode] = @stockCode AND [Status] = 1

        SET @tempDate = NULL
        SELECT TOP 1 @tempdate = [dbo].[fn_GetDataTimeOffsetQuarterFirstDay]([StartDate]), @stockCode = [StockCode]
        FROM         @startDate
        WHERE        [status] = 1 AND [startdate] IS NOT NULL
    END

    MERGE INTO [dbo].[StockOperation] [sot]
    USING @stockOperation [sov]
    ON [sot].[Code] = [sov].[Code] AND [sot].[ActionDate] = [sov].[ActionDate]
        WHEN NOT MATCHED
        THEN INSERT([Code], [Action], [Content], [Status], [ElapsedMilliseconds], [ActionDate]) VALUES([sov].[code], 0, [sov].[ActionDate], 0, 0, [sov].[ActionDate]);

    SELECT [Code], [ActionDate], [Id]
    FROM   [dbo].[StockOperation] [so]
    JOIN @startDate [st] ON [st].[StockCode] = [so].[Code]
    WHERE  ([so].[Status] = 0 AND [so].[Action] = 0) OR ([so].[Status] < 0)
END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetSummaryDateListByCode]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetSummaryDateListByCode] @stockCode NVARCHAR(50)
AS
BEGIN
    DECLARE @stockOperation TABLE([Code]       NVARCHAR(50) NOT NULL,
                                  [ActionDate] DATETIMEOFFSET(7) NOT NULL)
    DECLARE @action INT
    DECLARE @tempDate DATETIMEOFFSET
    DECLARE @startDate DATETIMEOFFSET

    SET @action = 2

    UPDATE [StockOperation]
    SET    [Status] = -1
    WHERE  DATEDIFF([ss], [LastUpdateTime], SYSDATETIMEOFFSET()) > 30 AND (([Action] = @action AND [Status] = 0) OR [Action] = 1)

    SELECT @startDate = DATEADD([q], 1, MAX([ActionDate]))
    FROM   [dbo].[StockOperation]
    WHERE  [Code] = @stockCode
    GROUP BY [Code]
    IF @startDate IS NULL
        SELECT @startDate = [ListingDate]
        FROM   [dbo].[StockInfo]
        WHERE  [Code] = @stockCode

    SELECT @tempdate = [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](@startDate)

    WHILE @tempDate IS NOT NULL AND @tempDate <= [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](SYSDATETIMEOFFSET())
    BEGIN
        INSERT INTO                @stockOperation([Code], [ActionDate])
        VALUES                     (@stockCode, @tempDate)
        SET @tempDate = DATEADD([q], 1, @tempDate)
    END

    MERGE INTO [dbo].[StockOperation] [sot]
    USING @stockOperation [sov]
    ON [sot].[Code] = [sov].[Code] AND [sot].[ActionDate] = [sov].[ActionDate]
        WHEN NOT MATCHED
        THEN INSERT([Code], [Action], [Content], [Status], [ElapsedMilliseconds], [ActionDate]) VALUES([sov].[code], 0, [sov].[ActionDate], 0, 0, [sov].[ActionDate]);

    SELECT [ActionDate], [Id]
    FROM   [dbo].[StockOperation] [so]
    WHERE  (([so].[Status] = 0 AND [so].[Action] = 0) OR ([so].[Status] < 0)) AND [code] = @stockCode
END

GO
/****** Object:  StoredProcedure [dbo].[usp_SaveStockContent]    Script Date: 2/20/2017 12:48:19 ******/
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
/****** Object:  StoredProcedure [dbo].[usp_SaveStockDetail]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SaveStockDetail] @stockDetailSql NVARCHAR(MAX),
                                             @actionDate     DATETIMEOFFSET,
                                             @stockCode      NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @transCount INT

    SET @transCount = @@tranCount
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ

    IF(@transCount = 0)
    BEGIN TRAN

    IF EXISTS(SELECT 1
              FROM   [dbo].[Detail] WITH (NOLOCK)
              WHERE  [DateTime] >= @actionDate AND [DateTime] < DATEADD([d], 1, @actionDate) AND [Code] = @stockCode)
    BEGIN
        DELETE [dbo].[Detail]
        WHERE  [id] IN(SELECT [id]
                       FROM   [dbo].[Detail] WITH (NOLOCK)
                       WHERE  [DateTime] >= @actionDate AND [DateTime] < DATEADD([d], 1, @actionDate) AND [Code] = @stockCode)
    END

    IF(@stockDetailSql IS NOT NULL)
        EXEC [sp_executesql] @stockDetailSql

    IF(@transCount = 0 AND @@tranCount > 0)
        COMMIT TRAN

    SET NOCOUNT OFF

END

GO
/****** Object:  StoredProcedure [dbo].[usp_SaveStockSummary]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SaveStockSummary] @stockSummaryXml XML
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @transCount INT
    SET @transCount = @@tranCount

    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
    IF(@transCount = 0)
    BEGIN TRAN

    DECLARE @stockSummaryTable TABLE([DateTime]     [DATETIMEOFFSET](7) NOT NULL,
                                     [Code]         [NVARCHAR](50) NOT NULL,
                                     [OpenPrice]    [DECIMAL](18, 3) NOT NULL,
                                     [HighestPrice] [DECIMAL](18, 3) NOT NULL,
                                     [LowestPrice]  [DECIMAL](18, 3) NOT NULL,
                                     [ClosePrice]   [DECIMAL](18, 3) NOT NULL,
                                     [Volume]       [INT] NOT NULL,
                                     [Amount]       [DECIMAL](18, 3) NOT NULL)

    INSERT INTO                   @stockSummaryTable([DateTime], [Code], [OpenPrice], [HighestPrice], [LowestPrice], [ClosePrice], [Volume], [Amount])
    SELECT [SummaryItems].[Item].value('DateTime[1]', '[DATETIMEOFFSET](7)') AS [DateTime], [SummaryItems].[Item].value('Code[1]', '[nvarchar](50)') AS [Code], [SummaryItems].[Item].value('OpenPrice[1]', '[DECIMAL](18, 3)') AS [OpenPrice], [SummaryItems].[Item].value('HighestPrice[1]', '[DECIMAL](18, 3)') AS [HighestPrice], [SummaryItems].[Item].value('LowestPrice[1]', '[DECIMAL](18, 3)') AS [LowestPrice], [SummaryItems].[Item].value('ClosePrice[1]', '[DECIMAL](18, 3)') AS [ClosePrice], [SummaryItems].[Item].value('Volume[1]', '[INT]') AS [Volume], [SummaryItems].[Item].value('Amount[1]', '[DECIMAL](18, 3)') AS [Amount]
    FROM   @stockSummaryXml.nodes('SummaryItems/Item') AS [SummaryItems]([Item])

    SET NOCOUNT OFF

    MERGE INTO [dbo].[Summary] [s]
    USING @stockSummaryTable [t]
    ON [s].DateTime = [t].DateTime AND [s].[Code] = [t].[Code]
        WHEN NOT MATCHED
        THEN INSERT([DateTime], [Code], [OpenPrice], [HighestPrice], [LowestPrice], [ClosePrice], [Volume], [Amount]) VALUES([t].[DateTime], [t].[Code], [t].[OpenPrice], [t].[HighestPrice], [t].[LowestPrice], [t].[ClosePrice], [t].[Volume], [t].[Amount]);
    IF(@transCount = 0 AND @@tranCount > 0)
        COMMIT TRAN
END

GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateStockOperationStatus]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_UpdateStockOperationStatus] @id                  UNIQUEIDENTIFIER,
                                                  @action              INT,
                                                  @status              INT,
                                                  @content             NVARCHAR(50)     = NULL,
                                                  @message             NVARCHAR(2000)   = NULL,
                                                  @elapsedMilliseconds INT              = 0
AS
BEGIN
    UPDATE [dbo].[StockOperation]
    SET    [Action] = @action, -- int
    [Content] = @content, -- nvarchar
    [Status] = @status, -- int
    [ElapsedMilliseconds] = @elapsedMilliseconds, -- int
    [Message] = @message, -- nvarchar
    [LastUpdateTime] = SYSDATETIMEOFFSET() -- datetimeoffset 
    WHERE  [Id] = @id
END

GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateStockSummaryStatus]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_UpdateStockSummaryStatus] @id                  UNIQUEIDENTIFIER,
                                                @status              INT              = 1,
                                                @content             NVARCHAR(50)     = NULL,
                                                @elapsedMilliseconds INT              = 0
AS
BEGIN
    UPDATE [dbo].[Summary]
    SET [Content] = COALESCE(@content, [Content]), -- nvarchar
	   [Status] = @status, -- int
	   [ElapsedMilliseconds] = @elapsedMilliseconds, -- int
	   [LastUpdateTime] = SYSDATETIMEOFFSET() -- datetimeoffset 
    WHERE  [Id] = @id
END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetDataTimeOffsetQuarterFirstDay]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_GetDataTimeOffsetQuarterFirstDay](@date DATETIMEOFFSET)
RETURNS DATETIMEOFFSET
AS
BEGIN
    --RETURN CONVERT(CHAR(8), DATEPART(YEAR, @date) * 10000 + (DATEPART(QUARTER, @date) * 3 - 2) * 100 + 1)+' 00:00:00'+DATENAME([tz], @date)
    RETURN TODATETIMEOFFSET(CAST(DATEADD([qq], DATEDIFF([qq], 0, @date), 0) AS DATETIMEOFFSET), DATENAME([tz], @date))
END

GO
/****** Object:  Table [dbo].[Detail]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Detail](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Price] [decimal](18, 3) NOT NULL,
	[PriceOffset] [decimal](18, 3) NOT NULL,
	[Volume] [int] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[Direction] [nvarchar](50) NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Detail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RealTimeData]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RealTimeData](
	[Id] [uniqueidentifier] NOT NULL,
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
	[LastModifyDate] [datetime] NOT NULL,
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
	[SaleVolume5] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RealTimeData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[startDate]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[startDate](
	[StockCode] [nvarchar](15) NOT NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[Status] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StockInfo]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ListingDate] [datetimeoffset](7) NULL,
	[CompanyInfo] [xml] NULL,
	[Category] [xml] NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StockOperation]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockOperation](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Action] [int] NOT NULL,
	[Content] [nvarchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[ElapsedMilliseconds] [int] NOT NULL,
	[Message] [nvarchar](2000) NULL,
	[ActionDate] [datetimeoffset](7) NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_StockOperation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Summary]    Script Date: 2/20/2017 12:48:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Summary](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[OpenPrice] [decimal](18, 3) NOT NULL,
	[HighestPrice] [decimal](18, 3) NOT NULL,
	[LowestPrice] [decimal](18, 3) NOT NULL,
	[ClosePrice] [decimal](18, 3) NOT NULL,
	[Volume] [int] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
	[Status] [int] NOT NULL,
	[Content] [nvarchar](50) NOT NULL,
	[ElapsedMilliseconds] [int] NOT NULL,
 CONSTRAINT [PK_Summary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Detail] ADD  CONSTRAINT [DF_Detail_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[Detail] ADD  CONSTRAINT [DF_Detail_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
ALTER TABLE [dbo].[RealTimeData] ADD  CONSTRAINT [DF_RealTimeData_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[RealTimeData] ADD  CONSTRAINT [DF_RealTimeData_LastModifyDate]  DEFAULT (getdate()) FOR [LastModifyDate]
GO
ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [DF_StockInfo_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [DF_StockInfon_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
ALTER TABLE [dbo].[StockOperation] ADD  CONSTRAINT [DF_StockOperation_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[StockOperation] ADD  CONSTRAINT [DF_StockOperation_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
ALTER TABLE [dbo].[Summary] ADD  CONSTRAINT [DF_Summary_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[Summary] ADD  CONSTRAINT [DF_Summary_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
ALTER TABLE [dbo].[Summary] ADD  CONSTRAINT [DF__Summary__Status]  DEFAULT ((0)) FOR [Status]
GO
USE [master]
GO
ALTER DATABASE [Stock] SET  READ_WRITE 
GO
