using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace QLNguoiDung.Models
{
    [Table("tbChucNang_NguoiDung")]
    public partial class tbChucNang_NguoiDung
    {
        [Key]
        public int IdChucNang_NguoiDung { get; set; }
        public int? IdNguoiDung { get; set; }
        public int? IdChucNang { get; set; }
        public bool? C { get; set; }
        public bool? R { get; set; }
        public bool? U { get; set; }
        public bool? D { get; set; }
    }
}
