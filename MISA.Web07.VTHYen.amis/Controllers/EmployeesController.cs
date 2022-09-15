using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Swashbuckle.AspNetCore.Annotations;
using MySqlConnector;

namespace MISA.Web07.YenVTH.amis.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
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
                string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
                var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

                string storedProcedureName = "Proc_employee_GetPaging";

                var parameters = new DynamicParameters();
                parameters.Add("@v_Offset", (pageNumber - 1) * pageSize);
                parameters.Add("@v_Limit", pageSize);
                parameters.Add("@v_Sort", "ModifiedDate DESC");
                var orConditions = new List<string>();
                string whereClause = "";
                if (keyword != null)
                {
                    orConditions.Add($"EmployeeCode LIKE '%{keyword}%'");
                    orConditions.Add($"EmployeeName LIKE '%{keyword}%'");
                }
                if (orConditions.Count > 0)
                {
                    whereClause = $"({string.Join(" OR ", orConditions)})";
                }
                parameters.Add("@v_Where", whereClause);

                var multipleResults = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (multipleResults != null)
                {
                    var employees = multipleResults.Read<Employee>().ToList();
                    var totalRecord = multipleResults.Read<long>().Single();
                    int totalPage = 0;
                    if (totalRecord % pageSize != 0)
                    {
                        totalPage = (int)((totalRecord / pageSize) + 1);
                    }
                    else
                    {
                        totalPage = (int)totalRecord / pageSize;
                    }
                    return StatusCode(StatusCodes.Status200OK, new PagingData<Employee>()
                    {
                        Data = employees,
                        TotalRecord = totalRecord,
                        TotalPage = totalPage,
                        CurrentPage = pageNumber,
                        CurrentPageRecords = pageSize
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }

        }

        /// <summary>
        /// API Thêm mới 1 nhân viên
        /// </summary>
        /// <param name="employee">Đối tượng nhân viên muốn thêm mới</param>
        /// <returns>ID của nhân viên vừa thêm mới</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
                var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

                string insertEmployeeCommand = "INSERT INTO employee(EmployeeID,EmployeeCode,EmployeeName,DateOfBirth,Gender," +
                    "IndentityNumber,IdentityIssuedDate,IdentityIssuedPlace,Email,Address,MobilePhoneNumber,LandlinePhoneNumber," +
                    "PositionName,DepartmentID,DepartmentName,BankAccount,BankName,BankBranch,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy)" +
                    "VALUES(@EmployeeID ,@EmployeeCode,@EmployeeName,@DateOfBirth,@Gender ,@IdentityNumber,@IdentityIssuedDate," +
                    "@IdentityIssuedPlace,@Email,@Address ,@MobilePhoneNumber ,@LandlinePhoneNumber,@PositionName," +
                    "@DepartmentID,@DepartmentName,@BankAccount,@BankName ,@BankBranch ,@CreatedDate ,@CreatedBy,@ModifiedDate,@ModifiedBy);";

                var employeeID = Guid.NewGuid();
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@Address", employee.Address);
                parameters.Add("@MobilePhoneNumber", employee.MobilePhoneNumber);
                parameters.Add("@LandlinePhoneNumber", employee.LandlinePhoneNumber);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@BankAccount", employee.BankAccount);
                parameters.Add("@BankName", employee.BankName);
                parameters.Add("@BankBranch", employee.BankBranch);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                int numberOfAffectedRows = mySqlConnection.Execute(insertEmployeeCommand, parameters);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (MySqlException mySqlException)
            {
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e003");
                }
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }


        }

        /// <summary>
        /// API Sửa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn sửa</param>
        /// <param name="employee">Đối tượng nhân viên muốn sửa</param>
        /// <returns>ID của nhân viên vừa sửa</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpPut("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateEmployee([FromBody] Employee employee, [FromRoute] Guid employeeID)
        {
            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
                var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

                string updateEmployeeCommand = "UPDATE employee " +
                    "SET EmployeeCode = @EmployeeCode," +
                    "EmployeeName = @EmployeeName," +
                    "DateOfBirth = @DateOfBirth," +
                    "Gender =  @Gender," +
                    "IndentityNumber = @IndentityNumber," +
                    "IdentityIssuedDate =@IdentityIssuedDate," +
                    "IdentityIssuedPlace = @IdentityIssuedPlace," +
                    "Email = @Email," +
                    "Address = @Address," +
                    "MobilePhoneNumber = @MobilePhoneNumber," +
                    "LandlinePhoneNumber = @LandlinePhoneNumber," +
                    "PositionName = @PositionName," +
                    "DepartmentID = @DepartmentID," +
                    "DepartmentName = @DepartmentName," +
                    "BankAccount = @BankAccount," +
                    "BankName = @BankName," +
                    "BankBranch = @BankBranch," +
                    "CreatedDate =  @CreatedDate," +
                    "CreatedBy = @CreatedBy," +
                    "ModifiedDate = @ModifiedDate," +
                    "ModifiedBy = @ModifiedBy" +
                    "WHERE EmployeeID = @EmployeeID;";
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@Address", employee.Address);
                parameters.Add("@MobilePhoneNumber", employee.MobilePhoneNumber);
                parameters.Add("@LandlinePhoneNumber", employee.LandlinePhoneNumber);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@BankAccount", employee.BankAccount);
                parameters.Add("@BankName", employee.BankName);
                parameters.Add("@BankBranch", employee.BankBranch);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                int numberOfAffectedRows = mySqlConnection.Execute(updateEmployeeCommand, parameters);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (MySqlException mySqlException)
            {
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e003");
                }
                Console.WriteLine(mySqlException.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
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
                string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
                var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

                string deleteEmployeeCommand = "DELETE FROM employee WHERE EmployeeID = @EmployeeID";

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);

                int numberOfAffectedRows = mySqlConnection.Execute(deleteEmployeeCommand, parameters);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }

        }

        /// <summary>
        /// API Lấy thông tin chi tiết của 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn lấy thông tin chi tiết</param>
        /// <returns>Đối tượng nhân viên muốn lấy thông tin chi tiết</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpGet("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Employee))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEmployeeByID([FromRoute] Guid employeeID)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new Employee
            {
                EmployeeID = Guid.NewGuid(),
                EmployeeCode = "NV00002",
                EmployeeName = "Nguyễn Minh Anh",
                DateOfBirth = DateTime.UtcNow,
                Gender = Gender.Male,
                IdentityNumber = "135456321",
                IdentityIssuedPlace = "CA Bắc Giang",
                IdentityIssuedDate = DateTime.UtcNow,
                Email = "anhnguyen@gmail.com",
                Address = "Hoàn Kiếm - Hà Nội",
                MobilePhoneNumber = "0355557796",
                LandlinePhoneNumber = "0342209987",
                PositionName = "HR",
                DepartmentID = Guid.NewGuid(),
                DepartmentName = "Khối sản xuất",
                BankAccount = "132165131",
                BankName = "ACB",
                BankBranch = "CN Hà Thành",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "admin",
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "admin"
            });
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
                string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
                var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

                string maxEmployeeCodeCommand = "SELECT MAX(EmployeeCode) FROM employee e";
                string maxEmployeeCode = mySqlConnection.QueryFirstOrDefault<string>(maxEmployeeCodeCommand);
                string newEmployeeCode = "NV" + (Int64.Parse(maxEmployeeCode.Substring(2)) + 1).ToString();
                return StatusCode(StatusCodes.Status200OK, newEmployeeCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }

        }
        #endregion
    }
}
