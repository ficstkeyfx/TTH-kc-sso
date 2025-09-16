using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
namespace api.Services.GeneralAPIServices
{
    public class APIService<TItem> : IAPIService<TItem> where TItem : class
    {
        private readonly dbAPIContext _context;
        private readonly DbSet<TItem> _dbSet;
        private static SqlConnection _db;
        public APIService(dbAPIContext context, SqlConnection db)
        {
            _db = db;
            _context = context;
            _dbSet = context.Set<TItem>();

        }
        /// <summary>
        /// Phương thức lấy thông tin một nhóm người dùng
        /// </summary>
        /// <param name="id">Mã định danh nhóm người dùng</param>
        /// <returns>Thông tin nhóm người dùng</returns>
        public async Task<ServiceResponse<TItem>> GetById(int id)
        {
            var response = new ServiceResponse<TItem>();
            try
            {
                //TItem item = await _context.tbNhoms.FirstOrDefaultAsync(item => item.IdNhom == id) ?? new tbNhom();
                //if (nhom.IdNhom == 0)
                //{
                //    throw new Exception($"Chức năng không tồn tại");
                //}
                //response.Data = nhom;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;

        }

        /// <summary>
        /// Cài đặt phương thức lấy thông tin toàn bộ các nhóm người dùng
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số bản ghi hiển thị trên trang</param>
        /// <returns>DS các nhóm người dùng</returns>
        public async Task<ServiceResponse<PageDataReturn<TItem>>> GetAll(int pageNumber, int pageSize, string lamda)
        {
            var response = new ServiceResponse<PageDataReturn<TItem>>();
            try
            {
                //Đoạn code này chuyển một chuổi biểu thức Lamda sang biểu thức Lamda
                //filterCon = "p => p.IdNguoi>55 && p.HoTen==\"Nguyễn Đình Nghĩa\"";
                //var options = ScriptOptions.Default.AddReferences(typeof(TItem).Assembly);
                //Func<TItem, bool> filterExpression = await CSharpScript.EvaluateAsync<Func<TItem, bool>>(filterCon, options);
                //if (lamda != "\" \"")
                //{
                //    lamda = string.Join("", lamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    strLamda = string.Join("", strLamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    lamda = lamda.Replace(strLamda, lamdaVal);
                //}

                // this will be used in all querys; add 'where' clauses etc if you want


                IQueryable<TItem> data;
                data = (lamda == "\" \"") ? data = _dbSet.AsQueryable() : _dbSet.Where(lamda).AsQueryable();
                PagedList<TItem> list = await PagedList<TItem>.ToPagedList(data, pageNumber, pageSize);
                response.Data = new PageDataReturn<TItem>() { Items = list, TotalCount = list.TotalCount };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức lấy thông tin toàn bộ các nhóm người dùng
        /// </summary>
        /// <returns>DS các nhóm người dùng</returns>
        public async Task<ServiceResponse<List<TItem>>> GetAll(string lamda = "")
        {
            var response = new ServiceResponse<List<TItem>>();
            try
            {
                //if (lamda != "\" \"")
                //{
                //    lamda = string.Join("", lamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    strLamda = string.Join("", strLamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    lamda = lamda.Replace(strLamda, lamdaVal);
                //}
                List<TItem> data;
                data = (lamda == "\" \"") ? data = await _dbSet.ToListAsync() : await _dbSet.Where(lamda).ToListAsync();
                response.Data = data;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<object>>> GetByColumns(string columns = "", string lamda = "")
        {
            var response = new ServiceResponse<List<object>>();
            try
            {
                List<object> data;
                // Select only properties in query to get data
                // Convert to using ternary operator to check if columns is empty or not

                if (columns != "")
                {
                    data = (lamda == "\" \"") ? await _dbSet.Select($"new ({columns})").Cast<object>().ToListAsync() : await _dbSet.Where(lamda).Select($"new ({columns})").Cast<object>().ToListAsync();

                }
                else
                {
                    data = (lamda == "\" \"") ? await _dbSet.Cast<object>().ToListAsync() : await _dbSet.Where(lamda).Cast<object>().ToListAsync();
                }
                response.Data = data;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        /// <summary>
        /// Cài đặt phương thức lấy thông tin toàn bộ các nhóm người dùng
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số bản ghi hiển thị trên trang</param>
        /// <returns>DS các nhóm người dùng</returns>
        public async Task<ServiceResponse<PageDataReturn<object>>> GetAll1(int pageNumber, int pageSize, string columns, string lamda)
        {
            var response = new ServiceResponse<PageDataReturn<object>>();
            try
            {
                //Đoạn code này chuyển một chuổi biểu thức Lamda sang biểu thức Lamda
                //filterCon = "p => p.IdNguoi>55 && p.HoTen==\"Nguyễn Đình Nghĩa\"";
                //var options = ScriptOptions.Default.AddReferences(typeof(TItem).Assembly);
                //Func<TItem, bool> filterExpression = await CSharpScript.EvaluateAsync<Func<TItem, bool>>(filterCon, options);
                //if (lamda != "\" \"")
                //{
                //    lamda = string.Join("", lamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    strLamda = string.Join("", strLamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //    lamda = lamda.Replace(strLamda, lamdaVal);
                //}

                // this will be used in all querys; add 'where' clauses etc if you want


                IQueryable<object> data;
                //data = (lamda == "\" \"") ? data = _dbSet.AsQueryable() : _dbSet.Where(lamda).AsQueryable();

                // Select only properties in query to get data
                // Convert to using ternary operator to check if columns is empty or not

                if (columns != "")
                {
                    data = (lamda == "\" \"") ? _dbSet.Select($"new ({columns})").Cast<object>().AsQueryable() : _dbSet.Where(lamda).Select($"new ({columns})").Cast<object>().AsQueryable();

                }
                else
                {
                    data = (lamda == "\" \"") ? _dbSet.Cast<object>().AsQueryable() : _dbSet.Where(lamda).Cast<object>().AsQueryable();
                }

                PagedList<object> list = await PagedList<object>.ToPagedList(data, pageNumber, pageSize);
                response.Data = new PageDataReturn<object>() { Items = list, TotalCount = list.TotalCount };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức tạo một nhóm người dùng mới
        /// </summary>
        /// <param name="record">Dữ liệu nhóm mới</param>
        /// <returns>True nếu tạo thành công; Flase nếu không thành công</returns>
        public async Task<ServiceResponse<bool>> Create(TItem item)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _dbSet.AddAsync(item);
                await _context.SaveChangesAsync();
                response.Data = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức tạo một nhóm người dùng mới
        /// </summary>
        /// <param name="record">Dữ liệu nhóm mới</param>
        /// <returns>True nếu tạo thành công; Flase nếu không thành công</returns>
        public async Task<ServiceResponse<bool>> Create(List<TItem> items)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _dbSet.AddRangeAsync(items);
                await _context.SaveChangesAsync();
                response.Data = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức cập nhật thông tin nhóm người dùng
        /// </summary>
        /// <param name="record">Thông tin nhóm người dùng cần cập nhật</param>
        /// <returns>true:cập nhật thành công; false:cập nhật không thành công</returns>
        public async Task<ServiceResponse<bool>> Update(TItem item)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                _dbSet.Update(item);
                await _context.SaveChangesAsync();
                response.Data = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức cập nhật thông tin nhóm người dùng
        /// </summary>
        /// <param name="record">Thông tin nhóm người dùng cần cập nhật</param>
        /// <returns>true:cập nhật thành công; false:cập nhật không thành công</returns>
        public async Task<ServiceResponse<bool>> Update(List<TItem> items)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                _dbSet.UpdateRange(items);
                await _context.SaveChangesAsync();
                response.Data = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
        /// <summary>
        /// Cài đặt phương thức xóa một nhóm người dùng
        /// </summary>
        /// <param name="record">Thông tin nhóm người dùng cần xóa</param>
        /// <returns>true: xóa thành công; flase: xóa không thành công</returns>
        public async Task<ServiceResponse<bool>> Delete(string lamda)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                //lamda = string.Join("", lamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //strLamda = string.Join("", strLamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //lamda = lamda.Replace(strLamda, lamdaVal);
                _dbSet.RemoveRange(_dbSet.Where(lamda).ToList());
                await _context.SaveChangesAsync();
                response.Data = true;

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;

        }


        public async Task<ServiceResponse<bool>> Delete(List<TItem> items)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                //lamda = string.Join("", lamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //strLamda = string.Join("", strLamda.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                //lamda = lamda.Replace(strLamda, lamdaVal);
                _dbSet.RemoveRange(items);
                await _context.SaveChangesAsync();
                response.Data = true;

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;

        }
        public async Task<ServiceResponse<int>> GetId(string seqName)
        {
            var response = new ServiceResponse<int>();
            try
            {
                response.Data = _db.ExecuteScalar<int>($"SELECT NEXT VALUE FOR {seqName}");

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
