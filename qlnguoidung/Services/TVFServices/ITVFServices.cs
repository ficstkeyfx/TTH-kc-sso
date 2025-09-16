using QLNguoiDung.Dto;
using QLNguoiDung.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace QLNguoiDung.Services.ITVFServices
{
    public interface ITVFServices
    {
        Task<List<tbChucNang>> getChucNang(string apiPath,int IdUser);     // GET request
        Task<List<tbUser>> getNguoiDungThuocNhom(string apiPath, int IdNhom);     // GET request
    }
}
