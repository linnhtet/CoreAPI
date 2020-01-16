using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers.v1
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
