using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Repository
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
    {
       
        RefreshToken GetRefreshToken(string key);
        void CreateRefreshToken(RefreshToken rt);
        void UpdateRefreshToken(RefreshToken rt);
        void DeleteRefreshToken(RefreshToken rt);
    }
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateRefreshToken(RefreshToken rt)
        {
            Create(rt);
            _repositoryContext.SaveChanges();
        }

        public void UpdateRefreshToken(RefreshToken rt)
        {
            Update(rt);
            _repositoryContext.SaveChanges();
        }

        public void DeleteRefreshToken(RefreshToken rt)
        {
            Delete(rt);
            _repositoryContext.SaveChanges();
        }

        public RefreshToken GetRefreshToken(string key)
        {
            var result = new RefreshToken();
            try
            {
                //var test = _repositoryContext.RefreshToken.ToList();
                result = _repositoryContext.RefreshToken.FirstOrDefault(x => x.Token.Equals(key));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
