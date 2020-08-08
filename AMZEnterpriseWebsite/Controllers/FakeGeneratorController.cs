using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AMZEnterpriseWebsite.Controllers
{
    public class FakeGeneratorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FakeGeneratorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            try
            {
                AddFakeTags(100);
                Console.WriteLine("Tags Added.");

                AddFakeCategories(10);
                Console.WriteLine("Categories Added.");

                AddFakeMedias(100);
                Console.WriteLine("Medias Added.");

                AddFakePosts(20);
                Console.WriteLine("Posts Added.");

                AddFakeComments(100);
                Console.WriteLine("Comments Added.");

                AddFakeProjects(100);
                Console.WriteLine("Projects Added.");

                AddFakeProjectRegisters(100);
                Console.WriteLine("Project Registers Added.");

                AddFakeMyProgresses(10);
                Console.WriteLine("MyProgresses Added.");


                AddFakeSurveyComments(30);
                Console.WriteLine("Survey Comments Added.");

                //Success
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=> Fake Data Added Successfully.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oops , there was an error ...");
                Console.WriteLine("Message : " + ex.Message);
                Console.WriteLine("Source : " + ex.Source);
                Console.WriteLine("******");
                Console.WriteLine("Raw Data :" + ex);
            }

            return Redirect("/");
        }



        public void AddFakeTags(int count)
        {

            List<Tag> tags = new List<Tag>();


            for (int i = 0; i < count; i++)
            {
                var myData = new Faker<Tag>()
                    .RuleFor(u => u.Name, (f, u) => f.Internet.UserName(u.Name))
                    .RuleFor(u => u.UrlName, (f, u) => f.Internet.Url())
                    .RuleFor(u => u.DateTime, u => u.Date.Past());



                myData.Generate();
                tags.Add(myData);
            }



            _context.Tags.AddRange(tags);
            _context.SaveChanges();
        }

        public void AddFakeCategories(int count)
        {
            List<Category> cats = new List<Category>();

            for (int i = 0; i < count; i++)
            {
                var myData = new Faker<Category>()
                    .RuleFor(u => u.Name, (f, u) => f.Name.JobTitle())
                    .RuleFor(u => u.DateTime, (f, u) => f.Date.Past(1, DateTime.Now))
                    .RuleFor(u => u.UrlName, (f, u) => f.Internet.Url());

                myData.Generate();
                cats.Add(myData);
            }


            _context.Categories.AddRange(cats);
            _context.SaveChanges();





            cats.Clear();

            cats = _context.Categories.ToList();

            //Add Parent Id To Some Of Them
            int[] randomIds = new int[40];
            Random rand = new Random();
            for (int i = 0; i < randomIds.Length; i++)
            {
                randomIds[i] = rand.Next(1, cats.Count());
            }

            int[] uniqCatIds = randomIds.Distinct().ToArray();


            for (int i = 0; i < uniqCatIds.Length; i++)
            {
                if (cats.First(c => c.Id == uniqCatIds[i]).ParentId == null)
                {
                    cats.First(c => c.Id == uniqCatIds[i]).ParentId =
                        rand.Next(1, cats.Count());
                }
            }



            _context.Categories.UpdateRange(cats);
            _context.SaveChanges();


        }

        public void AddFakeMyProgresses(int count)
        {
            List<MyProgress> myProgresses = new List<MyProgress>();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                var myData = new Faker<MyProgress>()
                    .RuleFor(u => u.Topic, u => u.Lorem.Letter(rand.Next(3, 100)))
                    .RuleFor(u => u.ProgressValuePercentage, u => rand.Next(0, 100));

                myData.Generate();

                myProgresses.Add(myData);
            }


            _context.AddRange(myProgresses);
            _context.SaveChanges();
        }

        public void AddFakeMedias(int count)
        {
            List<Media> medias = new List<Media>();
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {

                int mediaTypeRand = rand.Next(0, 4);
                MediaFileType mediaType = MediaFileType.Img;

                switch (mediaTypeRand)
                {
                    case 0:
                        mediaType = MediaFileType.Img;
                        break;
                    case 1:
                        mediaType = MediaFileType.Video;
                        break;
                    case 2:
                        mediaType = MediaFileType.Sound;
                        break;
                    case 3:
                        mediaType = MediaFileType.Other;
                        break;
                }

                var myDate = new Faker<Media>()
                    .RuleFor(u => u.Name, u => u.Name.JobTitle())
                    .RuleFor(u => u.DateTime, u => u.Date.Past())
                    .RuleFor(u => u.Size, u => u.System.Random.Double())
                    .RuleFor(u => u.MediaFileType, u => mediaType);

                myDate.Generate();
                medias.Add(myDate);
            }

            _context.AddRange(medias);
            _context.SaveChanges();

        }

        public void AddFakePosts(int count)
        {
            List<Post> posts = new List<Post>();


            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                int postStatusRand = rand.Next(0, 2);
                PostStatus status = PostStatus.Pending;
                switch (postStatusRand)
                {
                    case 0:
                        status = PostStatus.Sent;
                        break;
                    case 1:
                        status = PostStatus.Pending;
                        break;
                }



                int isCommentsOnRand = rand.Next(0, 2);
                bool isComments;
                if (isCommentsOnRand == 0)
                {
                    isComments = false;
                }
                else
                {
                    isComments = true;
                }


                var myData = new Faker<Post>()
                    .RuleFor(u => u.Title, u => u.Name.FullName())
                    .RuleFor(u => u.Body, u => u.Lorem.Paragraphs())
                    .RuleFor(u => u.CategoryId, u => rand.Next(1, _context.Categories.Count()))
                    .RuleFor(u => u.DateTime, u => u.Date.Past())
                    .RuleFor(u => u.LastEditDate, u => u.Date.Future())
                    .RuleFor(u => u.Status, u => status)
                    .RuleFor(u => u.IsCommentsOn, u => isComments)
                    .RuleFor(u => u.MediaId, u => rand.Next(1, _context.Medias.Count()))
                    .RuleFor(u => u.User, u => _userManager.GetUsersInRoleAsync(SD.AdminEndUser).Result[0]);

                myData.Generate();

                posts.Add(myData);
            }



            _context.Posts.AddRange(posts);
            _context.SaveChanges();


            AddFakePostsExtras(count);
        }

        private void AddFakePostsExtras(int count)
        {
            Random rand = new Random();



            for (int i = 0; i < count; i++)
            {
                List<PostAndTag> postAndTags = new List<PostAndTag>();

                int[] tagsIds = new int[rand.Next(1, 10)];

                for (int j = 0; j < tagsIds.Length; j++)
                {
                    tagsIds[j] = rand.Next(1, _context.Tags.Count());
                }


                int[] uniqTagIds = tagsIds.Distinct().ToArray();

                for (int j = 0; j < uniqTagIds.Length; j++)
                {
                    PostAndTag ptag = new PostAndTag()
                    {
                        PostId = i + 1,
                        TagId = uniqTagIds[j],
                    };
                    postAndTags.Add(ptag);
                }

                _context.PostAndTags.AddRange(postAndTags);
                _context.SaveChanges();
            }
        }

        public void AddFakeComments(int count)
        {

            Random rand = new Random();


            int[] postIds = new int[rand.Next(1, _context.Posts.Count())];

            for (int j = 0; j < postIds.Length; j++)
            {
                postIds[j] = rand.Next(1, _context.Posts.Count());
            }


            int[] uniqPostIds = postIds.Distinct().ToArray();


            for (var j = 0; j < uniqPostIds.Length; j++)
            {
                var t = uniqPostIds[j];

                int commentsCount = rand.Next(3, count);

                List<Comment> comments = new List<Comment>();

                for (int i = 0; i < commentsCount; i++)
                {
                    int? id = (int?)null;
                    if (comments.Count != 0)
                    {
                        id = rand.Next(1, comments.Count);
                    }

                    int? randParentId = (int?)null;
                    if (id != null)
                    {
                        randParentId = rand.Next(0, 2) == 0 ? (int?)null : id;
                    }


                    var myData = new Faker<Comment>()
                        .RuleFor(u => u.PostId, u => t)
                        .RuleFor(u => u.ParentId, u => randParentId)
                        .RuleFor(u => u.Body, u => u.Lorem.Paragraphs(1, 2))
                        .RuleFor(u => u.Username, u => u.Name.FullName())
                        .RuleFor(u => u.Email, u => u.Internet.Email())
                        .RuleFor(u => u.Ip, u => u.Internet.Ip())
                        .RuleFor(u => u.DateTime, u => u.Date.Past())
                        .RuleFor(u => u.IsEdited, u => rand.Next(0, 2) == 0 ? false : true);


                    myData.Generate();
                    comments.Add(myData);
                }



                _context.Comments.AddRange(comments);
                _context.SaveChanges();
                comments.Clear();

            }
        }

        public void AddFakeProjects(int count)
        {

            List<Project> projects = new List<Project>();

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                int projectTypeRand = rand.Next(0, 7);
                ProjectType projectType = ProjectType.Desktop;

                switch (projectTypeRand)
                {
                    case 0:
                        projectType = ProjectType.Desktop;
                        break;
                    case 1:
                        projectType = ProjectType.Website;
                        break;
                    case 2:
                        projectType = ProjectType.TemplateDesign;
                        break;
                    case 3:
                        projectType = ProjectType.Seo;
                        break;
                    case 4:
                        projectType = ProjectType.Optimization;
                        break;
                    case 5:
                        projectType = ProjectType.Teaching;
                        break;
                    case 6:
                        projectType = ProjectType.MobileApplication;
                        break;
                    case 7:
                        projectType = ProjectType.Other;
                        break;
                }



                var myDate = new Faker<Project>()
                    .RuleFor(u => u.Name, u => u.Name.FullName())
                    .RuleFor(u => u.Description, u => u.Lorem.Paragraphs(rand.Next(1, 5)))
                    .RuleFor(u => u.ProjectType, u => projectType)
                    .RuleFor(u => u.DateTime, u => u.Date.Past());




                myDate.Generate();

                projects.Add(myDate);
            }

            _context.AddRange(projects);
            _context.SaveChanges();

            AddFakeProjectsExtras(count);

        }

        private void AddFakeProjectsExtras(int count)
        {
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                List<ProjectAndMedia> projectAndMedia = new List<ProjectAndMedia>();

                int[] mediaIds = new int[rand.Next(1, 10)];

                for (int j = 0; j < mediaIds.Length; j++)
                {
                    mediaIds[j] = rand.Next(1, _context.Tags.Count());
                }


                int[] uniqMediasIds = mediaIds.Distinct().ToArray();

                for (int j = 0; j < uniqMediasIds.Length; j++)
                {
                    ProjectAndMedia pmedia = new ProjectAndMedia()
                    {
                        ProjectId = i + 1,
                        MediaId = uniqMediasIds[j],
                    };
                    projectAndMedia.Add(pmedia);
                }

                _context.ProjectAndMedias.AddRange(projectAndMedia);

                _context.SaveChanges();
            }
        }

        public void AddFakeProjectRegisters(int count)
        {

            List<ProjectRegister> projectRegisters = new List<ProjectRegister>();

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {

                int projectTypeRand = rand.Next(0, 7);
                ProjectType projectType = ProjectType.Desktop;

                switch (projectTypeRand)
                {
                    case 0:
                        projectType = ProjectType.Desktop;
                        break;
                    case 1:
                        projectType = ProjectType.Website;
                        break;
                    case 2:
                        projectType = ProjectType.TemplateDesign;
                        break;
                    case 3:
                        projectType = ProjectType.Seo;
                        break;
                    case 4:
                        projectType = ProjectType.Optimization;
                        break;
                    case 5:
                        projectType = ProjectType.Teaching;
                        break;
                    case 6:
                        projectType = ProjectType.MobileApplication;
                        break;
                    case 7:
                        projectType = ProjectType.Other;
                        break;
                }


                int projectStatusRand = rand.Next(0, 3);
                ProjectStatus projectStatus = ProjectStatus.UnClear;
                switch (projectStatusRand)
                {
                    case 0:
                        projectStatus = ProjectStatus.Done;
                        break;
                    case 1:
                        projectStatus = ProjectStatus.UnClear;
                        break;
                    case 2:
                        projectStatus = ProjectStatus.Rejected;
                        break;
                }

                var myData = new Faker<ProjectRegister>()
                    .RuleFor(u => u.Description, u => u.Lorem.Paragraphs(1, rand.Next(1, 3)))
                    .RuleFor(u => u.Title, u => u.Name.FirstName())
                    .RuleFor(u => u.FullName, u => u.Name.FullName())
                    .RuleFor(u => u.Phone, u => u.Phone.PhoneNumber("###########"))
                    .RuleFor(u => u.SocialMediaAccount, u => u.Internet.Url())
                    .RuleFor(u => u.DateTime, u => u.Date.Past())
                    .RuleFor(u => u.DoneDate, u => u.Date.Future())
                    .RuleFor(u => u.ProjectType, u => projectType)
                    .RuleFor(u => u.Status, u => projectStatus);

                myData.Generate();

                projectRegisters.Add(myData);

            }



            _context.ProjectRegisters.AddRange(projectRegisters);
            _context.SaveChanges();

        }

        public void AddFakeSurveyComments(int count)
        {

            List<SurveyComment> surveyComments = new List<SurveyComment>();

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                int commentStatusRand = rand.Next(0, 2);

                SurveyCommentStatus status = SurveyCommentStatus.Rejected;
                switch (commentStatusRand)
                {
                    case 0:
                        status = SurveyCommentStatus.Accepted;
                        break;
                    case 1:
                        status = SurveyCommentStatus.UnClear;
                        break;
                    case 2:
                        status = SurveyCommentStatus.Rejected;
                        break;
                }


                var myData = new Faker<SurveyComment>()
                    .RuleFor(u => u.Body, u => u.Lorem.Paragraphs(rand.Next(1, 4)))
                    .RuleFor(u => u.DateTime, u => u.Date.Past())
                    .RuleFor(u => u.Ip, u => u.Internet.Ip())
                    .RuleFor(u => u.Email, u => u.Internet.Email())
                    .RuleFor(u => u.Username, u => u.Name.FullName())
                    .RuleFor(u => u.Status, u => status)
                    .RuleFor(u => u.IsEdited, u => rand.Next(0, 2) == 0 ? false : true);

                myData.Generate();

                surveyComments.Add(myData);

            }



            _context.SurveyComments.AddRange(surveyComments);
            _context.SaveChanges();


        }
    }
}