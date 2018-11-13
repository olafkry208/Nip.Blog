using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nip.Blog.Services.Posts.API.Data;
using Nip.Blog.Services.Posts.API.Models;
using Nip.Blog.Services.Posts.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nip.Blog.Services.Posts.Api.Controllers
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/BlogPosts")]
    public class BlogPostsV2Controller : Controller
    {
        private readonly ILogger<BlogPostsV2Controller> _logger;
        private readonly IBlogPostRepository _postRepository;

        public BlogPostsV2Controller(ILogger<BlogPostsV2Controller> logger, IBlogPostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
        }

        // GET api/blogposts[?pageIndex=3&pageSize=10]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BlogPost>))]
        [ProducesResponseType(200, Type = typeof(PaginatedItems<BlogPost>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get([FromQuery]int pageIndex = -1, [FromQuery]int pageSize = 5)
        {
            _logger.LogInformation("Retrieving all posts");

            if (pageIndex < 0)
            {
                var posts = await _postRepository.GetAllAsync().ToList();
                return Ok(posts);
            } else
            {
                var posts = await _postRepository.GetAllPagedAsync(pageIndex, pageSize);
                var isLastPage = (pageIndex + 1) * pageSize >= posts.TotalItems;
                posts.NextPage = (!isLastPage ? Url.Link(null, new { pageIndex = pageIndex + 1, pageSize = pageSize }) : null);
                return Ok(posts);
            }
        }

        // GET api/blogposts/withtitle/test
        [HttpGet("withtitle/{title:minlength(1)}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(PaginatedItems<BlogPost>))]
        public async Task<IActionResult> Get(string title, [FromQuery]int pageIndex = 0, [FromQuery]int pageSize = 5)
        {
            _logger.LogInformation("Retrieving all posts with title {0}", title);

            var posts = await _postRepository.GetAllPagedAsync(pageIndex, pageSize, x => x.Title.Contains(title));
            return Ok(posts);
        }

        // GET api/blogposts/5
        [HttpGet("{id}", Name = "GetBlogPostV2")]
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

            return CreatedAtRoute("GetBlogPostV2", new { id = post.Id }, post);
        }

        // PUT api/blogposts/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
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
                try
                {
                    await _postRepository.UpdateAsync(post);
                } catch (DbUpdateConcurrencyException e)
                {
                    return Conflict(e);
                }

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

        [HttpGet("{id}/comments", Name = "GetBlogPostComments")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BlogPostComment>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<BlogPostComment>>> GetAllComments(long id) {
            _logger.LogInformation("Retrieving comments for post {0}", id);

            var post = await _postRepository.GetAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                var items = await _postRepository.GetCommentsAsync(id);
                _logger.LogInformation("Comments for post {0} retrieved successfully", id);
                return Ok(items);
            }
        }
        
        [HttpPost("{id}/comments")]
        [ProducesResponseType(201, Type = typeof(BlogPostComment))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostComment(long id, [FromBody] BlogPostComment comment) {
            _logger.LogInformation("Creating new comment for post {0}", id);
            _logger.LogDebug("Received comment with author: {1}'", comment.Author);

            var post = await _postRepository.GetAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post {0} not found", id);
                return NotFound();
            }
            else
            {
                await _postRepository.AddCommentAsync(id, comment);
                return CreatedAtRoute("GetBlogPostComments", new { id = comment.Id }, comment);
            }
        }
    }
}
