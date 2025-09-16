using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace api.Models
{
    [Table("tbPhanMem")]
    public partial class tbPhanMem
    {

        [Key]
        public int IdPhanMem { get; set; } 
        public string TenPhanMem { get; set; }
        public string? STT { get; set; }
    }
}
