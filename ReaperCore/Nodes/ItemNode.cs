namespace ReaperCore.Nodes
{
    public class ItemNode : ReaperNode
    {
        public ItemNode(
            string text,
            ReaperNode? parent
        ) : base(text, parent)
        {
            Type = Constants.NodeTypes.Item;
        }
    }
}
