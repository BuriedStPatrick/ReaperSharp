using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ReaperCore.Nodes;

namespace ReaperCore
{
    public interface IReaperParser
    {
        IReaperParseResult Parse(string filePath);
        Task<IReaperParseResult> ParseAsync(string filePath);
    }

    public class ReaperParser : IReaperParser
    {
        private readonly ILogger _logger;
        private string[] _contentLines;
        private int _currentLine;
        private ReaperNode? _currentNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnityReaperParser.ReaperParser"/> class.
        /// </summary>
        /// <param name="rppPath">The Absolute RppPath to the Reaper File .rpp.</param>
        /// <param name="logger">The logger</param>
        public ReaperParser(ILogger logger)
        {
            _logger = logger;
            _currentLine = 0;
            _contentLines = new string[]{};
        }

        /// <summary>
        /// Parse the reaper file
        /// </summary>
        /// <param name="filePath">The path to the RPP-file</param>
        /// <returns>The parsed result</returns>
        public IReaperParseResult Parse(string filePath)
        {
            // Read File and store it to member content
            try
            {
                var reader = new StreamReader(filePath);
                var content = reader.ReadToEnd();
                _contentLines = content.Split('\n');
                var rootNode = ParseNode(null);

                return new ReaperParseResult(rootNode, filePath, true);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return new ReaperParseResult(null, filePath, false);
            }
        }

        public Task<IReaperParseResult> ParseAsync(string filePath)
        {
            var res = Parse(filePath);
            return Task.FromResult(res); // TODO: Be my guest and implement properly ¯\_(ツ)_/¯
        }

        /// <summary>
        /// Parse the reaper node
        /// </summary>
        /// <returns>The parsed node</returns>
        /// <param name="parent">Parent node if exists in parent hierarchy</param>
        private ReaperNode? ParseNode(ReaperNode? parent)
        {
            var contentLine = _contentLines[_currentLine];

            if (string.IsNullOrEmpty(contentLine))
            {
                return _currentNode;
            }

            if (contentLine.Contains("<"))
            {
                var type = GetNodeType(contentLine);
                var newNode = type switch
                {
                    Constants.NodeTypes.Item => new ItemNode(contentLine, parent),
                    Constants.NodeTypes.Track => new TrackNode(contentLine, parent),
                    Constants.NodeTypes.Project => new ProjectNode(contentLine, parent),
                    _ => new ReaperNode(contentLine, parent)
                };

                _currentNode?.AddChild(newNode);
                _currentNode = newNode;
            }
            else if (contentLine.Contains(">"))
            {
                _currentNode = _currentNode?.Parent ?? _currentNode;
            }
            else
            {
                _currentNode?.AddChild(new ReaperNode(contentLine, _currentNode));
            }

            _currentLine++;

            if (_currentLine < _contentLines.Length)
            {
                ParseNode(_currentNode);
            }

            return _currentNode;
        }

        private static string GetNodeType(string contentLine) =>
            contentLine
                .Trim()
                .Split(' ')
                .First()
                .Replace("<", string.Empty);
    }
}
