using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.AMIS.Common.Entities.DTO
{
    /// <summary>
    /// Dữ liệu trả về khi tìm kiếm và phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của đối tượng trong mảng trả về</typeparam>
    public class PagingData<T>
    {
        /// <summary>
        /// Mảng đối tượng thỏa mãn điều kiện tìm kiếm và phân trang
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Tổng số bản ghi thỏa mãn điều kiện
        /// </summary>
        public long TotalRecord { get; set; }

        /// <summary>
        /// Tổng số trang thỏa mãn điều kiện
        /// </summary>
        public long TotalPage { get; set; }

        /// <summary>
        /// Số bản ghi trong 1 trang hiện tại
        /// </summary>
        public long CurrentPage { get; set; }

        public long CurrentPageRecords { get; set; }
    }
}
