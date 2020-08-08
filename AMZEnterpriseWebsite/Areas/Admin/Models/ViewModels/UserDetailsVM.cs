using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Areas.Admin.Models.ViewModels
{
    public class UserDetailsVM
    {

        public string Id { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [EmailAddress(ErrorMessage = ErrMsg.RegexMsg)]
        public string Email { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string LastName { get; set; }

        [Display(Name = "تلفن")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MaxLength(20, ErrorMessage = ErrMsg.MaxLengthMsg)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string ProfileImagePath { get; set; }
        [Display(Name = "تاریخ عضویت")]
        public DateTime DateTime { get; set; }
        [Display(Name = "توضیحات")]
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
