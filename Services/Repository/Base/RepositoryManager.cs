using Microsoft.Extensions.Configuration;

namespace Services.Repository
{
    public interface IRepositoryManager
    {
        IAccountRepository AccountRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }

        Task SaveAsync();

    }

    public class RepositoryManager : IRepositoryManager
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RepositoryContext _repositoryContext;
        private readonly IConfiguration _configurationt;

        public RepositoryManager(RepositoryContext repositoryContext, IConfiguration configuration)
        {
            _repositoryContext = repositoryContext;
            _configurationt = configuration;
        }
        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        public IAccountRepository AccountRepository => _accountRepository ?? new AccountRepository(_repositoryContext);
        public ICategoryRepository CategoryRepository => _categoryRepository ?? new CategoryRepository(_repositoryContext);
        public IProductRepository ProductRepository => _productRepository ?? new ProductRepository(_repositoryContext, _configurationt);
        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ?? new RefreshTokenRepository(_repositoryContext);
    }
}
