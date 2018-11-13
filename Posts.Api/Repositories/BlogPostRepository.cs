using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nip.Blog.Services.Posts.API.Data;
using Nip.Blog.Services.Posts.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;

namespace Nip.Blog.Services.Posts.Api.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogPostContext _postsDbContext;

        public BlogPostRepository(BlogPostContext postsDbContext)
        {
            _postsDbContext = postsDbContext;
        }

        public IAsyncEnumerable<BlogPost> GetAllAsync()
        {   
            return _postsDbContext.BlogPosts.ToAsyncEnumerable();
        }

        public async Task<PaginatedItems<BlogPost>> GetAllPagedAsync(int pageIndex, int pageSize, Expression<Func<BlogPost, bool>> filter = null)
        {
            IQueryable<BlogPost> query = _postsDbContext.BlogPosts;
            if (filter != null) {
                query = query.Where(filter);
            }
            var totalItems = await query.CountAsync();
            var posts = query.OrderByDescending(c => c.Id).Skip(pageIndex * pageSize).Take(pageSize);

            var items = await posts.ToListAsync();

            return new PaginatedItems<BlogPost>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<BlogPost> GetAsync(long id)
        {
            return await _postsDbContext.BlogPosts.FindAsync(id);
        }
        
        public async Task AddAsync(BlogPost post)
        {
            await _postsDbContext.BlogPosts.AddAsync(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogPost post)
        {
            _postsDbContext.BlogPosts.Update(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            BlogPost post = await this.GetAsync(id);
            _postsDbContext.BlogPosts.Remove(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentsAsync(long blogPostId)
        {
            var post = await _postsDbContext.BlogPosts.Include(x => x.Comments).Where(x => x.Id == blogPostId).FirstAsync();
            return post.Comments;
        }

        public async Task AddCommentAsync(long blogPostId, BlogPostComment comment)
        {
            var post = await _postsDbContext.BlogPosts.Include(x => x.Comments).Where(x => x.Id == blogPostId).FirstAsync();
            if (post != null)
            {
                post.Comments.Add(comment);
                await this.UpdateAsync(post);
            }
        }
    }
}
