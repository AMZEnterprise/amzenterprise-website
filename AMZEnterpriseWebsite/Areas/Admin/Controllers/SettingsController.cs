using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public SettingsController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _env = environment;
        }

        // GET: Settings
        public IActionResult Index()
        {
            var setting = _context.Settings.First(s => s.Id == 1);
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? id, Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Change Old Website Image
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count > 0)
                    {
                        var folderPath = "img/";
                        var uploadPath = Path.Combine(_env.WebRootPath, folderPath);
                        string oldFileName = setting.SiteLogo;
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            var oldFilePath = Path.Combine(uploadPath, oldFileName);

                            try
                            {

                                //Remove Old Profile Image
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }

                            }
                            catch
                            {
                            }
                        }


                        //Copying New Profile Image
                        string newFileName = Guid.NewGuid() + Path.GetExtension(files[0].FileName);
                        var fullPath = Path.Combine(uploadPath, newFileName);
                        using (var fs = new FileStream(fullPath, FileMode.Create))
                        {
                            await files[0].CopyToAsync(fs);
                            setting.SiteLogo = newFileName;
                        }

                    }


                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
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
            return View(setting);

        }
        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
