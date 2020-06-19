using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using s17738_cw3.DAL;
using s17738_cw3.DTO;
using s17738_cw3.OrmModels;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    //[Authorize(Roles = "employee")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnrollStudentRequest enrollStudentRequest)
        {
            if (await _dbService.GetStudent(enrollStudentRequest.IndexNumber) != null)
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

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {

            if (!_dbService.EnrollmentExist(promoteStudentsRequest.Semester, promoteStudentsRequest.Studies))
            {
                return NotFound();
            }

            Enrollment enrollment = _dbService.PromoteStudents(promoteStudentsRequest.Semester, promoteStudentsRequest.Studies);

            if (enrollment == null)
            {
                throw new System.Exception("Unknown Error");
            }

            return Created("", enrollment);
        }
    }
}
