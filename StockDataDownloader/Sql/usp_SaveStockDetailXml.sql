IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_SaveStockDetail')
    DROP PROCEDURE [dbo].[usp_SaveStockDetail]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SaveStockDetail] @stockDetailXml XML
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @transCount INT
    SET @transCount = @@tranCount

    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
    IF(@transCount = 0)
    BEGIN TRAN

    DECLARE @stockDetailTable TABLE([DateTime]    [DATETIMEOFFSET](7) NOT NULL,
                                    [Code]        [NVARCHAR](50) NOT NULL,
                                    [Price]       [DECIMAL](18, 3) NOT NULL,
                                    [PriceOffset] [DECIMAL](18, 3) NOT NULL,
                                    [Volume]      [INT] NOT NULL,
                                    [Amount]      [DECIMAL](18, 3) NOT NULL,
                                    [Direction]   [NVARCHAR](50) NULL)

    INSERT INTO                  @stockDetailTable([DateTime], [Code], [Price], [PriceOffset], [Volume], [Amount], [Direction])
    SELECT [DetailItems].[Item].value('DateTime[1]', '[DATETIMEOFFSET](7)') AS [DateTime], [DetailItems].[Item].value('Code[1]', '[nvarchar](50)') AS [Code], [DetailItems].[Item].value('Price[1]', '[DECIMAL](18, 3)') AS [Price], [DetailItems].[Item].value('PriceOffset[1]', '[DECIMAL](18, 3)') AS [PriceOffset], [DetailItems].[Item].value('Volume[1]', '[INT]') AS [Volume], [DetailItems].[Item].value('Amount[1]', '[DECIMAL](18, 3)') AS [Amount], [DetailItems].[Item].value('Direction[1]', '[nvarchar](50)') AS [Direction]
    FROM   @stockDetailXml.nodes('DetailItems/Item') AS [DetailItems]([Item])

    SET NOCOUNT OFF

    MERGE INTO [dbo].[Detail] [s]
    USING @stockDetailTable [t]
    ON [s].DateTime = [t].DateTime AND [s].[Code] = [t].[Code]
        WHEN NOT MATCHED
        THEN INSERT([DateTime], [Code], [Price], [PriceOffset], [Volume], [Amount], [Direction]) VALUES([t].[DateTime], [t].[Code], [t].[Price], [t].[PriceOffset], [t].[Volume], [t].[Amount], [t].[Direction]);

    IF(@transCount = 0 AND @@tranCount > 0)
        COMMIT TRAN
END
GO

