using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.BL.Exceptions
{
    public class ValidateException:Exception
    {
        IDictionary ErrorMessage;
        public ValidateException(List<string> message)
        {
            ErrorMessage = new Dictionary<string, List<string>>();
            ErrorMessage.Add("error",message);
        }

        public override IDictionary Data => this.ErrorMessage;
    }
}
