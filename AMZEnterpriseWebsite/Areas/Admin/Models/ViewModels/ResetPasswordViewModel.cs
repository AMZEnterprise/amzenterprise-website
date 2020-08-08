using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Areas.Admin.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [EmailAddress(ErrorMessage = ErrMsg.RegexMsg)]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(6, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(30, ErrorMessage = ErrMsg.MaxLengthMsg)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = ErrMsg.ComapareMsg)]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
