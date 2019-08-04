using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Models;
using BoxStorage.Exceptions;
using BoxStorage.Globals;

namespace BoxStorage.BL
{   //  contains boxes sorted by height
    internal sealed class SubTree : AbstractTree<Box> 
    {
        #region Data
        private Node<SubTree> _creator;
        #endregion

        #region Ctor
        public SubTree(Box firstBox, Node<SubTree> creator)
        {
            _creator = creator;
            Insert(firstBox);
        }

        #endregion

        #region Prop
        // get one of boxes to main-tree for compare
        public Box PeekRoot
        {
            get
            {
                if (!IsEmpty)
                    return _root.Data;
                else return null;
            }
        }

        #endregion

        #region Insert
        // implementation for abstract method to insert new box to storage
        protected override Node<Box> Insert(Box newBox, ref Node<Box> currentNode)
        {
            if (currentNode == null)
            {
                Respone<Box> feedback = new Respone<Box>();
                if (newBox.Quantity > FromConfig.MaxBoxesSameSize)
                {
                    feedback.IsWorking = false;
                    if (FromConfig.MaxBoxesSameSize > 0)
                    {
                        newBox.Quantity = FromConfig.MaxBoxesSameSize;
                        feedback.Message = $"Tried insert more boxes than maximum capacity. Inserted only {newBox.Quantity} boxes";
                    }
                    else throw new ConfigInvalidDataException("Configuration error: maximum capasity is 0 or less!");
                }
                else
                    feedback.IsWorking = true;

                currentNode = new CombinedNode(_creator) { Data = newBox };
                LastUsedBox = (CombinedNode)currentNode;

                feedback.Data = currentNode.Data;
                InsertFeedback = feedback;
            }
            else if (Compare(currentNode.Data, newBox) < 0)
                Insert(newBox, ref currentNode.Right).Root = currentNode;
            else if (Compare(currentNode.Data, newBox) > 0)
                Insert(newBox, ref currentNode.Left).Root = currentNode;
            else
            {
                Respone<Box> feedback = new Respone<Box>();
                if((currentNode.Data.Quantity + newBox.Quantity) > FromConfig.MaxBoxesSameSize)
                {
                    if(FromConfig.MaxBoxesSameSize<=0)
                        throw new ConfigInvalidDataException("Configuration error: maximum capasity is 0 or less!");

                    if (FromConfig.MaxBoxesSameSize < currentNode.Data.Quantity)
                        throw new ConfigInvalidDataException("Configuration error: quantity of existing boxes is more then maximum capacity");

                    feedback.IsWorking = false;
                    if (FromConfig.MaxBoxesSameSize == currentNode.Data.Quantity)
                        feedback.Message = "Can't insert boxes of this size: it's already maximum quantity";
                    else
                    {
                        feedback.Message = $"Tried insert more boxes than maximum capacity. Inserted only {FromConfig.MaxBoxesSameSize - currentNode.Data.Quantity} box(es)";
                        currentNode.Data.Quantity = FromConfig.MaxBoxesSameSize;
                    }

                }
                else
                {
                    currentNode.Data.Quantity += newBox.Quantity;
                    feedback.IsWorking = true;
                }

                feedback.Data = currentNode.Data;
                InsertFeedback = feedback;
                LastUsedBox = null;
            }
            return currentNode;
        }
        #endregion

        #region PullBox
        // methods for get 1 box with correct height to gift (search the box with the same height or greater, get data, decrease counter, refresh last-used node, if it ended boxes - remove node)

        // protected logic searching correct box (implementation for abstract base-method)
        protected override Box Search(Gift gift, Node<Box> currentNode)
        {
            // nothing founded
            if (currentNode == null)
                return null;
            // this is exactly the box!
            else if (Compare(gift, currentNode.Data) == 0)
                return PullBox(gift, currentNode);
            // need search biger box
            else if (Compare(gift, currentNode.Data) > 0)
                return Search(gift, currentNode.Right);
            // this is to big box
            else
            {
                // try find smaller box
                Box smaller = Search(gift, currentNode.Left);
                if (smaller != null)
                    return smaller;
                // ok, biger box is not bad, get it
                else
                    return PullBox(gift, currentNode);
            }
        }

        // get data, decrease counter, refresh last-used node, if it ended boxes - remove node (implementation for abstract base-method)
        protected override Box PullBox(Gift gift, Node<Box> correctNode)
        {
            Box temp = correctNode.Data;
            if (gift.Bottom <= temp.Bottom && gift.Height <= temp.Height)
            {
                correctNode.Data.Quantity--;
                LastUsedBox = (CombinedNode)correctNode;
                if (correctNode.Data.IsEmpty)
                    DelAndReconnect(correctNode);
                return temp;
            }
            else return null;
        }



        #endregion

        #region Delete
        // Public remove by CombynedNode (for queue-use & call by main-tree) - implementation for abstract-method
        public override void Delete(CombinedNode removingNode)
        {
            if (removingNode.Data == null)
                throw new EmptyBoxSectionException("Error: tried delete node without data from sub-tree");

            DelAndReconnect(removingNode);
        }
        #endregion

        #region Get All Data In Order
        // Get list of all data sorted (for printing)
        // implementation of abstract-method
        protected override void InOrder(Node<Box> currentNode, ref List<Box> list)
        {
            if (currentNode == null)
                return;

            InOrder(currentNode.Left, ref list);
            list.Add(currentNode.Data);
            InOrder(currentNode.Right, ref list);
        }
        #endregion

        #region IComparer
        // compare by height (get Box as Gift for find box by height into sub-BST, that sorted by bottom)
        public override int Compare(Gift x, Gift y)
        {
            if (x.Height < y.Height)
                return -1;
            else if (x.Height > y.Height)
                return 1;
            else
                return 0;
        }
        #endregion
    }
}
