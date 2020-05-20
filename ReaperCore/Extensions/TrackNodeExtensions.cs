using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReaperCore.Nodes;

namespace ReaperCore.Extensions
{
    public static class TrackNodeExtensions
    {
        public static IEnumerable<ItemNode> FindItems(
            this TrackNode trackNode,
            Expression<Func<ItemNode, bool>> predicate
        )
        {
            var compiledPredicate = predicate.Compile();
            return trackNode.Children
                .Where(c => c.Type == Constants.NodeTypes.Item)
                .Cast<ItemNode>()
                .Where(compiledPredicate);
        }
    }
}
