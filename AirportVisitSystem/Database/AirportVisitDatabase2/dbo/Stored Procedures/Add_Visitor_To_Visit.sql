CREATE PROCEDURE Add_Visitor_To_Visit
    @VisitID INT,
    @VisitorID INT,
    @RequestedEmployeeID INT,
    @ApprovingManagerID INT = NULL
AS
BEGIN
    DECLARE @NewVisitVisitorID INT;

    INSERT INTO VisitVisitor (VisitorID, VisitID, BadgeID, CheckIn, CheckOut)
    VALUES (@VisitorID, @VisitID, NULL, NULL, NULL);

    SET @NewVisitVisitorID = SCOPE_IDENTITY();

    INSERT INTO Approval (VisitVisitorID, ApprovalStatus, RequestedEmployeeID, ApprovingManagerID, RequestDate)
    VALUES (@NewVisitVisitorID, 'Pending', @RequestedEmployeeID, @ApprovingManagerID, GETDATE());
END;
