using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Exceptions;
using BoxStorage.Models;
using BoxStorage.Globals;
     

namespace BoxStorage.BL
{
    // storage-manager
    // creating only 1 instance
    public sealed class Storage //////////////// temporary public (may be)
    {
        #region Data
        MainTree _storage;
        Queue _queueToTrash;


        // for singleton
        public static Storage _instanse = null;
        private static readonly object _lock = new object();
        #endregion

        #region Singleton  Ctor
        // private constructor
        private Storage()
        {
            FromConfig.LoadConfigData();
            _storage = new MainTree();
            _queueToTrash = new Queue();
        }

        // property that used like public-constructor
        public static Storage GetInstanse
        {
            get
            {
                //  lock allow only 1 thread at a time to access the block of code inside it
                lock (_lock)
                {
                    if (_instanse == null)
                        _instanse = new Storage();
                    return _instanse;
                }
            }
        }
        #endregion

        #region Prop

        // Is empty
        public bool IsEmpty
        {
            get
            {
                return _queueToTrash.IsEmpty;
            }
        }

        #endregion

        #region Methods

        // Insert
        public Respone<Box> Insert(Box box)
        {
            try
            {
                _storage.Insert(box);
                _queueToTrash.Enqueue(_storage.LastUsedBox);
            }
            catch(Exception e)
            {
                Respone<Box> feedback = new Respone<Box>();
                feedback.IsWorking = false;
                feedback.Message = e.Message;
                return feedback;
            }
            return _storage.InsertFeedback;
        }

        // Pull Box
        public Respone<Box> TryPullBox(Gift gift)
        {
            Respone<Box> feedback = new Respone<Box>();
            Box temp;

            try
            {
                if (_storage.TryPullBox(gift, out temp))
                {
                    _queueToTrash.MoveToEnd(_storage.LastUsedBox);
                    feedback.IsWorking = true;
                    if (temp.Quantity == 0)
                        feedback.Message = "This is the last box of this size in the storage!";
                    else if (temp.Quantity <= FromConfig.CriticalMinBoxes)
                        feedback.Message = "The boxes of this size will soon end!";
                    feedback.Data = temp;
                }
                else
                {
                    feedback.IsWorking = false;
                    feedback.Message = "Box not found";
                }
            }
            catch(Exception e)
            {
                feedback.IsWorking = false;
                feedback.Message = e.Message;
            }

            return feedback;
        }

        // get all data from the tree (for printing)
        public List<Box> StorageInOrder()
        {
            return _storage.InOrder();
        }

        // get all data from queue (for printing)
        public List<Box> QueueToTrashInOrder()
        {
            return _queueToTrash.InOrder();
        }

        // Delete unused boxes from queue and storage (return deleted boxes)
        public Respone<List<Box>> MoveToTrash()
        {
            Respone<List<Box>> feedback = new Respone<List<Box>>();
            feedback.Data = new List<Box>();

            while (!IsEmpty)
            {
                // check and delete first box in queue if in unused too long time
                TimeSpan unused = DateTime.Now.Subtract(_queueToTrash.PeekHead.LastUse);
                if (TimeSpan.Compare(unused, FromConfig.UnusingBoxMaxTime) >= 0)
                {
                    feedback.Data.Add(_queueToTrash.PeekHead);
                    _storage.Delete(_queueToTrash.Dequeue());
                }
                else
                    break;  // found used-box, finish checking
            }

            // check if exist deleted boxes in respone and return status of action & message (message - if IsWork - true)
            if (feedback.Data.Count > 0)
            {
                feedback.IsWorking = true;
                feedback.Message = $"Deleted {feedback.Data.Count} unused box(es)";
            }
            else
            {
                feedback.Data = null;
                feedback.IsWorking = false;
            }

            return feedback;
        }
        



        #endregion
    }
}
