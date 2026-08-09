CREATE TABLE [dbo].[VisitType] (
    [VisitTypeID] INT          IDENTITY (1, 1) NOT NULL,
    [Type]        VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([VisitTypeID] ASC)
);

