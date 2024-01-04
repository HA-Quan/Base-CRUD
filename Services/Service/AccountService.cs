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
    public interface IAccountService
    {
        Account GetAccount(LoginModel loginModel);
        Account GetAccountById(int key);
        ApiReponse AddAccount(SingUpModel acc);
        TokenModel GenerateToken(Account account);
        ApiReponse RenewToken(TokenModel tokenModel);
        string GenerateRefreshToken();
        string validate(SingUpModel acc);
        string validateRefreshToken(TokenModel tokenModel);
    }

    internal class AccountService : BaseService, IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshService _refreshService;
        public AccountService(IRepositoryManager repositoryManager, IConfiguration configuration, IRefreshService refreshService) : base(repositoryManager)
        {
            _configuration = configuration;
            _refreshService = refreshService;
        }
        public Account GetAccount(LoginModel loginModel)
        {
            return _repositoryManager.AccountRepository.GetAccount(loginModel);
        }

        public Account GetAccountById(int key)
        {
            return _repositoryManager.AccountRepository.GetAccountById(key);
        }

        public TokenModel GenerateToken(Account account)
        {
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyByte = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", account.AccountID.ToString()),
                    new Claim("User", account.Username),
                    new Claim(ClaimTypes.Name, account.FullName),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte),
                                                             SecurityAlgorithms.HmacSha512Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenModel = new RefreshToken()
            {
                RefreshTokenID = Guid.NewGuid(),
                AccountID = account.AccountID,
                JwtID = new Guid(token.Id),
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(30),
            };
            _repositoryManager.RefreshTokenRepository.CreateRefreshToken(refreshTokenModel);
            return new TokenModel()
            {
                AccessToken = jwtTokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
                
        }

        public ApiReponse RenewToken(TokenModel tokenModel)
        {
            var validateMessage = validateRefreshToken(tokenModel);
            if (validateMessage.IsNullOrEmpty())
            {
                var refreshTokenCurrent = _refreshService.GetRefreshToken(tokenModel.RefreshToken);
                refreshTokenCurrent.IsUsed = true;
                refreshTokenCurrent.IsRevoked = true;
                _refreshService.UpdateRefreshToken(refreshTokenCurrent);

                var acc = GetAccountById(refreshTokenCurrent.AccountID);
                var newToken = GenerateToken(acc);
                return new ApiReponse()
                {
                    Success = true,
                    Message = "Renew Token Acces!",
                    Data = newToken,
                };
            }
            else
            {
                return new ApiReponse()
                {
                    Success = false,
                    Message = validateMessage
                };
            }
            

        }

        public string GenerateRefreshToken()
        {
            var random = new Byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public ApiReponse AddAccount(SingUpModel acc)
        {
            if (validate(acc).IsNullOrEmpty())
            {
                Account account = _repositoryManager.AccountRepository.CreateAccount(acc);
                return new ApiReponse() {
                    Success = true,
                    Message = "SingUp Account Succes!",
                    Data = GenerateToken(account) };
            }
            else
                return new ApiReponse()
                {
                    Success = false,
                    Message = validate(acc)
                };
        }
        public string validate(SingUpModel acc)
        {
            if (acc.UserName.IsNullOrEmpty())
                return "Username not empty!";
            if (acc.Password.IsNullOrEmpty())
                return "Password not empty!";
            if (acc.FullName.IsNullOrEmpty())
                return "FullName not empty!";
            if (acc.Email.IsNullOrEmpty())
                return "Email not empty!";
            string strRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(strRegex);
            if (!regex.IsMatch(acc.Email))
                return "Email is not in correct format";
            return _repositoryManager.AccountRepository.ChekcUsernameAndEmail(acc.UserName, acc.Email);
        }

        public string validateRefreshToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyByte = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var validateTokenParam = new TokenValidationParameters
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                // ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),

                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            try
            {
                //check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, validateTokenParam,
                                                                      out var validateToken);

                //check 2: Check Alg AccessToken 
                if (validateToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, 
                                                                     StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                        return "Invalid Alg Token";
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x =>
                                                                                x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                    return "Access token not expired!";

                //check 4: Check refreshtoken exist in DB
                var checkExist = _refreshService.GetRefreshToken(tokenModel.RefreshToken);
                if (checkExist == null)
                    return "Refresh token does not exist!";

                //check 5: check refreshToken is used/revoked?
                if (checkExist.IsUsed)
                    return "Refresh token has been used!";
                if (checkExist.IsRevoked)
                    return "Refresh token has been revoked!";

                //check 6: AccessToken id == JwtId in RefreshToken
                /*
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (checkExist.JwtID != jti)
                    return "Token doesn't match!";*/
                return "";
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
            
        }

        public DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            
        }
    }
}
