using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace QLNguoiDung.Models
{
    [Table("tbSystemUser")]

    public partial class tbSystemUser
    {
        [Key]
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public int? IdCanBo { get; set; }
        public int? IdNhom { get; set; }
        public bool? IsLDAPAccount { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        public string token { set; get; }
        [NotMapped]
        public bool Checked { get; set; }
        [NotMapped]
        public bool IsAuthenticated { get; set; }
    }
}

