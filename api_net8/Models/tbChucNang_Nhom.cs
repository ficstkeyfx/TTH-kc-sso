using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace api.Models
{
    [Table("tbChucNang_Nhom")]
    public partial class tbChucNang_Nhom
    {
        [Key]
        public int IdChucNang_Nhom { get; set; }
        public int? IdNhom { get; set; }
        public int? IdChucNang { get; set; }
        public bool? C { get; set; }
        public bool? R { get; set; }
        public bool? U { get; set; }
        public bool? D { get; set; }

        [ForeignKey(nameof(IdChucNang))]
        [InverseProperty(nameof(tbChucNang.tbChucNang_Nhoms))]
        public virtual tbChucNang IdChucNangNavigation { get; set; }
    }
}
