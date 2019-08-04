using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Models;
using BoxStorage.Exceptions;

namespace BoxStorage.BL
{
    // it's the queue of boxes by last-using time. Not used boxes time-X will deleted. The Queue can NOT create his own Nodes but using CombinedNodes that created in sub-tree.
    internal sealed class Queue
    {
        #region Data
        private CombinedNode _head;
        private CombinedNode _tail;

        #endregion


        #region Queue public interface

        // property for check if its empty
        public bool IsEmpty
        {
            get
            {
                return _head == null;
            }
        }

        // get first box in the queue for deside if delete it or not
        public Box PeekHead
        {
            get
            {
                if (!IsEmpty)
                    return _head.Data;
                else return null;
            }
        }

        // enter new node with box in the gueue (insert tail), check if it exist here already
        public void Enqueue(CombinedNode newNode)
        {
            if (newNode != null)
            {
                // avoid using nodes which data is null
                if (newNode.Data == null)
                    throw new EmptyBoxSectionException("Error: tried enter node without data in the queue");

                // check it it exist by checking his pre and next nodes (+ check if it the same instase of tail and head - it may heppend when the que have only 1 node - so this node have no pre and next)
                if (newNode.Pre == null && newNode.Next == null && newNode != _head && newNode != _tail)
                    InsertTail(newNode);
            }
        }

        // delete first node from the queue (head) and return this node for delete from the tree
        public CombinedNode Dequeue()
        {
            return DeleteHead();
        }

        // get node whith used box that exist in queue and move to the end of queue (if this node is empty - delete)
        public void MoveToEnd(CombinedNode usedNode)
        {
            // avoid using nodes which data is null
            if (usedNode.Data == null)
                throw new EmptyBoxSectionException("Error: tried move or delete node without data in the queue");

            // check if it exist
            if ((usedNode.Pre != null || usedNode.Next != null) || (usedNode == _tail && usedNode == _head))
            {
                // if used-box became empty-section - delete it
                if (usedNode.Data.IsEmpty)
                    Delete(usedNode);

                // if it's not empty and not already in the end
                else if (usedNode != _tail)
                {
                    Delete(usedNode);
                    Enqueue(usedNode);
                }
            }
        }

        // get list of all boxes in the queue (for printing)
        public List<Box> InOrder()
        {
            if (!IsEmpty)
            {
                List<Box> temp = new List<Box>();
                CombinedNode current = _head;
                while (current != null)
                {
                    temp.Add(current.Data);
                    current = current.Next;
                }
                return temp;
            }
            else return null;
        }

        #endregion

        #region LinkedList private logic

        // enter new node with box in the gueue
        private void InsertTail(CombinedNode newNode)
        {
            if (IsEmpty)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                newNode.Pre = _tail;
                _tail = newNode;
            }
        }

        // zeroing pre and next of returning deleted node (if the same instanse wil pushed in the queue again
        private CombinedNode CleanLinks(CombinedNode deleted)
        {
            deleted.Pre = null;
            deleted.Next = null;
            return deleted;
        }

        // delete methods
        // when exist only 1 node in the queue
        private CombinedNode DeleteSingleNode()
        {
            if (_head == _tail)
            {
                CombinedNode temp = _head;
                _tail = null;
                _head = null;

                return CleanLinks(temp);
            }
            else
                return null;
        }

        // delete first node
        private CombinedNode DeleteHead()
        {
            if (!IsEmpty)
            {
                if (_head != _tail)
                {
                    CombinedNode temp = _head;
                    _head.Next.Pre = null;
                    _head = _head.Next;
                    return CleanLinks(temp);
                }
                else return DeleteSingleNode();
            }
            else return null;
        }
        
        // delete last node
        private void DeleteTail()
        {
            if (!IsEmpty)
            {
                if (_head != _tail)
                {
                    CombinedNode temp = _tail;
                    _tail.Pre.Next = null;
                    _tail = _tail.Pre;
                    CleanLinks(temp);
                }
                else
                    DeleteSingleNode();
            }
        }

        // delete node from unknown place
        private void Delete(CombinedNode delNode)
        {
            if (!IsEmpty)
            {
                if (delNode == _head)
                    DeleteHead();
                else if (delNode == _tail)
                    DeleteTail();
                else
                {
                    delNode.Pre.Next = delNode.Next;
                    delNode.Next.Pre = delNode.Pre;
                    CleanLinks(delNode);
                }
            }
        }

        #endregion
    }
}
