USE [master]
GO
ALTER DATABASE [Stock001] ADD FILEGROUP [MinuteDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Minute_001', FILENAME = N'D:\DataBase\Data\Stock001\Minute_001.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [MinuteDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Minute_002', FILENAME = N'D:\DataBase\Data\Stock001\Minute_002.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [MinuteDataGroup]
GO
ALTER DATABASE [Stock001] ADD FILE ( NAME = N'Minute_003', FILENAME = N'D:\DataBase\Data\Stock001\Minute_003.ndf' , SIZE = 5120KB , FILEGROWTH = 10240KB ) TO FILEGROUP [MinuteDataGroup]
GO

------------------------------------------------------------------------------------------------------------------------------------------------------------------------
USE [Stock001]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Minute](
	[Id] [uniqueidentifier] NOT NULL,
	[DateTime] [datetimeoffset](7) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[OpenPrice] [decimal](18, 3) NOT NULL,
	[HighestPrice] [decimal](18, 3) NOT NULL,
	[ClosePrice] [decimal](18, 3) NOT NULL,
	[LowestPrice] [decimal](18, 3) NOT NULL,
	[Volume] [int] NOT NULL,
	[Amount] [decimal](18, 3) NOT NULL,
	[FlowingAmount] [decimal](18, 3) NOT NULL,
	[FlowingAmount100K] [decimal](18, 3) NOT NULL,
	[FlowingAmount500K] [decimal](18, 3) NOT NULL,
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
	[LastUpdateTime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Minute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (DATA_COMPRESSION = PAGE,PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [MinuteDataGroup]
) ON [MinuteDataGroup]

GO

ALTER TABLE [dbo].[Minute] ADD  CONSTRAINT [DF_Minute_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[Minute] ADD  CONSTRAINT [DF_Minute_LastUpdateTime]  DEFAULT (sysdatetimeoffset()) FOR [LastUpdateTime]
GO
CREATE NONCLUSTERED INDEX [IX_Minute_CodeDatetime] ON [dbo].[Minute]
(
	[Code] ASC,
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [MinuteDataGroup]
GO

USE stock001
GO

IF EXISTS(SELECT *
          FROM   [INFORMATION_SCHEMA].[ROUTINES]
          WHERE  [SPECIFIC_SCHEMA] = N'dbo' AND [SPECIFIC_NAME] = N'usp_GetMinutelyDataExport')
    DROP PROCEDURE [dbo].[usp_GetMinutelyDataExport]
GO

CREATE PROCEDURE [usp_GetMinutelyDataExport] @code NVARCHAR(50),@beginDate DATETIMEOFFSET, @endDate DATETIMEOFFSET, @itervalCount INT = 1
AS
BEGIN
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
    from detail with(nolock)
    where code=@code and datetime >= @beginDate and datetime < @endDate
    group by code,dateadd(ss,-datepart(ss,datetime),datetime))
    select 
    d.code,
    d.datetime,
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
    from d with(nolock)
    join [RightsOfferingInfo] roi with(nolock) on d.code=roi.code and TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime
    join detail l with(nolock) on [ClosePriceId]=l.id
    order by datetime
END
GO

[usp_GetMinutelyDataExport] 'sh600020','2012-03-01','2012-06-01'
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------
select distinct code,dt into missingDate from
(select d.code code, TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime)) dt,roi.datetime roidt FROM [dbo].[Detail] d with(nolock) LEFT JOIN [dbo].[RightsOfferingInfo]  roi 
ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code) as t where roidt is null

--001
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600023', 4.510 * 1.344, 4.540 * 1.344, 4.540 * 1.344, 4.490 * 1.344, 10853137, 49058376, 1.344),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600020', 2.230 * 3.486, 2.240 * 3.486, 2.230 * 3.486, 2.210 * 3.486, 4657045,  10370315, 3.486),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600030', 11.500 * 4.894, 11.550 * 4.894, 11.500 * 4.894, 11.400 * 4.894, 45519992, 521870816, 4.894),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600031', 5.040 * 44.327, 5.040 * 44.327, 5.020 * 44.327, 4.990 * 44.327, 12328240, 61775076, 44.327),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600039', 5.560 * 3.638, 5.640 * 3.638, 5.630 * 3.638, 5.520 * 3.638, 4603328, 25701808, 3.638),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600057', 6.080 * 5.667, 6.110 * 5.667, 6.090 * 5.667, 6.030 * 5.667, 2273347, 13793867, 5.667),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600078', 5.730 * 6.539, 5.760 * 6.539, 5.720 * 6.539, 5.680 * 6.539, 7551609, 43139552, 6.539)

