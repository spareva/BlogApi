using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blog.Model;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Controllers
{
    public class CommentController : BaseApiController
    {
        [HttpPut]
        public HttpResponseMessage PutComment(string sessionKey, int postId, 
            CommentModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var context = new BlogContext();
                  using (context)
                  {
                      var post = (from posts in context.Posts
                                  where posts.Id == postId
                                  select posts).FirstOrDefault();

                      if (post == null)
                      {
                          throw new InvalidOperationException("Such post does not exist");
                      }
                      var comment = new Comment()
                      {
                          Post = post,
                          PostDate = DateTime.Now,
                          Text = model.Text,                          
                          User = (from users in context.Users
                                    where users.SessionKey == sessionKey
                                      select users).FirstOrDefault(),
                      };

                      var commentModelPut = new CommentModelPut()
                      {
                          Text = comment.Text,
                      };

                      post.Comments.Add(comment);
                      context.Comments.Add(comment);
                      context.SaveChanges();
                  }

                  var response =
                          this.Request.CreateResponse(HttpStatusCode.OK);
                  return response;
              });

           return responseMsg;
        }
    }
}
