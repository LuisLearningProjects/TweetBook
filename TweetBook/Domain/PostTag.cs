using System.ComponentModel.DataAnnotations.Schema;

namespace TweetBook.Domain
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        
        public virtual Post Post { get; set; }

        
        public string TagName { get; set; }


        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }

    }
}
