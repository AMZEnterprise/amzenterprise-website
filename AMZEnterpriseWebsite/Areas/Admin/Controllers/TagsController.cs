using System;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            //For Sorting Data
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UrlNameSortParm"] = sortOrder == "UrlName" ? "urlname_desc" : "UrlName";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            var tags = from t in _context.Tags
                       select t;



            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            //For Filtering Data
            ViewData["CurrentFilter"] = searchString;


            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                tags = tags.Where(t => t.Name.Contains(searchString) || 
                                       t.UrlName.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "name_desc":
                    tags = tags.OrderByDescending(t => t.Name);
                    break;

                case "UrlName":
                    tags = tags.OrderBy(t => t.UrlName);
                    break;

                case "urlname_desc":
                    tags = tags.OrderByDescending(t => t.UrlName);
                    break;

                case "Date":
                    tags = tags.OrderBy(t => t.DateTime);
                    break;

                case "date_desc":
                    tags = tags.OrderByDescending(t => t.DateTime);
                    break;

                default:
                    tags = tags.OrderBy(t => t.Name);
                    break;

            }


            int pageSize = 10;
            return View(await PaginatedList<Tag>.CreateAsync(tags.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (ModelState.IsValid)
            {

                if (TagNameExists(tag.Id, tag.Name))
                {
                    ModelState.AddModelError(nameof(Tag.Name), "نام تکراری می باشد.");
                    return View(tag);
                }
                if (TagUrlNameExists(tag.Id, tag.UrlName))
                {
                    ModelState.AddModelError(nameof(Tag.UrlName), "نامک تکراری می باشد.");
                    return View(tag);
                }


                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

      
        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(tag);
        }

        // POST: Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tag tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (TagNameExists(tag.Id, tag.Name))
                {
                    ModelState.AddModelError(nameof(Tag.Name), "نام تکراری می باشد.");
                    return View(tag);
                }
                if (TagUrlNameExists(tag.Id, tag.UrlName))
                {
                    ModelState.AddModelError(nameof(Tag.UrlName), "نامک تکراری می باشد.");
                    return View(tag);
                }


                try
                {
                    tag.DateTime = DateTime.Now;
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool TagUrlNameExists(int id, string urlName)
        {
            return _context.Tags.Any(e => e.UrlName == urlName && e.Id != id);
        }

        private bool TagNameExists(int id, string name)
        {
            return _context.Tags.Any(e => e.Name == name && e.Id != id);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }

    }
}
