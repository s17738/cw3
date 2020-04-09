using System;
using Microsoft.AspNetCore.Mvc;
using s17738_cw3.DAL;
using s17738_cw3.Models;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{indexNumber}")]
        public IActionResult Get([FromRoute] string indexNumber)
        {
            Student student = _dbService.GetStudent(indexNumber);
            if (student != null)
            {
                return Ok(student);
            }
            return NotFound();
        }

        [HttpGet("{indexNumber}/enrollments")]
        public IActionResult GetEnrollments([FromRoute] string indexNumber)
        {
            return Ok(_dbService.GetStudentEnrollments(indexNumber));
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
