using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Repository
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        List<Category> GetAllCategory();
        void CreateCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public List<Category> GetAllCategory()
        {
            List<Category> result = new List<Category>();
            try
            {
                //LinQ
                result = (from c in _repositoryContext.Category
                          select c).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
        public void CreateCategory(Category category)
        {
            Create(category);
        }

        public void UpdateCategory(Category category)
        {
            Update(category);
        }

        public void DeleteCategory(Category category)
        {
            Delete(category);
        }
    }
}
