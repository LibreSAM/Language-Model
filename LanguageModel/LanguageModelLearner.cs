using LanguageModel.Smoothing;
using Microsoft.Extensions.Logging;

namespace LanguageModel;

/// <summary>
/// Provides method to train language models.
/// </summary>
public class LanguageModelLearner
{
    /// <summary>
    /// The number of occurrences of all ngrams that are found during training the language model.
    /// </summary>
    public IList<NGramCounter> NGramCounts;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageModelLearner"/> class with the given loggerfactory.
    /// </summary>
    /// <param name="loggerFactory">The factory used to create required loggers.</param>
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

    /// <summary>
    /// Learn the language model using the text in the provided stream.Populates the
    /// <see cref="LanguageModelLearner.NGramCounts"/> field with ngrams and their occurrence counts.
    /// </summary>
    /// <param name="input">The stream that the text that will be used for learning will be read from.</param>
    public void Learn(StreamReader input)
    {
        _logger.LogInformation("Starting to learn the language model");

        string? currentLine;
        try
        {
            while ((currentLine = input.ReadLine()) != null)
            {
                _logger.LogTrace($"Using input text \"{currentLine}\"");
                string[] words = currentLine.Split(' ');
                foreach (var item in NGramCounts)
                {
                    _logger.LogDebug($"Learning {item.Size}-Grams");
                    item.Learn(words);
                }
            }
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error reading input: IO error");
        }

        _logger.LogInformation("Finished learning language model");
    }

    /// <summary>
    /// Creates a new <see cref="NGramLanguageModel"/> object with the probabilities of all ngrams that were encountered while training this object.
    /// </summary>
    /// <param name="smoother">An object that will be used to calculate the probabilities of each ngram.</param>
    /// <returns>A language model that includes the ngrams and their probabilities that were calculated based on the data in this object.</returns>
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
