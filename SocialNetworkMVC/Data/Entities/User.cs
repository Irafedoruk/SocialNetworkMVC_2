using Microsoft.AspNetCore.Identity;

namespace SocialNetworkMVC.Data.Entities
{
    public class User : IdentityUser
    {        
        public DateTime? Birthdate { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Photo>? Photos { get; set; }
        public ICollection<Message>? SentMessages { get; set; }
        public ICollection<Message>? ReceivedMessages { get; set; }
        public ICollection<User>? Friends { get; set; }
    }
}
