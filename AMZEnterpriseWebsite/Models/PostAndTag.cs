﻿namespace AMZEnterpriseWebsite.Models
{
    public class PostAndTag
    {
        public int TagId { get; set; }
        public int PostId { get; set; }

        public Tag Tag { get; set; }
        public Post Post { get; set; }
    }
}
