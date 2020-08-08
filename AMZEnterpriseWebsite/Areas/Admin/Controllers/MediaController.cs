using System;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class MediaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public MediaController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _env = environment;
        }


        // GET: Media
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SizeSortParm"] = sortOrder == "Size" ? "size_desc" : "Size";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            

            var medias = from m in _context.Medias
                         select m;

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

                medias = medias.Where(m => m.Name.Contains(searchString) ||
                                           m.Size.ToString().Contains(searchString));
            }


            switch (sortOrder)
            {
                case "name_desc":
                    medias = medias.OrderByDescending(m => m.Name);
                    break;
                case "Size":
                    medias = medias.OrderBy(m => m.Size);
                    break;
                case "size_desc":
                    medias = medias.OrderByDescending(m => m.Size);
                    break;
                case "Date":
                    medias = medias.OrderBy(m => m.DateTime);
                    break;
                case "date_desc":
                    medias = medias.OrderByDescending(m => m.DateTime);
                    break;
                
                default:
                    medias = medias.OrderBy(m => m.Name);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<Media>.CreateAsync(medias.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Media/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var media = await _context.Medias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }


            string fileName = media.Name;
            ViewData["MediaFileType"] = FileUploadUtil.GetFileType(fileName);
            ViewData["MediaFilePath"] = "/" + FileUploadUtil.GetFilePath(fileName) + "/" + fileName;

            return View(media);
        }

        // GET: Media/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Media/Create
        [HttpPost, ActionName(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {

            var files = HttpContext.Request.Form.Files;
            var extension = Path.GetExtension(files[0].FileName);
            var folderPath = FileUploadUtil.GetFilePath(extension);
            var uploadPath = Path.Combine(_env.WebRootPath, folderPath);


            var fullPath = Path.Combine(uploadPath, files[0].FileName);
            if (files.Count > 0)
            {
                string fileName = files[0].FileName;

                MediaFileType mediaType = FileUploadUtil.GetFileType(fileName);

                var isFileExist = _context.Medias.FirstOrDefault
                                      (x => x.Name == files[0].FileName && x.MediaFileType == mediaType) != null;

                if (!isFileExist)
                {
                    using (var fs = new FileStream(fullPath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(fs);

                        _context.Medias.Add(new Media()
                        {
                            DateTime = DateTime.Now,
                            Name = fileName,
                            Size = files[0].Length,
                            MediaFileType = FileUploadUtil.GetFileType(fileName)
                        });
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Create));
        }


        // GET: Media/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var media = await _context.Medias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }

            return View(media);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var media = await _context.Medias.FindAsync(id);


            var folderPath = FileUploadUtil.GetFilePath(media.MediaFileType);
            var uploadPath = Path.Combine(_env.WebRootPath, folderPath);
            var fullPath = Path.Combine(uploadPath, media.Name);


            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            _context.Medias.Remove(media);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MediaExists(int id)
        {
            return _context.Medias.Any(e => e.Id == id);
        }
    }
}
