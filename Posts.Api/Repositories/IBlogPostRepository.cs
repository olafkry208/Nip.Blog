using Nip.Blog.Services.Posts.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nip.Blog.Services.Posts.Api.Repositories
{
    public interface IBlogPostRepository
    {
        IAsyncEnumerable<BlogPost> GetAllAsync();
        Task<PaginatedItems<BlogPost>> GetAllPagedAsync(int pageIndex, int pageSize, Expression<Func<BlogPost, bool>> filter = null);
        Task<BlogPost> GetAsync(long id);
        Task AddAsync(BlogPost post);
        Task UpdateAsync(BlogPost post);
        Task DeleteAsync(long id);

        Task<IEnumerable<BlogPostComment>> GetCommentsAsync(long blogPostId);
        Task<BlogPostComment> GetCommentAsync(long blogPostId, long commentId);
        Task AddCommentAsync(long blogPostId, BlogPostComment comment);
        Task DeleteCommentAsync(long blogPostId, long commentId);
    }
}
