using Dapper;
using MISA.AMIS.Common.Entities;
using MISA.AMIS.Common.Ultilities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.DL
{
    public class BaseDL<T> : IBaseDL<T>
    {
        #region Method
        public virtual IEnumerable<T> getAllRecords()
        {
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                string className = typeof(T).Name;
                string storedProcedureName = $"Proc_{className}_GetAll{className}";

                var records = mySqlConnection.Query<T>(storedProcedureName,commandType: System.Data.CommandType.StoredProcedure);

                return records;
            }


        }

        public T GetRecordByID(Guid ID)
        {
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                string className = typeof(T).Name;

                string storedProcedureName = $"Proc_{className}_Get{className}ById";
                var parameters = new DynamicParameters();
                parameters.Add($"@v_{className}ID", ID);
                return mySqlConnection.QueryFirstOrDefault<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

        }

        public int InsertOneRecord(T record)
        {
            string tableName = EntityUltilities.getTableName<T>();
            string storedProcedureName = $"Proc_{tableName}_Insert{tableName}";
            var propertyID = $"v_{tableName}ID";
            var properties = typeof(T).GetProperties();
            var parameters = new DynamicParameters();
            var recordID = Guid.NewGuid();
            Console.WriteLine(recordID);
            foreach (var property in properties)
            {
                string propertyName = $"v_{property.Name}";
                var propertyValue = (propertyName.ToLower() == propertyID.ToLower()) ? recordID : property.GetValue(record);
                parameters.Add(propertyName, propertyValue);
            }
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                return mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

        }

        public int UpdateOneRecord(T record, Guid recordID)
        {
            string tableName = EntityUltilities.getTableName<T>();
            string storedProcedureName = $"Proc_{tableName}_Update{tableName}";
            var propertyID = $"v_{tableName}ID";
            var properties = typeof(T).GetProperties();
            var parameters = new DynamicParameters();

            foreach (var property in properties)
            {
                string propertyName = $"v_{property.Name}";

                var propertyValue = ((propertyName.ToLower() == propertyID.ToLower()) ? recordID : property.GetValue(record));

                parameters.Add(propertyName, propertyValue);
            }
            using (var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString))
            {
                return mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        #endregion

    }
}
