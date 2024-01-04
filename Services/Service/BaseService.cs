using Services.Repository;

namespace Services.Service
{
    internal class BaseService
    {
        protected IRepositoryManager _repositoryManager;
        public BaseService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
        }
    }
}
