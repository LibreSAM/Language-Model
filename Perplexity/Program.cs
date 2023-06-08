using CommandLine;
using LanguageModel;
using Microsoft.Extensions.Logging;

namespace Perplexity;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<PerplexityCalcOptions>(args).WithParsed(CalcPerplexity);
    }

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