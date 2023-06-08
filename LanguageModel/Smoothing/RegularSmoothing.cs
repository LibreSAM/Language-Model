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
        int size = (context.Count() != 0 ? 1 : 0) + context.Count((c) => c == ' ');
        IDictionary<string, uint> ngram = ngrams[size].NGrams[context];
        uint occurrences = ngram[next];
        uint contextCount = (uint)ngram.Sum((next) => next.Value);
        double p = (double)occurrences / contextCount;
        return Math.Log10(p);
    }
}