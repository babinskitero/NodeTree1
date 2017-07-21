using System.Threading.Tasks;

namespace Tree
{
    interface INodeWriter
    {
        Task WriteToFileAsync(Node node, string filePath);
    }
}
