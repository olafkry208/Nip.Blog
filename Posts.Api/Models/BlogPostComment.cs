using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nip.Blog.Services.Posts.API.Models
{
    public class BlogPostComment
    {
        public long Id { get; set; }

        [Required]
        [StringLength(24, MinimumLength = 3)]
        public string Author { get; set; }

        [StringLength(256)]
        public string Content { get; set; }
    }
}
