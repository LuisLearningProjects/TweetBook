using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TweetBook.Cache;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Requests.Queries;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Helpers;
using TweetBook.Services;
using static TweetBook.Contracts.V1.ApiRoutes;

namespace TweetBook.Controllers.V1
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }



        //GET

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll(GetAllPostsQuery query, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var filter = _mapper.Map<GetAllPostsFilter>(query);


            var posts = await _postService.GetPostsAsync(filter, pagination);
            var postsResponse = _mapper.Map<List<PostResponse>>(posts);


            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 0)
            {
                return Ok(new PagedResponse<PostResponse>(postsResponse));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, postsResponse);

            return Ok(paginationResponse);
        }



        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            var postResponse = _mapper.Map<PostResponse>(post);

            return Ok(new Response<PostResponse>(postResponse));
        }


        //PUT
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest updatePostRequest)
        {

            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { Error = "You do not own this post" });
            }

            var post = await _postService.GetPostByIdAsync(postId);

            post.Name = updatePostRequest.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
            {
                var updatedPost = _mapper.Map<PostResponse>(post);
                return Ok(new Response<PostResponse>(updatedPost));
            }
            return NotFound();
        }



        //DELETE
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { Error = "You do not own this post" });
            }

            var deleted = await _postService.DeletePostAsync(postId);

            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }



        //POST
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(x => new PostTag { PostId = newPostId, TagName = x}).ToList()
            };

            var created = await _postService.CreatePostAsync(post);

            if (!created)
            {
                return BadRequest();
            }

            var postResponse = _mapper.Map<PostResponse>(post);

            //var baseUrl = $"{Request.Scheme}/{Request.Host.ToUriComponent}";
            //var locationUri = $"{baseUrl}/{ApiRoutes.Posts.Get}".Replace("{postId}", postResponse.Id.ToString());
            var locationUri = _uriService.GetPostUri(post.Id.ToString());

            return Created(locationUri,new Response<PostResponse>(postResponse));
        }
    }   
}