--002
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600009', 12.990 * 2.815, 13.080 * 2.815, 13.070 * 2.815, 12.960 * 2.815, 7517660, 98040816, 2.815),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600027', 3.120 * 1.518, 3.350 * 1.518, 3.330 * 1.518, 3.110 * 1.518, 103721016, 338235648, 1.518),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600059', 7.790 * 7.865, 7.840 * 7.865, 7.820 * 7.865, 7.760 * 7.865, 5578273, 43602576, 7.865),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600080', 10.690 * 6.947, 10.720 * 6.947, 10.650 * 6.947, 10.310 * 6.947, 8901866, 93880600, 6.947),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600085', 17.510 * 10.589, 17.570 * 10.589, 17.500 * 10.589, 17.340 * 10.589, 4021526, 70097560, 10.589),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600089', 8.560 * 32.883, 8.570 * 32.883, 8.510 * 32.883, 8.450 * 32.883, 25662492, 218037728, 32.883)

--003
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600012',4.450 * 1.883, 4.490 * 1.883, 4.480 * 1.883, 4.450 * 1.883, 2391679, 10702357   ,1.883),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600026',4.160 * 14.596, 4.190 * 14.596, 4.180 * 14.596, 4.140 * 14.596, 3966753, 16505832   ,1.596),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600037',10.730 * 4.896, 10.740 * 4.896, 10.640 * 4.896, 10.400 * 4.896, 12658347, 133727256  ,4.896),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600060',9.520 * 5.307, 9.600 * 5.307, 9.580 * 5.307, 9.500 * 5.307, 8333624, 79679632  ,5.307),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600063',2.740 * 13.16, 2.810 * 13.16, 2.710 * 13.16, 2.700 * 13.16, 59505592, 162661040  ,13.16),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600073',7.640 * 3.022, 7.870 * 3.022, 7.680 * 3.022, 7.640 * 3.022, 20434504, 158269216  ,3.022),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600075',5.020 * 7.280, 5.060 * 7.280, 5.010 * 7.280, 4.970 * 7.280, 2896925, 14504763   ,7.280),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600079',30.710 * 6.513, 31.400 * 6.513, 31.010 * 6.513, 30.700 * 6.513, 3494365, 108422248  ,6.513),
('2015-10-30 00:00:00.0000000 +08:00', 'sz002661',40.540 * 1.061, 42.280 * 1.061, 41.380 * 1.061, 40.010 * 1.061, 1557859, 64297844  ,1.061)

--004
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600007',10.120 * 1.624, 10.150 * 1.624, 9.910 * 1.624, 9.810 * 1.624, 1477800, 14715534 ,1.624 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600011',5.740 * 3.709, 5.920 * 3.709, 5.900 * 3.709, 5.730 * 3.709, 20554802, 120289576 ,3.709 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600015',8.200 * 2.554, 8.210 * 2.554, 8.210 * 2.554, 8.150 * 2.554, 15827840, 129429288  ,2.554 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600022',1.560 * 4.280, 1.570 * 4.280, 1.570 * 4.280, 1.550 * 4.280, 14568389, 22767312 ,4.280 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600028',5.030 * 2.351, 5.060 * 2.351, 5.050 * 2.351, 5.010 * 2.351, 57907424, 291240864 ,2.351 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600062',17.070 * 5.036, 17.250 * 5.036, 17.240 * 5.036, 17.050 * 5.036, 1552199, 26662448 ,5.036 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600076',5.200 * 3.175, 5.350 * 3.175, 5.290 * 3.175, 5.160 * 3.175, 7819715, 41158608 ,3.175 ),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600077',3.690 * 8.153, 3.810 * 8.153, 3.760 * 8.153, 3.660 * 8.153, 10641792, 39877600 ,8.153 )

