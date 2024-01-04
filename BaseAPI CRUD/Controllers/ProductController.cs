using Microsoft.AspNetCore.Mvc;
using Services.Service;
using Services.Model;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace BaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("")]
        public IActionResult GetPagingProduct([FromQuery] string? keyword, [FromQuery] int categoryID = 0, 
                                                [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = _productService.GetPagingProduct(pageSize, pageNumber, categoryID, keyword);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{productID}")]
        public IActionResult GetProductByID([FromRoute] int productID)
        {
            try
            {
                var result = _productService.GetProductByID(productID);
                if (result.Success)
                    return StatusCode(StatusCodes.Status200OK, result.Data);
                else
                    return StatusCode(StatusCodes.Status400BadRequest, result.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("import")]
        public IActionResult ImportProduct([FromQuery] string link)
        {
            try
            {
                var result = _productService.ImportProduct(link);
                if (!result.Any()) return StatusCode(StatusCodes.Status400BadRequest);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("export")]
        public IActionResult ExportProduct([FromQuery] int? categoryID, [FromQuery] string? keyword,
                                                [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var listP = _productService.GetPagingProduct(pageSize, pageNumber, categoryID, keyword).Data;
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "demo.xlsx";
                var content = _productService.ExportProduct(listP);
                return File(content, contentType, fileName);
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("ImportSingle")]
        public IActionResult ImportSingle()
        {
            var files = Request.Form.Files;
            var result = _productService.ImportSingleProduct(files);
            if (result.Success)
                return StatusCode(StatusCodes.Status200OK, result.Message);
            else
                return StatusCode(StatusCodes.Status400BadRequest, result.Message);
        }

        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage([FromBody] Demo demo)
        {
            var x = demo;
            Console.WriteLine(x);
            var files = Request.Form.Files;
            var result = await _productService.AddImage(files);
            if (result.Success)
                return StatusCode(StatusCodes.Status200OK, result.Message);
            else
                return StatusCode(StatusCodes.Status400BadRequest, result.Message);
        }

        [HttpPost("Add")]
        public IActionResult AddProduct([FromBody] Product p)
        {
            try
            {

                if(_productService.CheckExistProductName(p.ProductName, null))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Tên sản phẩm đã tồn tại");
                }
                var result = _productService.SaveProduct(p);
                if (result.Success)
                {
                    return StatusCode(StatusCodes.Status200OK, result.Message);
                }
                else
                    return StatusCode(StatusCodes.Status400BadRequest, result.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{productID}")]
        public IActionResult UpdateProduct([FromBody] Product p)
        {
            try
            {
                if (_productService.CheckExistProductName(p.ProductName, p.ProductID))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Tên sản phẩm đã tồn tại");
                }
                var result = _productService.SaveProduct(p);
                if (result.Success)
                {
                    return StatusCode(StatusCodes.Status200OK, result.Message);
                }
                else
                    return StatusCode(StatusCodes.Status400BadRequest, result.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{productID}")]
        public IActionResult DeleteProduct([FromRoute] int productID)
        {
            try
            {
                var result = _productService.DeleteProduct(productID);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("multiple")]
        public IActionResult DeleteMultipleProduct([FromBody] List<int> listID)
        {
            try
            {
                var result = _productService.DeleteMultipleProduct(listID);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
