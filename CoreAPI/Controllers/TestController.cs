using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    public class TestController:Controller
    {
        [HttpGet("api/users")]
        public ObjectResult Get()
        {
            return Ok(new { result = "oki" });
        }
    }
}
