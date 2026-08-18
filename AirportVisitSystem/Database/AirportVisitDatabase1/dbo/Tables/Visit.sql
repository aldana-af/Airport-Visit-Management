CREATE TABLE [dbo].[Visit] (
    [VisitID]          INT           IDENTITY (1, 1) NOT NULL,
    [VisitTitle]       VARCHAR (100) NOT NULL,
    [VisitDescription] VARCHAR (500) NULL,
    [DepartmentID]     INT           NOT NULL,
    [HostEmployeeID]   INT           NOT NULL,
    [VisitStatus]      VARCHAR (50)  NOT NULL,
    [Status]            VARCHAR (20)  NOT NULL,
    [CreatedDate]      DATETIME      DEFAULT (getdate()) NULL,
    [VisitDate]        DATE          NOT NULL,
    [StartTime]        TIME (7)      NOT NULL,
    [EndTime]          TIME (7)      NOT NULL,
    [VisitTypeID]      INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([VisitID] ASC),
    CONSTRAINT [CHK_Visit_Status] CHECK ([Status]='Active' OR [Status]='Cancelled'),
    CHECK ([VisitStatus]='Complete' OR [VisitStatus]='Rejected' OR [VisitStatus]='Approved' OR [VisitStatus]='Pending'),
    CONSTRAINT [FK_HostEmployeeID] FOREIGN KEY ([HostEmployeeID]) REFERENCES [dbo].[EmployeeHost] ([EmployeeID]),
    CONSTRAINT [FK_Visit_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]),
    CONSTRAINT [FK_VisitTypeID] FOREIGN KEY ([VisitTypeID]) REFERENCES [dbo].[VisitType] ([VisitTypeID])
);

