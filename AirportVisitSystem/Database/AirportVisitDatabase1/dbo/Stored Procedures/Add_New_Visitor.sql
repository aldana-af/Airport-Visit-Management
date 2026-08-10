CREATE PROCEDURE Add_New_Visitor
    @Name VARCHAR(100), 
    @Organization VARCHAR(100), 
    @Position VARCHAR(50),
    @Phone VARCHAR(15), 
    @Email VARCHAR(100), 
    @VisitorStatus VARCHAR(20) = 'Pending'
AS
BEGIN
    INSERT INTO Visitor (Name, Organization, Position, Phone, Email, VisitorStatus)
    VALUES (@Name, @Organization, @Position, @Phone, @Email, @VisitorStatus);
END;