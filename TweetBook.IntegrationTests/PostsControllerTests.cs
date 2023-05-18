using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Contracts.V1.Requests;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace TweetBook.IntegrationTests
{
    public class PostsControllerTests : IntegrationTest
    {
        
        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInDatabase()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Name = "Test Post",
                Tags = new List<string> { }
            });

            // Act
            var response = await testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}",createdPost.Data.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Id.Should().Be(createdPost.Data.Id);
            returnedPost.Data.Name.Should().Be("Test Post");
        }


        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Name = "Test Post",
                Tags = new List<string> { }
            });
            var response = await testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Data.Id.ToString()));
            var userId = createdPost.Data.UserId.ToString();
            var postId = createdPost.Data.Id.ToString();
            await testClient.DeleteAsync(ApiRoutes.Posts.Delete.Replace("{postId}", postId));
            
            // Act
            var responseGetAll = await testClient.GetAsync(ApiRoutes.Posts.GetAll + "?UserId=" + userId);

            // Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await responseGetAll.Content.ReadAsAsync<PagedResponse<PostResponse>>();
            content.Data.Should().BeEmpty();
        }
    }
}
