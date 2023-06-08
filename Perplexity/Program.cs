using CommandLine;
using LanguageModel;
using Microsoft.Extensions.Logging;

namespace Perplexity;

/// <summary>
/// The static main entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The entrypoint of the application. Just parses the supplied arguments and then calls <see cref="CalcPerplexity(PerplexityCalcOptions)(LearnOptions)"/> with them.
    /// </summary>
    /// <param name="args">The commandline arguments supplied when starting the application.</param>
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<PerplexityCalcOptions>(args).WithParsed(CalcPerplexity);
    }

    /// <summary>
    /// The main application logic. Controls the applications control flow. 
    /// </summary>
    /// <param name="options">The parsed options that were supplied when launching the application.</param>
    private static void CalcPerplexity(PerplexityCalcOptions options)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            LogLevel logLevel = options.Verbose ? LogLevel.Trace : LogLevel.Information;
            builder.SetMinimumLevel(logLevel);
            builder.AddSimpleConsole();
        });

        Console.WriteLine("Loading LM from file...");
        NGramLanguageModel lm = NGramLanguageModel.LoadFrom(options.LmArpaInputFilePath);
        Console.WriteLine("Computing perplexity...");
        double perplexity = lm.GetPerplexity(options.InputText);
        Console.WriteLine();

        Console.WriteLine($"Text: \"{options.InputText}\"");
        Console.WriteLine($"Path to LanguageModel: \"{options.LmArpaInputFilePath}\"");
        Console.WriteLine($"Perplexity: {perplexity}");
    }
}