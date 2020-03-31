using Microsoft.AspNetCore.Mvc;


namespace s17738_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Nowak, Kowalski, Piotrowicz";
        }

    }
}
