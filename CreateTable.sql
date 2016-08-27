USE [mvcapp]
GO

/****** Object:  Table [dbo].[tbl_interview]    Script Date: 08/27/2016 20:31:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_interview](
	[Gender] [nvarchar](6) NULL,
	[Title] [nvarchar](50) NULL,
	[Occupation] [nvarchar](255) NULL,
	[Company] [nvarchar](255) NULL,
	[GivenName] [nvarchar](50) NULL,
	[MiddleInitial] [nvarchar](10) NULL,
	[Surname] [nvarchar](50) NULL,
	[BloodType] [nvarchar](3) NULL,
	[EmailAddress] [nvarchar](50) NULL
) ON [PRIMARY]

GO

