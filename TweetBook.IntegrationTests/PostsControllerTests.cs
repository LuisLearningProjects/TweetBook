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
        protected string UserId;
        
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
            UserId = createdPost.Data.UserId;
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

            // Act
            var response = await testClient.GetAsync(ApiRoutes.Posts.GetAll + "?UserId=" + UserId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            //(await response.Content.ReadAsAsync<List<PostResponse>>()).Should().BeEmpty();
        }
    }
}
