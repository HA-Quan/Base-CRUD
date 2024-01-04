using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Model;
using Services.Repository;
using Services.Service;
using System.Transactions;

namespace Services.Service
{
    public interface IProductService
    {
        PagingData<Product> GetPagingProduct(int pageSize, int pageNumber, int? categoryID, string keyword);
        ApiReponse GetProductByID(int productID);
        bool CheckExistProductName(string productName, int? productID);
        ApiReponse SaveProduct(Product p);
        string DeleteProduct(int pid);
        string DeleteMultipleProduct(List<int> pid);
        Byte[] ExportProduct(List<Product> listP);
        List<Product> ImportProduct(string link);
        ApiReponse ImportSingleProduct(IFormFileCollection path);
        Task<ApiReponse> AddImage(IFormFileCollection listImage);
    }

    internal class ProductService : BaseService, IProductService
    {
        public ProductService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
        }

        public PagingData<Product> GetPagingProduct(int pageSize, int pageNumber, int? categoryID, string keyword)
        {
            string key="";
            if (keyword != null)
                key = keyword.ToLower();
            return _repositoryManager.ProductRepository.GetPagingProduct(pageSize, pageNumber, categoryID, key);
        }

        public ApiReponse GetProductByID(int productID)
        {
            var result = _repositoryManager.ProductRepository.GetProductByID(productID);
            if (result != null)
                return new ApiReponse()
                {
                    Success = true,
                    Data = result
                };
            else
                return new ApiReponse()
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm có id : " + productID.ToString() 
                };
        }
        public ApiReponse SaveProduct(Product p)
        {
            ApiReponse result = _repositoryManager.ProductRepository.SaveProduct(p);
            return result;
        }
        public bool CheckExistProductName(string productName, int? productID) { 
            return _repositoryManager.ProductRepository.CheckExistProductName(productName, productID);
        }
        public string DeleteProduct(int pid)
        {
            string result = _repositoryManager.ProductRepository.DeleteProduct(pid);
            return result;
        }
        public string DeleteMultipleProduct(List<int> pid)
        {
            return _repositoryManager.ProductRepository.DeleteMultipleProduct(pid);
        }
        public Byte[] ExportProduct(List<Product> listP)
        {
            return _repositoryManager.ProductRepository.ExportProduct(listP);
        }
        public List<Product> ImportProduct(string link)
        {
            return _repositoryManager.ProductRepository.ImportProduct(link);
        }

        public ApiReponse ImportSingleProduct(IFormFileCollection path)
        {
            return _repositoryManager.ProductRepository.ImportSingleProduct(path[0]);
        }

        public async Task<ApiReponse> AddImage(IFormFileCollection listImage)
        {
            foreach (var file in listImage)
            {
                bool check = await _repositoryManager.ProductRepository.AddImage(file);
                if (!check)
                {
                    return new ApiReponse()
                    {
                        Success = false,
                        Message = "Thêm ảnh thất bại"
                    };
                }
            }

            return new ApiReponse()
            {
                Success = true,
                Message = "Thêm ảnh thành công"
            };

        }
    }
}
