CREATE TABLE [dbo].[Visitor] (
    [VisitorID]     INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (100) NOT NULL,
    [Organization]  VARCHAR (100) NULL,
    [Position]      VARCHAR (100) NULL,
    [Phone]         VARCHAR (15)  NULL,
    [Email]         VARCHAR (100) NULL,
    [VisitorStatus] VARCHAR (20)  NOT NULL,
    PRIMARY KEY CLUSTERED ([VisitorID] ASC),
    CONSTRAINT [CHK_VisitorStatus] CHECK ([VisitorStatus]='Pending' OR [VisitorStatus]='Denied' OR [VisitorStatus]='Allowed')
);

