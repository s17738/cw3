using Microsoft.AspNetCore.Mvc;
using s17738_cw3.DAL;
using s17738_cw3.DTO;
using s17738_cw3.Models;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] EnrollStudentRequest enrollStudentRequest)
        {
            if (_dbService.GetStudent(enrollStudentRequest.IndexNumber) != null)
            {
                return BadRequest("Student already exist");
            }

            Enrollment enrollment = _dbService.EnrollStudent(enrollStudentRequest);
            if (enrollment == null)
            {
                throw new System.Exception("Unknown Error");
            }

            var enrollStudentResponse = new EnrollStudentResponse
            {
                LastName = enrollStudentRequest.LastName,
                Semester = enrollment.Semester,
                StartDate = enrollment.StartDate
            };

            return Created("", enrollStudentResponse);
        }
    }
}
