using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Areas.Admin.Models.ViewModels;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Extensions;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Services;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public ManageController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ManageController> logger,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IHostingEnvironment environment,
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _env = environment;
            _emailSender = emailSender;
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult Index()
        {
            return View(_userManager.Users.OrderBy(u => u.DateTime));
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM model)
        {

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count <= 0)
                {
                    ModelState.AddModelError(nameof(UserCreateVM.ProfileImagePath), "تصویر پروفایل را انتخاب کنید.");
                    return View(model);
                }

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Description = model.Description,
                    DateTime = DateTime.Now,
                    EmailConfirmed = true
                };

                var addToIdentityResult = await _userManager.CreateAsync(user, model.Password);

                if (!addToIdentityResult.Succeeded)
                {
                    if (addToIdentityResult.Errors.Any(c => c.Code.Contains("Email")))
                    {
                        ModelState.AddModelError(nameof(UserCreateVM.Email),
                            "ایمیل وارد شده از قبل در سیستم موجود می باشد.");
                    }

                    if (addToIdentityResult.Errors.Any(c => c.Code.Contains("Password")))
                    {
                        ModelState.AddModelError(nameof(UserCreateVM.Password),
                            "کلمه عبور وارد شده معتبر نمی باشد. مقدار دیگری را امتحان کنید.");
                    }

                    if (addToIdentityResult.Errors.Any(c => c.Code.Contains("User")))
                    {
                        ModelState.AddModelError(nameof(UserCreateVM.Username),
                            "نام کاربری وارد شده از قبل در سیستم موجود می باشد.");
                    }

                    return View(model);
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, SD.WriterEndUser);
                if (!addToRoleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error.");
                    return View(model);
                }



                //Copy Uploaded Photo To UsersProfiles Directory

                var folderPath = FileUploadUtil.PathUserProfiles;
                var uploadPath = Path.Combine(_env.WebRootPath, folderPath);

                string newFileName = user.Id + Path.GetExtension(files[0].FileName);

                var fullPath = Path.Combine(uploadPath, newFileName);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {

                    await files[0].CopyToAsync(fs);

                    var currentUser = await _userManager.FindByIdAsync(user.Id);
                    if (currentUser != null)
                    {
                        currentUser.ProfileImagePath = newFileName;
                        await _userManager.UpdateAsync(currentUser);
                    }
                }



                return RedirectToAction(nameof(Index));
            }


            return View(model);
        }




        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVM loginVm, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginVm.Username);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();


                    var result =
                        await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RememberMe, false);

                    if (result.Succeeded)
                    {
                        ViewBag.returnUrl = returnUrl;
                        return Redirect(returnUrl ?? "/");
                    }

                    ModelState.AddModelError(nameof(loginVm.Password), "کلمه عبور اشتباه است.");
                }
                else
                {
                    ModelState.AddModelError(nameof(loginVm.Username), "نام کاربری یافت نشد.");
                }

            }

            ViewBag.returnUrl = returnUrl;
            return View(loginVm);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var editVm = new UserEditVM()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfileImagePath = user.ProfileImagePath,
                DateTime = user.DateTime,
                Description = user.Description
            };

            return View(editVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM model)
        {
            var oldUser = await _userManager.FindByIdAsync(model.Id);

            if (oldUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var removePassResult = await _userManager.RemovePasswordAsync(oldUser);

                    if (!removePassResult.Succeeded)
                    {
                        foreach (IdentityError error in removePassResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

                    oldUser.UserName = model.Username;
                    oldUser.FirstName = model.FirstName;
                    oldUser.LastName = model.LastName;
                    oldUser.Email = model.Email;
                    oldUser.PhoneNumber = model.PhoneNumber;
                    oldUser.Description = model.Description;
                    oldUser.PasswordHash = _passwordHasher.HashPassword(oldUser, model.Password);
                }
                else
                {
                    oldUser.UserName = model.Username;
                    oldUser.FirstName = model.FirstName;
                    oldUser.LastName = model.LastName;
                    oldUser.Email = model.Email;
                    oldUser.PhoneNumber = model.PhoneNumber;
                    oldUser.Description = model.Description;
                }



                var result = await _userManager.UpdateAsync(oldUser);

                if (result.Succeeded)
                {

                    //Change Old Profile Image
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count > 0)
                    {
                        var folderPath = FileUploadUtil.PathUserProfiles;
                        var uploadPath = Path.Combine(_env.WebRootPath, folderPath);
                        string oldFileName = oldUser.ProfileImagePath;
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
                        string newFileName = oldUser.Id + Path.GetExtension(files[0].FileName);
                        var fullPath = Path.Combine(uploadPath, newFileName);
                        using (var fs = new FileStream(fullPath, FileMode.Create))
                        {

                            await files[0].CopyToAsync(fs);

                            oldUser.ProfileImagePath = newFileName;
                            await _userManager.UpdateAsync(oldUser);
                        }

                    }


                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
            }


            return View(model);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .Include(u => u.Posts)
                .ThenInclude(p => p.Comments)
                .ThenInclude(c => c.Children)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var profile = new UserDetailsVM()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfileImagePath = user.ProfileImagePath,
                DateTime = user.DateTime,
                Description = user.Description
            };

            return View(profile);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost]
        public async Task<IActionResult> DeletePOST(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Posts)
                .ThenInclude(p => p.Comments)
                .ThenInclude(c => c.Children)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                await _signInManager.SignOutAsync();
                _logger.LogInformation($"User {user.UserName} deleted successfully.");
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.AdminEndUser + "," + SD.WriterEndUser)]
        public async Task<IActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            var profile = new UserDetailsVM()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfileImagePath = user.ProfileImagePath,
                DateTime = user.DateTime,
                Description = user.Description
            };

            return View(profile);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            //AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}