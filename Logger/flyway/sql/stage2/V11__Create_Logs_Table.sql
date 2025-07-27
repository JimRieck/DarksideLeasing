-- USE [Darkside_Logging]
-- GO

-- ALTER TABLE [dbo].[Logs] DROP CONSTRAINT [FK_Logs_Tenants]
-- GO

-- ALTER TABLE [dbo].[Logs] DROP CONSTRAINT [DF__Logs__Timestamp__7A672E12]
-- GO

-- ALTER TABLE [dbo].[Logs] DROP CONSTRAINT [DF__Logs__Id__797309D9]
-- GO

/****** Object:  Table [dbo].[Logs]    Script Date: 6/7/2025 7:00:03 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
DROP TABLE [dbo].[Logs]
GO

/****** Object:  Table [dbo].[Logs]    Script Date: 6/7/2025 7:00:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logs](
	[Id] [uniqueidentifier] NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[LogLevel] [nvarchar](20) NOT NULL,
	[Application] [nvarchar](100) NULL,
	[Module] [nvarchar](100) NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[Properties] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Logs] ADD  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Logs] ADD  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO



