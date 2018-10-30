using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nip.Blog.Services.Posts.Api.Controllers
{
    [Route("api/v1/Error")]
    public class ErrorController : Controller
    {
        // GET api/blogposts
        [HttpGet]
        [ProducesResponseType(500)]
        public IActionResult Index()
        {
            return StatusCode(500, new { error = "Unhandled exception" });
        }
    }
}