using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nip.Blog.Services.Posts.API.Data;
using Nip.Blog.Services.Posts.API.Models;
using Microsoft.Extensions.Logging;

namespace Nip.Blog.Services.Posts.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class BlogPostsController : Controller
    {
        private readonly BlogPostContext _postsDbContext;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(BlogPostContext postsDbContext, ILogger<BlogPostsController> logger)
        {
            _postsDbContext = postsDbContext;
            _logger = logger;
        }

        // GET api/blogposts
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<BlogPost>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Retrieving all posts");

            return Ok(await _postsDbContext.BlogPosts.ToAsyncEnumerable().ToList());
        }

        // GET api/blogposts/5
        [HttpGet("{id}", Name = "GetBlogPost")]
        [ProducesResponseType(200, Type = typeof(BlogPost))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(long id)
        {
            _logger.LogInformation("Retrieving post {0}", id);

            var item = await _postsDbContext.BlogPosts.FindAsync(id);
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

            await _postsDbContext.BlogPosts.AddAsync(post);
            await _postsDbContext.SaveChangesAsync();

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

            var post = await _postsDbContext.BlogPosts.FindAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                post.Title = updatedPost.Title;
                post.Description = updatedPost.Description;
                _postsDbContext.BlogPosts.Update(post);
                await _postsDbContext.SaveChangesAsync();

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

            var post = await _postsDbContext.BlogPosts.FindAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                _postsDbContext.BlogPosts.Remove(post);
                await _postsDbContext.SaveChangesAsync();

                _logger.LogInformation("Post {0} deleted successfully", id);
                return NoContent();
            }
        }
    }
}
