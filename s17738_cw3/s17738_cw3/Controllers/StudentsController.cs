using Microsoft.AspNetCore.Mvc;


namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok("Nowak, Kowalski, Piotrowicz");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if(id == 1)
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

    }
}
