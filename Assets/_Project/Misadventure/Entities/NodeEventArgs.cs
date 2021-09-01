using System;
using System.Collections.Generic;

namespace Misadventure.Entities
{
    public class NodeEventArgs : EventArgs
    {
        public LinkedListNode<CaterpillarNode> Node { get; }

        public NodeEventArgs(LinkedListNode<CaterpillarNode> node)
        {
            Node = node;
        }
    }
}