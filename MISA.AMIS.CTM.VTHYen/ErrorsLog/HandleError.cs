
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MISA.AMIS.Common.Enums;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.AMIS.Common.Entities.DTO;

namespace MISA.AMIS.CTM.VTHYen.ErrorsLog
{
    /// <summary>
    /// Class chứa các hàm xử lý lỗi khi gọi API
    /// </summary>
    public static class HandleError
    {
        /// <summary>
        /// Lấy ra các lỗi xảy ra khi validate một Entity
        /// </summary>
        /// <param name="modelState"> Đối tượng modelState hứng được khi gọi API</param>
        /// <param name="httpContext">Context khi gọi Api, sử dụng để lấy được traceId </param>
        /// <returns>đối tượng chứa thông tin lỗi</returns>
        /// Created by: VTHYEN (25/08/2022)
        public static ErrorResult? ValidateEntity(ModelStateDictionary modelState, HttpContext httpContext)
        {
            if (!modelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var state in modelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                var errorResult = new ErrorResult(
                    ErrorCode.Validate, 
                    "Dữ liệu không hợp lệ",
                    errors,
                    "https://openapi.misa.com.vn/errorcode/e002",
                    Activity.Current?.Id ?? httpContext?.TraceIdentifier);
                return errorResult;
            }
            return null;
        }
        /// <summary>
        /// Đối tượng trả về lỗi khi gặp exception chưa dự đoán được
        /// </summary>
        /// <param name="exception">exception gặp phải</param>
        /// <param name="httpContext">Context khi gọi Api, sử dụng để lấy được traceId </param>
        /// <returns>đối tượng chứa thông tin lỗi</returns>
        /// Created by: VTHYEN (25/08/2022)
        public static ErrorResult? GetExecptionResult(Exception exception, HttpContext httpContext)
        {
            Console.WriteLine(exception.Message);
            return new ErrorResult(
                ErrorCode.Validate, 
                "Có lỗi xảy ra. Vui lòng liên hệ MISA!", 
                "Catched an Exception", 
                "https://openapi.misa.com.vn/errorcode/e002",
                Activity.Current?.Id ?? httpContext?.TraceIdentifier);
        }

        /// <summary>
        /// Đối tượng trả về lỗi khi bị trùng code
        /// </summary>
        /// <param name="mySqlException">exception gặp phải</param>
        /// <param name="httpContext">Context khi gọi Api, sử dụng để lấy được traceId </param>
        /// <returns>đối tượng chứa thông tin lỗi</returns>
        /// Created by: VTHYEN (25/08/2022)
        public static ErrorResult? GetDuplicateCodeErrorResult(MySqlException mySqlException, HttpContext httpContext)
        {
            if(mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                var errorResult = new ErrorResult(
                    ErrorCode.DuplicateCode,
                    "Trùng mã",
                    $"{mySqlException.Message}",
                    "https://openapi.misa.com.vn/errorcode/e003",
                    Activity.Current?.Id ?? httpContext?.TraceIdentifier);
                    return errorResult;
            }
            Console.WriteLine(mySqlException.Message);
            return new ErrorResult(
                ErrorCode.Validate,
               Common.Resources.Resource.error_expception,
                "Catched an exception",
                "https://openapi.misa.com.vn/errorcode/e002",
                Activity.Current?.Id ?? httpContext?.TraceIdentifier);
        }
    }
}
