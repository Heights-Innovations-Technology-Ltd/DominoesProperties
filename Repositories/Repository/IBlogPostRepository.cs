using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IBlogPostRepository
    {
        List<Blogpost> BlogPosts();

        bool AddBlogPosts(Blogpost blogPosts);

        bool UpdateBlogPosts(Blogpost blogPosts);

        bool DeleteBlogPosts(int id);
    }
}