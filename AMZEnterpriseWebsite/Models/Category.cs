using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMZEnterpriseWebsite.Consts;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.Models
{
    public class Category
    {
        public int Id { get; set; }


        //Self join (Inheritance)
        [Display(Name = "دسته بندی پدر")]
        public int? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; }

        [Display(Name = "نام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Name { get; set; }
        [Display(Name = "نامک")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string UrlName { get; set; }

        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime DateTime { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
