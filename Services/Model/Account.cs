using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Services.Model
{
    [Table("Account")]
    public class Account
    {
        [Key]
        [Column("idAccount")]
        public int AccountID { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("pwd")]
        public string Password { get; set; }

        [Column("position")]
        public int? Position { get; set; }

        [Column("fullName")]
        public string? FullName { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("address")]
        public string? Address { get; set; }
    }
}
