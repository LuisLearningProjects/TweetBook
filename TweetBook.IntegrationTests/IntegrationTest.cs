using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Data;

namespace TweetBook.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient testClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>().
                WithWebHostBuilder( builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext)) ;
                        services.AddDbContext<DataContext>(options => 
                        { 
                            options.UseInMemoryDatabase("testDb"); 
                        });
                    });
                });

            testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            var jwt = await GetJwtAsync();
            testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", jwt);
        }

        protected async Task<string> GetJwtAsync()
        {
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Identity.Register,
                new UserRegistrationRequest 
                {
                    Email = "lprfernandes10@gmail.com",
                    Password = "Testando1234!"
                });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }

        protected async Task<Response<PostResponse>> CreatePostAsync(CreatePostRequest request)
        {
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);

            return await response.Content.ReadAsAsync<Response<PostResponse>>();
        }
    }
}
