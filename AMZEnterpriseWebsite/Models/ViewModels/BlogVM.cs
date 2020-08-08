using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class BlogVM
    {
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<BlogVMArchiveDate> ArchivesDate { get; set; }
    }

    public class BlogVMArchiveDate
    {
        public int Year;
        public int Month;
    }
}
