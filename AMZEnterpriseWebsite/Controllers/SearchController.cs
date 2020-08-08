using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {

            var posts = _context.PostAndTags
                .Include(p => p.Post)
                .Include(p => p.Post.Category)
                .Include(p => p.Tag)
                .Where
                (
                    p => (p.Post.Body.Contains(searchString) ||
                         p.Post.Title.Contains(searchString) ||
                         p.Post.Category.Name.Contains(searchString) ||
                         p.Tag.Name.Contains(searchString))

                    &&

                    p.Post.Status == PostStatus.Sent
                )
                .Select
                (
                    p => p.Post
                )
                .Include(p => p.Comments)
                .Include(p=>p.User)
                .Include(p=>p.Media)
                .Distinct();

            ViewData["TotalCount"] = posts.Count();
            ViewData["SearchQuery"] = searchString;

            ViewData["CurrentPage"] = pageNumber ?? 1;
            int pageSize = 8;
            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> SearchByCategory(int? id, int? pageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var posts = _context.Posts
                .Include(p => p.Category)
                .Where(p => p.Category.Id == id && p.Status == PostStatus.Sent)
                .Include(p => p.PostAndTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .Include(p => p.User)
                .Include(p => p.Media);

            if (posts == null)
            {
                return NotFound();
            }

            ViewData["SearchQuery"] = category.Name;
            ViewData["TotalCount"] = posts.Count();

            ViewData["CurrentPage"] = pageNumber ?? 1;
            int pageSize = 8;
            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> SearchByTag(int? id, int? pageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }


            var posts = _context.PostAndTags
                .Include(p => p.Post)
                .Include(p => p.Post.Category)
                .Include(p => p.Tag)
                .Where(p => p.TagId == id && p.Post.Status == PostStatus.Sent)
                .Select
                (
                    p => p.Post
                )
                .Include(p => p.Comments)
                .Include(p => p.User)
                .Include(p => p.Media)
                .Distinct();

            if (posts == null)
            {
                return NotFound();
            }

            ViewData["SearchQuery"] = tag.Name;
            ViewData["TotalCount"] = posts.Count();

            ViewData["CurrentPage"] = pageNumber ?? 1;
            int pageSize = 8;
            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        public async Task<IActionResult> SearchByArchiveDate(int? archiveYear, int? archiveMonth, int? pageNumber)
        {
            if (archiveYear == null)
            {
                return NotFound();
            }

            if (archiveMonth == null)
            {
                return NotFound();
            }


            var posts = _context.Posts
                        .Include(p => p.Category)
                        .Include(p => p.PostAndTags)
                        .ThenInclude(pt => pt.Tag)
                        .Include(p => p.Comments)
                        .Where(p => p.DateTime.Year == archiveYear && p.DateTime.Month == archiveMonth && p.Status == PostStatus.Sent)
                        .OrderByDescending(p => p.DateTime)
                        .Include(p => p.User)
                        .Include(p => p.Media);

            if (posts == null)
            {
                return NotFound();
            }

            DateTime date = new DateTime((int)archiveYear,(int)archiveMonth,1);

            ViewData["SearchQuery"] = date.ToPersianDateTime().MonthName + " ماه " + date.ToPersianDateTime().Year;

            ViewData["TotalCount"] = posts.Count();

            ViewData["CurrentPage"] = pageNumber ?? 1;

            ViewData["ArchiveYear"] = archiveYear;
            ViewData["ArchiveMonth"] = archiveMonth;

            int pageSize = 8;
            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
    }
}