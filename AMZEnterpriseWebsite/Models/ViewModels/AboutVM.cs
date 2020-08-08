using System.Collections.Generic;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class AboutVM
    {
        public IEnumerable<MyProgress> MyProgresses { get; set; }
        public Setting Setting { get; set; }
    }
}
