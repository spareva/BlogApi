using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blog.WebAPI.Models
{
    [DataContract]
    public class CommentModelPut
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}
