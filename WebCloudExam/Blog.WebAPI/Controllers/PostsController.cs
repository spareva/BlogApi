using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blog.WebAPI.Models;
using Blog.Model;

namespace Blog.WebAPI.Controllers
{
    public class PostsController : BaseApiController
    {
        protected char[] SplitSeparators = new char[] { ' ', ',', '.', '!', '?', ':' };

        public PostsController()
        {
        }

        [HttpGet]
        public IQueryable<PostModel> GetAll(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
             () =>
             {
                 var context = new BlogContext();
                 var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                 if (user == null)
                 {
                     throw new InvalidOperationException("Invalid sessionkey");
                 }

                var allPosts = context.Posts;
                var models = (from post in allPosts
                            select new PostModel()
                            {
                                Id = post.Id,
                                Title = post.Title,
                                PostedBy = post.User.Nickname,
                                PostDate = post.PostDate,
                                Text = post.Text,
                                Tags = (from tag in post.Tags
                                        select tag.Name),
                                Comments = (from comment in post.Comments
                                            select new CommentModel()
                                            {
                                                Text = comment.Text,
                                                CommentedBy = comment.User.Nickname,
                                                PostDate = comment.PostDate,
                                            }),

                            });
                return models.OrderByDescending(x => x.PostDate);
             });
            return responseMsg;
        }

        [HttpGet]
        public IQueryable<PostModel> GetPaging(int page, int count, string sessionKey)
        {
            var models = this.GetAll(sessionKey).Skip((page-1) * count).Take(count);
            return models;
        }

        [HttpGet]
        public IQueryable<PostModel> GetKeyword(string keyword, string sessionKey)
        {
            var models = this.GetAll(sessionKey)
                .Where(t => t.Title.ToLower().Contains(keyword.ToLower()))
                .OrderByDescending(t => t.PostDate);

            return models;
        }


        [HttpGet]
        public IQueryable<PostModel> GetAllTags(string tags, string sessionKey)
        { //this used to work but display more results and decided to stop
            var alltags = tags.Split(',');

            var models = this.GetAll(sessionKey);

            List<PostModel> posts = new List<PostModel>();
            bool toAdd = true;

            foreach (var model in models)
            {
                var curTags = model.Tags;
                foreach (var tag in alltags)
                {
                    if (!curTags.Contains(tag))
                    {
                        toAdd = false;
                    }
                }
                if (toAdd == true)
                {
                    posts.Add(model);
                }
            }

            //var models = from post in this.GetAll(sessionKey) //get all postmodels
            //                 .Where(p => p.Tags.All // get all the tags of that post that return TRUE to the condition
            //                     (t => alltags.Contains(t))) // that alltags contains them 
            //             select post;

            //this.GetAll(sessionKey)
            //.Where(at => at.Tags.Any(alltags));
            return posts.AsQueryable();
        }

        [HttpPost]
        [ActionName("posts")]
        public HttpResponseMessage PostPost(PostModel model, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   var context = new BlogContext();
                   using (context)
                   {
                       foreach (var tag in model.Tags) // add all new tags
                       {
                           if (!context.Tags.Any(t => t.Name.ToLower() == tag.ToLower()))
                           {
                               var tagToAdd = new Tag()
                               {
                                   Name = tag.ToLower(), // all tags are stored in lowercase
                               };
                               context.Tags.Add(tagToAdd);
                           }
                           context.SaveChanges();
                       }

                       string[] tagsFromTitle = model.Title.Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries);

                       foreach (var tag in tagsFromTitle) // add all new tags from the title
                       {
                           if (!context.Tags.Any(t => t.Name.ToLower() == tag.ToLower()))
                           {
                               var tagToAdd = new Tag()
                               {
                                   Name = tag.ToLower(), // all tags are stored in lowercase
                               };
                               context.Tags.Add(tagToAdd);
                           }
                           context.SaveChanges();
                       }

                       var post = new Post()
                       {
                           Title = model.Title,
                           PostDate = DateTime.Now,
                           Tags = (from tags in context.Tags
                                  where model.Tags.Contains(tags.Name)
                                  select tags).ToList(),
                           //Tags = (from tags in model.Tags //this was working, but adding tags with repetative Name
                           //        select new Tag()
                           //        {
                           //            Name = tags,
                           //        }).ToList(),
                           Text = model.Text,
                           User = (from users in context.Users
                                  where users.SessionKey == sessionKey
                                  select users).FirstOrDefault(),
                       };

                       foreach (var tag in tagsFromTitle)
                       {
                           post.Tags.Add((from tags in context.Tags
                                         where tags.Name == tag
                                         select tags).FirstOrDefault());
                       }

                       context.Posts.Add(post);
                       context.SaveChanges();

                       var postAddResponseModel = new PostAddResponseModel()
                       {
                           Title = post.Title,
                           Id = post.Id,
                       };
                       var response =
                           this.Request.CreateResponse(HttpStatusCode.Created,
                                postAddResponseModel);
                       return response;
                   }
            });
            return responseMsg;
        }
    }
}
