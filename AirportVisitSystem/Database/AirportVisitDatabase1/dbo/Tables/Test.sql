CREATE TABLE [dbo].[Test] (
    [id]    INT           IDENTITY (1, 1) NOT NULL,
    [name]  VARCHAR (255) NOT NULL,
    [email] VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

