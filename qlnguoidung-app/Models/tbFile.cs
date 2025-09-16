using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLNguoiDung.Models;

[Table("tbFile")]
public partial class tbFile
{

    [Key]
    [StringLength(36)]
    public string uid { get; set; } = null!;
    [Required]
    public string filename { get; set; }

    [StringLength(10)]
    public string? bucket { get; set; }

    [StringLength(32)]
    public string hash { get; set; }
    public int? pageCount { get; set; }
    [Required]
    public string loaiFile { get; set; }
    public string? fileContent { get; set; }
    // public string? fileSize { get; set; }
}
