ALTER TABLE Visit ADD Status VARCHAR(20) NOT NULL DEFAULT 'Active';
GO
ALTER TABLE Visit ADD CONSTRAINT CHK_Visit_Status CHECK (Status='Active' OR Status='Cancelled');
GO