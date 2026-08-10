CREATE PROCEDURE Add_New_Visit
    @VisitTitle VARCHAR(100), 
    @VisitDescription VARCHAR(500), 
    @DepartmentID INT,
    @HostEmployeeID INT, 
    @VisitTypeID INT, 
    @VisitDate DATE, 
    @StartTime TIME, 
    @EndTime TIME
AS
BEGIN
    INSERT INTO Visit (VisitTitle, VisitDescription, DepartmentID, HostEmployeeID,
                        VisitTypeID, VisitStatus, VisitDate, StartTime, EndTime)
    VALUES (@VisitTitle, @VisitDescription, @DepartmentID, @HostEmployeeID,
            @VisitTypeID, 'Pending', @VisitDate, @StartTime, @EndTime);
END;
