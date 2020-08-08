using System;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class SurveyCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _accessor;
        public SurveyCommentsController(ApplicationDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        // GET: Admin/SurveyComments
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            //OrderBy Latest Date
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            var surveyComments = _context.SurveyComments.AsQueryable();

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();

                surveyComments = surveyComments
                    .Where(sc => sc.Body.Contains(searchString) ||
                                 sc.Username.Contains(searchString) ||
                                 sc.Ip.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Date":
                    surveyComments = surveyComments.OrderBy(sc => sc.DateTime);
                    break;
                case "Status":
                    surveyComments = surveyComments.OrderBy(sc => sc.Status);
                    break;
                case "status_desc":
                    surveyComments = surveyComments.OrderByDescending(sc => sc.Status);
                    break;

                default:
                    surveyComments = surveyComments.OrderByDescending(sc => sc.DateTime);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<SurveyComment>
                .CreateAsync(
                    surveyComments.AsNoTracking(),
                    pageNumber ?? 1,
                    pageSize)
            );
        }

        // GET: Admin/SurveyComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyComment = await _context.SurveyComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surveyComment == null)
            {
                return NotFound();
            }

            return View(surveyComment);
        }

        // GET: Admin/SurveyComments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SurveyComments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyComment surveyComment)
        {
            if (ModelState.IsValid)
            {
                //get client ip address
                surveyComment.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                surveyComment.Status = SurveyCommentStatus.Accepted;
                surveyComment.DateTime = DateTime.Now;
                surveyComment.IsEdited = false;

                _context.Add(surveyComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surveyComment);
        }

        // GET: Admin/SurveyComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyComment = await _context.SurveyComments.FindAsync(id);
            if (surveyComment == null)
            {
                return NotFound();
            }
            return View(surveyComment);
        }

        // POST: Admin/SurveyComments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyComment surveyComment)
        {
            if (id != surveyComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                surveyComment.IsEdited = true;
                
                try
                {
                    _context.Update(surveyComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyCommentExists(surveyComment.Id))
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
            return View(surveyComment);
        }


        // GET: Admin/SurveyComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyComment = await _context.SurveyComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surveyComment == null)
            {
                return NotFound();
            }

            return View(surveyComment);
        }

        // POST: Admin/SurveyComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveyComment = await _context.SurveyComments.FindAsync(id);
            _context.SurveyComments.Remove(surveyComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyCommentExists(int id)
        {
            return _context.SurveyComments.Any(e => e.Id == id);
        }
    }
}
