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
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ProjectTypeSortParm"] = sortOrder == "ProjectType" ? "projecttype_desc" : "ProjectType";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


            var projects = from proj in _context.Projects
                           select proj;

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
                projects = projects.Where(p => p.Name.Contains(searchString) ||
                                               p.DateTime.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "ProjectType":
                    projects = projects.OrderBy(p => p.ProjectType);
                    break;
                case "projecttype_desc":
                    projects = projects.OrderByDescending(p => p.ProjectType);
                    break;
                case "Date":
                    projects = projects.OrderBy(p => p.DateTime);
                    break;
                case "date_desc":
                    projects = projects.OrderByDescending(p => p.DateTime);
                    break;
                default:
                    projects = projects.OrderBy(p => p.Name);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, string mediasIds)
        {

            if (mediasIds == null)
            {
                TempData["ErrorMsg"] = "هیچ رسانه ای انتخاب نشده است. حداقل یک تصویر باید به عنوان رسانه انتخاب کنید.";

                return View(project);
            }

            if (ModelState.IsValid)
            {
                project.DateTime = DateTime.Now;

                string[] mediasIdArr = mediasIds.Split(',');

                if (mediasIdArr != null)
                {
                    mediasIdArr = mediasIdArr.Distinct().ToArray();

                    project.ProjectAndMedias = new List<ProjectAndMedia>();

                    for (int i = 0; i < mediasIdArr.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(mediasIdArr[i]))
                        {
                            ProjectAndMedia projectMedia = new ProjectAndMedia()
                            {
                                Project = project,
                                Media = _context.Medias.First(t => t.Id == Convert.ToInt32(mediasIdArr[i]))
                            };
                            project.ProjectAndMedias.Add(projectMedia);
                        }
                    }
                }


                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, string mediasIds)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    //Remove old medias from project
                    _context.ProjectAndMedias.RemoveRange(_context.ProjectAndMedias.Where(pt => pt.ProjectId == id));

                    //Add new medias to project
                    string[] mediasIdArr = mediasIds.Split(',');

                    if (mediasIdArr != null)
                    {
                        mediasIdArr = mediasIdArr.Distinct().ToArray();

                        project.ProjectAndMedias = new List<ProjectAndMedia>();

                        for (int i = 0; i < mediasIdArr.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(mediasIdArr[i]))
                            {
                                ProjectAndMedia projectMedia = new ProjectAndMedia()
                                {
                                    Project = project,
                                    Media = _context.Medias.First(t => t.Id == Convert.ToInt32(mediasIdArr[i]))
                                };
                                project.ProjectAndMedias.Add(projectMedia);
                            }
                        }
                    }

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
