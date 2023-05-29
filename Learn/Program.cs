using System.Text;
using CommandLine;
using LanguageModel;
using LanguageModel.Smoothing;
using Microsoft.Extensions.Logging;

namespace Learn;
public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<LearnOptions>(args).WithParsed<LearnOptions>(Learn);
    }

    public static void Learn(LearnOptions options)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            LogLevel logLevel = options.Verbose ? LogLevel.Trace : LogLevel.Information;
            builder.SetMinimumLevel(logLevel);
            builder.AddSimpleConsole();
        });

        var lmLearner = new LanguageModelLearner(loggerFactory);
        lmLearner.Learn(options.InputFilePath);

        NGramLanguageModel languageModel = lmLearner.BuildLanguageModel(Smoothing.Get(options.Smoothing));

        var outputBuffer = new MemoryStream();
        languageModel.GetArpaRepresentation(outputBuffer);
        
        Thread.Sleep(100);

        Console.WriteLine(Encoding.UTF8.GetString(outputBuffer.ToArray()));
    }
}
