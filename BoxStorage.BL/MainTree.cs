using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Models;
using BoxStorage.Exceptions;

namespace BoxStorage.BL
{
    internal sealed class MainTree : AbstractTree<SubTree> 
    {

        #region Insert
        // implementation for abstract method to insert new box to storage
        protected override Node<SubTree> Insert(Box newBox, ref Node<SubTree> currentNode)
        {
            if (currentNode == null)
            {
                currentNode = new Node<SubTree>();
                currentNode.Data = new SubTree(newBox, currentNode);
                LastUsedBox = currentNode.Data.LastUsedBox;
                InsertFeedback = currentNode.Data.InsertFeedback;
            }
            else if (Compare(currentNode.Data.PeekRoot, newBox) < 0)
                Insert(newBox, ref currentNode.Right).Root = currentNode;
            else if (Compare(currentNode.Data.PeekRoot, newBox) > 0)
                Insert(newBox, ref currentNode.Left).Root = currentNode;
            else
            {
                currentNode.Data.Insert(newBox);
                LastUsedBox = currentNode.Data.LastUsedBox;
                InsertFeedback = currentNode.Data.InsertFeedback;
            }
            return currentNode;
        }
        #endregion

        #region PullBox
        // methods for get 1 box by comparing bottom and call Pull-method in correct sub-tree

        // search sub-tree by bottom
        protected override Box Search(Gift gift, Node<SubTree> currentNode)
        {
            // nothing founded
            if (currentNode == null)
                return null;
            // this is exactly the tree!
            else if (Compare(gift, currentNode.Data.PeekRoot) == 0)
            {
                // try find box in this tree
                Box curBox = PullBox(gift, currentNode);
                if (curBox != null)
                    return curBox;
                // try find box in biger tree
                else
                    return Search(gift, currentNode.Right);
            }
            // need search tree with biger bottom
            else if (Compare(gift, currentNode.Data.PeekRoot) > 0)
                return Search(gift, currentNode.Right);
            // this is to big box
            else
            {
                // try find smaller box
                Box smaller = Search(gift, currentNode.Left);
                if (smaller != null)
                    return smaller;
                // ok, biger box is not bad, try get it
                else
                {
                    Box curBox = PullBox(gift, currentNode);
                    if (curBox != null)
                        return curBox;
                    // try find box in more biger tree
                    else
                        return Search(gift, currentNode.Right);
                }
            }
        }

        // call sub-tree-method for try get box, refresh LastUsed, if sub-tree is empty - remove it
        protected override Box PullBox(Gift gift, Node<SubTree> correctNode)
        {
            Box temp;
            if (correctNode.Data.TryPullBox(gift, out temp))
            {
                LastUsedBox = correctNode.Data.LastUsedBox;
                if (correctNode.Data.IsEmpty)
                    DelAndReconnect(correctNode);
                return temp;
            }
            else
                return null;
        }

        
        #endregion

        #region Delete
        // Public remove by CombynedNode (for queue-use) - implementation for abstract-method
        public override void Delete(CombinedNode removingNode)
        {
            // avoid using nodes which data is null
            if (removingNode.Data == null)
                throw new EmptyBoxSectionException("Error: tried delete node without data in the main-tree");

            Node<SubTree> current = removingNode.MainNode;
            current.Data.Delete(removingNode);
            if (current.Data.IsEmpty)
                DelAndReconnect(current);
        }
        #endregion

        #region Get All Data In Order
        // Get list of all data sorted (for printing)
        // implementation of abstract-method
        protected override void InOrder(Node<SubTree> currentNode, ref List<Box> list)
        {
            if (currentNode == null)
                return;

            InOrder(currentNode.Left, ref list);
            List<Box> temp = currentNode.Data.InOrder();
            if(temp!=null)
                list.AddRange(temp);
            InOrder(currentNode.Right, ref list);
        }
        #endregion

        #region IComparer
        // compare by bottom (get Box as Gift for find sub-BST by bottom)
        public override int Compare(Gift x, Gift y)
        {
            if (x.Bottom < y.Bottom)
                return -1;
            else if (x.Bottom > y.Bottom)
                return 1;
            else
                return 0;
        }
        #endregion
    }
}
