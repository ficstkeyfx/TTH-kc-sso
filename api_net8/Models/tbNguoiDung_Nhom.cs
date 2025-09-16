using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace api.Models
{
    [Table("tbNguoiDung_Nhom")]
    public partial class tbNguoiDung_Nhom
    {

        [Key]
        public int IdNguoiDung_Nhom { get; set; }
        public int IdNhom { get; set; }
        public int IdNguoiDung { get; set; }
    }
}
