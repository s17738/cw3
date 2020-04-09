using System.Collections.Generic;
using System.Data.SqlClient;
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
                        IndexNumber = sqlDataReader["IndexNumber"].ToString()
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
                        IndexNumber = sqlDataReader["IndexNumber"].ToString()
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
                        Semester = sqlDataReader["Semester"].ToString(),
                        StartDate = sqlDataReader["StartDate"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
