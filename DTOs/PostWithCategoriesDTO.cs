using AwarenessCampaign.Models;

namespace AwarenessCampaign.DTOs
{
    public class PostWithCategoriesDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostName { get; set; }
        public string Description { get; set; }
        public List<Category> Categories { get; set; }
    }
}
