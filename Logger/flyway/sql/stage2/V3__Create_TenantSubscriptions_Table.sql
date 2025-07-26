CREATE TABLE [dbo].[TenantSubscriptions] (
    [SubscriptionId] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [PlanId]         UNIQUEIDENTIFIER NOT NULL,
    [StartDate]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [EndDate]        DATETIME         NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    [CreatedBy]      NVARCHAR (255)   NOT NULL,
    [CreatedOn]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]     NVARCHAR (255)   NULL,
    [ModifiedOn]     DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([SubscriptionId] ASC),
    FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plans] ([PlanId]),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([TenantId])
);