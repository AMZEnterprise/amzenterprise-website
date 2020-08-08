using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [Required(AllowEmptyStrings =false,ErrorMessage =ErrMsg.RequiredMsg)]
        [MinLength(3 , ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100 , ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(100, ErrorMessage = ErrMsg.MinLengthMsg)]
        public string Description { get; set; }

        [Display(Name = "نوع پروژه")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        public ProjectType ProjectType { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime DateTime { get; set; }
        
        [Display(Name = "رسانه ها")]
        public virtual ICollection<ProjectAndMedia> ProjectAndMedias { get; set; }

    }

   
}
