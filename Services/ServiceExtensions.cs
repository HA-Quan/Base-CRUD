using Services.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Service;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Services;

public static class ServiceExtensions
{
    
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config.GetConnectionString("DBconnection");
        services.AddDbContext<RepositoryContext>(
            options => options.UseSqlServer(connectionString));

        //services.AddDbContextPool<RepositoryContext>(o => o.UseMySql(connectionString, serverVersion), 300);
    }
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration config)
    {
        //JWT
        var secretKey = config["JWT:SecretKey"];
        var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(otp =>
        {
            otp.TokenValidationParameters = new TokenValidationParameters
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                // ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero
            };
        });
    }
    public static void ConfigureCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRefreshService, RefreshService>();
    }
}
