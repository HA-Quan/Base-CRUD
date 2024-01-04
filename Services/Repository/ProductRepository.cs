using Microsoft.EntityFrameworkCore;
using Services.Model;
using System.Data.Entity;
using Minio;
using System.IO;
using System.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.Win32;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Services.Service.Common;
using Microsoft.Extensions.Configuration;

namespace Services.Repository
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        
        PagingData<Product> GetPagingProduct(int pageSize, int pageNumber, int? categoryID, string keyword);
        Product GetProductByID(int productID);
        ApiReponse SaveProduct(Product product);
        ApiReponse SaveMultipleProduct(List<Product> listP);
        bool CheckExist(Product product);
        bool CheckExistProductName(string productName, int? productID);
        string DeleteProduct(int pid);
        string DeleteMultipleProduct(List<int> pid);
        Byte[] ExportProduct(List<Product> listP);
        List<Product> ImportProduct(string link);
        ApiReponse ImportSingleProduct(IFormFile path);
        Task<bool> AddImage(IFormFile file);

    }
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly IConfiguration _configuration;
        public ProductRepository(RepositoryContext repositoryContext, IConfiguration configuration) : base(repositoryContext)
        {
            _configuration = configuration;
        }

        public PagingData<Product> GetPagingProduct(int pageSize, int pageNumber, int? categoryID, string keyword)
        {
            PagingData<Product> result = new PagingData<Product>();
            result.TotalCount = _repositoryContext.Product.Count();
            try
            {
                //LinQ
                if (categoryID == 0)
                    result.Data = (from p in _repositoryContext.Product
                              where p.ProductName.ToLower().Contains(keyword)
                              select p).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                else
                    result.Data = (from p in _repositoryContext.Product
                              where p.CategoryID == categoryID
                              where p.ProductName.ToLower().Contains(keyword)
                              select p).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public Product GetProductByID(int productID)
        {
            return _repositoryContext.Product.FirstOrDefault(p =>
             p.ProductID == productID);
        }

        public ApiReponse SaveProduct(Product product)
        {
            ApiReponse result = new ApiReponse();
            try
            {
                if (product.ProductID != null)
                {
                    if (CheckExist(product))
                    {
                        Update(product);
                        _repositoryContext.SaveChanges();
                        result.Success = true;
                        result.Message = "Cập nhật thông tin sản phẩm thành công";
                    }
                    else
                    {
                        result.Success = true;
                        result.Message = "Sản phẩm muốn cập nhật không còn tồn tại trong hệ thống!";
                    }
                    
                }
                else
                {
                    Create(product);
                    _repositoryContext.SaveChanges();
                    result.Success = true;
                    result.Message = "Thêm mới sản phẩm thành công";
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Success = false;
                result.Message = "Không thể lưu được thông tin sản phẩm";
            }
            return result;
            
        }

        public ApiReponse SaveMultipleProduct(List<Product> listP)
        {
            using (var transaction = _repositoryContext.Database.BeginTransaction())
            {
                try
                {
                    bool check = true;
                    foreach (Product p in listP)
                    {
                        if(CheckExistProductName(p.ProductName, p.ProductID))
                        {
                            check = false;
                            break;
                        }
                        else
                        {
                            Create(p);
                            _repositoryContext.SaveChanges();
                        }

                    }
                    if (check)
                    {
                        transaction.Commit();
                        return new ApiReponse()
                        {
                            Success = true,
                            Message = "Import succes!"
                        };
                    }
                    else
                    {
                        transaction.Rollback();
                        return new ApiReponse()
                        {
                            Success = false,
                            Message = "Import  failed!"
                        };
                    }
                        

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    return new ApiReponse()
                    {
                        Success = false,
                        Message = "Import  failed!"
                    };
                }
            }
        }

        public bool CheckExist(Product product)
        {
            var count = (from p in _repositoryContext.Product
                         where p.ProductID == product.ProductID
                         select p).Count();
            if (count > 0)
                return true;
            return false;
        }

        public bool CheckExistProductName(string productName, int? productID)
        {
            int count = 0;
            if (productID != null)
            {
                count = (from p in _repositoryContext.Product
                         where p.ProductName == productName && p.ProductID != productID
                         select p).Count();
            }
            else
            {
                count = (from p in _repositoryContext.Product
                         where p.ProductName == productName
                         select p).Count();
            }
            if (count > 0)
                return true;
            return false ;
        }

        public string DeleteProduct(int pid)
        {
            string result = "";
            try
            {
                var product=(from p in _repositoryContext.Product
                                            where p.ProductID == pid
                                            select p).Cast<Product>().FirstOrDefault();
                if (product!=null)
                {
                    Delete(product);
                    _repositoryContext.SaveChanges();
                    result = "Xoa san pham thanh cong";
                }
                else
                    result = "San pham muon xoa khong con toi tai trong he thong!";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = "Khong the xoa sam pham";
            }
            return result;
        }
        public string DeleteMultipleProduct(List<int> pid)
        {
            string result = "";
            using (var transaction = _repositoryContext.Database.BeginTransaction())
            {
                try
                {
                    bool check = true;
                    foreach (int pidItem in pid)
                    {
                        var product = (from p in _repositoryContext.Product
                                       where p.ProductID == pidItem
                                       select p).Cast<Product>().FirstOrDefault();
                        if (product != null)
                        {
                            Delete(product);
                            _repositoryContext.SaveChanges();
                            result = "Xoa san pham thanh cong";
                        }
                        else
                        {
                            check = false;
                            result = "San pham muon xoa khong con toi tai trong he thong!";
                            break;
                        }
                            
                    }
                    if (check)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = "Khong the xoa sam pham";
                    transaction.Rollback();
                }
                return result;
            }
                
        }
        public Byte[] ExportProduct(List<Product> listP)
        {
            Byte[] result = null;
            try
            {
                // Tạo danh sách các column header
                string[] arrColumnHeader = {"ID", "Name", "Image","Title","Description",
                                            "Quantity","Price","idCategory","idAccount"};
                using (var workbook = new XLWorkbook())
                {
                    // Thêm mới 1 sheet
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Demo");
                    worksheet.Style.Font.FontName = "Times New Roman";

                    // Set giá trị và style cho tiêu đề
                    worksheet.Cell(1, 1).Value = "DEMO EXPORT DATA";
                    worksheet.Range("A1:I1").Row(1).Merge();
                    worksheet.Range("A1:I1").Row(1).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range("A1:I1").Row(1).Merge().Style.Font.FontSize = 16;
                    worksheet.Range("A1:I1").Row(1).Merge().Style.Font.Bold = true;

                    worksheet.Range("A2:I2").Row(1).Merge();
                    int colIndex = 1;
                    foreach (var item in arrColumnHeader)
                    {
                        worksheet.Cell(3, colIndex).Value = item;
                        worksheet.Cell(3, colIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(3, colIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(3, colIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(3, colIndex).Style.Font.Bold = true;
                        colIndex++;
                    }
                    int rowIndex = 3;
                    foreach (var item in listP)
                    {
                        rowIndex++;
                        int col = 1;
                        var properties = typeof(Product).GetProperties();
                        foreach (var property in properties)
                        {

                            //gán giá trị cho từng cell    
                            var value = property.GetValue(item);
                            worksheet.Cell(rowIndex, col).Value = value?.ToString();
                            worksheet.Cell(rowIndex, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(rowIndex, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            col++;
                        }

                    }
                    // Chỉnh độ rộng của cột fit với nội dung
                    worksheet.Columns().AdjustToContents();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        result = stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;

        } 
        public List<Product> ImportProduct(string link)
        {
            List<Product> result = new List<Product> ();
            using (var workbook = new XLWorkbook(link))
            {
                var wb1 = workbook.Worksheet(1);
                var limit = wb1.RangeUsed().Rows().Count();
                for (int i= 4; i <= limit; i++)
                {
                    var p = new Product();
                    //p.ProductID = (int)wb1.Cell(i,1).Value;
                    //p.ProductName = (string)wb1.Cell(i, 2).Value;
                    //p.Image = (string)wb1.Cell(i, 3).Value;
                    //p.Title = (string)wb1.Cell(i, 4).Value;
                    //p.Description = (string)wb1.Cell(i, 5).Value;
                    //p.Quantity = (int)wb1.Cell(i, 6).Value;
                    //p.Price = Convert.ToDecimal(wb1.Cell(i, 7).Value.ToString());
                    //p.CategoryID = (int)wb1.Cell(i, 8).Value;
                    //p.AccountID = (int)wb1.Cell(i, 9).Value;
                    p.ProductID = Convert.ToInt32(wb1.Cell(i, 1).Value.ToString());
                    p.ProductName = (string)wb1.Cell(i, 2).Value;
                    p.Image = (string)wb1.Cell(i, 3).Value;
                    p.Title = (string)wb1.Cell(i, 4).Value;
                    p.Description = (string)wb1.Cell(i, 5).Value;
                    p.Quantity = Convert.ToInt32(wb1.Cell(i, 6).Value.ToString());
                    p.Price = Convert.ToDecimal(wb1.Cell(i, 7).Value.ToString());
                    p.CategoryID = Convert.ToInt32(wb1.Cell(i, 8).Value.ToString()); ;
                    p.AccountID = Convert.ToInt32(wb1.Cell(i, 9).Value.ToString()); ;
                    result.Add(p);
                }
            }
            return result;
        }

        public ApiReponse ImportSingleProduct(IFormFile path)
        {
            List<Product> result = new List<Product>();
            try
            {
                using var workbook = new XLWorkbook(path.OpenReadStream());
                var wb1 = workbook.Worksheet(1);
                var limit = wb1.RangeUsed().Rows().Count();
                var colum = wb1.RangeUsed().ColumnCount();
                if (colum != 9)
                    return new ApiReponse()
                    {
                        Success = false,
                        Message = "File Import sai định dạng !"
                    };
                for (int i = 4; i <= limit; i++)
                {
                    var p = new Product();
                    p.ProductName = (string)wb1.Cell(i, 2).Value;
                    p.Image = (string)wb1.Cell(i, 3).Value;
                    p.Title = (string)wb1.Cell(i, 4).Value;
                    p.Description = (string)wb1.Cell(i, 5).Value;
                    p.Quantity = Convert.ToInt32(wb1.Cell(i, 6).Value.ToString());
                    p.Price = Convert.ToDecimal(wb1.Cell(i, 7).Value.ToString());
                    p.CategoryID = Convert.ToInt32(wb1.Cell(i, 8).Value.ToString()); ;
                    p.AccountID = Convert.ToInt32(wb1.Cell(i, 9).Value.ToString()); ;
                    result.Add(p);
                }
                return SaveMultipleProduct(result);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ApiReponse()
                {
                    Success = false,
                    Message = "Dữ liệu file import không đúng !"
                };
            }
        }
        public async Task<bool> AddImage(IFormFile file)
        {
            CommonService CS = new CommonService(_configuration);
            MinioClient minio = CS.CreateMinio();
            try
            {
                var bucketName = "demo";
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                using(var data = file.OpenReadStream())
                {
                    var objectName = Guid.NewGuid().ToString() + '_' + file.FileName;
                    //var contentType = "application/octet-stream";
                    var contentType = file.ContentType;
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(data)
                        .WithObjectSize(data.Length)
                        .WithContentType(contentType);
                    await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }
    }
}
