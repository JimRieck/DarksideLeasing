CREATE TABLE [dbo].[BillingHistory] (
    [BillingId]     UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TenantId]      UNIQUEIDENTIFIER NOT NULL,
    [PeriodStart]   DATETIME         NOT NULL,
    [PeriodEnd]     DATETIME         NOT NULL,
    [AmountBilled]  DECIMAL (10, 2)  NOT NULL,
    [DocumentCount] INT              NOT NULL,
    [PaymentStatus] NVARCHAR (50)    NOT NULL,
    [CreatedBy]     NVARCHAR (255)   NOT NULL,
    [CreatedOn]     DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (255)   NULL,
    [ModifiedOn]    DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([BillingId] ASC),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([TenantId])
);