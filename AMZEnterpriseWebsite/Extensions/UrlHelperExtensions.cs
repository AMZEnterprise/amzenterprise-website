using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(ManageController.ResetPassword),
                controller: "Manage",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
