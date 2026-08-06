CREATE TABLE [dbo].[Badge] (
    [BadgeID]     INT          IDENTITY (1, 1) NOT NULL,
    [BadgeNumber] VARCHAR (50) NOT NULL,
    [Status]      VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([BadgeID] ASC),
    CHECK ([Status]='Inactive' OR [Status]='Active')
);

