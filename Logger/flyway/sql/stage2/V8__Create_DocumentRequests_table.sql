CREATE TABLE [dbo].[DocumentRequests] (
    [RequestId]         UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TemplateId]        UNIQUEIDENTIFIER NOT NULL,
    [RequestedByUserId] UNIQUEIDENTIFIER NOT NULL,
    [RequestedOn]       DATETIME         DEFAULT (getdate()) NOT NULL,
    [Status]            NVARCHAR (50)    NOT NULL,
    [GeneratedUrl]      NVARCHAR (MAX)   NULL,
    [CreatedBy]         NVARCHAR (255)   NOT NULL,
    [CreatedOn]         DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]        NVARCHAR (255)   NULL,
    [ModifiedOn]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([RequestId] ASC),
    FOREIGN KEY ([RequestedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([TemplateId])
);