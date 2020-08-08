using System;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;

namespace AMZEnterpriseWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _accessor;
        private AboutVM _aboutVm { get; set; }
        private HomeVM _homeVm { get; set; }
        public HomeController(ApplicationDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _aboutVm = new AboutVM()
            {
                MyProgresses = _context.MyProgresses,
                Setting = _context.Settings.FirstOrDefault(s => s.Id == 1)
            };
            _homeVm = new HomeVM()
            {
                Setting = _context.Settings.FirstOrDefault(s => s.Id == 1),

                Projects = _context.Projects
                    .Include(p => p.ProjectAndMedias)
                    .ThenInclude(pm => pm.Media)
                    .OrderBy(p => p.DateTime)
                    .Take(6),

                SurveyComments = _context.SurveyComments
                    .OrderByDescending(s=>s.DateTime)
                    .Where(s=>s.Status == SurveyCommentStatus.Accepted)
                    .Take(5),

                Posts = _context.Posts
                    .Include(p => p.Comments)
                    .Include(p=>p.User)
                    .Include(p=>p.Media)
                    .OrderByDescending(p=>p.DateTime)
                    .Take(2)
            };
            _accessor = accessor;
        }

      
        [DefaultBreadcrumb("صفحه اصلی")]
        public IActionResult Index()
        {
            return View(_homeVm);
        }


        [Breadcrumb("تماس با من")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("تماس با من")]
        public async Task<IActionResult> Contact(SurveyComment surveyComment)
        {
            if (ModelState.IsValid)
            {
                surveyComment.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                surveyComment.Status = SurveyCommentStatus.UnClear;
                surveyComment.DateTime = DateTime.Now;
                surveyComment.IsEdited = false;

                _context.Add(surveyComment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Contact));
            }

            return View(surveyComment);
        }
        [Breadcrumb("درباره")]
        public IActionResult About()
        {
            return View(_aboutVm);
        }

    }
}