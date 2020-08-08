using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private FooterVM _footerVm { get; set; }
        public FooterViewComponent(ApplicationDbContext context)
        {
            _context = context;

            var tags = _context.PostAndTags
                .Where(p=>p.Post.Status == PostStatus.Sent)
                .GroupBy(q => q.TagId)
                .OrderByDescending(gp => gp.Count())
                .Take(20)
                .Select(g => g.Key).ToList();

            _footerVm = new FooterVM()
            {
                Setting = _context.Settings.FirstOrDefault(s => s.Id == 1),
                Tags = _context.PostAndTags.Where(p=>tags.Contains(p.TagId)).Select(p=>p.Tag).Distinct()
            };
        }



        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_footerVm);
        }
    }
}
