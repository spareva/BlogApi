using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Blog.WebAPI.Models
{
    [DataContract]
    public class UserModel
    {
        public int Id { get; set; }

        [DataMember(Name="username")]
        public string Username { get; set; }

        [DataMember(Name = "displayName")]
        public string Nickname { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }
}