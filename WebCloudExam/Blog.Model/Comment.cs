using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Model
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public virtual User User { get; set; }

        [Column(TypeName="ntext")]
        public string Text { get; set; }

        public virtual Post Post { get; set; }

        public DateTime PostDate { get; set; }

        public Comment()
        {
        }
    }
}
