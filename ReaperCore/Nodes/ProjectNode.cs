namespace ReaperCore.Nodes
{
    public class ProjectNode : ReaperNode
    {
        public ProjectNode(
            string text,
            ReaperNode? parent
        ) : base(text, parent)
        {
            Type = Constants.NodeTypes.Project;
        }
    }
}
