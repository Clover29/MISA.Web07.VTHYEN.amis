using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.BL;
using MISA.AMIS.BL.Exceptions;
using MISA.AMIS.Common.Entities;
using MISA.AMIS.CTM.VTHYen.ErrorsLog;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace MISA.AMIS.CTM.VTHYen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase
    {
        #region Field

        private IBaseBL<T> _baseBL;

        #endregion

        #region Constructor

        public BasesController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// API lấy tất cả nhân viên
        /// </summary>
        /// <returns>Tất Cả nhân viên</returns>
        /// Created By: YenVTH (23/08/2022)
        [HttpGet("getAll")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult getAllRecords()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, _baseBL.GetAllRecords());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(ex, HttpContext));
            }
        }

        /// <summary>
        /// API Lấy thông tin chi tiết của 1 bản ghi
        /// </summary>
        /// <param name="ID">ID của bản ghi muốn lấy thông tin chi tiết</param>
        /// <returns>bản ghi muốn lấy thông tin chi tiết</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpGet("{ID}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetRecordByID([FromRoute] Guid ID)
        {
            try
            {
                var records = _baseBL.GetRecordByID(ID);
                if (records != null)
                {
                    return StatusCode(StatusCodes.Status200OK, records);
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


        /// <summary>
        /// API Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng bản ghi muốn thêm mới</param>
        /// <returns>ID của nhân viên vừa thêm mới</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertOneRecord([FromBody] T record)
        {
            try
            {
                var validateResult = HandleError.ValidateEntity(ModelState, HttpContext);
                if (validateResult != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, validateResult);
                }

                int numberOfAffectedRows = _baseBL.InsertOneRecord(record);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, 1);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
            }
            catch(ValidateException ex)
            {
                // Ghi log:

                var res = new
                {
                    devMsg = ex.Message,
                    userMsg = ex.Data
                };
                return StatusCode(StatusCodes.Status400BadRequest, res);
            }
            catch (MySqlException mySqlException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, HandleError.GetDuplicateCodeErrorResult(mySqlException, HttpContext));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(exception, HttpContext));
            }
        }

        /// <summary>
        /// API Sửa 1 bản ghi
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn sửa</param>
        /// <param name="record">bản ghi muốn sửa</param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// Created by: VTHYEN (16/08/2022)
        [HttpPut("{recordID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateOneRecord([FromBody] T record, [FromRoute] Guid recordID)
        {
            try
            {
                var validateResult = HandleError.ValidateEntity(ModelState, HttpContext);
                if (validateResult != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, validateResult);
                }

                int numberOfAffectedRows = _baseBL.UpdateOneRecord(record, recordID);

                if (numberOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, 1);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
            }
            catch (MySqlException mySqlException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, HandleError.GetDuplicateCodeErrorResult(mySqlException, HttpContext));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, HandleError.GetExecptionResult(exception, HttpContext));
            }

        }
        #endregion
    }
}
