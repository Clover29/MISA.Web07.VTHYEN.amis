using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Dapper;

namespace MISA.Web07.YenVTH.amis.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// API Lấy toàn bộ danh sách phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban</returns>
        /// Created by: VTHYEN (16/08/2022)
        //[HttpGet]
        //[SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<Department>))]
        //[SwaggerResponse(StatusCodes.Status400BadRequest)]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError)]
        //public IActionResult GetAllDepartments()
        //{
        //    try
        //    {
        //        string connectionString = "Server=localhost;Port=3306;Database=misa.web07.ctm.vthyen;Uid=root;Pwd=phancaoky1311@gmail.com";
        //        var mySqlConnection = new MySqlConnector.MySqlConnection(connectionString);

        //        string getAllDepartmentsCommand = "SELECT * FROM department;";

        //        var departments = mySqlConnection.Query<Department>(getAllDepartmentsCommand);

        //        if (departments != null)
        //        {
                   
        //            return StatusCode(StatusCodes.Status200OK, departments);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status400BadRequest, "e002");
        //        }
        //    }
        //    catch (Exception exception)
        //    {

        //        Console.WriteLine(exception.Message);
        //        return StatusCode(StatusCodes.Status400BadRequest, "e001");
        //    }
           
        //}
    }
}
