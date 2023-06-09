using CommandLine;

namespace Perplexity;

/// <summary>
/// Represents commandline parameters that are supported by the application, <seealso cref="Perplexity.Program.Main(string[])"/>.
/// </summary>
public class PerplexityCalcOptions
{
    /// <summary>
    /// Path to a file containing the language model to use for perplexity calculation. Must be in ARPA format.
    /// </summary>
    [Option('m', "model", Required = true, HelpText = "Path to a file containing the language model to use for perplexity calculation. Must be in ARPA format.")]
    public string LmArpaInputFilePath { get; set; } = "";

    /// <summary>
    /// The text to calculate the perplexity of.
    /// </summary>
    [Option('i', "text", Required = true, HelpText = "The text to calculate the perplexity of.")]
    public string InputText { get; set; } = "";

    /// <summary>
    /// Enable verbose logging.
    /// </summary>
    [Option('v', "verbose", Required = false, HelpText = "Enable verbose logging.")]
    public bool Verbose { get; set; }
}
