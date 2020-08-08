using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class Media
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Name { get; set; }
        [Display(Name = "نوع فایل")]
        public MediaFileType MediaFileType { get; set; }
        [Display(Name = "اندازه")]
        public double Size { get; set; }
        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime DateTime { get; set; }


        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<ProjectAndMedia> ProjectAndMedias { get; set; }

    }


    public enum MediaFileType
    {
        Img,
        Video,
        Sound,
        Other
    }
}
