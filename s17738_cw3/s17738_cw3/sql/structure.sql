
create table Enrollment
(
	IdEnrollment int not null
		constraint Enrollment_pk
			primary key,
	Semester int not null,
	IdStudy int not null
		constraint Enrollment_Studies
			references Studies,
	StartDate date not null
)
go

create table Student
(
	IndexNumber nvarchar(100) not null
		constraint Student_pk
			primary key,
	FirstName nvarchar(100) not null,
	LastName nvarchar(100) not null,
	BirthDate date not null,
	IdEnrollment int not null
		constraint Student_Enrollment
			references Enrollment,
	Password nvarchar(200),
	Role nvarchar(100),
	PasswordSalt nvarchar(200)
)
go

create table Studies
(
	IdStudy int not null
		constraint Studies_pk
			primary key,
	Name nvarchar(100) not null
)
go

create table Token
(
	Id nvarchar(100) not null
		constraint Token_pk
			primary key nonclustered,
	UserId nvarchar(100) not null
		constraint UserId_Student_IndexNumber_fk
			references Student
)
go



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

