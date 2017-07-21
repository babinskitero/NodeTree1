using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree
{
    public class NodeTransformer : INodeTransformer
    {
        public Node Transform(Node node)
        {
            var nodeType = node.GetType();
            if (nodeType == typeof(NoChildrenNode))
            {
                return node;
            } else
            {
                var @switch = new Dictionary<Type, Action>
                {
                    { typeof(SingleChildNode), () => node = TransformSingleNode((SingleChildNode)node) },
                    { typeof(TwoChildrenNode), () => node = TransformTwoNode((TwoChildrenNode)node) },
                    { typeof(ManyChildrenNode), () => node = TransformManyNode((ManyChildrenNode)node) },
                };
                @switch[nodeType]();
            }
            return node;
        }

        private Node TransformSingleNode(SingleChildNode node)
        {
            Node child = node.Child;
            if(child == null)
            {
                return new NoChildrenNode(node.Name);
            }
            return node;
        }

        private Node TransformTwoNode(TwoChildrenNode node)
        {
            Node firstChild = node.FirstChild;
            Node secondChild = node.SecondChild;
            if (firstChild == null && secondChild == null)
            {
                return new NoChildrenNode(node.Name);
            } else if (firstChild == null && secondChild != null)
            {
                return new SingleChildNode(node.Name, Transform(secondChild));
            } else if(firstChild != null && secondChild == null)
            {
                return new SingleChildNode(node.Name, Transform(firstChild));
            }
            return node;
        }

        private Node TransformManyNode(ManyChildrenNode node)
        {
            var children = node.Children;
            if (children == null)
            {
                return new NoChildrenNode(node.Name);
            }
            else
            {
                var childrenList = children.ToList();
                switch(childrenList.Count)
                {
                    case 0:
                        return new NoChildrenNode(node.Name);
                    case 1:
                        return new SingleChildNode(node.Name, Transform(childrenList[0]));
                    case 2:
                        return new TwoChildrenNode(node.Name, Transform(childrenList[0]), Transform(childrenList[1]));
                }
            }
            return node;
        }

    }
}
