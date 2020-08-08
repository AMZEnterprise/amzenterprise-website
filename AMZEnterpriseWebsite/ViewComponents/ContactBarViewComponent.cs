using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class ContactBarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ContactBarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Id == 1);

            return View(setting);
        }
    }
}
