using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Exceptions;

namespace BoxStorage.Models
{
    // base-class for Boxes (use as is for user's query to search box for the gift)
    public class Gift
    {
        // size
        public int Bottom { get; }  //size of botoom's 1 side
        public int Height { get; }

        // constrtuctor
        public Gift(int bottom, int height)
        {
            if (bottom <= 0 || height <= 0)
                throw new InvalidSizeException();
            Bottom = bottom;
            Height = height;
        }


        // get info
        public override string ToString()
        {
            return $"Bottom:{Bottom}X{Bottom} Height:{Height}";
        }

    }
}
