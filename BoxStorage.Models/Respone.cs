using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxStorage.Models
{
    // for get status of action
    public class Respone<T>
    {
        public string Message { get; set; }
        public bool IsWorking { get; set; }
        public T Data { get; set; }
    }
}
