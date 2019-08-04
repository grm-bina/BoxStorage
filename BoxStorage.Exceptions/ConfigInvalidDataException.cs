using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxStorage.Exceptions
{
    public class ConfigInvalidDataException : Exception
    {
        // when configuration data conflicted whith program-logic
        public ConfigInvalidDataException(string message) : base(message) { }

    }
}
