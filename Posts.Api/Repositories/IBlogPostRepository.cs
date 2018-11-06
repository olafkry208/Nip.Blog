using Nip.Blog.Services.Posts.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nip.Blog.Services.Posts.Api.Repositories
{
    public interface IBlogPostRepository
    {
        IAsyncEnumerable<BlogPost> GetAllAsync();
        Task<PaginatedItems<BlogPost>> GetAllPagedAsync(int pageIndex, int pageSize);
        Task<BlogPost> GetAsync(long id);
        Task AddAsync(BlogPost post);
        Task UpdateAsync(BlogPost post);
        Task DeleteAsync(long id);
    }
}
