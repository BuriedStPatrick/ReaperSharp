using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace ReaperCore.Nodes
{
    public class ReaperNode
    {
        /// <summary>
        /// The parent ReaperNode. If null, this node is the main ReaperSession Node.
        /// </summary>
        public ReaperNode? Parent { get; }

        /// <summary>
        /// The type of this Node as a string.
        /// </summary>
        public string? Type { get; protected set; }

        /// <summary>
        /// All Children of this ReaperNode as a instances of ReaperNode. 
        /// A child can be a 
        /// - TRACK
        /// - ITEM (a region on a track)
        /// - POSITION (which might be the position of an item on a track), 
        /// - FADEIN (which might be an array of numbers) or 
        /// - PT (which is a point of an envelope)
        /// - etc.
        /// There are many other properties and values. Have a look into
        /// the .rpp file (open in any text editor).
        /// </summary>
        /// <value>The child nodes of this node.</value>
        public List<ReaperNode> Children { get; }

        /// <summary>
        /// The values of this Node.
        /// </summary>
        public List<string> Values { get; }

        /// <summary>
        /// Gets the first value of this nodes values.
        /// </summary>
        /// <value>The first value of this nodes values.</value>
        public string Value => Values.Count > 0 ? Values.First() : "";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ReaperNode"/> class.
        /// </summary>
        /// <param name="text">The current line as a string from the reaper file.</param>
        /// <param name="parent">The parent node. Pass null if there is no parent.</param>
        public ReaperNode(
            string text,
            ReaperNode? parent
        ) {
            Children = new List<ReaperNode>();
            Values = new List<string>();
            Parent = parent;
            text = text.Trim();

            var matchList = new List<string>();
            var matches = Regex.Matches(text, "(\"[^\"]*\"|[\\S]+)"); // split the values

            foreach (Match match in matches)
            {
                matchList.Add(match.Value);
            }

            matchList = matchList.Select(s => s.Trim(' ', '\"', '<', '>')).ToList();

            /* 
             * Try to parse the first element of matches as a float. If it is a
             * number, the node has no type but only values. if it is not a
             * number, the first element is the type of the node.
             */
            try
            {
                Type = "";
                Values = matchList;
            }
            catch
            {
                if (matchList.Count > 0)
                {
                    Type = matchList[0];
                    matchList.RemoveAt(0);
                    Values = matchList;
                }
            }
        }

        /// <summary>
        /// Adds a child.
        /// </summary>
        /// <param name="o">O.</param>
        public void AddChild(ReaperNode o) => Children.Add(o);

        /// <summary>
        /// Gets all children of the given type with the given name
        /// </summary>
        /// <returns>All children of given type with the given name.</returns>
        /// <param name="type">Type.</param>
        /// <param name="name">Name.</param>
        /// <param name="recursive">Search child nodes too. False by default.</param>
        public List<ReaperNode> GetNodesByTypeAndName(string type, string name, bool recursive = false) => 
            GetNodes(type, recursive)
                .Where(node => node.GetNode("NAME").Value == name)
                .ToList();

        /// <summary>
        /// Find a node by it's type and name.
        /// </summary>
        /// <returns>The node by type and name.</returns>
        /// <param name="type">Type.</param>
        /// <param name="name">Name.</param>
        public ReaperNode GetNodeByTypeAndName(string type, string name) =>
            GetDescendantNodes(type).FirstOrDefault(
                node => node.GetNode("NAME").Value == name
            );

        /// <summary>
        /// Find a node by it's type.
        /// </summary>
        /// <returns>The first child node with type type.</returns>
        /// <param name="type">Type.</param>
        public ReaperNode GetNode(string type) =>
            Children.Find(x => x.Type == type);

        public string GetValue(string key) =>
            Children
                .Find(x => x.Value == key).Values
                .ElementAt(1);

        /// <summary>
        /// Gets the last child node with type type.
        /// </summary>
        /// <returns>The last child node with type type.</returns>
        /// <param name="type">Type.</param>
        public ReaperNode GetLastNode(string type) =>
            Children.FindLast(x => x.Type == type);

        /// <summary>
        /// Gets all child nodes with type type.
        /// </summary>
        /// <returns>The child nodes with type type.</returns>
        /// <param name="type">Type.</param>
        /// <param name="includeDescendants">If true search child nodes too. False by default.</param>
        public List<ReaperNode> GetNodes(string type, bool includeDescendants = false) =>
            !includeDescendants
                ? Children.FindAll(x => x.Type == type)
                : GetDescendantNodes(type);

        /// <summary>
        /// Search all children recursively. Finds children in children too and so on...
        /// </summary>
        /// <returns>All children of type type.</returns>
        /// <param name="type">Type.</param>
        private List<ReaperNode> GetDescendantNodes(string type)
        {
            var found = new List<ReaperNode>();
            GetAllDescendants(ref found, type);
            return found;
        }

        private void GetAllDescendants(ref List<ReaperNode> found, string type)
        {
            var children = GetNodes(type);
            found.AddRange(children);
            foreach (var child in children)
            {
                child.GetAllDescendants(ref found, type);
            }
        }
    }
}
