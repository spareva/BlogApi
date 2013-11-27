using System;
using System.Collections.Generic;
using System.Data.Entity;
using Blog.Model;

namespace DataLayer
{
    public class BlogContext : DbContext
    {
        public BlogContext()
            : base("BlogDb")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(usr => usr.SessionKey)
                .IsFixedLength()
                .HasMaxLength(40);
            base.OnModelCreating(modelBuilder);
        }
    }
}
