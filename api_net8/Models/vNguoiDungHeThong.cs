using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace api.Models
{
    [Table("vNguoiDungHeThong")]

    public partial class vNguoiDungHeThong
    {
        [Key]
        public int IdUser { get; set; }
        public int? IdNhom { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}

