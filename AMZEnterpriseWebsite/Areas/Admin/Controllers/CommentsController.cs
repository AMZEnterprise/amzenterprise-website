using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.DTOs;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class CommentsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["PostTitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "posttitle_desc" : "";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            IQueryable<Comment> comments = null;

            //if user is admin load all comments
            if (HttpContext.User.IsInRole(SD.AdminEndUser))
            {
                comments = _context.Comments
                    .Include(c => c.Post)
                    .AsQueryable();
            }
            else
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                comments = _context.Comments
                    .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                    .Where(c => c.Post.User == user)
                    .AsQueryable();
            }





            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!(String.IsNullOrEmpty(searchString)))
            {
                searchString = searchString.Trim();

                comments = comments.Where(c =>
                    c.Post.Title.Contains(searchString) || c.Body.Contains(searchString) ||
                    c.Username.Contains(searchString) || c.Ip.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "posttitle_desc":
                    comments = comments.OrderByDescending(c => c.Post.Title);
                    break;
                case "Status":
                    comments = comments.OrderBy(c => c.Status);
                    break;
                case "status_desc":
                    comments = comments.OrderByDescending(c => c.Status);
                    break;
                case "Date":
                    comments = comments.OrderBy(c => c.DateTime);
                    break;
                case "date_desc":
                    comments = comments.OrderByDescending(c => c.DateTime);
                    break;

                default:
                    comments = comments.OrderBy(c => c.Post.Title);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<Comment>.CreateAsync(comments.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            Comment comment = null;


            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                comment = await _context.Comments
                    .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Post.User == user);
            }
            else
            {
                comment = await _context.Comments
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }



            if (comment == null)
            {
                return NotFound();
            }

            ViewData["PostTitle"] = comment.Post.Title;

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts
                                                    .Where(p => p.Status == PostStatus.Sent),
                                                "Id",
                                                "Title");
            return View();
        }


        public IActionResult GetParentComments(int? postId, string q)
        {
            if (postId == null)
            {
                return NotFound();
            }


            var comments = _context.Posts
                .Where(c => c.Id == postId)
                .SelectMany(c => c.Comments);


            var commentList = new List<Select2DTO>();
            foreach (var comment in comments)
            {
                commentList.Add(new Select2DTO()
                {
                    id = comment.Id,
                    text = comment.Body
                });
            }


            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                string qLower = q.ToLower();
                commentList = commentList.Where(x => x.text.ToLower().Contains(qLower)).ToList();
            }

            return Json(new { items = commentList });
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DateTime = DateTime.Now;
                comment.Status = CommentStatus.Accepted;
                comment.User = await _userManager.GetUserAsync(HttpContext.User);

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment comment = null;


            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                comment = await _context.Comments
                    .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Post.User == user);
            }
            else
            {
                comment = await _context.Comments
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }







            if (comment == null)
            {
                return NotFound();
            }

            ViewData["PostTitle"] = comment.Post.Title;
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.IsEdited = true;

                    _context.Update(comment);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            Comment comment = null;


            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                comment = await _context.Comments
                    .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Post.User == user);
            }
            else
            {
                comment = await _context.Comments
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }


            if (comment == null)
            {
                return NotFound();
            }

            ViewData["PostTitle"] = comment.Post.Title;
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fatherComment = await _context.Comments.FindAsync(id);

            if (fatherComment == null)
            {
                return NotFound();
            }

            var childComments = await _context.Comments
                .Where(c => c.Id == id || c.ParentId == id)
                .Include(c => c.Children)
                .SelectMany(c => c.Children)
                .ToListAsync();

            if (childComments == null)
            {
                return NotFound();
            }

            childComments.Add(fatherComment);
            _context.Comments.RemoveRange(childComments);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }









        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }



        public async Task<IActionResult> AcceptComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Comment> comments = new List<Comment>();

            var result = _context.Comments
                .Include(c => c.Parent)
                .First(c => c.Id == id);

            comments.Add(result);

            while (result != null)
            {
                result = _context.Comments
                    .Where(c => c.Id == result.Id)
                    .Include(c => c.Parent)
                    .FirstOrDefault()?.Parent;
                if (result != null)
                {
                    comments.Add(result);
                }
            }



            foreach (var comment in comments)
            {
                comment.Status = CommentStatus.Accepted;
            }




            _context.Comments.UpdateRange(comments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> RejectComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fatherComment = await _context.Comments.FindAsync(id);

            if (fatherComment == null)
            {
                return NotFound();
            }

            var childComments = await _context.Comments
                .Where(c => c.Id == id || c.ParentId == id)
                .Include(c => c.Children)
                .SelectMany(c => c.Children)
                .ToListAsync();

            if (childComments == null)
            {
                return NotFound();
            }

            childComments.Add(fatherComment);


            foreach (var comment in childComments)
            {
                comment.Status = CommentStatus.Rejected;
            }


            _context.Comments.UpdateRange(childComments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
