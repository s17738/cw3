INSERT INTO s17738.dbo.Studies (IdStudy, Name) VALUES (1, N'IT');
INSERT INTO s17738.dbo.Studies (IdStudy, Name) VALUES (2, N'WIZ');

INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (1, 1, 1, N'2020-04-09');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (2, 2, 1, N'2020-07-16');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (3, 1, 2, N'2020-04-21');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (4, 3, 1, N'2020-04-21');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (5, 4, 1, N'2020-04-21');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (6, 5, 1, N'2020-04-21');
INSERT INTO s17738.dbo.Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (7, 6, 1, N'2020-04-21');

INSERT INTO s17738.dbo.Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment, Password, Role, PasswordSalt) VALUES (N's1234', N'Andrzej', N'Malewski', N'2019-01-06', 7, N'97ezu7f6tM5qHRkRA9wWj72PfXkrLsT7sKoEWFSET8s=', N'employee', N'ifHoJSCn7vKbyMSyulBYIA==');
dEnrollment, Semester, IdStudy, StartDate) VALUES (7, 6, 1, N'2020-04-21');