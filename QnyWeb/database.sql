USE [master]
GO
/****** Object:  Database [ShQny]    Script Date: 12/27/2018 17:33:58 ******/
CREATE DATABASE [ShQny]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ShQny', FILENAME = N'D:\Database\Data\ShQny.mdf' , SIZE = 23552KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ShQny_log', FILENAME = N'D:\Database\Data\ShQny_log.ldf' , SIZE = 16576KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ShQny] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ShQny].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ShQny] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ShQny] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ShQny] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ShQny] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ShQny] SET ARITHABORT OFF 
GO
ALTER DATABASE [ShQny] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ShQny] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ShQny] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ShQny] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ShQny] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ShQny] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ShQny] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ShQny] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ShQny] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ShQny] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ShQny] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ShQny] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ShQny] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ShQny] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ShQny] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ShQny] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ShQny] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ShQny] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ShQny] SET  MULTI_USER 
GO
ALTER DATABASE [ShQny] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ShQny] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ShQny] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ShQny] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ShQny] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ShQny', N'ON'
GO
USE [ShQny]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserPois]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserPois](
	[UserId] [nvarchar](128) NOT NULL,
	[poiId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserPois] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[poiId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [smalldatetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PmsPo]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PmsPo](
	[id] [bigint] NOT NULL,
	[poNo] [nvarchar](255) NOT NULL,
	[supplierId] [int] NOT NULL,
	[poiId] [int] NOT NULL,
	[preArrivalTime] [bigint] NULL,
	[arrivalTime] [bigint] NULL,
	[status] [int] NULL,
	[categoryName] [nvarchar](255) NULL,
	[totalSku] [int] NULL,
	[totalPrepoAmount] [int] NULL,
	[expairTime] [bigint] NULL,
	[skuPriceType] [int] NULL,
	[creator] [nvarchar](255) NULL,
	[ctime] [bigint] NULL,
	[utime] [bigint] NULL,
	[operator] [nvarchar](255) NULL,
	[preArrivalTime2] [datetime] NULL,
	[arrivalTime2] [datetime] NULL,
	[expairTime2] [datetime] NULL,
	[ctime2] [datetime] NULL,
	[utime2] [datetime] NULL,
 CONSTRAINT [PK_PmsPo_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PmsPoDetail]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PmsPoDetail](
	[supplierId] [int] NULL,
	[supplierName] [nvarchar](255) NULL,
	[poNo] [nvarchar](255) NOT NULL,
	[cTime] [bigint] NULL,
	[poiId] [int] NULL,
	[preArrivalTime] [bigint] NULL,
	[arrivalTime] [bigint] NULL,
	[creator] [nvarchar](255) NULL,
	[poiType] [int] NULL,
	[supplierCode] [nvarchar](255) NULL,
	[supplierPrimaryContactPhone] [nvarchar](255) NULL,
	[supplierPrimaryContactName] [nvarchar](255) NULL,
	[poiName] [nvarchar](255) NULL,
	[poiAddress] [nvarchar](255) NULL,
	[poiServicePhone] [nvarchar](255) NULL,
	[status] [int] NOT NULL,
	[poiContactName] [nvarchar](255) NULL,
	[skuPriceType] [int] NULL,
	[remark] [nvarchar](255) NULL,
	[supplyType] [int] NULL,
	[areaId] [int] NULL,
	[areaName] [nvarchar](255) NULL,
	[cTime2] [datetime] NULL,
	[preArrivalTime2] [datetime] NULL,
	[arrivalTime2] [datetime] NULL,
 CONSTRAINT [PK_PmsPoDetail] PRIMARY KEY CLUSTERED 
(
	[poNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PoiConfig]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoiConfig](
	[poiid] [int] NOT NULL,
	[phonenumber] [nvarchar](255) NULL,
 CONSTRAINT [PK_PoiConfig] PRIMARY KEY CLUSTERED 
(
	[poiid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PoiList]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoiList](
	[poiid] [int] NOT NULL,
	[poiname] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PoSkuRn]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoSkuRn](
	[poNo] [nvarchar](255) NOT NULL,
	[spuName] [nvarchar](255) NOT NULL,
	[poiName] [nvarchar](255) NULL,
	[cTime2] [datetime] NULL,
	[preArrivalTime2] [datetime] NULL,
	[skuId] [bigint] NULL,
	[skuSpec] [nvarchar](255) NULL,
	[skuDictUnitName] [nvarchar](255) NULL,
	[availableQuantity] [decimal](18, 2) NULL,
	[poAmount] [decimal](18, 1) NULL,
	[sumPoPrice] [decimal](18, 1) NULL,
	[buyAmount] [nvarchar](255) NULL,
	[buyPrice] [decimal](18, 2) NULL,
	[unitPrice] [decimal](18, 2) NULL,
	[remark] [nvarchar](255) NULL,
	[fileName] [nvarchar](255) NULL,
	[tableName] [nvarchar](255) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_PoSkuRn] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReceivingNote]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReceivingNote](
	[RnId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ReceivingNote_RnId]  DEFAULT (newsequentialid()),
	[skuId] [bigint] NOT NULL,
	[cDate] [date] NOT NULL,
	[Quantity] [decimal](18, 2) NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[TotalPrice] [decimal](18, 2) NULL,
	[ContrastPrice] [decimal](18, 2) NULL,
	[ContrastGrossProfit] [decimal](18, 2) NULL,
	[Supplier] [nvarchar](255) NULL,
	[PaymentStatus] [bit] NOT NULL CONSTRAINT [DF_ReceivingNote_PaymentStatus]  DEFAULT ((0)),
	[ApplyQuantity] [decimal](18, 2) NULL,
	[CreateTime] [datetime] NULL,
	[Creator] [nvarchar](255) NULL,
	[UpdateTime] [datetime] NULL,
	[operator] [nvarchar](255) NULL,
 CONSTRAINT [PK_ReceivingNote] PRIMARY KEY CLUSTERED 
(
	[RnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RnPoItem]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RnPoItem](
	[RnId] [uniqueidentifier] NOT NULL,
	[poNo] [nvarchar](255) NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[operator] [nvarchar](255) NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_RnPoItem] PRIMARY KEY CLUSTERED 
(
	[RnId] ASC,
	[poNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sku]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sku](
	[skuId] [bigint] NOT NULL,
	[poNo] [nvarchar](255) NOT NULL,
	[categoryName] [nvarchar](255) NULL,
	[spuName] [nvarchar](255) NULL,
	[skuMallCode] [nvarchar](255) NULL,
	[skuCode] [nvarchar](255) NULL,
	[storageTemperatureLevel] [int] NULL,
	[guaranteePeriod] [int] NULL,
	[skuSpec] [nvarchar](255) NULL,
	[skuCostPrice] [int] NULL,
	[skuDictUnitName] [nvarchar](255) NULL,
	[skuBoxQuantity] [decimal](18, 2) NULL,
	[prePoAmount] [decimal](18, 1) NULL,
	[poAmount] [decimal](18, 1) NULL,
	[productionDate] [bigint] NULL,
	[sumPrePoPrice] [decimal](18, 1) NULL,
	[sumPoPrice] [decimal](18, 1) NULL,
	[unitId] [int] NULL,
	[packageType] [int] NULL,
	[categoryId] [int] NULL,
	[tax] [decimal](18, 2) NULL,
	[guaranteePeriodType] [int] NULL,
	[availableQuantity] [decimal](18, 2) NULL,
	[availablePoPrice] [decimal](18, 2) NULL,
	[productionDate2] [datetime] NULL,
	[cTime2] [datetime] NOT NULL,
 CONSTRAINT [PK_Sku] PRIMARY KEY CLUSTERED 
(
	[skuId] ASC,
	[poNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Status]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[FinalStatus] [int] NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[PmsPoView]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[PmsPoView]
AS
SELECT        dbo.PmsPo.poNo, dbo.PmsPo.id, dbo.PmsPo.supplierId, dbo.PmsPo.categoryName, dbo.PmsPo.totalSku, dbo.PmsPo.totalPrepoAmount, dbo.PmsPo.creator, 
                         dbo.PmsPo.skuPriceType, dbo.PmsPo.preArrivalTime2, dbo.PmsPo.arrivalTime2, dbo.PmsPo.expairTime2, dbo.PmsPo.ctime2, dbo.PmsPo.utime2, 
                         dbo.PmsPo.operator, dbo.PmsPo.poiId, dbo.PoiList.poiname, dbo.PmsPo.status, dbo.Status.Name AS StatusName
FROM            dbo.PmsPo INNER JOIN
                         dbo.PoiList ON dbo.PmsPo.poiId = dbo.PoiList.poiid INNER JOIN
                         dbo.Status ON dbo.PmsPo.status = dbo.Status.Id

GO
/****** Object:  View [dbo].[ReceivingNoteItemView]    Script Date: 12/27/2018 17:33:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ReceivingNoteItemView]
AS
select t.*,r.Quantity,r.UnitPrice,r.TotalPrice,r.ContrastPrice,r.ContrastGrossProfit,r.Supplier,cast(coalesce(r.PaymentStatus,0) as bit) PaymentStatus,r.RnId 
from (select skuid,max(spuName) spuName,max(skuSpec) skuSpec, max(skuDictUnitName) skuDictUnitName, sum(prePoAmount) prePoAmount,cast(ctime2 as date) cDate 
from sku group by skuId,cast(ctime2 as date)) as t 
left join ReceivingNote r on t.skuId=r.skuId and t.cDate=r.cDate



GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_PmsPo_poNo]    Script Date: 12/27/2018 17:33:58 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PmsPo_poNo] ON [dbo].[PmsPo]
(
	[poNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReceivingNote_cDate_skuId]    Script Date: 12/27/2018 17:33:58 ******/
CREATE NONCLUSTERED INDEX [IX_ReceivingNote_cDate_skuId] ON [dbo].[ReceivingNote]
(
	[skuId] ASC,
	[cDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sku_skuId_cTime]    Script Date: 12/27/2018 17:33:58 ******/
CREATE NONCLUSTERED INDEX [IX_Sku_skuId_cTime] ON [dbo].[Sku]
(
	[skuId] ASC,
	[cTime2] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PmsPo"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 201
               Right = 226
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "PoiList"
            Begin Extent = 
               Top = 6
               Left = 264
               Bottom = 101
               Right = 434
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Status"
            Begin Extent = 
               Top = 105
               Left = 263
               Bottom = 217
               Right = 433
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PmsPoView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PmsPoView'
GO
USE [master]
GO
ALTER DATABASE [ShQny] SET  READ_WRITE 
GO
