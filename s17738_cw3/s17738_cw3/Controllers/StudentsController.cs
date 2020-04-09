using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using s17738_cw3.DAL;
using s17738_cw3.Models;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConnectionString = "Server=127.0.0.1;Database=s17738;User Id=sa;Password=my_secret_password;";
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents([FromQuery] string orderBy)
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


            //if (orderBy == "lastName")
            //{
            //    return Ok(_dbService.GetStudents().OrderBy(s => s.LastName));
            //}
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult Get([FromRoute] string indexNumber)
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
                    return Ok(new Student
                    {
                        FirstName = sqlDataReader["FirstName"].ToString(),
                        LastName = sqlDataReader["LastName"].ToString(),
                        IndexNumber = sqlDataReader["IndexNumber"].ToString()
                    });
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateStudent([FromBody] Student student)
        {
            student.IndexNumber = $"s{ new Random().Next(1, 30000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent([FromRoute] int id, [FromBody] Student student)
        {
            return Ok($"Student {id} has been updated to {student.FirstName} {student.LastName}");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent([FromRoute] int id)
        {
            return Ok($"Student {id} has been deleted");
        }

    }
}
