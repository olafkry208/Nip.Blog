using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nip.Blog.Services.Posts.API.Data;
using Nip.Blog.Services.Posts.API.Models;
using Microsoft.Extensions.Logging;
using Nip.Blog.Services.Posts.Api.Repositories;

namespace Nip.Blog.Services.Posts.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class BlogPostsController : Controller
    {
        private readonly ILogger<BlogPostsController> _logger;
        private readonly IBlogPostRepository _postRepository;

        public BlogPostsController(ILogger<BlogPostsController> logger, IBlogPostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
        }

        // GET api/blogposts
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<BlogPost>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Retrieving all posts");

            return Ok(await _postRepository.GetAllAsync().ToList());
        }

        // GET api/blogposts/5
        [HttpGet("{id}", Name = "GetBlogPost")]
        [ProducesResponseType(200, Type = typeof(BlogPost))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(long id)
        {
            _logger.LogInformation("Retrieving post {0}", id);

            var item = await _postRepository.GetAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                _logger.LogInformation("Post {0} retrieved successfully", id);
                return Ok(item);
            }
        }

        // POST api/blogposts
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(BlogPost))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] BlogPost post)
        {
            _logger.LogInformation("Creating new post");
            _logger.LogDebug("Received post with title: {1}'", post.Title);

            await _postRepository.AddAsync(post);

            return CreatedAtRoute("GetBlogPost", new { id = post.Id }, post);
        }

        // PUT api/blogposts/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(long id, [FromBody] BlogPost updatedPost)
        {
            _logger.LogInformation("Updating post {0}", id);
            _logger.LogDebug("Received post id {0} with new title: {1}'", id, updatedPost.Title);

            var post = await _postRepository.GetAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                post.Title = updatedPost.Title;
                post.Description = updatedPost.Description;
                await _postRepository.UpdateAsync(post);

                _logger.LogInformation("Post {0} updated successfully", id);
                return NoContent();
            }
        }

        // DELETE api/blogposts/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogInformation("Deleting post {0}", id);

            var post = await _postRepository.GetAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                await _postRepository.DeleteAsync(id);

                _logger.LogInformation("Post {0} deleted successfully", id);
                return NoContent();
            }
        }
    }
}
