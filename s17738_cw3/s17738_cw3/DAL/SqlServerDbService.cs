using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using s17738_cw3.DTO;
using s17738_cw3.OrmModels;
using s17738_cw3.Util;

namespace s17738_cw3.DAL
{
    public class SqlServerDbService : IDbService
    {
        private const string ConnectionString = "Server=127.0.0.1;Database=s17738;User Id=sa;Password=z276DtAfs5d!sfn459ndej;";

        private readonly S17738Context _context;

        public SqlServerDbService(S17738Context s17738Context)
        {
            _context = s17738Context;
        }

        public IEnumerable<Student> GetStudents()
        {
            return _context.Student.ToList();
        }

        public async Task<Student> GetStudent(string indexNumber)
        {
            return await _context.Student
                                 .Where(s => s.IndexNumber == indexNumber)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Student> CreateStudent(Student student)
        {
            var salt = PassUtil.GenerateSalt();
            var plainPass = student.Password;
            var s = new Student
            {
                IndexNumber = $"s{ new Random().Next(1, 30000)}",
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = student.BirthDate,
                Role = "student",
                PasswordSalt = salt,
                Password = PassUtil.GeneratePasswordHash(plainPass, salt),
                IdEnrollment = 1
            };

            _context.Student.Add(s);
            await _context.SaveChangesAsync();
            return s;
        }

        public async Task<bool> UpdateStudent(string indexNumber, Student student)
        {
            var s = new Student()
            {
                IndexNumber = indexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName
            };
            _context.Entry(s).Property("FirstName").IsModified = true;
            _context.Entry(s).Property("LastName").IsModified = true;

            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteStudent(string indexNumber)
        {
            var s = new Student()
            {
                IndexNumber = indexNumber
            };

            _context.Attach(s);
            _context.Remove(s);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<Enrollment> GetStudentEnrollments(string indexNumber)
        {
            var student = await _context.Student
                            .Include(s => s.IdEnrollmentNavigation)
                            .Where(s => s.IndexNumber == indexNumber)
                            .FirstOrDefaultAsync();
            return student.IdEnrollmentNavigation;
        }

        public bool EnrollmentExist(int semester, string studies)
        {
            var e = _context.Enrollment
                .Include(e => e.IdStudyNavigation)
                .Where(e => e.Semester == semester && e.IdStudyNavigation.Name == studies)
                .FirstOrDefault();
            return e == null;
        }

        public Enrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest)
        {
            Enrollment enrollment = new Enrollment
            {
                Semester = 1,
                StartDate = DateTime.Now
            };

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "select IdStudy from Studies where Name=@Name";
                    command.Parameters.AddWithValue("Name", enrollStudentRequest.Studies);

                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        transaction.Rollback();
                        return null;
                    }
                    int IdStudy = (int)reader["IdStudy"];
                    reader.Close();
                    command.Parameters.Clear();

                    command.CommandText = "select top 1 IdEnrollment, Semester, IdStudy, StartDate from Enrollment where IdStudy = @IdStudy and Semester = 1 order by StartDate";
                    command.Parameters.AddWithValue("IdStudy", IdStudy);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                    }
                    reader.Close();
                    command.Parameters.Clear();


                    if (enrollment.IdEnrollment == 0)
                    {
                        //zapis na studia nie istnieje
                        //generate IdEnrollment
                        command.CommandText = "SELECT top 1 IdEnrollment + 1 as IdEnrollment from Enrollment order by IdEnrollment desc";
                        reader = command.ExecuteReader();
                        if (!reader.Read())
                        {
                            transaction.Rollback();
                            return null;
                        }
                        enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                        reader.Close();
                        command.Parameters.Clear();

                        command.CommandText = "insert into Enrollment (IdEnrollment, Semester, IdStudy, StartDate) values (@IdEnrollment, @Semester, @IdStudy, @StartDate)";
                        command.Parameters.AddWithValue("IdEnrollment", enrollment.IdEnrollment);
                        command.Parameters.AddWithValue("Semester", 1);
                        command.Parameters.AddWithValue("IdStudy", IdStudy);
                        command.Parameters.AddWithValue("StartDate", enrollment.StartDate);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }


                    //add new student
                    command.CommandText = "insert into Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) values (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    command.Parameters.AddWithValue("IndexNumber", enrollStudentRequest.IndexNumber);
                    command.Parameters.AddWithValue("FirstName", enrollStudentRequest.FirstName);
                    command.Parameters.AddWithValue("LastName", enrollStudentRequest.LastName);
                    command.Parameters.AddWithValue("BirthDate", enrollStudentRequest.Birthdate);
                    command.Parameters.AddWithValue("IdEnrollment", enrollment.IdEnrollment);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    transaction.Commit();
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    throw exc;
                }
            }
            return enrollment;
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PromoteStudents";
                sqlCommand.Parameters.AddWithValue("Semester", semester);
                sqlCommand.Parameters.AddWithValue("Studies", studies);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return new Enrollment()
                    {
                        IdEnrollment = (int)sqlDataReader["IdEnrollment"],
                        Semester = (int)sqlDataReader["Semester"],
                        StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString())
                    };
                }
                return null;
            }
        }

        public Token GetToken(string token)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "select * from Token where Id = @Id";
                sqlCommand.Parameters.AddWithValue("Id", token);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return new Token
                    {
                        Id = sqlDataReader["Id"].ToString(),
                        UserId = sqlDataReader["userId"].ToString()
                    };
                }
            }
            return null;
        }

        public void SaveToken(Token authToken)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into Token (Id, UserId) values (@Id, @UserId)";
                    command.Parameters.AddWithValue("Id", authToken.Id);
                    command.Parameters.AddWithValue("UserId", authToken.UserId);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    transaction.Commit();
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    throw exc;
                }
            }
        }
    }
}
