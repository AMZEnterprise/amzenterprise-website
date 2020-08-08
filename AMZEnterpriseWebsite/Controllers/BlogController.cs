using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;

namespace AMZEnterpriseWebsite.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _accessor;
        public BlogController(ApplicationDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }


        [Breadcrumb("بلاگ")]
        public async Task<IActionResult> Index(int? pageNumber)
        {

            //Only Released Posts Are Visible
            var posts = _context.Posts
                .Include(p => p.Comments)
                .Where(p => p.Status == PostStatus.Sent)
                .OrderByDescending(p => p.DateTime)
                .Include(p => p.User)
                .Include(p => p.Media)
                .AsQueryable();


            ViewData["CurrentPage"] = pageNumber ?? 1;

            int pageSize = 8;
            return View(await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }





        [Breadcrumb("ViewData.BreadcrumbTitle")]
        public async Task<IActionResult> Posts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var post = await _context.Posts
                .Include(p => p.PostAndTags)
                .ThenInclude(p => p.Tag)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (post == null)
            {
                return NotFound();
            }


            var fathers = await _context.Comments
                .Where(c => c.PostId == id && c.Status == CommentStatus.Accepted)
                .ToListAsync();

            var childComments = await _context.Comments
                .Where(c => c.PostId == id || c.ParentId == id)
                .Include(c => c.Children)
                .SelectMany(c => c.Children.Where(ch => ch.Status == CommentStatus.Accepted))
                .ToListAsync();

            childComments.AddRange(fathers);

            post.Comments = childComments.Distinct().ToList();




            TempData["PostId"] = id;

            SinglePostVM postVm = new SinglePostVM()
            {
                Post = post,
                SingleComment = new SingleCommentVM()
            };

            return View(postVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int? postId, SingleCommentVM singleComment, int? parentId)
        {
            if (postId == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);


            if (post == null)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                Comment comment = new Comment()
                {
                    Username = singleComment.Username,
                    Email = singleComment.Email,
                    Body = singleComment.Body,
                    DateTime = DateTime.Now,
                    Status = CommentStatus.UnClear,
                    PostId = (int)postId,
                    Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };

                if (parentId != null)
                {
                    comment.ParentId = parentId;
                }
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Posts), new { id = postId });
        }
    }
}