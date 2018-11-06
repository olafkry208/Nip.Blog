using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Nip.Blog.Services.Posts.Api.Controllers
{
    [Route("api/v1/Error")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        // GET api/blogposts
        [HttpGet]
        [ProducesResponseType(500)]
        public IActionResult Index()
        {
            _logger.LogError("Unhandled exception");
            return StatusCode(500, new { error = "Unhandled exception" });
        }
    }
}