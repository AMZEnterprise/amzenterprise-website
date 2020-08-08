using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models.ViewModels
{
    public class SingleCommentVM
    {
        public int Id { get; set; }

        //Self join (Inheritance)
        [Display(Name = "نظر پدر")]
        public int? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Children { get; set; }

        //Admin Comment
        [Display(Name = "نویسنده")]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [Display(Name = "مطلب")]
        [Required]
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

      
        [Display(Name = "نام")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        public string Username { get; set; }
       
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = ErrMsg.RequiredMsg)]
        [EmailAddress(ErrorMessage = ErrMsg.RegexMsg)]
        public string Email { get; set; }
        [Display(Name = "نظر")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(1000, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Body { get; set; }
        [Display(Name = "وضعیت")]
        public CommentStatus Status { get; set; }
        [Display(Name = "آیپی فرستنده")]
        public string Ip { get; set; }
        [Display(Name = "تاریخ")]
        public DateTime DateTime { get; set; }

        [Display(Name = "ویرایش شده")]
        public bool IsEdited { get; set; }
    }
}
