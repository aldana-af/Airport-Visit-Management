CREATE TABLE [dbo].[VisitVisitor] (
    [VisitVisitorID] INT      IDENTITY (1, 1) NOT NULL,
    [VisitorID]      INT      NOT NULL,
    [VisitID]        INT      NOT NULL,
    [BadgeID]        INT      NOT NULL,
    [CheckIn]        DATETIME NOT NULL,
    [CheckOut]       DATETIME NULL,
    PRIMARY KEY CLUSTERED ([VisitVisitorID] ASC),
    CONSTRAINT [FK_VisitVisitor_BadgeID] FOREIGN KEY ([BadgeID]) REFERENCES [dbo].[Badge] ([BadgeID]),
    CONSTRAINT [FK_VisitVisitor_VisitID] FOREIGN KEY ([VisitID]) REFERENCES [dbo].[Visit] ([VisitID]),
    CONSTRAINT [FK_VisitVisitor_VisitorID] FOREIGN KEY ([VisitorID]) REFERENCES [dbo].[Visitor] ([VisitorID])
);

