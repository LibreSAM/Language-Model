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

        LanguageModelLearner lmLearner = new LanguageModelLearner(loggerFactory);

        try
        {
            using FileStream inputStream = File.OpenRead(options.InputFilePath);
            using StreamReader inputReader = new StreamReader(inputStream);
            lmLearner.Learn(inputReader);
        }
        catch (ArgumentException ex)
        {
            HandleException("Error: Argument \"inputFilePath\" is not valid.", ex, options.Verbose);
        }
        catch (PathTooLongException ex)
        {
            HandleException("Error: Filepath given in argument \"outputFilePath\" is too long.", ex, options.Verbose);
        }
        catch (DirectoryNotFoundException ex)
        {
            HandleException("Error: A part of the output filepath was not found.", ex, options.Verbose);
        }
        catch (UnauthorizedAccessException ex)
        {
            HandleException("Error: Can't write to specified target file. Access is denied.", ex, options.Verbose);
        }
        catch (FileNotFoundException ex)
        {
            HandleException("Error: Argument \"inputFilePath\" - No such file.", ex, options.Verbose);
        }
        catch (IOException ex)
        {
            HandleException("IO error.", ex, options.Verbose);
        }


        NGramLanguageModel languageModel = lmLearner.BuildLanguageModel(Smoothing.Get(options.Smoothing));
        var outputBuffer = new MemoryStream();
        languageModel.GetArpaRepresentation(outputBuffer);

        try
        {
            using FileStream outputFile = File.OpenWrite(options.OutputFilePath);
            outputBuffer.WriteTo(outputFile);
        }
        catch (UnauthorizedAccessException ex)
        {
            HandleException("Error: Can't write to specified target file. Access is denied.", ex, options.Verbose);
        }
        catch (ArgumentException ex)
        {
            HandleException("Error: Argument \"outputFilePath\" is not valid.", ex, options.Verbose);
        }
        catch (PathTooLongException ex)
        {
            HandleException("Error: Filepath given in argument \"outputFilePath\" is too long.", ex, options.Verbose);
        }
        catch (DirectoryNotFoundException ex)
        {
            HandleException("Error: A part of the output filepath was not found.", ex, options.Verbose);
        }
    }

    private static void HandleException(string errortext, Exception ex, bool verbose)
    {
        Console.Error.WriteLine(errortext);
        if (verbose)
        {
            Console.Error.WriteLine(ex.ToString());
        }
        Environment.Exit(1);
    }
}
