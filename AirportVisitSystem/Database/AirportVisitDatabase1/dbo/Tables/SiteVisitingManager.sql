CREATE TABLE [dbo].[SiteVisitingManager] (
    [ManagerID]      INT           NOT NULL,
    [Name]           VARCHAR (100) NOT NULL,
    [Phone]          VARCHAR (15)  NOT NULL,
    [Email]          VARCHAR (100) NOT NULL,
    [ManagerLoginID] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([ManagerID] ASC),
    CONSTRAINT [FK_LoginID] FOREIGN KEY ([ManagerLoginID]) REFERENCES [dbo].[Logins] ([LoginID])
);

