using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLNguoiDung.Models;

[Table("tbNguoiDung")]
public partial class tbNguoiDung
{
    [Key]
    public int IdNguoiDung { get; set; }

    [StringLength(50)]
    public string TenDangNhap { get; set; } = null!;

    [StringLength(64)]
    public string MatKhau { get; set; } = null!;

    public int IdCanBo { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [NotMapped]
    public List<int> ListIdNguoiDung { get; set; } = default!;
}
