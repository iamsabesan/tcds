using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace News.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return this.GetType().Namespace;
        }
    }
}
