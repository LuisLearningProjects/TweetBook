
using Refit;
using System.Diagnostics;
using System.Security.Principal;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Sdk;


const string TestEmail = "sdkaccount@gmail.com";
const string TestPassword = "Test1234!";


var cachedToken = string.Empty;

var identityApi = RestService.For<IIdentityApi>("https://localhost:7153");
var tweetBookApi = RestService.For<ITweetBookApi>("https://localhost:7153", 
    new RefitSettings { AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)});


var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest { Email = TestEmail, Password = TestPassword });
var loginResponse = await identityApi.LoginAsync(new UserLoginRequest { Email = TestEmail, Password = TestPassword });

cachedToken = loginResponse.Content.Token;

        

var allPosts = await tweetBookApi.GetAllAsync();
var createdPost = await tweetBookApi.CreateAsync(new CreatePostRequest 
{ 
    Name = "This is created by the sdk",
    Tags = new[] {"sdk"}
});

var retrievePost = await tweetBookApi.GetAsync(createdPost.Content.Id);
var updatedPost = await tweetBookApi.UpdateAsync(new UpdatePostRequest
{
    Name = "This is updated by the sdk"
});
var deletedPost = await tweetBookApi.DeleteAsync(createdPost.Content.Id);
