using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxStorage.Exceptions
{
    public class InvalidSizeException : Exception
    {
        public InvalidSizeException() : base() { }

        public override string Message
        {
            get
            {
                return "Error: size must be great than 0 & entered by using digits only";
            }
        }
    }
}
