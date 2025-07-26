CREATE TABLE [dbo].[Users] (
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [TenantId]   UNIQUEIDENTIFIER NOT NULL,
    [Email]      NVARCHAR (255)   NOT NULL,
    [Role]       NVARCHAR (50)    NOT NULL,
    [IsActive]   BIT              DEFAULT ((1)) NOT NULL,
    [CreatedBy]  NVARCHAR (255)   NOT NULL,
    [CreatedOn]  DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy] NVARCHAR (255)   NULL,
    [ModifiedOn] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([TenantId])
);