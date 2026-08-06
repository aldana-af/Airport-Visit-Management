CREATE TABLE [dbo].[Logins] (
    [LoginID]      INT           NOT NULL,
    [Username]     VARCHAR (50)  NOT NULL,
    [PasswordHash] VARCHAR (255) NOT NULL,
    [Role]         VARCHAR (20)  NOT NULL,
    PRIMARY KEY CLUSTERED ([LoginID] ASC),
    CHECK ([Role]='Employee' OR [Role]='Manager'),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

