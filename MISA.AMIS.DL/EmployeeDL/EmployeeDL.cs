using Dapper;
using MISA.AMIS.Common.Entities;
using MISA.AMIS.Common.Entities.DTO;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace MISA.AMIS.DL
{
    public class EmployeeDL : BaseDL<Employee>, IEmployeeDL
    {
        public PagingData<Employee> FillterEmployees(string? keyword, int pageSize, int pageNumber)
        {

            string storedprocedurename = "proc_employee_GetPaging";

            var parameters = new DynamicParameters();
            parameters.Add("@v_offset", (pageNumber - 1) * pageSize);
            parameters.Add("@v_limit", pageSize);
            parameters.Add("@v_sort", "modifieddate desc");
            var orconditions = new List<string>();
            string whereclause = "";
            var pagingData = new PagingData<Employee>();
            if (keyword != null)
            {
                orconditions.Add($"employeecode like '%{keyword}%'");
                orconditions.Add($"employeename like '%{keyword}%'");
                orconditions.Add($"MobilePhoneNumber like '%{keyword}%'");
                orconditions.Add($"LandlinePhoneNumber like '%{keyword}%'");
            }
            if (orconditions.Count > 0)
            {
                whereclause = $"({string.Join(" or ", orconditions)})";

            }
            parameters.Add("@v_where", whereclause);

            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                var multipleResults = mySqlConnection.QueryMultiple(storedprocedurename, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (multipleResults != null)
                {
                    pagingData.Data = multipleResults.Read<Employee>().ToList();
                    pagingData.TotalRecord = multipleResults.Read<long>().Single();
                    if (pagingData.TotalRecord % pageSize != 0)
                    {
                        pagingData.TotalPage = (int)((pagingData.TotalRecord / pageSize) + 1);
                    }
                    else
                    {
                        pagingData.TotalPage = (int)pagingData.TotalRecord / pageSize;
                    }
                    pagingData.CurrentPage = pageNumber;
                    pagingData.CurrentPageRecords = pageSize;

                }
                return pagingData;
            }

        }
        public string GetNewEmployeeCode()
        {
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                string storedProcedureName = "Proc_employee_GetMaxCode";
                string maxEmployeeCode = mySqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
                return "NV-" + (maxEmployeeCode.ToString());
            }

        }

        public int DeleteEmployeeByID(Guid employeeID)
        {
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                string storedProcedureName = "Proc_employee_DeleteEmployeeById";

                var parameters = new DynamicParameters();
                parameters.Add("v_EmployeeID", employeeID);

                return mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public int DeleteMultipleEmployeeByID(List<string> employeeIds)
        {
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                string storedProcedureName = "Proc_employee_DeleteMultipleEmployeeById";

                var parameters = new DynamicParameters();
                string employeeID = $"('{string.Join("', '", employeeIds)}')";
             
                parameters.Add("@v_ListEmployeeID", employeeID);
                var affetedRow = mySqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                return affetedRow;
            }
        }
    }
}
