USE [Stock001]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock002]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock003]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock004]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock005]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock006]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock007]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock008]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock009]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO

USE [Stock010]
GO
ALTER TABLE dbo.Detail ADD Factor decimal(18, 3) NULL
GO
UPDATE d SET [Factor] = roi.Factor FROM [dbo].[Detail] d JOIN [dbo].[RightsOfferingInfo]  roi ON TODATETIMEOFFSET(cast(d.datetime as date),datename(tz,d.datetime))=roi.datetime and d.code=roi.code
GO
