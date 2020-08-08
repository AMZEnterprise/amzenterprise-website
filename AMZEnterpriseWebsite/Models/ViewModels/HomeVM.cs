using System.Collections.Generic;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class HomeVM
    {
        public Setting Setting{ get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<SurveyComment> SurveyComments { get; set; }
    }
}
