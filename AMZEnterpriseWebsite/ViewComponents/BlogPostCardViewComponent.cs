using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class BlogPostCardViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public BlogPostCardViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Post blogPost)
        {
            return View(blogPost);
        }
    }
}
