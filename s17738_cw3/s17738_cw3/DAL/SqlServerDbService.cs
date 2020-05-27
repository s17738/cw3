using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using s17738_cw3.DTO;
using s17738_cw3.Models;

namespace s17738_cw3.DAL
{
    public class SqlServerDbService : IDbService
    {
        private const string ConnectionString = "Server=127.0.0.1;Database=s17738;User Id=sa;Password=my_secret_password;";

        public IEnumerable<Student> GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "select * from Student";

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    list.Add(new Student
                    {
                        FirstName = sqlDataReader["FirstName"].ToString(),
                        LastName = sqlDataReader["LastName"].ToString(),
                        IndexNumber = sqlDataReader["IndexNumber"].ToString(),
                        Password = sqlDataReader["Password"].ToString(),
                        PasswordSalt = sqlDataReader["PasswordSalt"].ToString(),
                        Role = sqlDataReader["Role"].ToString()
                    });
                }
            }
            return list;
        }

        public Student GetStudent(string indexNumber)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "select * from Student where IndexNumber = @indexNumber";
                sqlCommand.Parameters.AddWithValue("indexNumber", indexNumber);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return new Student
                    {
                        FirstName = sqlDataReader["FirstName"].ToString(),
                        LastName = sqlDataReader["LastName"].ToString(),
                        IndexNumber = sqlDataReader["IndexNumber"].ToString(),
                        Password = sqlDataReader["Password"].ToString(),
                        PasswordSalt = sqlDataReader["PasswordSalt"].ToString(),
                        Role = sqlDataReader["Role"].ToString()
                    };
                }
            }
            return null;
        }

        public IEnumerable<Enrollment> GetStudentEnrollments(string indexNumber)
        {
            var list = new List<Enrollment>();
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "select E.* from Student S join Enrollment E on S.IdEnrollment = E.IdEnrollment and S.IndexNumber = @indexNumber";
                sqlCommand.Parameters.AddWithValue("indexNumber", indexNumber);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    list.Add(new Enrollment
                    {
                        IdEnrollment = (int)sqlDataReader["IdEnrollment"],
                        Semester = (int)sqlDataReader["Semester"],
                        StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString())
                    });
                }
            }
            return list;
        }

        public bool EnrollmentExist(int semester, string studies)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "select IdEnrollment from Enrollment e join Studies s on e.IdStudy = s.IdStudy where s.Name = @Studies and e.Semester = @Semester";
                sqlCommand.Parameters.AddWithValue("Semester", semester);
                sqlCommand.Parameters.AddWithValue("Studies", studies);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                return sqlDataReader.Read();
            }
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

        public UserAuthToken getToken(string token)
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
                    return new UserAuthToken
                    {
                        Id = sqlDataReader["Id"].ToString(),
                        UserId = sqlDataReader["userId"].ToString()
                    };
                }
            }
            return null;
        }

        public void saveToken(UserAuthToken authToken)
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
