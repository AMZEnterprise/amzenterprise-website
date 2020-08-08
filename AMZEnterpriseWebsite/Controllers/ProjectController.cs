using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;

namespace AMZEnterpriseWebsite.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Breadcrumb("پروژه ها")]
        public async Task<IActionResult> Index(ProjectType? currentFilter)
        {

            IEnumerable<Project> projects = new List<Project>();
            switch (currentFilter)
            {
                case ProjectType.Desktop:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.Desktop)
                        .Include(p=>p.ProjectAndMedias)
                        .ThenInclude(pm=>pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.MobileApplication:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.MobileApplication)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.Optimization:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.Optimization)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.Seo:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.Seo)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.Teaching:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.Teaching)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.TemplateDesign:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.TemplateDesign)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

                case ProjectType.Website:
                    projects =await _context.Projects
                        .Where(p => p.ProjectType == ProjectType.Website)
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;


                default:
                    projects =await _context.Projects
                        .Include(p => p.ProjectAndMedias)
                        .ThenInclude(pm => pm.Media)
                        .AsQueryable()
                        .ToListAsync();
                    break;

            }

            if (currentFilter != null)
            {
                ViewData["filter"] = currentFilter;
            }
            else
            {
                ViewData["filter"] = ProjectType.Other;
            }

            

            return View(projects);
        }

        [Breadcrumb("ViewData.BreadcrumbTitle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p=>p.ProjectAndMedias)
                .ThenInclude(pm=>pm.Media)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Breadcrumb("سفارش پروژه")]
        public IActionResult Registration()
        {

            ViewData["IsSuccess"] = true;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("سفارش پروژه")]
        public async Task<IActionResult> Registration(ProjectRegister projectRegister)
        {
            if (ModelState.IsValid)
            {
                projectRegister.DateTime = DateTime.Now;
                projectRegister.DoneDate = null;

                projectRegister.Status = ProjectStatus.UnClear;

                _context.Add(projectRegister);
                await _context.SaveChangesAsync();

                TempData["IsSuccess"] = true;

                return RedirectToAction(nameof(Registration));
            }
            return View(projectRegister);
        }
    }
}