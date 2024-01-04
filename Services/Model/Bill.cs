using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Services.Model
{
    [Table("Bill")]
    public class Bill
    {
        [Key]
        [Column("idBill")]
        public int BillID { get; set; }

        [Column("chiTiet")]
        public string ChiTiet { get; set; }

    }
}
