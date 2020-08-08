using System;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class SurveyComment
    {
        public int Id { get; set; }

        [Display(Name = "نام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Username { get; set; }
        [Display(Name = "ایمیل")]
        [Required(AllowEmptyStrings = false,ErrorMessage = ErrMsg.RequiredMsg)]
        [EmailAddress(ErrorMessage = ErrMsg.RegexMsg)]
        public string Email { get; set; }
        [Display(Name = "نظر")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(1000, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Body { get; set; }

        [Display(Name = "وضعیت")]
        public SurveyCommentStatus Status { get; set; }
        [Display(Name = "آیپی فرستنده")]
        public string Ip { get; set; }
        [Display(Name = "تاریخ")]
        public DateTime DateTime { get; set; }
        [Display(Name = "ویرایش شده")]
        public bool IsEdited { get; set; }
    }

    public enum SurveyCommentStatus
    {
        [Display(Name = "تایید شده")]
        Accepted,
        [Display(Name = "نا مشخص")]
        UnClear,
        [Display(Name = "رد شده")]
        Rejected
    }

}
