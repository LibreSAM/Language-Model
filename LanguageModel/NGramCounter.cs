using Microsoft.Extensions.Logging;

namespace LanguageModel;

/// <summary>
/// Represents a counter for the occurrences of ngrams with a specified size. 
/// </summary>
public class NGramCounter
{
    /// <summary>
    /// The size of the ngrams that this instance counts.
    /// </summary>
    public readonly uint Size;

    /// <summary>
    /// The occurence counts of all ngrams that this instance has encountered.
    /// </summary>
    public IDictionary<string, IDictionary<string, uint>> NGrams;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of <see cref="NGramCounter"/> with a specified size that will use the specified logger object to create log messages.
    /// </summary>
    /// <param name="size">The size of ngrams that the instance should count.</param>
    /// <param name="logger">The logger object that the instance should use for creating log messages.</param>
    public NGramCounter(uint size, ILogger logger)
    {
        _logger = logger;
        Size = size;
        NGrams = new Dictionary<string, IDictionary<string, uint>>();
    }
    
    /// <summary>
    /// Learn's ngrams from a provided input sentence.
    /// </summary>
    /// <param name="line">The sentence to learn ngrams from.</param>
    public void Learn(string[] line)
    {
        _logger.LogDebug("Started learning {Size}-Gram", Size);
        _logger.LogTrace("Learning text: \"{currentLine}\"", string.Join(' ', line));

        // Count of sentence start & end padding - we only add a single padding start & end element
        int sentenceStartEndPadding = 1;

        // Prepare data structure and add padding to input
        Queue<string> temp = new();
        for (int i = 0; i < sentenceStartEndPadding; i++)
        {
            temp.Enqueue("<s>");
        }
        foreach (var item in line)
        {
            temp.Enqueue(item);
        }
        for (int i = 0; i < sentenceStartEndPadding; i++)
        {
            temp.Enqueue("</s>");
        }

        // Get all ngrams of size that this instance is counting
        while (temp.Count >= Size)
        {
            // Get next ngram
            var ngram = new List<string>
            {
                temp.Dequeue()
            };
            for (int i = 0; i < Size - 1; i++)
            {
                ngram.Add(temp.ElementAt(i));
            }
            var context = string.Join(' ', ngram.Take(ngram.Count - 1));
            var next = ngram.Last();

            // Check if this is an already known or a new ngram
            if (NGrams.ContainsKey(context))
            {
                if (NGrams[context].ContainsKey(next))
                {
                    // existing ngram -> increment occurrence count
                    _logger.LogTrace("Found occurance of existing ngram: \"{context} {next}\"", string.Join(' ', context), next);
                    NGrams[context][next]++;
                }
                else
                {
                    // Known context, but new next word -> add
                    _logger.LogTrace("Found new word that can follow on existing ngram context: \"{next}\" can follow on \"{context}\"", next, string.Join(' ', context));
                    NGrams[context].Add(next, 1);
                }
            }
            else
            {
                // New context -> lazy initialization of object and store the new ngram
                _logger.LogTrace("Found new ngram: \"{ngram}\"", string.Join(' ', ngram));
                NGrams.Add(context, new Dictionary<string, uint>() { { next, 1 } });
            }
        }

        _logger.LogDebug("Finished learning {Size}-Gram", Size);
    }
}
