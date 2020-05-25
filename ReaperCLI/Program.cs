using System;
using Fclp;
using ReaperCore;
using ReaperCore.Extensions;

namespace ReaperCLI
{
    public class ApplicationArguments
    {
        public string? File { get; set; }
    }

    public class Program
    {
        public static int Main(string[] args)
        {
            var p = new FluentCommandLineParser<ApplicationArguments>();
            p.SetupHelp("help", "h")
                .Callback(DisplayHelp)
                .UseForEmptyArgs()
                .WithHeader($"{Constants.ReaperTitle}");

            p.Setup(arg => arg.File)
                .As('f', "file")
                .WithDescription("The path to the REAPER project (RPP) file")
                .SetDefault(args.Length > 0 ? args[0] : string.Empty)
                .Required();

            var argumentsResult = p.Parse(args);
            if (argumentsResult.HasErrors)
            {
                foreach (var error in argumentsResult.Errors)
                {
                    Console.Error.WriteLine($"Missing argument: {error.Option.LongName}");
                }

                return -1;
            }

            var arguments = p.Object;

            if (arguments.File == null)
            {
                return 0;
            }

            var parser = new ReaperParser(new DebugLogger());
            var parseResult = parser.Parse(arguments.File);

            var track = parseResult.FindTrack(t => t.Name == "Ambient");

            Console.WriteLine(track?.TrackId.ToString() ?? "Missing ID :(");

            return 0;
        }

        public static void DisplayHelp(string text)
        {
            Console.WriteLine(text);
        }
    }

    public class DebugLogger : ILogger
    {
        public void LogError(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
