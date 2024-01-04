using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Model;
using Services.Repository;
using Services.Service;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Security.Cryptography;
namespace Services.Service
{
    public interface ICategoryService
    {
        List<Category> getAllCategory();
    }

    internal class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
        }
        public List<Category> getAllCategory()
        {
            return _repositoryManager.CategoryRepository.GetAllCategory();
        }
    }
}
