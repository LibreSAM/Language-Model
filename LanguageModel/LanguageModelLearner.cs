using LanguageModel.Smoothing;
using Microsoft.Extensions.Logging;

namespace LanguageModel;
public class LanguageModelLearner
{
    public IList<NGramCounter> NGramCounts;
    private readonly ILogger _logger;

    public LanguageModelLearner(ILoggerFactory loggerFactory)
    {
        uint size = 3;

        _logger = loggerFactory.CreateLogger<LanguageModelLearner>();
        NGramCounts = new List<NGramCounter>();
        for (uint i = 1; i <= size; i++)
        {
            NGramCounts.Add(new NGramCounter(i, loggerFactory.CreateLogger<NGramCounter>()));
        }
    }

    public void Learn(string inputFilePath)
    {
        _logger.LogInformation("Starting to learn the language model");
        _logger.LogDebug($"Input file for learning: \"{inputFilePath}\"");

        IEnumerable<string> inputLines;
        try
        {
            inputLines = File.ReadAllLines(inputFilePath);
            _logger.LogDebug("Finished read learning input file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read the contents of the input file for learning.");
            return;
        }

        foreach (string line in inputLines)
        {
            _logger.LogTrace($"Using input text \"{line}\"");
            string[] words = line.Split(' ');
            foreach (var item in NGramCounts)
            {
                _logger.LogDebug($"Learning {item.Size}-Grams");
                item.Learn(words);
            }
        }

        _logger.LogInformation("Finished learning language model");
    }

    public NGramLanguageModel BuildLanguageModel(ISmoothing smoother)
    {
        _logger.LogInformation("Computing ARPA-Representation of language model...");
        var languageModel = new NGramLanguageModel();

        foreach (var item in NGramCounts)
        {
            foreach (var context in item.NGrams)
            {
                foreach (var next in context.Value)
                {
                    string ngram = $"{context.Key} {next.Key}";
                    double p = smoother.Smooth(next.Key, context.Key, NGramCounts);
                    _logger.LogTrace($"NGram \"{ngram}\": Occurances = {next.Value}; smoothed P = {p}");
                    languageModel.AddNGram(item.Size, context.Key, next.Key, p);
                }
            }
        }

        _logger.LogInformation("Finished computing ARPA-Representation of language model");
        return languageModel;
    }
}
