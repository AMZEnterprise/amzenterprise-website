namespace AMZEnterpriseWebsite.Models
{
    public class PostAndMedia
    {
        public int MediaId { get; set; }
        public int PostId { get; set; }
        
        public Media Media { get; set; }
        public Post Post { get; set; }
    }
}
