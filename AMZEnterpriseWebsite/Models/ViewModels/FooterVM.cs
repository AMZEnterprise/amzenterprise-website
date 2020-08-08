using System.Collections.Generic;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class FooterVM
    {
        public IEnumerable<Tag> Tags{ get; set; }
        public Setting Setting { get; set; }
    }
}
