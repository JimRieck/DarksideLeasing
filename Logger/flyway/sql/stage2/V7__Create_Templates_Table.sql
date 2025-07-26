CREATE TABLE [dbo].[Templates] (
    [TemplateId]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TenantId]          UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (200)   NOT NULL,
    [StorageUrl]        NVARCHAR (MAX)   NOT NULL,
    [FieldMetadataJson] NVARCHAR (MAX)   NULL,
    [CreatedBy]         NVARCHAR (255)   NOT NULL,
    [CreatedOn]         DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]        NVARCHAR (255)   NULL,
    [ModifiedOn]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([TemplateId] ASC),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([TenantId])
);

