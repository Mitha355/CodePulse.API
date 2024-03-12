using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CodePulse.API.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category> CreateAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            _dbContext.SaveChanges();
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid Id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            var existingCategory= await _dbContext.Categories.FirstOrDefaultAsync(x=>x.Id==category.Id);
            if (existingCategory!=null)
            {
                _dbContext.Entry(existingCategory).CurrentValues.SetValues(category);
                await _dbContext.SaveChangesAsync();
                return existingCategory;
            }
            return null;
        }
        public async Task<Category?> DeleteAsync(Guid Id)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingCategory is null)
            {
                return null;
            }
            _dbContext.Categories.Remove(existingCategory);
            await _dbContext.SaveChangesAsync();
            return existingCategory;
        }

    }
}
