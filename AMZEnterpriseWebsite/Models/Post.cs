using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(AllowEmptyStrings =false,ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(100, ErrorMessage = ErrMsg.MinLengthMsg)]
        [DataType(DataType.MultilineText)]  
        public string Body { get; set; }
        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        public PostStatus Status { get; set; }
        [Display(Name = "تاریخ")]
        public DateTime DateTime { get; set; }
        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime LastEditDate { get; set; }


        [Required(AllowEmptyStrings = false,ErrorMessage = ErrMsg.RequiredMsg)]
        [Display(Name = "دسته بندی")]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }


        [Display(Name = "نویسنده")]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [Display(Name = "تصویر اصلی مطلب")]
        public int MediaId { get; set; }
        [ForeignKey(nameof(MediaId))]
        public virtual Media Media { get; set; }



        [Display(Name = "فعالسازی بخش نظرات")]
        public bool IsCommentsOn { get; set; }
        
        [Display(Name = "نظرات")]
        public ICollection<Comment> Comments { get; set; }
        [Display(Name = "برچسب ها")]
        public virtual ICollection<PostAndTag> PostAndTags { get; set; }



    }


    public enum PostStatus
    {
        [Display(Name = "انتشار")]
        Sent ,

        [Display(Name = "انتشار با زمانبندی")]
        Pending
    }
    
}
