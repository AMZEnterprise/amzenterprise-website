using System;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class ProjectRegister
    {
        public int Id { get; set; }
        [Display(Name = "عنوان یا موضوع پروژه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Title { get; set; }
        [Display(Name = "نام و نام خانوادگی")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string FullName { get; set; }
        [Display(Name = "نوع پروژه")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        public ProjectType ProjectType { get; set; }
        [Display(Name = "شماره تلفن همراه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MaxLength(11, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Phone { get; set; }
        [Display(Name = "آدرس ایمیل یا اکانت تلگرام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string SocialMediaAccount { get; set; }
        [Display(Name = "توضیحات")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(2000, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Description { get; set; }

        [Display(Name = "تاریخ سفارش")]
        public DateTime DateTime { get; set; }

        [Display(Name = "تاریخ انجام")]
        public DateTime? DoneDate { get; set; }

        [Display(Name = "وضعیت پروژه")]
        public ProjectStatus Status { get; set; }
    }


    public enum ProjectType
    {
        [Display(Name = "دسکتاپ")]
        Desktop,
        [Display(Name = "وبسایت")]
        Website,
        [Display(Name = "طراحی قالب وبسایت")]
        TemplateDesign,
        [Display(Name = "سئو و تولید محتوا")]
        Seo,
        [Display(Name = "بهینه سازی")]
        Optimization,
        [Display(Name = "آموزش")]
        Teaching,
        [Display(Name = "اپلیکیشن موبایل")]
        MobileApplication,
        [Display(Name = "دیگر موارد")]
        Other
    }

    public enum ProjectStatus
    {
        [Display(Name = "انجام شده")]
        Done,
        [Display(Name = "نا مشخص")]
        UnClear,
        [Display(Name = "رد شده")]
        Rejected
    }
}
