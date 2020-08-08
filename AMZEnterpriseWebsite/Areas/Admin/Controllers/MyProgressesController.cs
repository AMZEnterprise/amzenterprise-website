using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class MyProgressesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyProgressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/MyProgresses
        public async Task<IActionResult> Index()
        {
            return View(await _context.MyProgresses.ToListAsync());
        }

        // GET: Admin/MyProgresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProgress = await _context.MyProgresses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myProgress == null)
            {
                return NotFound();
            }

            return View(myProgress);
        }

        // GET: Admin/MyProgresses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/MyProgresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MyProgress myProgress)
        {
            if (ModelState.IsValid)
            {

                if (MyProgressTopicExists(myProgress.Id, myProgress.Topic))
                {
                    ModelState.AddModelError(nameof(MyProgress.Topic), "عنوان تکراری می باشد.");
                    return View(myProgress);
                }

                _context.Add(myProgress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myProgress);
        }

        // GET: Admin/MyProgresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProgress = await _context.MyProgresses.FindAsync(id);
            if (myProgress == null)
            {
                return NotFound();
            }
            return View(myProgress);
        }

        // POST: Admin/MyProgresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MyProgress myProgress)
        {
            if (id != myProgress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (MyProgressTopicExists(myProgress.Id, myProgress.Topic))
                {
                    ModelState.AddModelError(nameof(MyProgress.Topic), "عنوان تکراری می باشد.");
                    return View(myProgress);
                }


                try
                {
                    _context.Update(myProgress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyProgressExists(myProgress.Id))
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
            return View(myProgress);
        }

        // GET: Admin/MyProgresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProgress = await _context.MyProgresses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myProgress == null)
            {
                return NotFound();
            }

            return View(myProgress);
        }

        // POST: Admin/MyProgresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myProgress = await _context.MyProgresses.FindAsync(id);
            _context.MyProgresses.Remove(myProgress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool MyProgressTopicExists(int id, string topic)
        {
            return _context.MyProgresses.Any(e => e.Topic == topic && e.Id != id);
        }
        private bool MyProgressExists(int id)
        {
            return _context.MyProgresses.Any(e => e.Id == id);
        }
    }
}
