﻿namespace AwarenessCampaign.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
