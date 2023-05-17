using Microsoft.AspNetCore.Mvc;
using TweetBook.Filters;

namespace TweetBook.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController : Controller
    {

        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have no secrets");
        }
    }
}