dbo.usp_SaveStockDetail
'<DetailItems>
	<Item>
		<DateTime>1/4/2007 14:59:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>43000</Volume>
		<Amount>362060</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>66000</Volume>
		<Amount>556380</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7500</Volume>
		<Amount>63000</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>148800</Volume>
		<Amount>1250172</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>62100</Volume>
		<Amount>524731</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25290</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>37100</Volume>
		<Amount>312753</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>44500</Volume>
		<Amount>375580</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2900</Volume>
		<Amount>24476</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:59:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20200</Volume>
		<Amount>170286</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20400</Volume>
		<Amount>171972</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20600</Volume>
		<Amount>173658</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>15200</Volume>
		<Amount>128136</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4500</Volume>
		<Amount>37980</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>45700</Volume>
		<Amount>385824</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:58:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>125400</Volume>
		<Amount>1056137</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>38900</Volume>
		<Amount>327927</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1800</Volume>
		<Amount>15174</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>31900</Volume>
		<Amount>268917</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>7200</Volume>
		<Amount>60839</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14300</Volume>
		<Amount>120549</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>13900</Volume>
		<Amount>117750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:57:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>43900</Volume>
		<Amount>370954</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>88800</Volume>
		<Amount>749472</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13900</Volume>
		<Amount>117177</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>26900</Volume>
		<Amount>227019</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>28300</Volume>
		<Amount>238852</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29470</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>74300</Volume>
		<Amount>625606</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:56:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>10300</Volume>
		<Amount>86932</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>55800</Volume>
		<Amount>471509</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>40300</Volume>
		<Amount>338923</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8400</Volume>
		<Amount>70644</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>36800</Volume>
		<Amount>309488</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8200</Volume>
		<Amount>69550</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:55:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>445100</Volume>
		<Amount>3744031</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:54:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>588400</Volume>
		<Amount>4931378</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:53:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3800</Volume>
		<Amount>31767</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:53:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10100</Volume>
		<Amount>84436</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:53:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>55800</Volume>
		<Amount>466487</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:53:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>262900</Volume>
		<Amount>2198395</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>80800</Volume>
		<Amount>678720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>53600</Volume>
		<Amount>450776</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>45900</Volume>
		<Amount>385560</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11600</Volume>
		<Amount>97440</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:19 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>33300</Volume>
		<Amount>280053</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9900</Volume>
		<Amount>83544</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>269300</Volume>
		<Amount>2262204</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:51:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>14200</Volume>
		<Amount>119280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:50:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7300</Volume>
		<Amount>61393</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:50:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>757100</Volume>
		<Amount>6367967</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:49:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17500</Volume>
		<Amount>147875</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:49:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>49000</Volume>
		<Amount>414049</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:49:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>10700</Volume>
		<Amount>90629</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:49:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>160100</Volume>
		<Amount>1352845</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>20800</Volume>
		<Amount>177109</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>110200</Volume>
		<Amount>936700</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>89000</Volume>
		<Amount>756500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14700</Volume>
		<Amount>125097</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>41200</Volume>
		<Amount>350531</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8500</Volume>
		<Amount>72250</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>55600</Volume>
		<Amount>472600</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:48:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17400</Volume>
		<Amount>148074</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9200</Volume>
		<Amount>78292</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>27000</Volume>
		<Amount>230040</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5200</Volume>
		<Amount>44460</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17700</Volume>
		<Amount>151335</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26505</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>28000</Volume>
		<Amount>239400</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>29300</Volume>
		<Amount>250808</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>7400</Volume>
		<Amount>63492</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:47:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>29900</Volume>
		<Amount>257148</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>36400</Volume>
		<Amount>313040</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15200</Volume>
		<Amount>130720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14800</Volume>
		<Amount>127280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7600</Volume>
		<Amount>65360</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>32400</Volume>
		<Amount>278640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>42100</Volume>
		<Amount>362481</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14800</Volume>
		<Amount>127872</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:46:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>5500</Volume>
		<Amount>47465</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>8300</Volume>
		<Amount>71795</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>9900</Volume>
		<Amount>85437</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9100</Volume>
		<Amount>78715</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14600</Volume>
		<Amount>126290</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>15100</Volume>
		<Amount>130615</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>10800</Volume>
		<Amount>93528</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8200</Volume>
		<Amount>70930</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26600</Volume>
		<Amount>230494</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4200</Volume>
		<Amount>36372</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21500</Volume>
		<Amount>186190</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:45:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3700</Volume>
		<Amount>32079</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17360</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4500</Volume>
		<Amount>39015</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6800</Volume>
		<Amount>59024</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>14200</Volume>
		<Amount>123256</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>80200</Volume>
		<Amount>697740</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14773</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15700</Volume>
		<Amount>136590</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:44:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>66900</Volume>
		<Amount>582673</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:43:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3800</Volume>
		<Amount>33060</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:43:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7800</Volume>
		<Amount>67860</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:43:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>61700</Volume>
		<Amount>537016</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:43:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2300</Volume>
		<Amount>20010</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:43:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>222700</Volume>
		<Amount>1935263</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:42:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5300</Volume>
		<Amount>46057</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:42:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>30700</Volume>
		<Amount>267090</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:42:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5600</Volume>
		<Amount>48664</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:42:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7700</Volume>
		<Amount>66990</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:41:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>59400</Volume>
		<Amount>516779</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:41:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3700</Volume>
		<Amount>32152</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:41:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>28500</Volume>
		<Amount>247949</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:41:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>63700</Volume>
		<Amount>554143</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>57700</Volume>
		<Amount>500836</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>26040</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24400</Volume>
		<Amount>211792</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>33400</Volume>
		<Amount>290606</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8000</Volume>
		<Amount>69440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>86800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:40:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7400</Volume>
		<Amount>64232</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:39:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>46300</Volume>
		<Amount>402613</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:39:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>135900</Volume>
		<Amount>1174176</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:39:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:38:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>600</Volume>
		<Amount>5184</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:38:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1200</Volume>
		<Amount>10380</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:38:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>116200</Volume>
		<Amount>1004235</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:37:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2900</Volume>
		<Amount>24969</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:37:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1900</Volume>
		<Amount>16358</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:36:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3800</Volume>
		<Amount>32717</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:36:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>20200</Volume>
		<Amount>173720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:36:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>41900</Volume>
		<Amount>361447</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:35:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.62</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>5000</Volume>
		<Amount>43099</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:35:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17500</Volume>
		<Amount>150500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:35:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>38900</Volume>
		<Amount>334935</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:35:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>46900</Volume>
		<Amount>403340</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6500</Volume>
		<Amount>55900</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2500</Volume>
		<Amount>22084</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>83500</Volume>
		<Amount>718461</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2600</Volume>
		<Amount>22386</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>86100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:29 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6300</Volume>
		<Amount>54243</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7500</Volume>
		<Amount>64500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5300</Volume>
		<Amount>45580</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:33:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4500</Volume>
		<Amount>38745</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:32:59 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>28300</Volume>
		<Amount>244351</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:32:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.62</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>319500</Volume>
		<Amount>2754089</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:30:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4200</Volume>
		<Amount>36979</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:30:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.62</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25859</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:30:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.62</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6800</Volume>
		<Amount>58615</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:30:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>8300</Volume>
		<Amount>71463</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:29:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17260</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:29:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26691</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:29:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>44400</Volume>
		<Amount>383616</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:29:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>144400</Volume>
		<Amount>1248168</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8400</Volume>
		<Amount>73521</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7200</Volume>
		<Amount>62496</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2400</Volume>
		<Amount>20808</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17360</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7000</Volume>
		<Amount>60690</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2700</Volume>
		<Amount>23436</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>19118</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2700</Volume>
		<Amount>23463</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>71400</Volume>
		<Amount>619038</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:27:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9200</Volume>
		<Amount>79948</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>86800</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3500</Volume>
		<Amount>30380</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1900</Volume>
		<Amount>16511</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17360</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7100</Volume>
		<Amount>61628</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:26:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>126800</Volume>
		<Amount>1101892</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6600</Volume>
		<Amount>57354</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14700</Volume>
		<Amount>127742</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>900</Volume>
		<Amount>7821</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11400</Volume>
		<Amount>99066</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9900</Volume>
		<Amount>85932</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>900</Volume>
		<Amount>7821</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28000</Volume>
		<Amount>243320</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:25:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>40500</Volume>
		<Amount>351945</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1600</Volume>
		<Amount>13904</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>34200</Volume>
		<Amount>296856</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4300</Volume>
		<Amount>37281</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2604</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3600</Volume>
		<Amount>31212</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28800</Volume>
		<Amount>249869</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:24:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2100</Volume>
		<Amount>18207</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14739</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>76100</Volume>
		<Amount>659337</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2598</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>36300</Volume>
		<Amount>313995</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6500</Volume>
		<Amount>56225</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:23:04 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>13780</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:22:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>110200</Volume>
		<Amount>952516</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>800</Volume>
		<Amount>6880</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1400</Volume>
		<Amount>12040</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25800</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:34 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.61</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4800</Volume>
		<Amount>41328</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>121700</Volume>
		<Amount>1046766</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19400</Volume>
		<Amount>166840</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>69400</Volume>
		<Amount>596840</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:21:04 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>206200</Volume>
		<Amount>1773320</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8000</Volume>
		<Amount>68800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>159200</Volume>
		<Amount>1369120</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>72100</Volume>
		<Amount>620060</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6000</Volume>
		<Amount>51600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>13900</Volume>
		<Amount>119540</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:20:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>69200</Volume>
		<Amount>594428</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:19:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>22600</Volume>
		<Amount>194360</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:19:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28800</Volume>
		<Amount>247680</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:19:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9460</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:19:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>78700</Volume>
		<Amount>676820</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:19:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>41000</Volume>
		<Amount>351780</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.57</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>36000</Volume>
		<Amount>308520</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>39600</Volume>
		<Amount>338976</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1712</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3900</Volume>
		<Amount>33384</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23940</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:18:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>29000</Volume>
		<Amount>247967</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>55100</Volume>
		<Amount>471105</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>12000</Volume>
		<Amount>102600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7300</Volume>
		<Amount>62341</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>87500</Volume>
		<Amount>747249</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5500</Volume>
		<Amount>46969</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12794</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:17:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>54300</Volume>
		<Amount>463665</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:16:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>10000</Volume>
		<Amount>84900</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:16:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>65500</Volume>
		<Amount>556750</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:16:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>400</Volume>
		<Amount>3404</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:16:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>366900</Volume>
		<Amount>3112083</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:15:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2700</Volume>
		<Amount>22923</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:15:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14653</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:15:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>228200</Volume>
		<Amount>1935543</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:14:49 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7800</Volume>
		<Amount>65910</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:14:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>50100</Volume>
		<Amount>422844</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>500</Volume>
		<Amount>4220</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14415</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26800</Volume>
		<Amount>226192</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10900</Volume>
		<Amount>91996</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>84400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>16800</Volume>
		<Amount>141792</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:13:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5400</Volume>
		<Amount>45522</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2529</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>18100</Volume>
		<Amount>152764</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9400</Volume>
		<Amount>79242</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:19 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3900</Volume>
		<Amount>32877</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11800</Volume>
		<Amount>99474</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:12:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>18000</Volume>
		<Amount>151920</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:11:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3800</Volume>
		<Amount>32034</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:11:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11500</Volume>
		<Amount>96945</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:11:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>67200</Volume>
		<Amount>566496</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:11:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2300</Volume>
		<Amount>19389</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:10:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>300</Volume>
		<Amount>2529</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:10:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2400</Volume>
		<Amount>20232</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:10:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16860</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:10:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>12300</Volume>
		<Amount>103443</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>500</Volume>
		<Amount>4220</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4205</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3800</Volume>
		<Amount>32277</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1600</Volume>
		<Amount>13456</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4300</Volume>
		<Amount>36163</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:09:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11500</Volume>
		<Amount>96715</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:08:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7500</Volume>
		<Amount>63075</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:08:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>294100</Volume>
		<Amount>2473650</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:08:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>58400</Volume>
		<Amount>493479</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>84500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4400</Volume>
		<Amount>37180</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16900</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>36300</Volume>
		<Amount>306735</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>12200</Volume>
		<Amount>102968</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>600</Volume>
		<Amount>5070</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2400</Volume>
		<Amount>20280</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:07:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11000</Volume>
		<Amount>92949</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:06:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>172500</Volume>
		<Amount>1455900</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:05:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>42250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:05:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6300</Volume>
		<Amount>53234</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:05:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23659</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:05:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>800</Volume>
		<Amount>6759</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:04:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>49100</Volume>
		<Amount>415469</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:04:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12674</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:04:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14700</Volume>
		<Amount>124214</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:03:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>77200</Volume>
		<Amount>652340</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:03:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13200</Volume>
		<Amount>111936</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:03:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21200</Volume>
		<Amount>179911</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:03:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>224600</Volume>
		<Amount>1907567</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:02:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2700</Volume>
		<Amount>22950</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:02:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>1900</Volume>
		<Amount>16112</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:02:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>1900</Volume>
		<Amount>16150</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:02:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>17900</Volume>
		<Amount>151792</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10900</Volume>
		<Amount>93440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14450</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>564600</Volume>
		<Amount>4799100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5300</Volume>
		<Amount>45050</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>35100</Volume>
		<Amount>298350</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:01:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28000</Volume>
		<Amount>238000</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>42500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>29700</Volume>
		<Amount>252450</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19000</Volume>
		<Amount>161500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>22700</Volume>
		<Amount>192950</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8800</Volume>
		<Amount>74712</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>104300</Volume>
		<Amount>886550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>42500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>101200</Volume>
		<Amount>860200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>77300</Volume>
		<Amount>657050</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14450</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26350</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 14:00:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15000</Volume>
		<Amount>127500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19700</Volume>
		<Amount>167450</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>23100</Volume>
		<Amount>196350</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8000</Volume>
		<Amount>68000</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2500</Volume>
		<Amount>21250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:59:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>103900</Volume>
		<Amount>883150</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:58:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>32700</Volume>
		<Amount>277872</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:58:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2547</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:58:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3600</Volume>
		<Amount>30680</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:58:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>6900</Volume>
		<Amount>58868</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:58:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25470</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>84800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6500</Volume>
		<Amount>55120</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1694</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8470</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>42400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11200</Volume>
		<Amount>94976</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4700</Volume>
		<Amount>39809</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6800</Volume>
		<Amount>58036</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:57:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>240900</Volume>
		<Amount>2038183</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:56:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>34200</Volume>
		<Amount>288990</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:55:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4220</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:55:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>800</Volume>
		<Amount>6752</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:55:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11400</Volume>
		<Amount>96329</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:55:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>25500</Volume>
		<Amount>215474</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:54:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>72100</Volume>
		<Amount>607839</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>41700</Volume>
		<Amount>350280</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20500</Volume>
		<Amount>172200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4500</Volume>
		<Amount>37800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16600</Volume>
		<Amount>139440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4900</Volume>
		<Amount>41160</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>31600</Volume>
		<Amount>265440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>48800</Volume>
		<Amount>409920</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6900</Volume>
		<Amount>57960</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.05</PriceOffset>
		<Volume>618700</Volume>
		<Amount>5197164</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>58700</Volume>
		<Amount>496014</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:53:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16000</Volume>
		<Amount>135200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4225</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10400</Volume>
		<Amount>87879</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10700</Volume>
		<Amount>90414</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12674</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:39 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16000</Volume>
		<Amount>135200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3800</Volume>
		<Amount>32109</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4300</Volume>
		<Amount>36335</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>18500</Volume>
		<Amount>156325</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14900</Volume>
		<Amount>125904</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4225</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17000</Volume>
		<Amount>143650</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:52:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>78900</Volume>
		<Amount>667220</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:51:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>83900</Volume>
		<Amount>710123</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:51:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.05</PriceOffset>
		<Volume>175300</Volume>
		<Amount>1482028</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:50:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>111500</Volume>
		<Amount>936616</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:50:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15700</Volume>
		<Amount>131880</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:50:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.05</PriceOffset>
		<Volume>72700</Volume>
		<Amount>611503</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:50:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>331400</Volume>
		<Amount>2800329</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25260</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26071</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>23000</Volume>
		<Amount>194120</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>6000</Volume>
		<Amount>50460</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8100</Volume>
		<Amount>68202</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:49:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>227100</Volume>
		<Amount>1912182</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:48:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>37800</Volume>
		<Amount>317520</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:48:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15400</Volume>
		<Amount>129712</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5800</Volume>
		<Amount>49224</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>38000</Volume>
		<Amount>319200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>83300</Volume>
		<Amount>699720</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13300</Volume>
		<Amount>111720</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:47:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.05</PriceOffset>
		<Volume>444800</Volume>
		<Amount>3736857</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>10000</Volume>
		<Amount>84500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11700</Volume>
		<Amount>98748</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5800</Volume>
		<Amount>49009</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7000</Volume>
		<Amount>59080</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4900</Volume>
		<Amount>41356</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>200</Volume>
		<Amount>1688</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:45:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7800</Volume>
		<Amount>65910</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14348</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>60700</Volume>
		<Amount>512307</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8430</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9400</Volume>
		<Amount>79430</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14348</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:44:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11500</Volume>
		<Amount>97060</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9700</Volume>
		<Amount>81868</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9273</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4300</Volume>
		<Amount>36292</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16880</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3700</Volume>
		<Amount>31227</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5500</Volume>
		<Amount>46909</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1200</Volume>
		<Amount>10140</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>10600</Volume>
		<Amount>89818</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>12100</Volume>
		<Amount>102244</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23632</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:43:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7500</Volume>
		<Amount>63374</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:42:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>200</Volume>
		<Amount>1688</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:42:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>800</Volume>
		<Amount>7250</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:42:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4100</Volume>
		<Amount>34999</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:42:14 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.44</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>348400</Volume>
		<Amount>2940799</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:41:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14600</Volume>
		<Amount>123369</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:41:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2300</Volume>
		<Amount>19942</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:40:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16000</Volume>
		<Amount>135200</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:40:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16900</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:40:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>71900</Volume>
		<Amount>608612</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:40:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6400</Volume>
		<Amount>54144</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:40:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>59700</Volume>
		<Amount>505062</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:39:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>73400</Volume>
		<Amount>620964</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:39:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16920</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>37500</Volume>
		<Amount>317250</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>100</Volume>
		<Amount>847</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>44600</Volume>
		<Amount>377316</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>103100</Volume>
		<Amount>872226</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>62400</Volume>
		<Amount>528528</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:38:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>600</Volume>
		<Amount>5088</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6600</Volume>
		<Amount>55968</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>128900</Volume>
		<Amount>1093445</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16960</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>200</Volume>
		<Amount>1698</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7700</Volume>
		<Amount>65296</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:37:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>14300</Volume>
		<Amount>121264</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:36:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4800</Volume>
		<Amount>40752</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:36:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>267600</Volume>
		<Amount>2274608</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16200</Volume>
		<Amount>137700</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26300</Volume>
		<Amount>223550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3200</Volume>
		<Amount>27200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1800</Volume>
		<Amount>15300</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>29300</Volume>
		<Amount>249050</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:35:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>78500</Volume>
		<Amount>667250</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>18766</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17060</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26442</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>25589</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10000</Volume>
		<Amount>85300</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4000</Volume>
		<Amount>34120</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2900</Volume>
		<Amount>24736</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:34:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>366500</Volume>
		<Amount>3126244</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:33:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>42200</Volume>
		<Amount>360387</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:33:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17080</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:33:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>600</Volume>
		<Amount>5130</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:33:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3700</Volume>
		<Amount>31597</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:33:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8000</Volume>
		<Amount>68473</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:32:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>15100</Volume>
		<Amount>128802</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:32:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26473</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:32:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4300</Volume>
		<Amount>36679</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:32:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>149800</Volume>
		<Amount>1279291</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:31:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8400</Volume>
		<Amount>71736</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:31:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>30385</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:31:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>21500</Volume>
		<Amount>183609</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:30:59 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>32700</Volume>
		<Amount>279258</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:30:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29925</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:29:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6200</Volume>
		<Amount>53010</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:29:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>56100</Volume>
		<Amount>479655</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:29:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>18787</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:28:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13700</Volume>
		<Amount>116997</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:28:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4600</Volume>
		<Amount>39283</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:28:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6100</Volume>
		<Amount>52093</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:28:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8700</Volume>
		<Amount>74656</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3600</Volume>
		<Amount>30743</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2500</Volume>
		<Amount>21349</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2561</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8530</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4900</Volume>
		<Amount>41845</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9393</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2700</Volume>
		<Amount>23085</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>37100</Volume>
		<Amount>317205</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:27:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11500</Volume>
		<Amount>98235</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>39300</Volume>
		<Amount>335621</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>300</Volume>
		<Amount>2565</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8500</Volume>
		<Amount>72812</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4300</Volume>
		<Amount>36721</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12794</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>26600</Volume>
		<Amount>227163</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:26:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>6600</Volume>
		<Amount>56430</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:25:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>348100</Volume>
		<Amount>2970128</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:23:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5700</Volume>
		<Amount>48735</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:23:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15700</Volume>
		<Amount>134235</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:23:34 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>401000</Volume>
		<Amount>3428550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:22:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8540</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:22:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29889</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:22:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>358000</Volume>
		<Amount>3057319</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:20:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3600</Volume>
		<Amount>30780</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:20:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17300</Volume>
		<Amount>147915</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14400</Volume>
		<Amount>123120</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26100</Volume>
		<Amount>223155</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>120400</Volume>
		<Amount>1029420</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14600</Volume>
		<Amount>124830</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6000</Volume>
		<Amount>51300</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:19:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6900</Volume>
		<Amount>58995</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:18:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:18:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:18:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17000</Volume>
		<Amount>145180</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:18:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5200</Volume>
		<Amount>44407</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:18:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>44100</Volume>
		<Amount>376480</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4400</Volume>
		<Amount>37444</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3100</Volume>
		<Amount>27016</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7100</Volume>
		<Amount>60492</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9300</Volume>
		<Amount>79143</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5400</Volume>
		<Amount>46294</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:17:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8900</Volume>
		<Amount>76440</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4255</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7200</Volume>
		<Amount>61272</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>22700</Volume>
		<Amount>192950</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4400</Volume>
		<Amount>37444</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>25100</Volume>
		<Amount>213996</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:16:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>75000</Volume>
		<Amount>637500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2500</Volume>
		<Amount>21225</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20800</Volume>
		<Amount>176592</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8600</Volume>
		<Amount>73014</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11700</Volume>
		<Amount>99333</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19600</Volume>
		<Amount>166404</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29715</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:15:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>25700</Volume>
		<Amount>218193</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>27600</Volume>
		<Amount>234324</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>122300</Volume>
		<Amount>1038327</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16100</Volume>
		<Amount>136850</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3200</Volume>
		<Amount>27200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:39 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>59700</Volume>
		<Amount>506853</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24500</Volume>
		<Amount>208250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:14:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>101100</Volume>
		<Amount>860106</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>149900</Volume>
		<Amount>1274243</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>33500</Volume>
		<Amount>284750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>12400</Volume>
		<Amount>105276</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2100</Volume>
		<Amount>17829</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3500</Volume>
		<Amount>29715</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:13:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>91300</Volume>
		<Amount>776806</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19500</Volume>
		<Amount>165750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>91300</Volume>
		<Amount>776050</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>46800</Volume>
		<Amount>397800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5800</Volume>
		<Amount>49300</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17000</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>102300</Volume>
		<Amount>869550</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:12:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>22500</Volume>
		<Amount>191475</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:11:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>42300</Volume>
		<Amount>359550</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:11:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14450</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:11:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10100</Volume>
		<Amount>86104</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:11:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.51</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>311200</Volume>
		<Amount>2648703</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:10:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5900</Volume>
		<Amount>50326</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:10:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14517</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:10:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>27300</Volume>
		<Amount>232868</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:10:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>28100</Volume>
		<Amount>239973</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:09:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>80200</Volume>
		<Amount>685710</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:09:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5400</Volume>
		<Amount>46170</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:09:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>18700</Volume>
		<Amount>160072</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:09:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>87100</Volume>
		<Amount>745610</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:08:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.57</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26567</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:08:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>88400</Volume>
		<Amount>756704</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:08:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.57</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5000</Volume>
		<Amount>42850</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:08:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>10600</Volume>
		<Amount>90948</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:08:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.57</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>181000</Volume>
		<Amount>1551469</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9400</Volume>
		<Amount>80652</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>18900</Volume>
		<Amount>162162</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>36100</Volume>
		<Amount>310460</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>7100</Volume>
		<Amount>61060</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7700</Volume>
		<Amount>66066</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>29900</Volume>
		<Amount>257039</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>3200</Volume>
		<Amount>27520</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>25100</Volume>
		<Amount>215358</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:06:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21500</Volume>
		<Amount>184470</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:05:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2200</Volume>
		<Amount>18898</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:05:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>51800</Volume>
		<Amount>444444</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:05:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>131900</Volume>
		<Amount>1134589</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:05:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>117900</Volume>
		<Amount>1017477</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:05:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>26900</Volume>
		<Amount>232147</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:04:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>326800</Volume>
		<Amount>2823560</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:03:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>400</Volume>
		<Amount>3456</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:03:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8650</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3700</Volume>
		<Amount>31968</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4900</Volume>
		<Amount>43191</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12960</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24600</Volume>
		<Amount>212544</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>4900</Volume>
		<Amount>42336</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14800</Volume>
		<Amount>127724</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:02:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>22500</Volume>
		<Amount>194175</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11800</Volume>
		<Amount>101952</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:54 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>13400</Volume>
		<Amount>116389</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>43150</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12945</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>200</Volume>
		<Amount>1726</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>39200</Volume>
		<Amount>338688</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17260</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:01:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.63</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4900</Volume>
		<Amount>42287</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:00:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17280</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:00:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.62</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>513100</Volume>
		<Amount>4422922</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 13:00:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>121300</Volume>
		<Amount>1048032</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:30:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>600</Volume>
		<Amount>5184</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:30:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>68500</Volume>
		<Amount>592525</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:29:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3700</Volume>
		<Amount>32005</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:29:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.64</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>77600</Volume>
		<Amount>670464</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>38800</Volume>
		<Amount>336008</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9900</Volume>
		<Amount>85635</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>200</Volume>
		<Amount>1732</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:39 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10700</Volume>
		<Amount>92555</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.65</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>500</Volume>
		<Amount>4325</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>21500</Volume>
		<Amount>186190</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>700</Volume>
		<Amount>6062</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14000</Volume>
		<Amount>121240</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2600</Volume>
		<Amount>22516</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8100</Volume>
		<Amount>70146</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:28:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2300</Volume>
		<Amount>19918</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:27:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>300</Volume>
		<Amount>2601</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:27:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7500</Volume>
		<Amount>65025</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:27:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>88500</Volume>
		<Amount>767295</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8400</Volume>
		<Amount>72912</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5100</Volume>
		<Amount>44217</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.68</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>38300</Volume>
		<Amount>332444</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14773</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>30415</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:26:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>53400</Volume>
		<Amount>464046</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:25:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>82700</Volume>
		<Amount>719489</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:25:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>19118</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:25:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.69</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>14700</Volume>
		<Amount>127742</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:25:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>361300</Volume>
		<Amount>3143501</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5400</Volume>
		<Amount>47142</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1800</Volume>
		<Amount>15714</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>81300</Volume>
		<Amount>709792</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11900</Volume>
		<Amount>104006</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17460</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17480</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>23400</Volume>
		<Amount>204516</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1200</Volume>
		<Amount>10476</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8730</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:24:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>71600</Volume>
		<Amount>625574</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:23:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>26600</Volume>
		<Amount>232851</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:23:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2800</Volume>
		<Amount>24500</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:22:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>38900</Volume>
		<Amount>339986</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:22:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>134400</Volume>
		<Amount>1174656</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:22:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>500</Volume>
		<Amount>4365</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:21:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1800</Volume>
		<Amount>15732</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:21:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>343800</Volume>
		<Amount>3008250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:21:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>175100</Volume>
		<Amount>1529391</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:20:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8740</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5000</Volume>
		<Amount>43750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>600</Volume>
		<Amount>5244</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>157800</Volume>
		<Amount>1380750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7400</Volume>
		<Amount>64676</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1400</Volume>
		<Amount>12320</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>346100</Volume>
		<Amount>3029180</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17480</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:19:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>57400</Volume>
		<Amount>501676</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>400</Volume>
		<Amount>3496</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8740</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4000</Volume>
		<Amount>34960</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2100</Volume>
		<Amount>18354</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10700</Volume>
		<Amount>93625</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>145400</Volume>
		<Amount>1272320</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:18:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>51800</Volume>
		<Amount>454573</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:17:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>64400</Volume>
		<Amount>564144</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:16:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:16:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1400</Volume>
		<Amount>12264</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:16:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4400</Volume>
		<Amount>38544</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:16:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>45900</Volume>
		<Amount>402084</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:14:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>18600</Volume>
		<Amount>163122</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:13:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10900</Volume>
		<Amount>95510</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:13:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>27700</Volume>
		<Amount>243501</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:13:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>52200</Volume>
		<Amount>456750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:13:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14100</Volume>
		<Amount>123375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:12:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>24500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:12:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>43750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:12:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>53300</Volume>
		<Amount>466375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>300</Volume>
		<Amount>2625</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7800</Volume>
		<Amount>68250</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3400</Volume>
		<Amount>29750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13200</Volume>
		<Amount>115368</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8740</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:11:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4100</Volume>
		<Amount>35793</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:10:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>56400</Volume>
		<Amount>492936</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:10:19 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>3600</Volume>
		<Amount>31464</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:10:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1900</Volume>
		<Amount>16625</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:10:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>13125</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:10:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>125000</Volume>
		<Amount>1094423</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:09:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>53300</Volume>
		<Amount>466908</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:09:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5000</Volume>
		<Amount>43850</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:09:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>400</Volume>
		<Amount>3504</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:09:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8400</Volume>
		<Amount>73896</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:09:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4100</Volume>
		<Amount>35998</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:08:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>20900</Volume>
		<Amount>183730</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:08:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14200</Volume>
		<Amount>124817</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:08:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5700</Volume>
		<Amount>50695</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:07:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>16700</Volume>
		<Amount>146626</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:07:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>89300</Volume>
		<Amount>783161</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8770</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14925</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>39900</Volume>
		<Amount>350322</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17540</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>12400</Volume>
		<Amount>108871</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>10900</Volume>
		<Amount>95702</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4800</Volume>
		<Amount>42096</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:06:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>7300</Volume>
		<Amount>64093</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9647</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>16500</Volume>
		<Amount>144870</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11200</Volume>
		<Amount>98224</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5400</Volume>
		<Amount>47358</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4385</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>900</Volume>
		<Amount>7893</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:05:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>31300</Volume>
		<Amount>274746</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:04:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15900</Volume>
		<Amount>139602</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:04:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>18500</Volume>
		<Amount>162430</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:04:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>112600</Volume>
		<Amount>988944</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:03:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>12700</Volume>
		<Amount>111505</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:03:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>800</Volume>
		<Amount>7031</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:03:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>30600</Volume>
		<Amount>268974</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:02:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>75100</Volume>
		<Amount>661144</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:02:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21900</Volume>
		<Amount>192500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:02:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7500</Volume>
		<Amount>66105</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:02:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10600</Volume>
		<Amount>93280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:02:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>141100</Volume>
		<Amount>1242120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:01:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>19404</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:01:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8700</Volume>
		<Amount>76734</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:01:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>14300</Volume>
		<Amount>126126</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:00:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19200</Volume>
		<Amount>169536</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:00:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>19300</Volume>
		<Amount>170860</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 11:00:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.84</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>83900</Volume>
		<Amount>741676</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:59:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.84</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>65000</Volume>
		<Amount>574600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:59:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.85</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>47300</Volume>
		<Amount>418976</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:59:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.84</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>59800</Volume>
		<Amount>528632</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:58:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>97700</Volume>
		<Amount>862761</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.85</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>493700</Volume>
		<Amount>4369333</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4600</Volume>
		<Amount>40572</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8700</Volume>
		<Amount>76734</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5900</Volume>
		<Amount>52038</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.81</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>42100</Volume>
		<Amount>370909</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8830</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14400</Volume>
		<Amount>127152</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>1400</Volume>
		<Amount>12348</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:57:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.84</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>14700</Volume>
		<Amount>129948</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:56:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2700</Volume>
		<Amount>24069</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:56:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>15200</Volume>
		<Amount>134216</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:56:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>115200</Volume>
		<Amount>1016064</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:56:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>51800</Volume>
		<Amount>457493</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>300</Volume>
		<Amount>2646</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>88000</Volume>
		<Amount>777613</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.81</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11400</Volume>
		<Amount>100434</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.81</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9500</Volume>
		<Amount>83695</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.81</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>300</Volume>
		<Amount>2643</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:55:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.81</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>28100</Volume>
		<Amount>247561</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:54:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>103700</Volume>
		<Amount>912894</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:54:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>19300</Volume>
		<Amount>169840</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5100</Volume>
		<Amount>44828</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9680</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>12400</Volume>
		<Amount>109120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2600</Volume>
		<Amount>23584</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17700</Volume>
		<Amount>155936</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>37300</Volume>
		<Amount>328856</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:53:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>16000</Volume>
		<Amount>140640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1300</Volume>
		<Amount>11440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5100</Volume>
		<Amount>44828</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>3000</Volume>
		<Amount>26339</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>148200</Volume>
		<Amount>1304468</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8780</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>500</Volume>
		<Amount>4400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:52:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>52900</Volume>
		<Amount>464778</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5200</Volume>
		<Amount>45656</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2600</Volume>
		<Amount>23591</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6700</Volume>
		<Amount>58759</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8200</Volume>
		<Amount>71914</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2700</Volume>
		<Amount>23652</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1600</Volume>
		<Amount>14016</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:51:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>410900</Volume>
		<Amount>3600132</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:49:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1600</Volume>
		<Amount>14217</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:49:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>16400</Volume>
		<Amount>143664</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:49:39 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.77</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>18500</Volume>
		<Amount>162245</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:49:09 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>20800</Volume>
		<Amount>182208</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:49:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5500</Volume>
		<Amount>48125</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:48:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8750</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:48:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9900</Volume>
		<Amount>86625</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:48:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2600</Volume>
		<Amount>22776</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:48:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>97100</Volume>
		<Amount>850342</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:47:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>73200</Volume>
		<Amount>641232</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:47:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3500</Volume>
		<Amount>30625</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:47:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3700</Volume>
		<Amount>32375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:47:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>320600</Volume>
		<Amount>2805468</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:47:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>19900</Volume>
		<Amount>173926</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3000</Volume>
		<Amount>26250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7000</Volume>
		<Amount>61180</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9400</Volume>
		<Amount>82250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>65400</Volume>
		<Amount>572477</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>70300</Volume>
		<Amount>615772</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:46:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>34800</Volume>
		<Amount>304152</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4900</Volume>
		<Amount>42826</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5900</Volume>
		<Amount>51566</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>50500</Volume>
		<Amount>441370</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7000</Volume>
		<Amount>61180</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2200</Volume>
		<Amount>19250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5500</Volume>
		<Amount>48125</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:45:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6700</Volume>
		<Amount>58625</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>51800</Volume>
		<Amount>453250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17500</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.74</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>118400</Volume>
		<Amount>1034833</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1200</Volume>
		<Amount>10512</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2100</Volume>
		<Amount>19022</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:44:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>167800</Volume>
		<Amount>1468250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:43:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>152700</Volume>
		<Amount>1336125</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:43:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2000</Volume>
		<Amount>17440</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:43:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>63600</Volume>
		<Amount>553956</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:42:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>221200</Volume>
		<Amount>1929544</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:41:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11200</Volume>
		<Amount>97439</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:41:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6300</Volume>
		<Amount>54873</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:41:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9900</Volume>
		<Amount>86229</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:41:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>19500</Volume>
		<Amount>169650</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:41:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14700</Volume>
		<Amount>128037</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>55000</Volume>
		<Amount>478499</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6900</Volume>
		<Amount>60099</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24500</Volume>
		<Amount>213395</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.71</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>5600</Volume>
		<Amount>48776</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>14900</Volume>
		<Amount>130077</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1500</Volume>
		<Amount>13080</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:40:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>45100</Volume>
		<Amount>393272</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:39:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8000</Volume>
		<Amount>70080</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:39:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:39:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4500</Volume>
		<Amount>39375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:39:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>-0.05</PriceOffset>
		<Volume>240100</Volume>
		<Amount>2101715</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:38:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11500</Volume>
		<Amount>101200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:38:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5700</Volume>
		<Amount>50102</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:38:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7200</Volume>
		<Amount>63360</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:37:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10600</Volume>
		<Amount>93280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:37:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>42500</Volume>
		<Amount>374642</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:37:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>382500</Volume>
		<Amount>3366000</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:37:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>272100</Volume>
		<Amount>2394700</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>117700</Volume>
		<Amount>1036129</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5000</Volume>
		<Amount>44100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.82</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>25800</Volume>
		<Amount>227847</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>39200</Volume>
		<Amount>344960</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>48000</Volume>
		<Amount>422400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:36:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>38000</Volume>
		<Amount>334400</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>57700</Volume>
		<Amount>508068</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.83</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>16700</Volume>
		<Amount>147461</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15900</Volume>
		<Amount>139920</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14900</Volume>
		<Amount>131120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>180300</Volume>
		<Amount>1587150</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>21900</Volume>
		<Amount>193380</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>12900</Volume>
		<Amount>113390</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>35200</Volume>
		<Amount>309760</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>33000</Volume>
		<Amount>290400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:35:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16000</Volume>
		<Amount>140640</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>900</Volume>
		<Amount>7910</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>37400</Volume>
		<Amount>328811</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.8</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>63100</Volume>
		<Amount>555632</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6900</Volume>
		<Amount>60650</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>224000</Volume>
		<Amount>1967079</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.79</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>116600</Volume>
		<Amount>1024913</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.78</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>106900</Volume>
		<Amount>939152</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.76</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>22200</Volume>
		<Amount>194472</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:34:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.75</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>408300</Volume>
		<Amount>3572975</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:33:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>49600</Volume>
		<Amount>432512</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:33:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>22700</Volume>
		<Amount>198886</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>30200</Volume>
		<Amount>263344</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.73</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>23100</Volume>
		<Amount>201663</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.72</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>39000</Volume>
		<Amount>340080</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>31400</Volume>
		<Amount>273180</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3100</Volume>
		<Amount>26969</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>46800</Volume>
		<Amount>407159</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2600</Volume>
		<Amount>22619</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:32:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.7</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>207600</Volume>
		<Amount>1806528</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>240800</Volume>
		<Amount>2087736</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>24276</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26600</Volume>
		<Amount>230622</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.67</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>13300</Volume>
		<Amount>115311</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>103000</Volume>
		<Amount>891980</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:31:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.66</Price>
		<PriceOffset>0.06</PriceOffset>
		<Volume>646100</Volume>
		<Amount>5595477</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:30:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20200</Volume>
		<Amount>173720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:30:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2900</Volume>
		<Amount>24940</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:30:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.6</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>176900</Volume>
		<Amount>1521684</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:30:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14900</Volume>
		<Amount>127991</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:59 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>171700</Volume>
		<Amount>1474903</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8100</Volume>
		<Amount>69579</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>22000</Volume>
		<Amount>189409</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>37400</Volume>
		<Amount>321266</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.59</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>81200</Volume>
		<Amount>697533</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:29:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.58</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>239700</Volume>
		<Amount>2056643</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:28:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.56</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>36200</Volume>
		<Amount>309872</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:28:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6100</Volume>
		<Amount>52155</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:28:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14400</Volume>
		<Amount>123120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:28:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4275</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20100</Volume>
		<Amount>172504</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>53100</Volume>
		<Amount>454005</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28300</Volume>
		<Amount>241965</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>110900</Volume>
		<Amount>948879</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>78500</Volume>
		<Amount>671175</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20800</Volume>
		<Amount>177840</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>136500</Volume>
		<Amount>1167075</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>117800</Volume>
		<Amount>1007190</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:27:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>60800</Volume>
		<Amount>520147</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:26:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>10000</Volume>
		<Amount>85399</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:26:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1704</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:26:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>2100</Volume>
		<Amount>17892</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:26:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>327300</Volume>
		<Amount>2795218</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:25:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>496500</Volume>
		<Amount>4220828</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:24:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>17500</Volume>
		<Amount>148400</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:24:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>26700</Volume>
		<Amount>226683</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:24:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26100</Volume>
		<Amount>221328</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:24:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>317200</Volume>
		<Amount>2689856</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:23:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>18100</Volume>
		<Amount>153488</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:23:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15000</Volume>
		<Amount>127200</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:23:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>8200</Volume>
		<Amount>69536</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:23:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>37100</Volume>
		<Amount>314979</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:23:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>360700</Volume>
		<Amount>3062343</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:22:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>77700</Volume>
		<Amount>662004</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>22100</Volume>
		<Amount>188292</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6900</Volume>
		<Amount>58856</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>16800</Volume>
		<Amount>143304</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15100</Volume>
		<Amount>128953</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1400</Volume>
		<Amount>11955</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:21:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.54</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1016900</Volume>
		<Amount>8684804</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:20:04 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.55</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>790100</Volume>
		<Amount>6755731</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:18:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4000</Volume>
		<Amount>34080</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:18:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>29200</Volume>
		<Amount>248784</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:18:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.52</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21900</Volume>
		<Amount>186588</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:18:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.53</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>164000</Volume>
		<Amount>1399440</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>37300</Volume>
		<Amount>317050</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>78600</Volume>
		<Amount>668100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>25800</Volume>
		<Amount>219042</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19600</Volume>
		<Amount>166600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>168700</Volume>
		<Amount>1434553</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.5</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>87300</Volume>
		<Amount>742390</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10900</Volume>
		<Amount>92323</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>27200</Volume>
		<Amount>230384</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:17:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.49</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>53200</Volume>
		<Amount>451668</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:16:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.47</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>56400</Volume>
		<Amount>477708</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:16:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>62200</Volume>
		<Amount>527456</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:16:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>34700</Volume>
		<Amount>293215</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:16:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>71200</Volume>
		<Amount>601952</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:16:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>0.05</PriceOffset>
		<Volume>863200</Volume>
		<Amount>7294868</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>44400</Volume>
		<Amount>373531</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.39</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11400</Volume>
		<Amount>95646</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.39</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>77400</Volume>
		<Amount>649386</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.39</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>66700</Volume>
		<Amount>559613</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>72600</Volume>
		<Amount>609840</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>89400</Volume>
		<Amount>750960</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.39</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>21300</Volume>
		<Amount>178707</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>27000</Volume>
		<Amount>226260</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:14:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5700</Volume>
		<Amount>47766</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>99300</Volume>
		<Amount>832301</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11000</Volume>
		<Amount>91960</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.37</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>16300</Volume>
		<Amount>137025</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1500</Volume>
		<Amount>12525</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11200</Volume>
		<Amount>93632</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:13:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>28400</Volume>
		<Amount>237423</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9185</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4800</Volume>
		<Amount>40080</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>4400</Volume>
		<Amount>36740</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>110300</Volume>
		<Amount>921005</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14178</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>68000</Volume>
		<Amount>567800</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:12:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24400</Volume>
		<Amount>203252</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>25200</Volume>
		<Amount>209916</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>285600</Volume>
		<Amount>2376940</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8000</Volume>
		<Amount>66560</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8700</Volume>
		<Amount>72384</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>500</Volume>
		<Amount>4155</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:05 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2700</Volume>
		<Amount>22464</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:11:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>20000</Volume>
		<Amount>166400</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16600</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>6400</Volume>
		<Amount>53248</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>31100</Volume>
		<Amount>258437</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>14000</Volume>
		<Amount>116200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>25100</Volume>
		<Amount>208330</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>448000</Volume>
		<Amount>3718707</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:10:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8500</Volume>
		<Amount>70720</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>600</Volume>
		<Amount>4986</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:49 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5400</Volume>
		<Amount>44928</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:35 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9800</Volume>
		<Amount>81562</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>20400</Volume>
		<Amount>169524</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>600</Volume>
		<Amount>4988</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4150</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>24900</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>98800</Volume>
		<Amount>820430</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:09:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23240</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19100</Volume>
		<Amount>158530</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>61200</Volume>
		<Amount>507960</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16200</Volume>
		<Amount>134460</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>71400</Volume>
		<Amount>592620</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>22100</Volume>
		<Amount>183430</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>37500</Volume>
		<Amount>310874</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9600</Volume>
		<Amount>79583</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>200</Volume>
		<Amount>1660</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7100</Volume>
		<Amount>58930</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>32000</Volume>
		<Amount>265600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:08:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>55300</Volume>
		<Amount>458436</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:07:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7600</Volume>
		<Amount>63080</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:07:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>232700</Volume>
		<Amount>1931742</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>61000</Volume>
		<Amount>506300</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>26000</Volume>
		<Amount>215800</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16620</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>19400</Volume>
		<Amount>161214</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>12100</Volume>
		<Amount>100551</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:06:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5900</Volume>
		<Amount>49029</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:05:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5400</Volume>
		<Amount>44928</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:05:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>100</Volume>
		<Amount>832</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:05:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>45400</Volume>
		<Amount>376820</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:05:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>33700</Volume>
		<Amount>280721</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:05:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>85300</Volume>
		<Amount>709696</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:04:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20000</Volume>
		<Amount>166600</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:04:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>21200</Volume>
		<Amount>176596</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:04:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>13900</Volume>
		<Amount>115926</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:04:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1900</Volume>
		<Amount>15865</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:04:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>27900</Volume>
		<Amount>232686</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15200</Volume>
		<Amount>126768</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>18700</Volume>
		<Amount>155958</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3700</Volume>
		<Amount>30895</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>17200</Volume>
		<Amount>143620</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>25500</Volume>
		<Amount>212925</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:03:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>51600</Volume>
		<Amount>431375</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.37</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>700</Volume>
		<Amount>5858</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>16800</Volume>
		<Amount>141211</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.37</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>33900</Volume>
		<Amount>283743</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13100</Volume>
		<Amount>109778</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>49700</Volume>
		<Amount>416486</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:02:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.39</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>261700</Volume>
		<Amount>2195704</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:01:24 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>108400</Volume>
		<Amount>910560</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:01:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>53600</Volume>
		<Amount>451848</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:01:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16100</Volume>
		<Amount>136199</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:01:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>12900</Volume>
		<Amount>108489</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:01:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>32700</Volume>
		<Amount>274680</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:00:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2400</Volume>
		<Amount>20184</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:00:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>8400</Volume>
		<Amount>70644</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:00:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.41</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>28400</Volume>
		<Amount>238844</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:00:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26600</Volume>
		<Amount>223440</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 10:00:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0.05</PriceOffset>
		<Volume>1511100</Volume>
		<Amount>12693937</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:58:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16700</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:58:25 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.38</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>27700</Volume>
		<Amount>232126</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:58:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28600</Volume>
		<Amount>238810</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:58:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>13000</Volume>
		<Amount>108550</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:58:11 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>335400</Volume>
		<Amount>2803994</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:57:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.07</PriceOffset>
		<Volume>1244800</Volume>
		<Amount>10394213</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2100</Volume>
		<Amount>17388</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.27</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23974</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:41 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10800</Volume>
		<Amount>89100</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>182100</Volume>
		<Amount>1502325</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>6300</Volume>
		<Amount>51975</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:55:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>10000</Volume>
		<Amount>82400</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:54:55 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>39300</Volume>
		<Amount>324225</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:54:53 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15000</Volume>
		<Amount>123600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:54:47 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>25700</Volume>
		<Amount>211768</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:54:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>42400</Volume>
		<Amount>349895</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:54:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.22</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>813100</Volume>
		<Amount>6684380</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:52:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.2</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>351800</Volume>
		<Amount>2884759</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:51:23 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>10400</Volume>
		<Amount>85696</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:51:17 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.23</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2100</Volume>
		<Amount>17283</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:51:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>23800</Volume>
		<Amount>196112</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:51:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7900</Volume>
		<Amount>65096</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:50:59 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>50500</Volume>
		<Amount>416120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:50:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13000</Volume>
		<Amount>107250</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:50:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>95500</Volume>
		<Amount>787875</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:50:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.24</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>2300</Volume>
		<Amount>18952</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>9600</Volume>
		<Amount>79200</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23100</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>55000</Volume>
		<Amount>453750</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>24700</Volume>
		<Amount>203775</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>23600</Volume>
		<Amount>194700</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>700</Volume>
		<Amount>5775</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1100</Volume>
		<Amount>9086</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>24780</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>8100</Volume>
		<Amount>66906</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26100</Volume>
		<Amount>215325</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>52400</Volume>
		<Amount>432300</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:49:00 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>3300</Volume>
		<Amount>27258</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>16500</Volume>
		<Amount>136125</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>17700</Volume>
		<Amount>146025</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.27</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>31000</Volume>
		<Amount>256370</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>9500</Volume>
		<Amount>78470</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>10300</Volume>
		<Amount>84975</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>5000</Volume>
		<Amount>41300</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:48:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>350400</Volume>
		<Amount>2901312</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.26</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>3700</Volume>
		<Amount>30562</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>11900</Volume>
		<Amount>98531</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.27</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>5500</Volume>
		<Amount>45485</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:30 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20000</Volume>
		<Amount>165600</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>5800</Volume>
		<Amount>48023</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.27</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>41700</Volume>
		<Amount>344859</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>15300</Volume>
		<Amount>126683</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:47:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>83000</Volume>
		<Amount>688069</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:46:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5700</Volume>
		<Amount>47252</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:46:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>7300</Volume>
		<Amount>60516</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:46:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>7700</Volume>
		<Amount>63832</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:46:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>18500</Volume>
		<Amount>153550</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:46:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>948100</Volume>
		<Amount>7859831</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2100</Volume>
		<Amount>17430</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>137700</Volume>
		<Amount>1143657</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>70600</Volume>
		<Amount>585273</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26500</Volume>
		<Amount>219950</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:10 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>29900</Volume>
		<Amount>248170</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:44:08 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20600</Volume>
		<Amount>170980</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:43:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>176400</Volume>
		<Amount>1464684</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:43:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1600</Volume>
		<Amount>13280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:43:40 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>126700</Volume>
		<Amount>1051875</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:43:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>52200</Volume>
		<Amount>433260</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:43:16 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>80500</Volume>
		<Amount>668174</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:58 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>28600</Volume>
		<Amount>237380</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:52 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>24900</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:46 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14110</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:44 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>51600</Volume>
		<Amount>429085</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:38 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.05</PriceOffset>
		<Volume>24200</Volume>
		<Amount>200860</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.25</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>276600</Volume>
		<Amount>2282115</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:28 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.28</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>87300</Volume>
		<Amount>722844</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:42:22 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>377900</Volume>
		<Amount>3133106</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:41:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>27300</Volume>
		<Amount>226590</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:41:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>26100</Volume>
		<Amount>216630</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:41:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>197200</Volume>
		<Amount>1636760</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>10700</Volume>
		<Amount>89238</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>4800</Volume>
		<Amount>40032</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:31 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6200</Volume>
		<Amount>51770</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>213600</Volume>
		<Amount>1783560</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>5300</Volume>
		<Amount>44255</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3600</Volume>
		<Amount>30060</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>20800</Volume>
		<Amount>173680</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>1700</Volume>
		<Amount>14195</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:40:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>13500</Volume>
		<Amount>112590</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:39:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>9000</Volume>
		<Amount>75060</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:39:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>27700</Volume>
		<Amount>231295</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:39:29 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>11700</Volume>
		<Amount>97578</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:39:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>65200</Volume>
		<Amount>544420</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:38:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.35</Price>
		<PriceOffset>0.02</PriceOffset>
		<Volume>15400</Volume>
		<Amount>128590</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:38:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>500</Volume>
		<Amount>4165</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:38:43 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.32</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>55000</Volume>
		<Amount>457600</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:38:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>5500</Volume>
		<Amount>45705</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:38:01 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4165</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:37:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.33</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>526800</Volume>
		<Amount>4388835</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:36:37 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>276400</Volume>
		<Amount>2294178</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:36:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1400</Volume>
		<Amount>11620</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:36:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>35800</Volume>
		<Amount>297140</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:35:57 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>15800</Volume>
		<Amount>131140</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:35:51 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>3200</Volume>
		<Amount>26560</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:35:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>154400</Volume>
		<Amount>1288421</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:34:45 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.34</Price>
		<PriceOffset>-0.08</PriceOffset>
		<Volume>127600</Volume>
		<Amount>1064184</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:33 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>73600</Volume>
		<Amount>619712</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:27 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>15200</Volume>
		<Amount>127984</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:21 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1000</Volume>
		<Amount>8430</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:15 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>-0.02</PriceOffset>
		<Volume>2000</Volume>
		<Amount>16860</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:13 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>41500</Volume>
		<Amount>350674</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:07 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2800</Volume>
		<Amount>23744</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:33:03 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>4900</Volume>
		<Amount>41552</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.45</Price>
		<PriceOffset>-0.03</PriceOffset>
		<Volume>30600</Volume>
		<Amount>258569</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>500</Volume>
		<Amount>4240</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.48</Price>
		<PriceOffset>0.06</PriceOffset>
		<Volume>31300</Volume>
		<Amount>265424</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.42</Price>
		<PriceOffset>-0.04</PriceOffset>
		<Volume>91800</Volume>
		<Amount>772956</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.46</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>4500</Volume>
		<Amount>38070</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.43</Price>
		<PriceOffset>0.03</PriceOffset>
		<Volume>67400</Volume>
		<Amount>568182</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:12 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>18400</Volume>
		<Amount>154560</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:06 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1300</Volume>
		<Amount>11340</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:32:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6500</Volume>
		<Amount>54600</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6800</Volume>
		<Amount>57120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>1800</Volume>
		<Amount>15120</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:48 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>11700</Volume>
		<Amount>98280</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>900</Volume>
		<Amount>7560</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.4</Price>
		<PriceOffset>0.04</PriceOffset>
		<Volume>49100</Volume>
		<Amount>413028</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.36</Price>
		<PriceOffset>0.05</PriceOffset>
		<Volume>57100</Volume>
		<Amount>477355</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:31:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.31</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>2600</Volume>
		<Amount>21606</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:56 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>3000</Volume>
		<Amount>24900</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:50 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>464300</Volume>
		<Amount>3853739</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:42 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>30800</Volume>
		<Amount>255640</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:36 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>6200</Volume>
		<Amount>51460</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:32 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>31900</Volume>
		<Amount>264770</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:26 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>0.01</PriceOffset>
		<Volume>55800</Volume>
		<Amount>463140</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:20 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>0</PriceOffset>
		<Volume>2400</Volume>
		<Amount>19895</Amount>
		<Direction>ÖÐÐÔÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:30:18 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.29</Price>
		<PriceOffset>-0.01</PriceOffset>
		<Volume>1006800</Volume>
		<Amount>8346720</Amount>
		<Direction>ÂôÅÌ</Direction>
	</Item>
	<Item>
		<DateTime>1/4/2007 09:25:02 +08:00</DateTime>
		<Code>sh600104</Code>
		<Price>8.3</Price>
		<PriceOffset>8.3</PriceOffset>
		<Volume>169000</Volume>
		<Amount>1402700</Amount>
		<Direction>ÂòÅÌ</Direction>
	</Item>
</DetailItems>'