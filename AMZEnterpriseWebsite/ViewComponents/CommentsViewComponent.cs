using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CommentsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(ArticleCommentTreeViewModel commentTree)
        {
            var articleCommentTree = new ArticleCommentTreeViewModel { CommentSeed = commentTree.CommentSeed, Comments = commentTree.Comments };

            return await Task.FromResult(View(articleCommentTree));
        }
    }
}
