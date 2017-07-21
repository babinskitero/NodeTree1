using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Tree
{
    public class NodeDescriber : INodeDescriber
    {
        INodeTransformer _nodeTransformer;

        public NodeDescriber (INodeTransformer nodeTransformer)
        {
            _nodeTransformer = nodeTransformer;
        }
        public string Describe(Node node)
        {
            StringWriter sw = new StringWriter();
            IndentedTextWriter itw = new IndentedTextWriter(sw, "    ");

            itw.Indent = 0;

			var transformNode = _nodeTransformer.Transform(node);
			var isNoChildrenNodeLast = transformNode.GetType() == typeof(NoChildrenNode) ? true : false;

			WriteNode(itw, transformNode, isNoChildrenNodeLast);

			return sw.ToString();
		}

		private void WriteNode(IndentedTextWriter itw, Node node, bool isNoChildrenNode)
		{
			var nodeType = node.GetType();
			var partialType = (nodeType.ToString().Split('.'))[1];

			if (nodeType == typeof(NoChildrenNode))
			{
				if (isNoChildrenNode)
				{
					itw.Write(@"new {0}(""{1}"")", partialType, node.Name);
				}
				else
				{
					itw.WriteLine(@"new {0}(""{1}""),", partialType, node.Name);
				}
			}
			else
			{
				itw.WriteLine(@"new {0}(""{1}"",", partialType, node.Name);
				itw.Indent++;

				var @switch = new Dictionary<Type, Action>
				{
					{ typeof(SingleChildNode), () => WriteSingleNode(itw, node) },
					{ typeof(TwoChildrenNode), () => WriteTwoNode(itw, node) },
					{ typeof(ManyChildrenNode), () => WriteManyNode(itw, node) },
				};

				@switch[nodeType]();

				itw.Write(")");
			}
		}

		private void WriteSingleNode(IndentedTextWriter itw, Node node)
		{
			var singleChildNode = ((SingleChildNode)node).Child;
			var isSingleChildNodeLast = singleChildNode.GetType() == typeof(NoChildrenNode) ? true : false;
			WriteNode(itw, singleChildNode, isSingleChildNodeLast);
		}

		private void WriteTwoNode(IndentedTextWriter itw, Node node)
		{
			var firstChildNode = ((TwoChildrenNode)node).FirstChild;
			var secondChildNode = ((TwoChildrenNode)node).SecondChild;
			var isfirstChildNodeLast = firstChildNode.GetType() == typeof(NoChildrenNode) ? true : false;
			var isSecondChildNodeLast = secondChildNode.GetType() == typeof(NoChildrenNode) ? true : false;
			var isfirstSecondChildNodeLast = isfirstChildNodeLast && isSecondChildNodeLast ? true : false;

			WriteNode(itw, firstChildNode, false);
			WriteNode(itw, secondChildNode, isfirstSecondChildNodeLast);
		}

		private void WriteManyNode(IndentedTextWriter itw, Node node)
		{
			IEnumerable<Node> children = ((ManyChildrenNode)node).Children;

			var childrenList = children.ToList();

			foreach (var child in childrenList)
			{
				var isLast = true;
				if (child.GetType() != typeof(NoChildrenNode))
					isLast = false;
				WriteNode(itw, child, isLast);
			}
		}
	}
}