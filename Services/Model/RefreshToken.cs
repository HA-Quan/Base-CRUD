using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Services.Model
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        [Column("idRefreshToken")]
        public Guid RefreshTokenID { get; set; }

        [ForeignKey("Account")]
        [Column("idAccount")]
        public int AccountID { get; set; }

        [Column("Token")]
        public string Token { get; set; }

        [Column("JwtID")]
        public Guid JwtID { get; set; }

        [Column("IsUsed")]
        public bool IsUsed { get; set; }

        [Column("isRevoked")]
        public bool IsRevoked { get; set; }

        [Column("issuedAt")]
        public DateTime IssuedAt { get; set; }
        [Column("expiredAt")]
        public DateTime ExpiredAt { get; set; }
    }
}
