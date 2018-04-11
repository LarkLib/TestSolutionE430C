IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetSummaryDateList')
    DROP PROCEDURE [dbo].[usp_GetSummaryDateList]
GO

CREATE PROCEDURE [usp_GetSummaryDateList] @idListXml XML
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

[usp_GetSummaryDateList] '<IdList>
  <Item Id="sz399001" />
  <Item Id="sz399101" />
  <Item Id="sz399102" />
  <Item Id="sh000001" />
  <Item Id="sz399300" />
  <Item Id="sz000001" />
  <Item Id="sz000002" />
  <Item Id="sz000008" />
  <Item Id="sz000009" />
  <Item Id="sz000027" />
  <Item Id="sz000039" />
  <Item Id="sz000060" />
  <Item Id="sz000061" />
  <Item Id="sz000063" />
  <Item Id="sz000069" />
  <Item Id="sz000100" />
  <Item Id="sz000156" />
  <Item Id="sz000157" />
  <Item Id="sz000166" />
  <Item Id="sz000333" />
  <Item Id="sz000338" />
  <Item Id="sz000402" />
  <Item Id="sz000413" />
  <Item Id="sz000415" />
  <Item Id="sz000423" />
  <Item Id="sz000425" />
  <Item Id="sz000503" />
  <Item Id="sz000538" />
  <Item Id="sz000540" />
  <Item Id="sz000555" />
  <Item Id="sz000559" />
  <Item Id="sz000568" />
  <Item Id="sz000623" />
  <Item Id="sz000625" />
  <Item Id="sz000627" />
  <Item Id="sz000630" />
  <Item Id="sz000651" />
  <Item Id="sz000671" />
  <Item Id="sz000686" />
  <Item Id="sz000709" />
  <Item Id="sz000712" />
  <Item Id="sz000718" />
  <Item Id="sz000725" />
  <Item Id="sz000728" />
  <Item Id="sz000738" />
  <Item Id="sz000750" />
  <Item Id="sz000768" />
  <Item Id="sz000776" />
  <Item Id="sz000778" />
  <Item Id="sz000783" />
  <Item Id="sz000792" />
  <Item Id="sz000793" />
  <Item Id="sz000800" />
  <Item Id="sz000826" />
  <Item Id="sz000839" />
  <Item Id="sz000858" />
  <Item Id="sz000876" />
  <Item Id="sz000895" />
  <Item Id="sz000917" />
  <Item Id="sz000938" />
  <Item Id="sz000963" />
  <Item Id="sz000977" />
  <Item Id="sz000983" />
  <Item Id="sz001979" />
  <Item Id="sz002007" />
  <Item Id="sz002008" />
  <Item Id="sz002024" />
  <Item Id="sz002027" />
  <Item Id="sz002049" />
  <Item Id="sz002065" />
  <Item Id="sz002074" />
  <Item Id="sz002081" />
  <Item Id="sz002085" />
  <Item Id="sz002129" />
  <Item Id="sz002131" />
  <Item Id="sz002142" />
  <Item Id="sz002146" />
  <Item Id="sz002152" />
  <Item Id="sz002153" />
  <Item Id="sz002174" />
  <Item Id="sz002183" />
  <Item Id="sz002195" />
  <Item Id="sz002202" />
  <Item Id="sz002230" />
  <Item Id="sz002236" />
  <Item Id="sz002241" />
  <Item Id="sz002252" />
  <Item Id="sz002292" />
  <Item Id="sz002299" />
  <Item Id="sz002304" />
  <Item Id="sz002310" />
  <Item Id="sz002385" />
  <Item Id="sz002415" />
  <Item Id="sz002424" />
  <Item Id="sz002426" />
  <Item Id="sz002450" />
  <Item Id="sz002456" />
  <Item Id="sz002465" />
  <Item Id="sz002466" />
  <Item Id="sz002470" />
  <Item Id="sz002475" />
  <Item Id="sz002500" />
  <Item Id="sz002568" />
  <Item Id="sz002594" />
  <Item Id="sz002673" />
  <Item Id="sz002714" />
  <Item Id="sz002736" />
  <Item Id="sz002739" />
  <Item Id="sz002797" />
  <Item Id="sz300002" />
  <Item Id="sz300015" />
  <Item Id="sz300017" />
  <Item Id="sz300024" />
  <Item Id="sz300027" />
  <Item Id="sz300033" />
  <Item Id="sz300058" />
  <Item Id="sz300059" />
  <Item Id="sz300070" />
  <Item Id="sz300072" />
  <Item Id="sz300085" />
  <Item Id="sz300104" />
  <Item Id="sz300124" />
  <Item Id="sz300133" />
  <Item Id="sz300144" />
  <Item Id="sz300146" />
  <Item Id="sz300168" />
  <Item Id="sz300182" />
  <Item Id="sz300251" />
  <Item Id="sz300315" />
  <Item Id="sh600000" />
  <Item Id="sh600005" />
  <Item Id="sh600008" />
  <Item Id="sh600009" />
  <Item Id="sh600010" />
  <Item Id="sh600015" />
  <Item Id="sh600016" />
  <Item Id="sh600018" />
  <Item Id="sh600019" />
  <Item Id="sh600021" />
  <Item Id="sh600023" />
  <Item Id="sh600028" />
  <Item Id="sh600029" />
  <Item Id="sh600030" />
  <Item Id="sh600031" />
  <Item Id="sh600036" />
  <Item Id="sh600037" />
  <Item Id="sh600038" />
  <Item Id="sh600048" />
  <Item Id="sh600050" />
  <Item Id="sh600060" />
  <Item Id="sh600061" />
  <Item Id="sh600066" />
  <Item Id="sh600068" />
  <Item Id="sh600074" />
  <Item Id="sh600085" />
  <Item Id="sh600089" />
  <Item Id="sh600100" />
  <Item Id="sh600104" />
  <Item Id="sh600109" />
  <Item Id="sh600111" />
  <Item Id="sh600115" />
  <Item Id="sh600118" />
  <Item Id="sh600150" />
  <Item Id="sh600153" />
  <Item Id="sh600157" />
  <Item Id="sh600170" />
  <Item Id="sh600177" />
  <Item Id="sh600188" />
  <Item Id="sh600196" />
  <Item Id="sh600208" />
  <Item Id="sh600221" />
  <Item Id="sh600252" />
  <Item Id="sh600256" />
  <Item Id="sh600271" />
  <Item Id="sh600276" />
  <Item Id="sh600297" />
  <Item Id="sh600309" />
  <Item Id="sh600332" />
  <Item Id="sh600340" />
  <Item Id="sh600352" />
  <Item Id="sh600362" />
  <Item Id="sh600369" />
  <Item Id="sh600372" />
  <Item Id="sh600373" />
  <Item Id="sh600376" />
  <Item Id="sh600383" />
  <Item Id="sh600406" />
  <Item Id="sh600415" />
  <Item Id="sh600446" />
  <Item Id="sh600482" />
  <Item Id="sh600485" />
  <Item Id="sh600489" />
  <Item Id="sh600498" />
  <Item Id="sh600518" />
  <Item Id="sh600519" />
  <Item Id="sh600535" />
  <Item Id="sh600547" />
  <Item Id="sh600570" />
  <Item Id="sh600582" />
  <Item Id="sh600583" />
  <Item Id="sh600585" />
  <Item Id="sh600588" />
  <Item Id="sh600606" />
  <Item Id="sh600637" />
  <Item Id="sh600648" />
  <Item Id="sh600649" />
  <Item Id="sh600654" />
  <Item Id="sh600660" />
  <Item Id="sh600663" />
  <Item Id="sh600666" />
  <Item Id="sh600674" />
  <Item Id="sh600685" />
  <Item Id="sh600688" />
  <Item Id="sh600690" />
  <Item Id="sh600703" />
  <Item Id="sh600704" />
  <Item Id="sh600705" />
  <Item Id="sh600718" />
  <Item Id="sh600737" />
  <Item Id="sh600739" />
  <Item Id="sh600741" />
  <Item Id="sh600754" />
  <Item Id="sh600783" />
  <Item Id="sh600795" />
  <Item Id="sh600804" />
  <Item Id="sh600816" />
  <Item Id="sh600820" />
  <Item Id="sh600827" />
  <Item Id="sh600837" />
  <Item Id="sh600839" />
  <Item Id="sh600867" />
  <Item Id="sh600871" />
  <Item Id="sh600873" />
  <Item Id="sh600875" />
  <Item Id="sh600886" />
  <Item Id="sh600887" />
  <Item Id="sh600893" />
  <Item Id="sh600895" />
  <Item Id="sh600900" />
  <Item Id="sh600958" />
  <Item Id="sh600959" />
  <Item Id="sh600999" />
  <Item Id="sh601006" />
  <Item Id="sh601009" />
  <Item Id="sh601018" />
  <Item Id="sh601021" />
  <Item Id="sh601088" />
  <Item Id="sh601099" />
  <Item Id="sh601111" />
  <Item Id="sh601118" />
  <Item Id="sh601127" />
  <Item Id="sh601155" />
  <Item Id="sh601166" />
  <Item Id="sh601169" />
  <Item Id="sh601186" />
  <Item Id="sh601198" />
  <Item Id="sh601211" />
  <Item Id="sh601216" />
  <Item Id="sh601225" />
  <Item Id="sh601258" />
  <Item Id="sh601288" />
  <Item Id="sh601318" />
  <Item Id="sh601328" />
  <Item Id="sh601333" />
  <Item Id="sh601336" />
  <Item Id="sh601377" />
  <Item Id="sh601390" />
  <Item Id="sh601398" />
  <Item Id="sh601555" />
  <Item Id="sh601600" />
  <Item Id="sh601601" />
  <Item Id="sh601607" />
  <Item Id="sh601608" />
  <Item Id="sh601611" />
  <Item Id="sh601618" />
  <Item Id="sh601628" />
  <Item Id="sh601633" />
  <Item Id="sh601668" />
  <Item Id="sh601669" />
  <Item Id="sh601688" />
  <Item Id="sh601718" />
  <Item Id="sh601727" />
  <Item Id="sh601766" />
  <Item Id="sh601788" />
  <Item Id="sh601800" />
  <Item Id="sh601818" />
  <Item Id="sh601857" />
  <Item Id="sh601866" />
  <Item Id="sh601872" />
  <Item Id="sh601877" />
  <Item Id="sh601888" />
  <Item Id="sh601899" />
  <Item Id="sh601901" />
  <Item Id="sh601919" />
  <Item Id="sh601928" />
  <Item Id="sh601933" />
  <Item Id="sh601939" />
  <Item Id="sh601958" />
  <Item Id="sh601985" />
  <Item Id="sh601988" />
  <Item Id="sh601989" />
  <Item Id="sh601998" />
  <Item Id="sh603000" />
  <Item Id="sh603885" />
  <Item Id="sh603993" />
</IdList>'