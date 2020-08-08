namespace AMZEnterpriseWebsite.Models
{
    public class ProjectAndMedia
    {
        public int MediaId { get; set; }
        public int ProjectId { get; set; }

        public Media Media { get; set; }
        public Project Project { get; set; }
    }
}
