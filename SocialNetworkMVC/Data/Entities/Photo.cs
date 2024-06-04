namespace SocialNetworkMVC.Data.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
