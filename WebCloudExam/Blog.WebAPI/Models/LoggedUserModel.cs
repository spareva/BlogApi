using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Blog.WebAPI.Models
{
    [DataContract]
    public class LoggedUserModel
    {
        [DataMember(Name="sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name="displayName")]
        public string Nickname { get; set; }
    }
}