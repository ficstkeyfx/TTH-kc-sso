namespace QLNguoiDung.Dto
{
    public class UserDto
    {
        public int IdUser { get; set; }
        public int? IdNhom { get; set; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
