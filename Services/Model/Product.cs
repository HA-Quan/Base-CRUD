using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Services.Model
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [Column("idProduct")]
        public int? ProductID { get; set; }

        [Column("productName")]
        public string ProductName { get; set; }

        [Column("image")]
        public string Image { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        [Column("idCategory")]
        public int CategoryID { get; set; }

        [ForeignKey("Account")]
        [Column("idAccount")]
        public int AccountID { get; set; }
    }
}
