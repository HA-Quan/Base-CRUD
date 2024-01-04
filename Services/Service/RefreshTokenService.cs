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
    public interface IRefreshService
    {
        void CreateRefreshToken(RefreshToken rt);
        void UpdateRefreshToken(RefreshToken rt);
        RefreshToken GetRefreshToken(string key);
    }

    internal class RefreshService : BaseService, IRefreshService
    {
        private readonly IConfiguration _configuration;
        public RefreshService(IRepositoryManager repositoryManager, IConfiguration configuration) : base(repositoryManager)
        {
            _configuration = configuration;
        }
        public void CreateRefreshToken(RefreshToken rt)
        {
            _repositoryManager.RefreshTokenRepository.CreateRefreshToken(rt);
        }
        public void UpdateRefreshToken(RefreshToken rt)
        {
            _repositoryManager.RefreshTokenRepository.UpdateRefreshToken(rt);
        }
        public RefreshToken GetRefreshToken(string key)
        {
            return _repositoryManager.RefreshTokenRepository.GetRefreshToken(key);
        }

    }
}
