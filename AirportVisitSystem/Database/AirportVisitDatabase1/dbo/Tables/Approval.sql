CREATE TABLE [dbo].[Approval] (
    [ApprovalID]          INT          IDENTITY (1, 1) NOT NULL,
    [VisitVisitorID]      INT          NOT NULL,
    [ApprovalStatus]      VARCHAR (50) NOT NULL,
    [RequestedEmployeeID] INT          NOT NULL,
    [ApprovingManagerID]  INT          NOT NULL,
    [RequestDate]         DATETIME     DEFAULT (getdate()) NOT NULL,
    [DecisionDate]        DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([ApprovalID] ASC),
    CHECK ([ApprovalStatus]='Rejected' OR [ApprovalStatus]='Approved' OR [ApprovalStatus]='Pending'),
    CONSTRAINT [FK_Approval_ApprovingManager] FOREIGN KEY ([ApprovingManagerID]) REFERENCES [dbo].[SiteVisitingManager] ([ManagerID]),
    CONSTRAINT [FK_Approval_RequestedEmployee] FOREIGN KEY ([RequestedEmployeeID]) REFERENCES [dbo].[EmployeeHost] ([EmployeeID]),
    CONSTRAINT [FK_Approval_VisitVisitor] FOREIGN KEY ([VisitVisitorID]) REFERENCES [dbo].[VisitVisitor] ([VisitVisitorID])
);

