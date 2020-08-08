using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Areas.Admin.Models.ViewModels
{
    public class UserLoginVM
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(6, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(30, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(6, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(30, ErrorMessage = ErrMsg.MaxLengthMsg)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
