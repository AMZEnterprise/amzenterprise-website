using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Bogus.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AMZEnterpriseWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Post & Tag => n to n relation
            modelBuilder.Entity<PostAndTag>()
                .HasKey(e => new { e.PostId, e.TagId });

            modelBuilder.Entity<PostAndTag>()
                .HasOne(e => e.Post)
                .WithMany(e => e.PostAndTags)
                .HasForeignKey(e => e.PostId);

            modelBuilder.Entity<PostAndTag>()
                .HasOne(e => e.Tag)
                .WithMany(e => e.PostAndTags)
                .HasForeignKey(e => e.TagId);





            modelBuilder.Entity<ProjectAndMedia>()
                .HasKey(e => new { e.ProjectId, e.MediaId });

            modelBuilder.Entity<ProjectAndMedia>()
                .HasOne(e => e.Project)
                .WithMany(e => e.ProjectAndMedias)
                .HasForeignKey(e => e.ProjectId);

            modelBuilder.Entity<ProjectAndMedia>()
                .HasOne(e => e.Media)
                .WithMany(e => e.ProjectAndMedias)
                .HasForeignKey(e => e.MediaId);


            //Comments Self Referencing
            modelBuilder.Entity<Comment>()
                .HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId);


            //Categories Self Referencing
            modelBuilder.Entity<Category>()
                .HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId);
        }



        //Panel Users
        public DbSet<ApplicationUser> ApplicationUser { get; set; }


        // Blog Tables
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostAndTag> PostAndTags { get; set; }
        public DbSet<Comment> Comments { get; set; }

        //Blog And Project Medias Table
        public DbSet<Media> Medias { get; set; }
        //public DbSet<PostAndMedia> PostAndMedias { get; set; }
        public DbSet<ProjectAndMedia> ProjectAndMedias { get; set; }

        //Projects Info Table
        public DbSet<Project> Projects { get; set; }

        //Project Registers Table
        public DbSet<ProjectRegister> ProjectRegisters { get; set; }


        //Settings
        public DbSet<Setting> Settings { get; set; }

        //My Progress
        public DbSet<MyProgress> MyProgresses { get; set; }

        //Survey Comments
        public DbSet<SurveyComment> SurveyComments { get; set; }

    }
}
