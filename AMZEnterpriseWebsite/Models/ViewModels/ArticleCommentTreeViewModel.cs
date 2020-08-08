using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class ArticleCommentTreeViewModel
    {
        public int? CommentSeed { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
