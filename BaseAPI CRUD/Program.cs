using Services;

namespace BaseAPI_CRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //                          policy =>
            //                          {
            //                              policy.WithOrigins("http://localhost:8080")
            //                                                  .AllowAnyHeader()
            //                                                  .AllowAnyMethod();
            //                          });
            //});

            builder.Services.AddCors(option =>
            {
                option.AddPolicy(name: MyAllowSpecificOrigins,
                                policy =>
                                {
                                    policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                                });
            });


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.ConfigureSqlContext(builder.Configuration);

            // Repository
            builder.Services.ConfigureCustomServices();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(MyAllowSpecificOrigins);

            app.MapControllers();

            app.Run();
        }
    }
}