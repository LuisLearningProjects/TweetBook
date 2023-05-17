using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Services;
using static TweetBook.Contracts.V1.ApiRoutes;

namespace TweetBook.Controllers.V1
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin,Poster")]

    [Authorize(Policy = "MustWorkInCompany")]
    [Produces("application/json")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;


        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }



        //GET


        ///<summary>
        ///Returns all the tags in the system
        ///</summary>
        ///<response code="201">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            var tagsResponses = _mapper.Map<List<TagResponse>>(tags);
            return Ok(new Response<List<TagResponse>> (tagsResponses));
        }



        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);

            if (tag == null)
            {
                NotFound();
            }

            var tagResponse = _mapper.Map<TagResponse>(tag);

            return Ok(new Response<TagResponse>(tagResponse));
        }



        //POST


        ///<summary>
        ///Creates a tag in the system
        ///</summary>
        ///<response code="201">Creates a tag in the system</response>
        ///<response code="400">Unable to create the tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {

            var newTag = new Tag
            {
                Name = request.TagName,
                CreatorId = HttpContext.GetUserId(),
                CreatedBy = User.FindFirstValue(ClaimTypes.Email),
            CreatedOn = DateTime.UtcNow
            };

            var created = await _postService.CreateTagAsync(newTag);

            if (!created)
            {
                return BadRequest(new ErrorResponse 
                { Errors = new List<ErrorModel> 
                { new ErrorModel { Message = "Unable to create tag" } } });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name);

            var newTagResponse = _mapper.Map<TagResponse>(newTag);

            return Created(locationUri, new Response<TagResponse>(newTagResponse));    
        }

        //DELETE


        //[Authorize(Roles = "Admin")]
        [HttpDelete(ApiRoutes.Tags.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();

        }

    }
}
