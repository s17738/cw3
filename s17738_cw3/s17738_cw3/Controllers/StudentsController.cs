using System;
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

        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents([FromQuery] string orderBy)
        {
            if (orderBy == "lastName")
            {
                return Ok(_dbService.GetStudents().OrderBy(s => s.LastName));
            }
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            var student = _dbService.GetStudents().SingleOrDefault(s => s.IdStudent == id);

            if (student == null)
            {
                return NotFound("Student not found");
            }
            else
            {
                return Ok(student);
            }
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
