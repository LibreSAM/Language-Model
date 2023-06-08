namespace LanguageModel.Smoothing;

/// <summary>
/// Implements regular smoothing that just divides the count of occurrences of the complete ngram by the count of occurrences of the context.
/// </summary>
public class RegularSmoothing : ISmoothing
{
    /// <summary>
    /// Calculate the smoothed probability for a ngram using regular smoothing.
    /// </summary>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="ngrams">A list of all ngrams to consider in the language model.</param>
    /// <returns>The smoothed probability of the ngram.</returns>
    public double Smooth(string next, string context, IList<NGramCounter> ngrams)
    {
        // Calculate size of the context as we need this to count ngrams in the next step.
        // This counts zero-based. If the context is not empty, it has atleast one word.
        // When we add the count of spaces, we get the count of words in the context.
        int size = (!String.IsNullOrWhiteSpace(context) ? 1 : 0) + context.Count((c) => c == ' ');

        // Count occurrences of complete ngrams
        IDictionary<string, uint> ngram = ngrams[size].NGrams[context];
        uint occurrences = ngram[next];

        // Count occurrences of context
        uint contextCount = (uint)ngram.Sum((next) => next.Value);

        // Calculate probability of ngram based on occurrence counts
        double p = (double)occurrences / contextCount;
        return Math.Log10(p);
    }
}