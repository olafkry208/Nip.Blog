using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nip.Blog.Services.Posts.API.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Nip.Blog.Services.Posts.API.Models;
using Nip.Blog.Services.Posts.Api.Repositories;

namespace Nip.Blog.Services.Posts.Api
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Configuring services");
            String connection;
            var dbType = "sqlite";

            services.AddMvc();

            switch (dbType) {
                case "mssql":
                    connection = @"Server=(localdb)\mssqllocaldb;Database=BlogPostsDb;Trusted_Connection=True;ConnectRetryCount=0";
                    services.AddDbContextPool<BlogPostContext>(options => options.UseSqlServer(connection));
                    break;

                case "sqlite":
                    connection = @"Data Source=Data/Posts.db";
                    services.AddDbContextPool<BlogPostContext>(opt => opt.UseSqlite(connection));
                    break;

                default:
                    services.AddDbContext<BlogPostContext>(opt => opt.UseInMemoryDatabase("BlogPosts"));
                    break;
            }

            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "Blog Posts API",
                    Version = "v1",
                    Description = "Blog Posts API for 2018/19 NiPwPP class at Silesian University of Technology.",
                    Contact = new Contact {
                        Url = "https://github.com/olafkry208/Nip.Blog"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BlogPostContext context)
        {
            if (env.IsDevelopment())
            {
                _logger.LogInformation("App is in development environment");
                app.UseDeveloperExceptionPage();

                context.Database.EnsureCreated();
                if (!context.BlogPosts.Any())
                {
                    var posts = new List<BlogPost>
                    {
                        new BlogPost{Id = 1, Title = "Blog Post #1", Description = "Lorem ipsum..."},
                        new BlogPost{Id = 2, Title = "Blog Post #2", Description = "Lorem ipsum..."},
                        new BlogPost{Id = 3, Title = "Blog Post #3", Description = "Lorem ipsum..."},
                    };
                    context.BlogPosts.AddRange(posts);
                    context.SaveChanges();
                }
            } else
            {
                _logger.LogInformation("App is in production environment");
                app.UseExceptionHandler("/api/v1/Error");
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog Posts API v1");
            });
        }
    }
}
