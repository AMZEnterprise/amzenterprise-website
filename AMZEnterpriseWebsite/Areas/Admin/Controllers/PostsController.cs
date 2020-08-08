using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.DTOs;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PostsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["EditDateSortParm"] = sortOrder == "EditDate" ? "editdate_desc" : "EditDate";



            IQueryable<Post> posts = null;
            //if user is admin load all posts
            if (HttpContext.User.IsInRole(SD.AdminEndUser))
            {
                posts = _context.Posts.
                    Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(p => p.Tag)
                    .AsQueryable();
            }
            else
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                posts = _context.Posts.
                    Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(p => p.Tag)
                    .Include(p => p.User)
                    .Where(p => p.User == user)
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

                posts = posts.Where(p => p.Title.Contains(searchString) ||
                                         p.Category.Name.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "title_desc":
                    posts = posts.OrderByDescending(p => p.Title);
                    break;

                case "Category":
                    posts = posts.OrderBy(p => p.Category.Name);
                    break;
                case "category_desc":
                    posts = posts.OrderByDescending(p => p.Category.Name);
                    break;

                case "Date":
                    posts = posts.OrderBy(p => p.DateTime);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(p => p.DateTime);
                    break;

                case "Status":
                    posts = posts.OrderBy(p => p.Status);
                    break;
                case "status_desc":
                    posts = posts.OrderByDescending(p => p.Status);
                    break;

                case "EditDate":
                    posts = posts.OrderBy(p => p.LastEditDate);
                    break;
                case "editdate_desc":
                    posts = posts.OrderByDescending(p => p.LastEditDate);
                    break;

                default:
                    posts = posts.OrderBy(p => p.Title);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = null;
            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                post = await _context.Posts
                    .Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(pt => pt.Tag)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id && p.User == user);
            }
            else
            {
                post = await _context.Posts
                    .Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(pt => pt.Tag)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (post == null)
            {
                return NotFound();
            }



            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, string tags, string pendingdate)
        {

            if (ModelState.IsValid)
            {
                //Set Writer Id
                var user = await _userManager.GetUserAsync(HttpContext.User);
                post.UserId = user.Id;


                post.DateTime = DateTime.Now;


                //Set Last Edit Date
                if (post.Status == PostStatus.Sent)
                {
                    post.LastEditDate = DateTime.Now;
                }
                else
                {
                    post.LastEditDate = DateTime.Parse(pendingdate);
                }


                if (!string.IsNullOrEmpty(tags))
                {
                    string[] tagsArr = tags.Split(',');

                    for (int i = 0; i < tagsArr.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(tagsArr[i]))
                        {
                            PostAndTag posttag = new PostAndTag()
                            {
                                Post = post,
                                Tag = _context.Tags.First(t => t.Id == Convert.ToInt32(tagsArr[i]))
                            };
                            post.PostAndTags.Add(posttag);
                        }
                    }
                }






                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            if (post.MediaId == 0)
            {
                TempData["ErrorMsg"]= "تصویر اصلی مطلب را انتخاب کنید.";
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }


        //Return Tags For Tag Input
        public JsonResult GetAllTags(string q)
        {
            _context.Tags.ToList();

            var tags = new List<Select2DTO>();
            foreach (var tag in _context.Tags)
            {
                tags.Add(new Select2DTO()
                {
                    id = tag.Id,
                    text = tag.Name
                });
            }



            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                string qLower = q.ToLower();
                tags = tags.Where(x => x.text.ToLower().StartsWith(qLower)).ToList();
            }
            return Json(new { items = tags });
        }

        public IActionResult GetPostTags(string postId)
        {
            if (postId == null)
            {
                return NotFound();
            }

            var postTags = _context.PostAndTags
                .Include(t => t.Tag)
                .Where(pt => pt.PostId == int.Parse(postId));

            var tags = new List<Select2DTO>();
            foreach (var tag in postTags)
            {
                tags.Add(new Select2DTO()
                {
                    id = tag.TagId,
                    text = tag.Tag.Name
                });
            }


            return Json(new { items = tags });
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = null;
            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                post = await _context.Posts.Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id && p.User == user);
            }
            else
            {
                post = await _context.Posts.FindAsync(id);
            }




            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post, string tags, string pendingdate)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    //Set last edit datetime
                    if (post.Status == PostStatus.Sent)
                    {
                        post.LastEditDate = DateTime.Now;
                    }
                    else
                    {
                        post.LastEditDate = DateTime.Parse(pendingdate);
                    }


                    //Remove old tags
                    _context.PostAndTags.RemoveRange(_context.PostAndTags.Where(pt => pt.PostId == id));

                    //Add new tags

                    if (!string.IsNullOrEmpty(tags))
                    {
                        string[] tagsArr = tags.Split(',');

                        for (int i = 0; i < tagsArr.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(tagsArr[i]))
                            {
                                PostAndTag posttag = new PostAndTag()
                                {
                                    Post = post,
                                    Tag = _context.Tags.First(t => t.Id == Convert.ToInt32(tagsArr[i]))
                                };
                                post.PostAndTags.Add(posttag);
                            }
                        }
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = null;
            if (HttpContext.User.IsInRole(SD.WriterEndUser))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                post = await _context.Posts
                    .Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(pt => pt.Tag)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id && p.User == user);
            }
            else
            {
                post = await _context.Posts
                    .Include(p => p.Category)
                    .Include(p => p.PostAndTags)
                    .ThenInclude(pt => pt.Tag)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }



            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

    }
}
