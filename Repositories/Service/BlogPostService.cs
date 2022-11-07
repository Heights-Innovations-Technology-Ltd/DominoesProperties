using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class BlogPostService : BaseRepository, IBlogPostRepository
    {
        public BlogPostService(dominoespropertiesContext context) : base(context)
        {

        }

        public bool AddBlogPosts(Blogpost blogPosts)
        {
            _context.Blogposts.Add(blogPosts);
            _context.SaveChanges();
            return true;
        }

        public List<Blogpost> BlogPosts()
        {
            return _context.Blogposts
                .Include(x=>x.CreatedByNavigation)
                .OrderByDescending(x => x.CreatedOn)
                .ToList();
        }

        public bool DeleteBlogPosts(int id)
        {
            var blogPost = _context.Blogposts.FirstOrDefault(c => c.Id == id);
            if (blogPost != null) _context.Blogposts.Remove(blogPost);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateBlogPosts(Blogpost blogPosts)
        {
            if (blogPosts == null) return false;
            _context.Blogposts.Update(blogPosts);
            _context.SaveChanges();
            return true;
        }
    }
}
