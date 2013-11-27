using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Blog.WebAPI.Controllers
{
    [DataContract]
    public class PostAddResponseModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }
    }
}
