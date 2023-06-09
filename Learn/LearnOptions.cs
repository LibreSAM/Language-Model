using CommandLine;
using LanguageModel.Smoothing;

namespace Learn;

/// <summary>
/// Represents commandline parameters that are supported by the application, <seealso cref="Learn.Program.Main(string[])"/>.
/// </summary>
public class LearnOptions
{
    /// <summary>
    /// Path to file with input text to learn.
    /// </summary>
    [Option('i', "inputFilePath", Required = true, HelpText = "Path to file with input text to learn.")]
    public string InputFilePath { get; set; } = "";

    /// <summary>
    /// Filepath that the learned language model will be saved to using ARPA representation. If the file already exists, it will be overwritten!
    /// </summary>
    [Option('o', "outputFilePath", Required = true, HelpText = "Filepath that the learned language model will be saved to using ARPA representation. If the file already exists, it will be overwritten!")]
    public string OutputFilePath {get; set; } = "";

    /// <summary>
    /// Type of smoothing to apply. Supported values: "Regular" and "KneserNey"
    /// </summary>
    [Option('s', "smoothing", Required = true, HelpText = "Type of smoothing to apply. Supported values: \"Regular\" and \"KneserNey\"")]
    public SmoothingType Smoothing { get; set; }

    /// <summary>
    /// Enable verbose logging.
    /// </summary>
    [Option('v', "verbose", Required = false, HelpText = "Enable verbose logging.")]
    public bool Verbose { get; set; }
}
