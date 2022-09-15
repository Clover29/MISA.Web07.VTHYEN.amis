using MISA.AMIS.Common.Entities;
using MISA.AMIS.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.BL
{
    public  class BaseBL<T> : IBaseBL<T>
    {
        #region Field

        private IBaseDL<T> _baseDL;

        #endregion

        #region Constructor

        protected BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }
        #endregion

        public virtual IEnumerable<T> GetAllRecords()
        {
            return _baseDL.getAllRecords();
        }

        public T GetRecordByID(Guid ID)
        {
           return _baseDL.GetRecordByID(ID);
        }

        public int InsertOneRecord(T record)
        {
            // validate dữ liệu:
            Validate(record);
            return _baseDL.InsertOneRecord(record);
        }

        public int UpdateOneRecord(T record, Guid recordID)
        {
            return _baseDL.UpdateOneRecord(record, recordID);
        }

        protected virtual void Validate(T record)
        {

        }
    }
}
