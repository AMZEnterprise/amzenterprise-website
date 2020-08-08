using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class SinglePostVM
    {
        public Post Post { get; set; }
        public SingleCommentVM SingleComment { get; set; }
    }
}
