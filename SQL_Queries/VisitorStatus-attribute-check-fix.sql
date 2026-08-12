ALTER TABLE VisitVisitor DROP CONSTRAINT CK__VisitVisi__Visit__73852659;

ALTER TABLE VisitVisitor ADD CONSTRAINT CHK_VisitVisitor_VisitorStatus
    CHECK (VisitorStatus IN ('Allowed', 'Denied', 'Pending'));
