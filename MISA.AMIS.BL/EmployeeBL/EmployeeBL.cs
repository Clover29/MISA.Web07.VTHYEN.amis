using MISA.AMIS.Common.Entities.DTO;
using MISA.AMIS.Common.Entities;
using MISA.AMIS.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.AMIS.DL;
using MISA.AMIS.BL;
using MISA.AMIS.BL.Exceptions;

namespace MISA.AMIS.BL
{
    public class EmployeeBL : BaseBL<Employee>, IEmployeeBL
    {
        #region Feild
        List<string> Errors = new List<string>();
        private IEmployeeDL _employeeDL;

        #endregion

        #region Constructor

        public EmployeeBL(IEmployeeDL employeeDL) : base(employeeDL)
        {
            _employeeDL = employeeDL;
        }

        protected override void Validate(Employee record)
        {
            // mã nhân không được phép trùng:
            Errors.Add("Mã nhân viên không được phép trùng");
            
            // Ngày sinh không được lớn hơn ngày hiện tại:
            Errors.Add("Ngày sinh không được lớn hơn ngày hiện tại");

            if (Errors.Count > 0)
            {
                throw new ValidateException(Errors);
            }

        }

        #endregion

        public PagingData<Employee> FillterEmployees(string? keyword, int pageSize = 10, int pageNumber = 1)
        {
            return _employeeDL.FillterEmployees(keyword,pageSize,pageNumber);
        }

        public string GetNewEmployeeCode()
        {
            return _employeeDL.GetNewEmployeeCode();
        }

        public int DeleteEmployeeByID(Guid employeeID)
        {
            return _employeeDL.DeleteEmployeeByID(employeeID);
        }

        public int DeleteMultipleEmployeeByID(List<string> employeeIds)
        {
            return _employeeDL.DeleteMultipleEmployeeByID(employeeIds);
        }
    }
}
