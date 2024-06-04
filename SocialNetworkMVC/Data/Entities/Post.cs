namespace SocialNetworkMVC.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DatePublish { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public string UserProfileId { get; set; } // ID профілю користувача, на якому розміщено пост
        public User UserProfile { get; set; } // Профіль користувача, на якому розміщено пост
    }
}
