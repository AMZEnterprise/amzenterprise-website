using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class NavbarLogoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NavbarLogoViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var setting = await _context.Settings.FindAsync(1);
            string logoPath = @"\img\" + setting.SiteLogo;
            return View(model:logoPath);
        }
    }
}
