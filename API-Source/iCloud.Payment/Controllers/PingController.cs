using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Payment.Controllers
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
