using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.ViewComponents
{
    public class SurveyCommentsSliderViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public SurveyCommentsSliderViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var surveyComments = _context.SurveyComments
                .OrderByDescending(sc => sc.DateTime)
                .Where(sc => sc.Status == SurveyCommentStatus.Accepted)
                .Take(5);

            return View(surveyComments);
        }
    }
}
