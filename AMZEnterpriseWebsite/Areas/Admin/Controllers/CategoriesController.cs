using System;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            //For Sorting Data
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UrlNameSortParm"] = sortOrder == "UrlName" ? "urlname_desc" : "UrlName";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


            var cats = _context.Categories.Include(c => c.Parent).AsQueryable();


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
                cats = cats.Where(c => c.Name.Contains(searchString) || 
                                       c.UrlName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    cats = cats.OrderByDescending(c => c.Name);
                    break;
                case "UrlName":
                    cats = cats.OrderBy(c => c.UrlName);
                    break;
                case "urlname_desc":
                    cats = cats.OrderByDescending(c => c.UrlName);
                    break;
                case "Date":
                    cats = cats.OrderBy(c => c.DateTime);
                    break;
                case "date_desc":
                    cats = cats.OrderByDescending(c => c.DateTime);
                    break;
                default:
                    cats = cats.OrderBy(c => c.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Category>.CreateAsync(cats.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if (CategoryNameExists(category.Id, category.Name))
                {
                    ModelState.AddModelError(nameof(Category.Name), "نام تکراری می باشد.");
                    ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
                    return View(category);
                }
                if (CategoryUrlNameExists(category.Id, category.UrlName))
                {
                    ModelState.AddModelError(nameof(Category.UrlName), "نامک تکراری می باشد.");
                    ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
                    return View(category);
                }

                category.DateTime = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (CategoryNameExists(category.Id, category.Name))
                {
                    ModelState.AddModelError(nameof(Category.Name), "نام تکراری می باشد.");
                    ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
                    return View(category);
                }


                if (CategoryUrlNameExists(category.Id, category.UrlName))
                {
                    ModelState.AddModelError(nameof(Category.UrlName), "نامک تکراری می باشد.");
                    ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
                    return View(category);
                }



                try
                {
                    category.DateTime = DateTime.Now;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }


            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c=>c.Children)
                .Where(c=>c.Id == id || c.ParentId == id)
                .ToListAsync();

            _context.Categories.RemoveRange(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryNameExists(int id, string name)
        {
            return _context.Categories.Any(e => e.Name == name && e.Id != id);
        }

        private bool CategoryUrlNameExists(int id, string urlName)
        {
            return _context.Categories.Any(e => e.UrlName == urlName && e.Id != id);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
