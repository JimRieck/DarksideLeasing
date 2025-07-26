CREATE TABLE [dbo].[Tenants] (
    [TenantId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TenantName] NVARCHAR (200)   NOT NULL,
    [IsActive]   BIT              DEFAULT ((1)) NOT NULL,
    [CreatedBy]  NVARCHAR (255)   NOT NULL,
    [CreatedOn]  DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy] NVARCHAR (255)   NULL,
    [ModifiedOn] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([TenantId] ASC)
);