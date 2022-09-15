using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Swashbuckle.AspNetCore.Annotations;
using MySqlConnector;
using MISA.AMIS.BL;
using MISA.AMIS.Common.Entities.DTO;
using MISA.AMIS.Common.Entities;
using MISA.AMIS.Common.Enum;
using MISA.AMIS.CTM.VTHYen.ErrorsLog;
using System.Collections;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Salesforce.Common.Models.Json;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Data.Common;

namespace MISA.AMIS.CTM.VTHYen.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : BasesController<Employee>
    {
        #region Field

        private IEmployeeBL _employeeBL;
        #endregion

        #region Constructor

        public EmployeesController(IEmployeeBL employeeBL) : base(employeeBL)
        {
            _employeeBL = employeeBL;
        }

        #endregion

        #region Method

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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PagingData<Employee>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult FillterEmployees([FromQuery] string? keyword, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var pagingData = _employeeBL.FillterEmployees(keyword, pageSize, pageNumber);

                if (pagingData != null)
                {

                    return StatusCode(StatusCodes.Status200OK, pagingData);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(exception, HttpContext));
            }

        }

        /// <summary>
        /// Lấy mã nhân viên tự động tăng
        /// </summary>
        /// <returns>
        /// Mã nhân viên tự động tăng
        /// </returns>
        ///  Created by: VTHYEN (16/08/2022)
        [HttpGet("new-code")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetNewEmployeeCode()
        {
            try
            {
                string newEmployeeCode = _employeeBL.GetNewEmployeeCode();
                return StatusCode(StatusCodes.Status200OK, newEmployeeCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(exception, HttpContext));
            }

        }

        /// <summary>
        /// API Xóa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn xóa</param>
        /// <returns>ID của nhân viên vừa xóa</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpDelete("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {
                int numberOfAffectedRows = _employeeBL.DeleteEmployeeByID(employeeID);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, HandleError.GetExecptionResult(exception, HttpContext));
            }

        }

        /// <summary>
        /// API Xóa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn xóa</param>
        /// <returns>ID của nhân viên vừa xóa</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteMultipleEmployeeByID([FromBody] List<string> employeeIds)
        {
            try
            {
                int numberOfAffectedRows = _employeeBL.DeleteMultipleEmployeeByID(employeeIds);
                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, numberOfAffectedRows);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(exception, HttpContext));
            }

        }

        [HttpGet("exportExcel")]
        public ActionResult Export([FromQuery] string? keyword)
        {
            var list = _employeeBL.FillterEmployees(keyword, -1, 1).Data;

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DANH SACH NHAN VIEN");
                // workSheet.Cells.LoadFromCollection(list, true);
                BindingFormatForExcel(workSheet, list);
                package.Save();
            }
            stream.Position = 0;
            string excelName = "Danh sach nhan vien.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        private void BindingFormatForExcel(ExcelWorksheet worksheet, List<Employee> employees)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 20;
            worksheet.Column(1).Width = 5;
            worksheet.Column(9).Width = 35;
            worksheet.Column(5).Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.DefaultRowHeight = 45;
            worksheet.Cells.Style.WrapText = true;

            using (var range = worksheet.Cells["A1:J1"])
            {
                range.Merge = true;
                range.Value = "DANH SÁCH NHÂN VIÊN";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.SetFromFont("Arial", 16, true);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Tạo headers
            worksheet.Cells[3, 1].Value = "STT";
            worksheet.Cells[3, 2].Value = "MÃ NHÂN VIÊN";
            worksheet.Cells[3, 3].Value = "TÊN NHÂN VIÊN";
            worksheet.Cells[3, 4].Value = "GIỚI TÍNH";
            worksheet.Cells[3, 5].Value = "NGÀY SINH";
            worksheet.Cells[3, 6].Value = "CHỨC DANH";
            worksheet.Cells[3, 7].Value = "TÊN ĐƠN VỊ";
            worksheet.Cells[3, 8].Value = "SỐ TÀI KHOẢN";
            worksheet.Cells[3, 9].Value = "TÊN NGÂN HÀNG";
            worksheet.Cells[3, 10].Value = "CHI NHÁNH";
            for (int j = 0; j < 10; j++)
            {
                worksheet.Cells[3, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            // Lấy range vào tạo format cho range đó ở đây là từ A1 tới D1
            using (var range = worksheet.Cells["A3:J3"])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                // Set Màu cho Background
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Set Font cho text  trong Range hiện tại
                range.Style.Font.SetFromFont("Arial", 10, true);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Đỗ dữ liệu từ list vào 
            for (int i = 0; i < employees.Count; i++)
            {
                var item = employees[i];
                int row = i + 4;
                worksheet.Cells[row, 1].Value = i + 1;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[row, 2].Value = item.EmployeeCode;
                worksheet.Cells[row, 3].Value = item.EmployeeName;
                worksheet.Cells[row, 4].Value = formatGender((Gender)item.Gender);
                worksheet.Cells[row, 5].Value =item.DateOfBirth;
                worksheet.Cells[row, 6].Value = item.PositionName;
                worksheet.Cells[row, 7].Value = item.DepartmentName;
                worksheet.Cells[row, 8].Value = item.BankAccount;
                worksheet.Cells[row, 9].Value = item.BankName;
                worksheet.Cells[row, 10].Value = item.BankBranch;
                for(int j = 0; j < 10; j++)
                {
                    worksheet.Cells[row, j+1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[row, j + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }
        }
        private string formatGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return "Nam";
                    break;
                case Gender.Female:
                    return "Nữ";
                    break;
                default:
                    return "Khác";
                    break;
            }
        }
        #endregion
    }
}
