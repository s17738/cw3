using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using s17738_cw3.DAL;
using s17738_cw3.Models;
using System.Text;
using s17738_cw3.DTO;
using s17738_cw3.Util;
using System.Collections.Generic;

namespace s17738_cw3.Controllers
{
    [ApiController]
    [Authorize]
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

        [HttpGet]
        public IActionResult GetStudents()
        {
            var response = new List<StudentResponse>();
            foreach (Student s in _dbService.GetStudents())
            {
                response.Add(new StudentResponse
                {
                    IndexNumber = s.IndexNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Role = s.Role
                });
            }

            return Ok(response);
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
            string plainPass = student.Password;
            student.IndexNumber = $"s{ new Random().Next(1, 30000)}";
            student.PasswordSalt = PassUtil.GenerateSalt();
            student.Password = PassUtil.GeneratePasswordHash(plainPass, student.PasswordSalt);
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

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            Student student = _dbService.GetStudent(loginRequestDto.Login);
            if (student == null || !PassUtil.ValidatePassword(loginRequestDto.Password, student.PasswordSalt, student.Password))
            {
                return BadRequest("Invalid login or password");
            }

            UserAuthToken authToken = new UserAuthToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = student.IndexNumber
            };

            _dbService.saveToken(authToken);

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
        public IActionResult RefreshToken([FromRoute] string token)
        {

            UserAuthToken authToken = _dbService.getToken(token);
            if (authToken == null)
            {
                return BadRequest("Invalid or expired token");
            }

            Student student = _dbService.GetStudent(authToken.UserId);
            if (student == null)
            {
                return BadRequest("Invalid user");
            }

            UserAuthToken newAuthToken = new UserAuthToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = student.IndexNumber
            };

            _dbService.saveToken(newAuthToken);

            return Ok(new
            {
                accessToken = CreateJwtToken(student),
                refreshToken = newAuthToken.Id
            });
        }

    }
}
