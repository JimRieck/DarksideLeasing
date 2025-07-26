CREATE TABLE [dbo].[Payments] (
    [PaymentId]     UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [BillingId]     UNIQUEIDENTIFIER NOT NULL,
    [PaymentDate]   DATETIME         NOT NULL,
    [Amount]        DECIMAL (10, 2)  NOT NULL,
    [Provider]      NVARCHAR (50)    NOT NULL,
    [TransactionId] NVARCHAR (100)   NOT NULL,
    [CreatedBy]     NVARCHAR (255)   NOT NULL,
    [CreatedOn]     DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (255)   NULL,
    [ModifiedOn]    DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([PaymentId] ASC),
    FOREIGN KEY ([BillingId]) REFERENCES [dbo].[BillingHistory] ([BillingId])
);