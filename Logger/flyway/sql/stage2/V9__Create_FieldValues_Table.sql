CREATE TABLE [dbo].[FieldValues] (
    [FieldValueId] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [RequestId]    UNIQUEIDENTIFIER NOT NULL,
    [FieldName]    NVARCHAR (200)   NOT NULL,
    [Value]        NVARCHAR (MAX)   NULL,
    [CreatedBy]    NVARCHAR (255)   NOT NULL,
    [CreatedOn]    DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]   NVARCHAR (255)   NULL,
    [ModifiedOn]   DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([FieldValueId] ASC),
    FOREIGN KEY ([RequestId]) REFERENCES [dbo].[DocumentRequests] ([RequestId])
);