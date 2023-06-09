namespace LanguageModel.Smoothing;

/// <summary>
/// Represents a type that computes ngram probabilities using a smoothing algorithm.
/// </summary>
public interface ISmoothing
{
    /// <summary>
    /// Calculate the smoothed probability for a ngram.
    /// </summary>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="ngrams">A list of all ngrams to consider in the language model.</param>
    /// <returns>The smoothed probability of the ngram.</returns>
    public double Smooth(string next, string context, IList<NGramCounter> ngrams);
}