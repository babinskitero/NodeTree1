using System;
using System.IO;
using System.Threading.Tasks;

namespace Tree
{
    public class NodeWriter : INodeWriter
    {
        INodeDescriber _nodeDescriber;

        public NodeWriter (INodeDescriber nodeDescriber)
        {
            _nodeDescriber = nodeDescriber;
        }
        public async Task WriteToFileAsync(Node node, string filePath)
        {
            var nodeDescription = _nodeDescriber.Describe(node);
            try
            {
                 File.WriteAllText(filePath, nodeDescription);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
