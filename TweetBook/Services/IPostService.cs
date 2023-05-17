using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter, PaginationFilter paginationFilte);

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UserOwnsPostAsync(Guid postId, string userId);




        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);

        Task<bool> CreateTagAsync(Tag tag);

        Task<bool> DeleteTagAsync(string tagName);

    }
}
