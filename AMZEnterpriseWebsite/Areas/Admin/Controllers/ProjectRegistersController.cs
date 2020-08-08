using System;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class ProjectRegistersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectRegistersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProjectRegisters
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["ProjectTypeSortParm"] = sortOrder == "ProjectType" ? "projecttype_desc" : "ProjectType";
            ViewData["ProjectStatusParm"] = sortOrder == "ProjectStatus" ? "projectstatus_desc" : "ProjectStatus";


            var projectRegisters = from pr in _context.ProjectRegisters
                                   select pr;

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

                projectRegisters = projectRegisters
                    .Where(pr => pr.Description.Contains(searchString)
                                 || pr.Title.Contains(searchString) || 
                                 pr.DateTime.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Date":
                    projectRegisters = projectRegisters.OrderBy(pr => pr.DateTime);
                    break;
                case "ProjectType":
                    projectRegisters = projectRegisters.OrderBy(pr => pr.ProjectType);
                    break;
                case "Title":
                    projectRegisters = projectRegisters.OrderBy(pr => pr.Title);
                    break;
                case "title_desc":
                    projectRegisters = projectRegisters.OrderByDescending(pr => pr.Title);
                    break;
                case "projecttype_desc":
                    projectRegisters = projectRegisters.OrderByDescending(pr => pr.ProjectType);
                    break;
                case "ProjectStatus":
                    projectRegisters = projectRegisters.OrderBy(pr => pr.Status);
                    break;
                case "projectstatus_desc":
                    projectRegisters = projectRegisters.OrderByDescending(pr => pr.Status);
                    break;

                default:
                    projectRegisters = projectRegisters.OrderByDescending(pr => pr.DateTime);
                    break;

            }

            int pageSize = 10;

            return View(await PaginatedList<ProjectRegister>.CreateAsync(projectRegisters.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Admin/ProjectRegisters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectRegister = await _context.ProjectRegisters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectRegister == null)
            {
                return NotFound();
            }

            return View(projectRegister);
        }

        // GET: Admin/ProjectRegisters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProjectRegisters/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectRegister projectRegister)
        {
            if (ModelState.IsValid)
            {
                projectRegister.DateTime = DateTime.Now;
                projectRegister.DoneDate = null;

                projectRegister.Status = ProjectStatus.UnClear;

                _context.Add(projectRegister);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectRegister);
        }

        // GET: Admin/ProjectRegisters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectRegister = await _context.ProjectRegisters.FindAsync(id);
            if (projectRegister == null)
            {
                return NotFound();
            }
            return View(projectRegister);
        }

        // POST: Admin/ProjectRegisters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProjectRegister projectRegister)
        {
            if (id != projectRegister.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (projectRegister.Status == ProjectStatus.Done)
                    {
                        projectRegister.DoneDate = DateTime.Now;
                    }

                    _context.Update(projectRegister);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectRegisterExists(projectRegister.Id))
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
            return View(projectRegister);
        }

        // GET: Admin/ProjectRegisters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectRegister = await _context.ProjectRegisters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectRegister == null)
            {
                return NotFound();
            }

            return View(projectRegister);
        }

        // POST: Admin/ProjectRegisters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectRegister = await _context.ProjectRegisters.FindAsync(id);
            _context.ProjectRegisters.Remove(projectRegister);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectRegisterExists(int id)
        {
            return _context.ProjectRegisters.Any(e => e.Id == id);
        }
    }
}
