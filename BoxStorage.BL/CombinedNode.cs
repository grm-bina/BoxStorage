using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Models;

namespace BoxStorage.BL
{
    // the node that each instans of this exist & used in SubTree & Queue
    // use also fields from basic class: Left, Right, Root, Data

    internal sealed class CombinedNode : Node<Box> /////////// temporary public
    {
        public CombinedNode(Node<SubTree> mainNode)
        {
            if (mainNode != null)
                MainNode = mainNode;
            else
                throw new NullReferenceException("Error: tried create CombineNode that not linked to main-node");
        }

        public readonly Node<SubTree> MainNode; // save link to node in main-tree
        public CombinedNode Pre;
        public CombinedNode Next;
    }
}
