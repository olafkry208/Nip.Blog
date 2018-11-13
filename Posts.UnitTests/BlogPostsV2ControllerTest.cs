using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nip.Blog.Services.Posts.API.Data;
using Nip.Blog.Services.Posts.API.Models;
using Nip.Blog.Services.Posts.Api.Controllers;
using Nip.Blog.Services.Posts.Api.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Nip.Blog.Services.Posts.UnitTests
{
    public class BlogPostsV2ControllerTest
    {
        [Fact]
        public async Task ShouldReturnEmptyPageWhenCallingGetWithParamsOnEmptyRepo()
        {
            // Given
            var mockLogger = new Mock<ILogger<BlogPostsV2Controller>>();
            var mockRepo = new Mock<IBlogPostRepository>();
            mockRepo.Setup(repo => repo.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null))
            .ReturnsAsync(new PaginatedItems<BlogPost>());
            var controller = new BlogPostsV2Controller(mockLogger.Object, mockRepo.Object);
            // When
            var result = await controller.Get(0, 5);
            // Then
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaginatedItems<BlogPost>>(actionResult.Value);
            Assert.Null(returnValue.Items);
            Assert.Equal(0, returnValue.PageIndex);
            Assert.Equal(0, returnValue.PageSize);
            Assert.Equal(0, returnValue.TotalItems);
            Assert.Null(returnValue.NextPage);
        }

        [Fact]
        public async Task ShouldReturnCreatedBlogPostWhenAddingNewBlogPost()
        {
            // Given
            var mockLogger = new Mock<ILogger<BlogPostsV2Controller>>();
            var mockRepo = new Mock<IBlogPostRepository>();
            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<BlogPost>()))
            .Returns(Task.CompletedTask).Verifiable();
            var controller = new BlogPostsV2Controller(mockLogger.Object, mockRepo.Object);
            var somePost = new BlogPost { Title = "Some Post", Description = "Desc" };
            // When
            var result = await controller.Post(somePost);
            // Then
            var actionResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnValue = Assert.IsType<BlogPost>(actionResult.Value);
            Assert.Equal(somePost, returnValue);
            mockRepo.Verify();
        }

        [Fact]
        public async Task ShouldReturnNotFoundResultWhenDeletingNonExistingBlogPost()
        {
            // Given
            var mockLogger = new Mock<ILogger<BlogPostsV2Controller>>();
            var mockRepo = new Mock<IBlogPostRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<long>()))
            .ReturnsAsync((BlogPost)null);
            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<long>()))
            .Returns(Task.CompletedTask);
            var controller = new BlogPostsV2Controller(mockLogger.Object, mockRepo.Object);
            // When
            var nonExistingBlogPostId = 6789;
            var result = await controller.Delete(nonExistingBlogPostId);
            // Then
            var actionResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
