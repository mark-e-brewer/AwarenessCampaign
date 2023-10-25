using Npgsql.PostgresTypes;

namespace AwarenessCampaign.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostName { get; set; }
        public string Description { get; set; }
        public ICollection<Category> Categories { get; set; }
        public User User { get; set; }
    }
}
