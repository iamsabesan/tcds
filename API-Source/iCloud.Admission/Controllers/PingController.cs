using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Admission.Controllers
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
