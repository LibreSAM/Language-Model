using CommandLine;
using LanguageModel.Smoothing;

namespace Learn;
public class LearnOptions
{
    [Option('i', "inputFilePath", Required = true, HelpText = "Path to file with input text to learn.")]
    public string InputFilePath { get; set; } = "";

    [Option('s', "smoothing", Required = true, HelpText = "Type of smoothing to apply. Supported values: \"Regular\" and \"KneserNey\"")]
    public SmoothingType Smoothing { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Enable verbose logging.")]
    public bool Verbose { get; set; }
}
