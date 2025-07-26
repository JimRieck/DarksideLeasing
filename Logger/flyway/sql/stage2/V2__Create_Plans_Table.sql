CREATE TABLE [dbo].[Plans] (
    [PlanId]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]          NVARCHAR (100)   NOT NULL,
    [PricePerMonth] DECIMAL (10, 2)  NOT NULL,
    [MaxDocuments]  INT              NOT NULL,
    [MaxUsers]      INT              NULL,
    [IsActive]      BIT              DEFAULT ((1)) NOT NULL,
    [CreatedBy]     NVARCHAR (255)   NOT NULL,
    [CreatedOn]     DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (255)   NULL,
    [ModifiedOn]    DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([PlanId] ASC)
);