using AutoMapper;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;

namespace TweetBook.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Tag, TagResponse>();
            CreateMap<Post, PostResponse>().ForMember(
                dest=> dest.Tags, 
                opt=> opt.MapFrom(src=>src.Tags.Select(x=> new TagResponse { Name = x.TagName})));
        }
    }
}
