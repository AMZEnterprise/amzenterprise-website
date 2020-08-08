using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [Required(AllowEmptyStrings =false,ErrorMessage =ErrMsg.RequiredMsg)]
        [MinLength(3,ErrorMessage =ErrMsg.MinLengthMsg)]
        [MaxLength(100,ErrorMessage =ErrMsg.MaxLengthMsg)]
        public string Name { get; set; }
        [Display(Name = "نامک")]
        [Required(AllowEmptyStrings =false,ErrorMessage =ErrMsg.RequiredMsg)]
        [MinLength(3,ErrorMessage =ErrMsg.MinLengthMsg)]
        [MaxLength(100,ErrorMessage =ErrMsg.MaxLengthMsg)]
        public string UrlName { get; set; }

        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime DateTime { get; set; }

        public ICollection<PostAndTag> PostAndTags { get; set; }
    }
}
