using MISA.AMIS.Common.Entities.DTO;
using MISA.AMIS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.AMIS.BL;

namespace MISA.AMIS.BL 
{
    public interface IEmployeeBL: IBaseBL<Employee>
    {
        /// <summary>
        /// API Lấy danh sách nhân viên cho phép lọc và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa muốn tìm kiếm</param> 
        /// <param name="pageSize">Số trang muốn lấy</param>
        /// <param name="pageNumber">Thứ tự trang muốn lấy</param>
        /// <returns>Một đối tượng gồm:
        /// + Danh sách nhân viên thỏa mãn điều kiện lọc và phân trang
        /// + Tổng số nhân viên thỏa mãn điều kiện</returns>
        /// Created by: VTHYEN (16/08/2022)

        public PagingData<Employee> FillterEmployees(string? keyword, int pageSize = 10, int pageNumber = 1);

        /// <summary>
        /// Lấy mã nhân viên tự động tăng
        /// </summary>
        /// <returns>
        /// Mã nhân viên tự động tăng
        /// </returns>
        ///  Created by: VTHYEN (16/08/2022)
        public string GetNewEmployeeCode();

        /// <summary>
        /// API Xóa 1 bản ghi nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên cần xóa</param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// Created by: VTHYEN (16/08/2022)
        public int DeleteEmployeeByID(Guid employeeID);

        /// <summary>
        /// API Xóa nhiều bản ghi nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên cần xóa</param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// Created by: VTHYEN (05/09/2022)
        public int DeleteMultipleEmployeeByID(List<string> employeeIds);
    }
}
