using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Models;
using BoxStorage.Exceptions;

namespace BoxStorage.BL
{
    internal abstract class AbstractTree<T> : IComparer<Gift>
    {
        #region Data
        protected Node<T> _root;
        private CombinedNode _lastUsed;

        #endregion

        #region Prop
        // For feedback about insert-status
        public Respone<Box> InsertFeedback { get; protected set; }

        // property for look wich box was created or taked out for refresh queue
        public CombinedNode LastUsedBox
        {
            // can get onece after the data was refreshed, than link is removed for avoid excess runing relinking nodes in the queue
            get
            {
                CombinedNode temp = _lastUsed;
                _lastUsed = null;
                return temp;
            }
            protected set
            {
                _lastUsed = value;
            }
        }

        // Check if the tree is empty
        public bool IsEmpty
        {
            get
            {
                if (_root == null || _root.Data == null)
                    return true;
                else return false;
            }
        }

        #endregion

        #region Insert
        // methods for insert new box to storage
        // public
        public void Insert(Box newBox)
        {
            if (!IsEmpty)
                Insert(newBox, ref _root).Root = _root;
            else
                Insert(newBox, ref _root);
        }

        // protected abstract logic for insert
        protected abstract Node<T> Insert(Box newBox, ref Node<T> currentNode);
        #endregion

        #region PullBox
        // methods for get 1 box with correct bottom & height to gift
        // public
        // public method
        public bool TryPullBox(Gift gift, out Box box)
        {
            box = Search(gift, _root);
            return box != null;
        }

        // protected abstract logic for search
        protected abstract Box Search(Gift gift, Node<T> currentNode);

        // protected abstract logic for take box from correct node
        protected abstract Box PullBox(Gift gift, Node<T> correctNode);
        #endregion

        #region Delete
        // Methods For Remove Node
        // Public apstract remove by CombynedNode (for queue-use & internal call Del-method)
        public abstract void Delete(CombinedNode removingNode);

        // Protected method to remove node and reconect his root
        protected void DelAndReconnect(Node<T> removingNode)
        {
            // if the removing-node is not the _root
            if (removingNode.Root != null)
            {
                Node<T> tempRoot = removingNode.Root;
                // removing-node is right-child of tempRoot
                if (tempRoot.Right == removingNode)
                {
                    tempRoot.Right = Del(removingNode);
                    if (tempRoot.Right != null)
                        tempRoot.Right.Root = tempRoot;
                }
                // removing-node is left-child of tempRoot
                if (tempRoot.Left == removingNode)
                {
                    tempRoot.Left = Del(removingNode);
                    if (tempRoot.Left != null)
                        tempRoot.Left.Root = tempRoot;
                }
            }
            // if the removing-node is the root
            else
            {
                _root = Del(removingNode);
                if (_root != null)
                    _root.Root = null;
            }
        }

        // private logic for remove by node
        private Node<T> Del(Node<T> removingNode)
        {
            // if the node has 0 or 1 children
            if (removingNode.Left == null)
                return removingNode.Right;
            else if (removingNode.Right == null)
                return removingNode.Left;
            // if the node has 2 childrens
            else
            {
                // find the less node in right side from deleting node, save it in temp, and delete the source
                Node<T> temp = GetMin(removingNode.Right);
                DelAndReconnect(temp);
                // replace removing node with temp (relink his left and right and return to his root)
                temp.Left = removingNode.Left;
                temp.Left.Root = temp;
                temp.Right = removingNode.Right;
                if (temp.Right != null)
                    temp.Right.Root = temp;
                return temp;
            }
        }

        // Get Node with minimum value (from node that entered as parameter)
        private Node<T> GetMin(Node<T> currentNode)
        {
            if (currentNode.Left == null)
                return currentNode.Left;
            else
                return GetMin(currentNode.Left);
        }

        #endregion

        #region Get All Data In Order
        // Get list of all data sorted (for printing)
        // public
        public List<Box> InOrder()
        {
            if (IsEmpty)
                return null;
            else
            {
                List<Box> temp = new List<Box>();
                InOrder(_root, ref temp);
                return temp;
            }
        }

        // protected abstract method
        protected abstract void InOrder(Node<T> currentNode, ref List<Box> list);
        #endregion

        #region IComparer
        // abstract metod to IComparer-interface
        public abstract int Compare(Gift x, Gift y);
        #endregion


    }
}
