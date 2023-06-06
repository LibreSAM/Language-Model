using CommandLine;

namespace Perplexity;

public class PerplexityCalcOptions
{
    [Option('m', "model", Required = true, HelpText = "Path to a file containing the language model to use for perplexity calculation. Must be in ARPA format.")]
    public string LmArpaInputFilePath { get; set; } = "";

    [Option('i', "text", Required = true, HelpText = "The text to calculate the perplexity of.")]
    public string InputText { get; set; } = "";

    [Option('v', "verbose", Required = false, HelpText = "Enable verbose logging.")]
    public bool Verbose { get; set; }
}
