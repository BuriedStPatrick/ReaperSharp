using ReaperCore.Nodes;

namespace ReaperCore
{
    public interface IReaperParseResult
    {
        ReaperNode? RootNode { get; }
        string Directory { get; }
        bool IsValid { get; }
    }

    public class ReaperParseResult : IReaperParseResult
    {
        public ReaperNode? RootNode { get; }
        public string Directory { get; }
        public bool IsValid { get; }

        public ReaperParseResult(
            ReaperNode? rootNode,
            string directory,
            bool isValid
        )
        {
            RootNode = rootNode;
            Directory = directory;
            IsValid = isValid;
        }
    }
}
