using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Controllers
{
    public class TagsController : BaseApiController
    {
        [HttpGet]
        public IQueryable<TagModel> GetAll(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
             () =>
             {
                 var context = new BlogContext();
                 var models = from tags in context.Tags
                              select new TagModel()
                              {
                                  Id = tags.Id,
                                  Name = tags.Name,
                                  Posts = (from posts in context.Posts
                                           where posts.Tags.Contains(tags)
                                           select posts).Count(),
                              };

                 return models.OrderBy(x => x.Id);
             });
            return responseMsg;
        }

        [HttpGet]
        public IQueryable<PostModel> GetByPost(int tagId, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
             () =>
             {
                 var context = new BlogContext();

                 var models = from posts in context.Posts
                             where posts.Tags.Any(t => t.Id == tagId)
                             select new PostModel()
                             {
                                 Id = posts.Id,
                                 Title = posts.Title,
                                 PostedBy = posts.User.Nickname,
                                 PostDate = posts.PostDate,
                                 Text = posts.Text,
                                 Tags = (from tag in posts.Tags
                                         select tag.Name),
                                 Comments = (from comment in posts.Comments
                                             select new CommentModel()
                                             {
                                                 Text = comment.Text,
                                                 CommentedBy = comment.User.Nickname,
                                                 PostDate = comment.PostDate,
                                             }),
                             };
                 return models.OrderByDescending(x => x.PostDate);
             });
            return responseMsg;
        }
    }
}
