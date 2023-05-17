using Swashbuckle.AspNetCore.Filters;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Examples.Requests
{
    public class CreateTagRequestExample : IExamplesProvider<CreateTagRequest>
    {
        public CreateTagRequest GetExamples()
        {
            return new CreateTagRequest
            {
                TagName = "new tag"
            };
        }
    }
}
