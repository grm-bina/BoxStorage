using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxStorage.Exceptions
{
    // try create box-instance's with counter = 0 or less or using nodes which data is null
    public class EmptyBoxSectionException : Exception
    {
        public EmptyBoxSectionException() { }
        public EmptyBoxSectionException(string message) : base(message) { }
    }
}