--005
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600008',6.130 * 3.536, 6.170 * 3.536, 6.170 * 3.536, 6.100 * 3.536, 5137168, 31540684 ,3.536),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600016',6.210 * 18.314, 6.230 * 18.314, 6.200 * 18.314, 6.180 * 18.314, 65037412, 403052160 ,18.314),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600038',29.600 * 2.949, 29.600 * 2.949, 28.590 * 2.949, 28.200 * 2.949, 7371144, 210944016 ,2.949),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600052',5.660 * 3.709, 5.930 * 3.709, 5.920 * 3.709, 5.540 * 3.709, 35434568, 203511872,3.709),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600053',8.250 * 2.129, 8.330 * 2.129, 8.250 * 2.129, 8.190 * 2.129, 1217027, 10045372 ,2.129),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600064',10.750 * 6.127, 10.820 * 6.127, 10.770 * 6.127, 10.680 * 6.127, 2008827, 21625048 ,6.127),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600066',16.200 * 22.467, 17.330 * 22.467, 17.190 * 22.467, 16.170 * 22.467, 24309482, 413449504 ,22.467),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600088',14.430 * 2.945, 14.700 * 2.945, 14.670 * 2.945, 14.170 * 2.945, 3664467, 53106712 ,2.945),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600090',6.830 * 4.894, 6.900 * 4.894, 6.800 * 4.894, 6.730 * 4.894, 3351846, 22846428 ,4.894)

--006
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600035',2.300 * 2.175, 2.310 * 2.175, 2.300 * 2.175, 2.290 * 2.175, 2842593, 6531383 ,2.175),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600051',7.800 * 4.277, 7.860 * 4.277, 7.810 * 4.277, 7.650 * 4.277, 3005513, 23305904 ,4.277),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600061',6.280 * 5.719, 6.340 * 5.719, 6.320 * 5.719, 6.260 * 5.719, 5759493, 36308400 ,5.719),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600081',14.900 * 8.757, 15.350 * 8.757, 15.320 * 8.757, 14.820 * 8.757, 4291484, 64969724 ,8.757),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600083',7.230 * 1.000, 7.230 * 1.000, 7.150 * 1.000, 7.120 * 1.000, 1721721, 12324043 ,1.000)

--007
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600004',7.070 * 1.559, 7.120 * 1.559, 7.110 * 1.559, 7.050 * 1.559, 2639046, 18724648 ,1.559),
('2016-06-01 00:00:00.0000000 +08:00', 'sh600004',12.230 * 1.673, 12.240 * 1.673, 12.190 * 1.673, 12.150 * 1.673, 5616280, 68471048 ,1.673),
('2016-08-29 00:00:00.0000000 +08:00', 'sh600004',14.120 * 1.713, 14.150 * 1.713, 13.980 * 1.713, 13.810 * 1.713, 7115507, 99589757 ,1.713),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600006',3.060 * 3.489, 3.060 * 3.489, 3.050 * 3.489, 3.020 * 3.489, 4218572, 12829823 ,3.489),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600017',2.810 * 3.208, 2.820 * 3.208, 2.810 * 3.208, 2.750 * 3.208, 39100472, 108967832 ,3.208),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600021',4.410 * 1.854, 4.450 * 1.854, 4.450 * 1.854, 4.360 * 1.854, 10456422, 46189204 ,1.854),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600036',10.320 * 3.892, 10.390 * 3.892, 10.370 * 3.892, 10.300 * 3.892, 36679128, 379699456,3.892),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600056',10.690 * 8.554, 10.790 * 8.554, 10.770 * 8.554, 10.580 * 8.554, 5533607, 59075948 ,8.554),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600091',5.190 * 2.575, 5.270 * 2.575, 5.210 * 2.575, 5.150 * 2.575, 1233808, 6423588 ,2.575)

