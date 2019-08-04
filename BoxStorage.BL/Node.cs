using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxStorage.BL
{
    // the node for main-BST & base-class for combinedNode
    internal class Node<T> ////////////////////////// temporary public
    {
        public Node<T> Left;
        public Node<T> Right;
        public Node<T> Root;
        public T Data;
    }
}
