using CommandLine;
using LanguageModel;
using LanguageModel.Smoothing;
using Microsoft.Extensions.Logging;

namespace Learn;

/// <summary>
/// The static main entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The entrypoint of the application. Just parses the supplied arguments and then calls <see cref="Learn(LearnOptions)"/> with them.
    /// </summary>
    /// <param name="args">The commandline arguments supplied when starting the application.</param>
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<LearnOptions>(args).WithParsed(Learn);
    }

    /// <summary>
    /// The main application logic. Controls the applications control flow. 
    /// </summary>
    /// <param name="options">The parsed options that were supplied when launching the application.</param>
    public static void Learn(LearnOptions options)
    {
        // Some logging initialization
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            LogLevel logLevel = options.Verbose ? LogLevel.Trace : LogLevel.Information;
            builder.SetMinimumLevel(logLevel);
            builder.AddSimpleConsole();
        });

        // Object initialization
        LanguageModelLearner lmLearner = new(loggerFactory);

        // Read inputs and count ngrams
        try
        {
            using FileStream inputStream = File.OpenRead(options.InputFilePath);
            using StreamReader inputReader = new(inputStream);
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

        // Create language model and serialize using ARPA representation
        NGramLanguageModel languageModel = lmLearner.BuildLanguageModel(Smoothing.Get(options.Smoothing));
        var outputBuffer = new MemoryStream();
        languageModel.GetArpaRepresentation(outputBuffer);

        // Write ARPA representation of the trained language model to the output file
        try
        {
            using FileStream outputFile = File.Create(options.OutputFilePath);
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

    /// <summary>
    /// A helper class for handling occurring exceptions to avoid code duplication.
    /// This is placed here only for simplicity reasons and could also be moved to another library instead.
    /// But as this is a quite small project, we won't create a new library for just a single function.
    /// </summary>
    /// <param name="errortext">The errortext to print.</param>
    /// <param name="ex">The exception that has occured. Will only be printed in verbose output mode.</param>
    /// <param name="verbose">If set more detailed output will be generated.</param>
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
