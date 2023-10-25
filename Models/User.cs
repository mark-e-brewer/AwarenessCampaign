namespace AwarenessCampaign.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string UserName { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
