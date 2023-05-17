using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TweetBook.Contracts.V1.Requests.Queries;
using TweetBook.Data;
using TweetBook.Domain;
using static TweetBook.Contracts.V1.ApiRoutes;

namespace TweetBook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;


        public PostService(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }




        public async Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Posts.AsQueryable();

            if (paginationFilter == null)
            {
                return await queryable.Include(x=> x.Tags).ToListAsync();
            }

            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                queryable = queryable.Where(x => x.UserId == filter.UserId);
            }

            var skip = (paginationFilter.PageNumber -1) * paginationFilter.PageSize;

            return await queryable.Include(x => x.Tags)
                .Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
        }




        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }




        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            var post = await GetPostByIdAsync(postToUpdate.Id);

            if (post == null)
                return false;

            _dataContext.Posts.Update(post);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }





        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (post == null)
                return false;
            
            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();  
            return deleted > 0;
        }



        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dataContext.Posts.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }




        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id== postId);

            if (post == null )
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }
            return true;

        }





        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dataContext.Tags.ToListAsync();
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _dataContext.Tags.SingleOrDefaultAsync(x => x.Name == tagName);
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            await _dataContext.Tags.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await GetTagByNameAsync(tagName);

            if (tag == null)
                return false;

            _dataContext.Tags.Remove(tag);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }


    }
}
