using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using s17738_cw3.DAL;
using s17738_cw3.OrmModels;
using System.Text;
using s17738_cw3.DTO;
using s17738_cw3.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace s17738_cw3.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

        private StudentResponse MapStudent(Student student)
        {
            return new StudentResponse
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = student.BirthDate,
                Role = student.Role
            };
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var response = new List<StudentResponse>();
            foreach (Student s in _dbService.GetStudents())
            {
                response.Add(MapStudent(s));
            }

            return Ok(response);
        }

        [HttpGet("{indexNumber}")]
        public async Task<IActionResult> Get([FromRoute] string indexNumber)
        {
            Student s = await _dbService.GetStudent(indexNumber);
            if (s != null)
            {
                return Ok(MapStudent(s));
            }
            return NotFound();
        }

        [HttpGet("{indexNumber}/enrollments")]
        public async Task<IActionResult> GetEnrollmentsAsync([FromRoute] string indexNumber)
        {
            var e = await _dbService.GetStudentEnrollments(indexNumber);
            if (e == null)
            {
                return NotFound();
            }
            return Ok(new StudentEnrollmentDto
            {
                IdEnrollment = e.IdEnrollment,
                Semester = e.Semester,
                IdStudy = e.IdStudy,
                StartDate = e.StartDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] Student student)
        {
            var s = await _dbService.CreateStudent(student);
            var newStudent = MapStudent(s);
            return Created("/api/students/" + newStudent.IndexNumber, newStudent);
        }

        [HttpPut("{indexNumber}")]
        public async Task<IActionResult> UpdateStudent([FromRoute] string indexNumber, [FromBody][Bind("FirstName,LastName")] Student student)
        {
            if (indexNumber == null)
            {
                return NotFound();
            }
            await _dbService.UpdateStudent(indexNumber, student);
            return NoContent();
        }

        [HttpDelete("{indexNumber}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] string indexNumber)
        {
            if (indexNumber == null)
            {
                return NotFound();
            }
            await _dbService.DeleteStudent(indexNumber);
            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            Student student = await _dbService.GetStudent(loginRequestDto.Login);
            if (student == null || !PassUtil.ValidatePassword(loginRequestDto.Password, student.PasswordSalt, student.Password))
            {
                return BadRequest("Invalid login or password");
            }

            Token authToken = new Token
            {
                Id = Guid.NewGuid().ToString(),
                UserId = student.IndexNumber
            };

            _dbService.SaveToken(authToken);

            return Ok(new
            {
                accessToken = CreateJwtToken(student),
                refreshToken = authToken.Id
            });
        }

        private string CreateJwtToken(Student student)
        {
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName + " " + student.LastName),
                new Claim(ClaimTypes.Role, student.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpPost("refresh-token/{token}")]
        public async Task<IActionResult> RefreshToken([FromRoute] string token)
        {

            Token authToken = _dbService.GetToken(token);
            if (authToken == null)
            {
                return BadRequest("Invalid or expired token");
            }

            Student student = await _dbService.GetStudent(authToken.UserId);
            if (student == null)
            {
                return BadRequest("Invalid user");
            }

            Token newAuthToken = new Token
            {
                Id = Guid.NewGuid().ToString(),
                UserId = student.IndexNumber
            };

            _dbService.SaveToken(newAuthToken);

            return Ok(new
            {
                accessToken = CreateJwtToken(student),
                refreshToken = newAuthToken.Id
            });
        }

    }
}
