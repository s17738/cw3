-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2020-03-22 07:12:20.376

-- tables
-- Table: Enrollment
CREATE TABLE Enrollment (
    IdEnrollment int  NOT NULL,
    Semester int  NOT NULL,
    IdStudy int  NOT NULL,
    StartDate date  NOT NULL,
    CONSTRAINT Enrollment_pk PRIMARY KEY  (IdEnrollment)
);

-- Table: Student
CREATE TABLE Student (
    IndexNumber nvarchar(100)  NOT NULL,
    FirstName nvarchar(100)  NOT NULL,
    LastName nvarchar(100)  NOT NULL,
    BirthDate date  NOT NULL,
    IdEnrollment int  NOT NULL,
    CONSTRAINT Student_pk PRIMARY KEY  (IndexNumber)
);

-- Table: Studies
CREATE TABLE Studies (
    IdStudy int  NOT NULL,
    Name nvarchar(100)  NOT NULL,
    CONSTRAINT Studies_pk PRIMARY KEY  (IdStudy)
);

-- foreign keys
-- Reference: Enrollment_Studies (table: Enrollment)
ALTER TABLE Enrollment ADD CONSTRAINT Enrollment_Studies
    FOREIGN KEY (IdStudy)
    REFERENCES Studies (IdStudy);

-- Reference: Student_Enrollment (table: Student)
ALTER TABLE Student ADD CONSTRAINT Student_Enrollment
    FOREIGN KEY (IdEnrollment)
    REFERENCES Enrollment (IdEnrollment);

-- End of file.


CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT OUTPUT
AS
BEGIN
    SET XACT_ABORT ON;

    DECLARE @IdStudy INT = (SELECT IdStudy FROM Studies WHERE Name = @Studies);
    IF @IdStudy IS NULL
        BEGIN
            RAISERROR (15600,-1,-1, 'PromoteStudents_Studies');
        END

    DECLARE @NewSemesterId INT = (SELECT e.IdEnrollment FROM Enrollment e WHERE e.Semester = @Semester + 1 AND e.IdStudy = @IdStudy);
    IF @NewSemesterId IS NULL
        BEGIN
            DECLARE @NextIdEnrollment INT = (SELECT TOP 1 IdEnrollment + 1 AS IdEnrollment
                                             FROM Enrollment
                                             ORDER BY IdEnrollment DESC);

            INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate)
            VALUES (@NextIdEnrollment, @Semester + 1, @IdStudy, current_timestamp);
            SET @NewSemesterId = @NextIdEnrollment;
        END

    UPDATE Student
    SET IdEnrollment = @NewSemesterId
    WHERE IdEnrollment = (SELECT e.IdEnrollment FROM Enrollment e WHERE e.Semester = @Semester AND e.IdStudy = @IdStudy);

    SELECT IdEnrollment, Semester, IdStudy, StartDate FROM Enrollment WHERE IdEnrollment = @NewSemesterId;
END