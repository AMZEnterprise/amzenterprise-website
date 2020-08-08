using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class MyProgress
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string Topic { get; set; }
        [Display(Name = "درصد پیشرفت")]
        [Required(AllowEmptyStrings = false,ErrorMessage = ErrMsg.RequiredMsg)]
        [Range(0,100,ErrorMessage = ErrMsg.RangeMsg)]
        public int ProgressValuePercentage { get; set; }
    }
}
