IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_SaveStockSummary')
    DROP PROCEDURE [dbo].[usp_SaveStockSummary]
GO
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

dbo.usp_SaveStockSummary
'<SummaryItems>
	<Item>
		<DateTime>2001-03-30</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.990</OpenPrice>
		<HighestPrice>37.540</HighestPrice>
		<ClosePrice>37.470</ClosePrice>
		<LowestPrice>36.900</LowestPrice>
		<Volume>574383</Volume>
		<Amount>21486436</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-29</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>37.000</OpenPrice>
		<HighestPrice>37.110</HighestPrice>
		<ClosePrice>37.000</ClosePrice>
		<LowestPrice>36.850</LowestPrice>
		<Volume>476571</Volume>
		<Amount>17623403</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-28</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>37.000</OpenPrice>
		<HighestPrice>37.280</HighestPrice>
		<ClosePrice>37.000</ClosePrice>
		<LowestPrice>36.800</LowestPrice>
		<Volume>779891</Volume>
		<Amount>28890767</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-27</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.000</OpenPrice>
		<HighestPrice>37.500</HighestPrice>
		<ClosePrice>37.050</ClosePrice>
		<LowestPrice>36.000</LowestPrice>
		<Volume>848274</Volume>
		<Amount>31134405</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-26</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.000</OpenPrice>
		<HighestPrice>36.200</HighestPrice>
		<ClosePrice>36.100</ClosePrice>
		<LowestPrice>35.760</LowestPrice>
		<Volume>581550</Volume>
		<Amount>20895453</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-23</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.250</OpenPrice>
		<HighestPrice>36.450</HighestPrice>
		<ClosePrice>36.220</ClosePrice>
		<LowestPrice>36.050</LowestPrice>
		<Volume>431845</Volume>
		<Amount>15639863</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-22</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.480</OpenPrice>
		<HighestPrice>36.600</HighestPrice>
		<ClosePrice>36.210</ClosePrice>
		<LowestPrice>36.050</LowestPrice>
		<Volume>515439</Volume>
		<Amount>18692512</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-21</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.200</OpenPrice>
		<HighestPrice>36.850</HighestPrice>
		<ClosePrice>36.480</ClosePrice>
		<LowestPrice>35.900</LowestPrice>
		<Volume>732292</Volume>
		<Amount>26623907</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-20</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>36.020</OpenPrice>
		<HighestPrice>36.430</HighestPrice>
		<ClosePrice>36.240</ClosePrice>
		<LowestPrice>35.490</LowestPrice>
		<Volume>1648378</Volume>
		<Amount>59204174</Amount>
	</Item>
	<Item>
		<DateTime>2001-03-19</DateTime>
		<Code>sh600518</Code>
		<OpenPrice>40.000</OpenPrice>
		<HighestPrice>40.160</HighestPrice>
		<ClosePrice>36.420</ClosePrice>
		<LowestPrice>36.160</LowestPrice>
		<Volume>10221800</Volume>
		<Amount>385950651</Amount>
	</Item>
</SummaryItems>'