ALTER TABLE Visit ADD CONSTRAINT CHK_Visit_WorkHours
CHECK (StartTime >= '8:00:00' AND EndTime <= '17:00:00');

ALTER TABLE Visit ADD CONSTRAINT CHK_Visit_TimeOrder
CHECK (EndTime > StartTime);