--008
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600010',3.560 * 2.976, 3.560 * 2.976, 3.530 * 2.976, 3.500 * 2.976, 21580422, 76135376 ,2.976),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600050',3.300 * 1.519, 3.330 * 1.519, 3.310 * 1.519, 3.270 * 1.519, 37983288, 125256400 ,1.519),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600054',12.400 * 2.761, 12.490 * 2.761, 12.460 * 2.761, 12.390 * 2.761, 1342261, 16688433 ,2.761),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600058',10.350 * 4.769, 10.500 * 4.769, 10.490 * 4.769, 10.350 * 4.769, 3232491, 33804616 ,4.769),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600070',6.880 * 7.357, 7.720 * 7.357, 7.720 * 7.357, 6.860 * 7.357, 17589854, 129715224 ,7.357),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600072',8.450 * 4.184, 8.750 * 4.184, 8.720 * 4.184, 8.410 * 4.184, 5640969, 48845904 ,4.184),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600074',5.650 * 9.608, 5.940 * 9.608, 5.870 * 9.608, 5.540 * 9.608, 34742176, 201437104 ,9.608),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600086',18.090 * 3.206, 18.380 * 3.206, 18.290 * 3.206, 17.920 * 3.206, 4544393, 82868752 ,3.206)

--009
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600000',9.050 * 7.823, 9.060 * 7.823, 9.050 * 7.823, 9.010 * 7.823, 52592964, 475316192 ,7.823),
('2016-06-01 00:00:00.0000000 +08:00', 'sh600000',18.300 * 8.240, 18.350 * 8.240, 18.190 * 8.240, 18.130 * 8.240, 18312424, 333790682 ,8.240),
('2016-08-29 00:00:00.0000000 +08:00', 'sh600000',16.520 * 9.398, 16.550 * 9.398, 16.500 * 9.398, 16.490 * 9.398, 10997408, 181620111 ,9.398),
('2016-10-21 00:00:00.0000000 +08:00', 'sh600000',16.290 * 9.398, 16.340 * 9.398, 16.300 * 9.398, 16.220 * 9.398, 10884798, 177155062 ,9.398),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600029',2.330 * 1.596, 2.350 * 1.596, 2.340 * 1.596, 2.310 * 1.596, 16550437, 38597172 ,1.596),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600048',5.180 * 17.516, 5.200 * 17.516, 5.150 * 17.516, 5.120 * 17.516, 63543796, 327506464 ,17.516),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600067',5.590 * 20.117, 6.040 * 20.117, 5.810 * 20.117, 5.590 * 20.117, 59942304, 352324160 ,20.117),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600068',3.790 * 5.428, 3.800 * 5.428, 3.800 * 5.428, 3.780 * 5.428, 8448160, 32057420 ,5.428),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600069',3.630 * 4.944, 3.660 * 4.944, 3.650 * 4.944, 3.600 * 4.944, 5603358, 20370492 ,4.944)

--010
INSERT INTO [dbo].[RightsOfferingInfo] ([DateTime], [Code], [OpenPrice], [HighestPrice], [ClosePrice], [LowestPrice], [Volume], [Amount], [Factor]) 
VALUES 
('2014-07-08 00:00:00.0000000 +08:00', 'sh600005',2.030 * 5.292, 2.040 * 5.292, 2.030 * 5.292, 2.020 * 5.292, 6307005, 12776615 ,5.292),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600019',4.080 * 2.031, 4.100 * 2.031, 4.070 * 2.031, 4.050 * 2.031, 8791483, 35766376 ,2.031),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600033',2.150 * 5.996, 2.170 * 5.996, 2.170 * 5.996, 2.140 * 5.996, 4948663, 10662813 ,5.996),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600071',13.080 * 5.762, 13.300 * 5.762, 13.240 * 5.762, 12.420 * 5.762, 26339808, 338357760 ,5.762),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600082',4.540 * 5.981, 4.550 * 5.981, 4.540 * 5.981, 4.510 * 5.981, 4028901, 18261888 ,5.981),
('2014-07-08 00:00:00.0000000 +08:00', 'sh600084',3.950 * 7.718, 3.970 * 7.718, 3.970 * 7.718, 3.900 * 7.718, 2369584, 9339641 ,7.178)
