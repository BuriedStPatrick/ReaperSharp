using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReaperCore.Nodes;

namespace ReaperCore.Extensions
{
    public static class ReaperParseResultExtensions
    {
        public static IEnumerable<TrackNode> FindTracks(
            this IReaperParseResult result,
            Expression<Func<TrackNode, bool>> predicate
        )
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RootNode == null)
            {
                throw new ArgumentNullException(nameof(result.RootNode));
            }

            var compiledPredicate = predicate.Compile();
            var stack = new Stack<ReaperNode>();
            var trackNodes = new List<TrackNode>();
            stack.Push(result.RootNode);
            while (stack.Any())
            {
                var current = stack.Pop();

                if (current.Type == Constants.NodeTypes.Track)
                {
                    trackNodes.Add((TrackNode) current);
                }

                current.Children.ForEach(stack.Push);
            }

            return trackNodes
                .Where(compiledPredicate)
                .ToList();
        }

        public static TrackNode? FindTrack(this IReaperParseResult result, Expression<Func<TrackNode, bool>> predicate) =>
            result
                .FindTracks(predicate)
                .FirstOrDefault();
    }
}
