using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Exceptions;
using BoxStorage.Globals;

namespace BoxStorage.Models
{
    // derived-class form Gift. It's exactly the boxes that contains & manage the storage
    public class Box : Gift
    {
        #region Data
        // number of boxes with the same size 
        private int _count;

        #endregion

        #region Ctor

        //  constructor
        public Box(int bottom, int height, int quantity=1) : base(bottom, height)
        {
            if (quantity <= 0)
                throw new EmptyBoxSectionException("Quantity of new boxes must be great than 0");
            Quantity = quantity;
            LastUse = DateTime.Now;
        }
        #endregion

        #region Prop
        // when the box was taken last time (if not taken - when the instance was created)
        public DateTime LastUse { get; set; }

        // number of boxes with the same size 
        public int Quantity
        {
            get { return _count; }
            set
            {
                // when box taked - refresh last-use-date
                if(value<_count)
                    LastUse = DateTime.Now;

                if (value <= 0)
                {
                    _count = 0;
                }
                else
                    _count = value;
            }
        }

        // Check if box-section is empty
        public bool IsEmpty
        {
            get
            {
                if (_count == 0)
                    return true;
                else return false;
            }
        }
        #endregion


        #region Methods

        // get info
        public override string ToString()
        {
            return base.ToString() + $" Quantity-in-the-Storage:{Quantity} Last-Use:{LastUse.Date.Day}/{LastUse.Date.Month}/{LastUse.Date.Year} {LastUse.Hour}:{LastUse.Minute}";
        }

        #endregion
    }
}
