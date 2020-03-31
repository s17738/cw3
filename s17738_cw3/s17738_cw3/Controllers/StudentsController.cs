using System;
using Microsoft.AspNetCore.Mvc;
using s17738_cw3.Models;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok($"Nowak, Kowalski, Piotrowicz sortowanie={orderBy}");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id == 1)
            {
                return Ok("Nowak");
            }
            if (id == 2)
            {
                return Ok("Kowalski");
            }
            if (id == 3)
            {
                return Ok("Piotrowicz");
            }
            return NotFound("Student not found");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{ new Random().Next(1, 10000)}";
            return Ok(student);
        }

    }
}
