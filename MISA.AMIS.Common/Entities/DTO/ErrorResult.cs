using MISA.AMIS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.Common.Entities.DTO
{
    /// <summary>
    /// Thông tin lỗi trả về cho client
    /// </summary>
    public class ErrorResult
    {
        #region Property

        /// <summary>
        /// Mã lỗi được định danh
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Thông báo gửi tới người dùng
        /// </summary>
        public string? UserMsg { get; set; }

        /// <summary>
        /// Thông báo gửi về phía dev
        /// </summary>
        public object? DevMsg { get; set; }

        /// <summary>
        /// Thông tin chi tiết hơn về lỗi (đường dẫn mô tả về lỗi)
        /// </summary>
        public string? MoreInfor { get; set; }

        /// <summary>
        /// Mã để tra cứu thông tin log: ELK, file log,...
        /// </summary>
        public string? TraceId { get; set; }

        #endregion

        #region Constructor
        public ErrorResult()
        {
        }

        public ErrorResult(ErrorCode errorCode, string? userMsg, object? devMsg, string? moreInfor, string? traceId)
        {
            ErrorCode = errorCode;
            UserMsg = userMsg;
            DevMsg = devMsg;
            MoreInfor = moreInfor;
            TraceId = traceId;
        }

        #endregion
    }
}
