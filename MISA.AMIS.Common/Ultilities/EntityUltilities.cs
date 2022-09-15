using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.Common.Ultilities
{
    /// <summary>
    /// Class chứa các hàm chung để xử lý các entity
    /// </summary>
    public static class EntityUltilities
    {
        public static string getTableName<T>(){
            string tableName = typeof(T).Name;
            var tableAttributes = typeof(T).GetTypeInfo().GetCustomAttributes<TableAttribute>();
            if (tableAttributes.Count() > 0) {

                tableName = tableAttributes.First().Name;
            }
              return tableName;
        }
    }
}
