using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace api.Models
{
    [Table("tbChucNang")]
    public partial class tbChucNang
    {
        public tbChucNang()
        {
            tbChucNang_Nhoms = new HashSet<tbChucNang_Nhom>();
        }

        [Key]
        public int IdChucNang { get; set; }
        [StringLength(200)]
        public string DuongDan { get; set; }
        [StringLength(200)]
        public string TenChucNang { get; set; }
        public int? ChucNangCha { get; set; }
        [StringLength(200)]
        public string BieuTuong { get; set; }
        public int? STTMuc { get; set; }
        [StringLength(500)]
        public string MoTa { get; set; }
        public int? STT { get; set; }

        [InverseProperty(nameof(tbChucNang_Nhom.IdChucNangNavigation))]
        public virtual ICollection<tbChucNang_Nhom> tbChucNang_Nhoms { get; set; }
    }
}
