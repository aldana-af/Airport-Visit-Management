CREATE TABLE [dbo].[EmployeeHost] (
    [EmployeeID]   INT           NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    [DepartmentID] INT           NOT NULL,
    [Phone]        VARCHAR (15)  NULL,
    [Email]        VARCHAR (100) NULL,
    [Role]         VARCHAR (50)  NULL,
    [LoginID]      INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC),
    CONSTRAINT [FK_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]),
    CONSTRAINT [FK_Employee_LoginID] FOREIGN KEY ([LoginID]) REFERENCES [dbo].[Logins] ([LoginID])
);

