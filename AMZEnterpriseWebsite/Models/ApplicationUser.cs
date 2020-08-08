using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Consts;
using Microsoft.AspNetCore.Identity;

namespace AMZEnterpriseWebsite.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateTime { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }



        //User Profile Picture
        public string ProfileImagePath { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
