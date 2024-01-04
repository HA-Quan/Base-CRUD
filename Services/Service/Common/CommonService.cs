using Microsoft.Extensions.Configuration;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service.Common
{
    public class CommonService
    {
        private readonly IConfiguration _configuration;

        public CommonService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MinioClient CreateMinio()
        {
            return new MinioClient().WithEndpoint(_configuration["MinIO:Endpoint"])
                                    .WithCredentials(_configuration["MinIO:AccessKey"], _configuration["MinIO:SecretKey"])
                                    .WithSSL(false)
                                    .Build();
        }
    }
}
