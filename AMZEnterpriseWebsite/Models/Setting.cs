using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AMZEnterpriseWebsite.Consts;

namespace AMZEnterpriseWebsite.Models
{
    public class Setting
    {
        [DefaultValue("1")]
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام سایت")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequiredMsg)]
        [MinLength(3, ErrorMessage = ErrMsg.MinLengthMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLengthMsg)]
        public string SiteName { get; set; }


        [Display(Name = "لوگوی سایت")]
        public string SiteLogo { get; set; }

        //Social Media Links
        [Display(Name = "شبکه اجتماعی 1")]
        public string SocialMedia1 { get; set; }
        [Display(Name = "شبکه اجتماعی 2")]
        public string SocialMedia2 { get; set; }
        [Display(Name = "شبکه اجتماعی 3")]
        public string SocialMedia3 { get; set; }
        [Display(Name = "شبکه اجتماعی 4")]
        public string SocialMedia4 { get; set; }
        [Display(Name = "شبکه اجتماعی 5")]
        public string SocialMedia5 { get; set; }
        [Display(Name = "شبکه اجتماعی 6")]
        public string SocialMedia6 { get; set; }
        [Display(Name = "شبکه اجتماعی 7")]
        public string SocialMedia7 { get; set; }
        [Display(Name = "شبکه اجتماعی 8")]
        public string SocialMedia8 { get; set; }


        //Phone Number
        [Display(Name = "شماره تلفن 1")]
        public string Phone1 { get; set; }
        [Display(Name = "شماره تلفن 2")]
        public string Phone2 { get; set; }
        [Display(Name = "شماره تلفن 3")]
        public string Phone3 { get; set; }


        //Email
        [Display(Name = "ایمیل 1")]
        public string Email1 { get; set; }
        [Display(Name = "ایمیل 2")]
        public string Email2 { get; set; }


        //SEO
        [Display(Name = "عنوان متا صفحه اصلی")]
        public string HomeMetaTitle { get; set; }
        [Display(Name = "کلمات کلیدی متا صفحه اصلی")]
        public string HomeMetaKeywords { get; set; }
        [Display(Name = "توضیحات متا صفحه اصلی")]
        public string HomeMetaDescription { get; set; }


        //Banking Info
        [Display(Name = "شماره حساب بانکی")]
        public string BankId { get; set; }
        [Display(Name = "آدرس کیف پول بیت کوین")]
        public string BitcoinWalletKey { get; set; }
    }
}
