namespace api.Services.TVFServices
{
    public interface ITVFServices
    {
        //Lấy danh sách các chức năng của người dùng có mà định danh là IdUser
        Task<ServiceResponse<List<tbChucNang>>> getChucNang(int IdUser);
        Task<ServiceResponse<List<tbUser>>> getNguoiDungThuocNhom(int IdNhom);
    }
}