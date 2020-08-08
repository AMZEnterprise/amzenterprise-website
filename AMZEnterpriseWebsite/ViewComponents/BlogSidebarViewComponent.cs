using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class BlogSidebarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private BlogVM _blogVm;
        public BlogSidebarViewComponent(ApplicationDbContext context)
        {
            _context = context;
            _blogVm = new BlogVM()
            {
                //Select categories which used in posts
                Categories = _context.Posts
                    .Include(p => p.Category)
                    .Where(p=>p.Status == PostStatus.Sent)
                    .Select
                        (
                            p => p.Category
                        )
                    .Distinct(),

                //Select latest tags which used in posts
                Tags = _context.PostAndTags
                    .Include(pt => pt.Post)
                    .Include(pt => pt.Tag)
                    .Where(p => p.Post.Status == PostStatus.Sent)
                    .OrderByDescending(pt => pt.Tag.DateTime)
                    .Select
                        (
                            pt => pt.Tag
                        )
                    .Take(20)
                    .Distinct(),

                Posts = _context.Posts
                    .Include(p=>p.Media)
                    .OrderByDescending(p => p.DateTime)
                    .Take(4),


                ArchivesDate = _context.Posts.Select(p => new BlogVMArchiveDate()
                {
                    Year = p.DateTime.Year,
                    Month = p.DateTime.Month
                })
                    .OrderByDescending(p => p.Year)
                    .ThenByDescending(p => p.Month)
                    .Distinct()
            };
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_blogVm);
        }
    }
}
