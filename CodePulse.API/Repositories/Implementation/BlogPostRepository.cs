using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await _dbContext.BlogPosts.AddAsync(blogPost);
            await _dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogpost)
        {
            var existingBlogpost = await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == blogpost.Id);
            if (existingBlogpost == null)
            {
                return null;
            }
            //update blogpost
            _dbContext.Entry(existingBlogpost).CurrentValues.SetValues(blogpost);

            //update categories
            existingBlogpost.Categories = blogpost.Categories;

            await _dbContext.SaveChangesAsync();
            return blogpost;

        }
        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogpost= await _dbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (existingBlogpost != null) 
            {
                _dbContext.BlogPosts.Remove(existingBlogpost);
               await _dbContext.SaveChangesAsync();
            }
            return existingBlogpost;
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }
    }
}
