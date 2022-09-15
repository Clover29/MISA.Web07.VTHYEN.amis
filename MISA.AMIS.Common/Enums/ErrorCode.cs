using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.Common.Enums
{
    /// <summary>
    /// định danh một số lỗi với mã lỗi tương ứng
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Lỗi chưa xác định được
        /// </summary>
        Exception = 1,

        /// <summary>
        /// Lỗi validate dữ liệu k đúng format
        /// </summary>
        Validate = 2,

        /// <summary>
        /// Lỗi trùng mã
        /// </summary>
        DuplicateCode = 3,

    }
}
