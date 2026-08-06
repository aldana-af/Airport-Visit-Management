CREATE PROCEDURE Add_New_SiteVisitingManager
	@ManagerID INT,
	@Name VARCHAR(100),
	@Phone VARCHAR(15),
	@Email VARCHAR(100),
	@InputUsername NVARCHAR(50)
AS
BEGIN
	-- Delcare variables for error handling
	DECLARE @FoundManagerID INT;
	DECLARE @FoundLoginID INT;

	-- Fecth the ManagerID and LoginID from the respective tables
	SELECT @FoundManagerID = ManagerID FROM SiteVisitingManager WHERE ManagerID = @ManagerID;
	SELECT @FoundLoginID = LoginID FROM Logins WHERE Username = @InputUsername;

	-- Check if the manager already exists
	IF @FoundManagerID IS NOT NULL
		BEGIN
			RAISERROR('Manager with ID %d already exists.', 16, 1, @ManagerID);
			RETURN;
		END

	-- Check if the input username exists in the Logins table
	IF @FoundLoginID IS NULL
		BEGIN
			RAISERROR('Input username %s does not exist in Logins table.', 16, 1, @InputUsername);
			RETURN;
		END

	-- Insert new manager into the SiteVisitingManager table
	INSERT INTO SiteVisitingManager (ManagerID, Name, Phone, Email, ManagerLoginID)
	VALUES (@ManagerID, @Name, @Phone, @Email, @FoundLoginID);
END;
