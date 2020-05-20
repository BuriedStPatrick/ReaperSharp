using System;

namespace ReaperCore.Nodes
{
    public class TrackNode : ReaperNode
    {
        public TrackNode(
            string text,
            ReaperNode? parent
        ) : base(text, parent)
        {
            Type = Constants.NodeTypes.Track;
        }

        public string? Name => GetValue("NAME");
        public Guid? TrackId => Guid.Parse(GetValue("TRACKID"));
    }
}
