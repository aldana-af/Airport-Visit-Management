CREATE PROCEDURE Add_New_EmployeeHost
	@EmployeeID INT,
	@Name VARCHAR(100),
	@InputDepartmentID INT,
	@Phone VARCHAR(15),
	@Email VARCHAR(100),
	@Role VARCHAR(50),
	@InputUsername VARCHAR(50)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM Logins WHERE Username = @InputUsername)
	BEGIN
		RAISERROR('Username does not exist in Logins table.', 16, 1);
		RETURN;
	END
	INSERT INTO EmployeeHost(EmployeeID, Name, DepartmentID, Phone, Email, Role, LoginID)
	VALUES(@EmployeeID, @Name, (SELECT DepartmentID FROM Department WHERE DepartmentID = @InputDepartmentID), 
	@Phone, @Email, @Role, (SELECT LoginID FROM Logins WHERE Username = @InputUsername));
END;
