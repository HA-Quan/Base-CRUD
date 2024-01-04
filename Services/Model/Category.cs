using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Services.Model
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [Column("idCategory")]
        public int CategoryID { get; set; }

        [Column("categoryName")]
        public string CategoryName { get; set; }

    }
}
