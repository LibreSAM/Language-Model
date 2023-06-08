namespace LanguageModel.Smoothing;

/// <summary>
/// Implements a Kneser-Ney smoothing algorithm based on the Kneser-ney probability equation.
/// </summary>
public class KneserNeySmoothing : ISmoothing
{
    // cf. https://medium.com/@dennyc/a-simple-numerical-example-for-kneser-ney-smoothing-nlp-4600addf38b8

    /// <summary>
    /// Calculate the smoothed probability for a ngram using Kneser-Ney smoothing.
    /// </summary>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="ngrams">A list of all ngrams to consider in the language model.</param>
    /// <returns>The smoothed probability of the ngram.</returns>
    public double Smooth(string next, string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Count() != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;
        IDictionary<string, IDictionary<string, uint>> ngramTable = ngrams[size - 1].NGrams;
        IDictionary<string, uint> ngramsWithContext = ngramTable[context];
        uint occurencesOfContextBeforeAnything = (uint)ngramsWithContext.Sum(next => next.Value);
        double discount = (size == ngrams.Count) ? 0 : 0.75;

        // firstTerm
        double firstTerm;
        if (size == ngrams.Count) // highest-order NGram
        {
            uint completeNGramOccurenceCount = ngramsWithContext[next];
            double fT_numerator = Math.Max(completeNGramOccurenceCount - discount, 0);
            firstTerm = fT_numerator / occurencesOfContextBeforeAnything;
        }
        else // lower-order NGram
        {
            double fT_numerator = ngrams[size].NGrams.Count(n => n.Key.EndsWith(context) && n.Value.ContainsKey(next));
            fT_numerator = Math.Max(fT_numerator - discount, 0);
            IEnumerable<string> ngramContext = context.Split(' ').ToList();
            string nextToSearch = ngramContext.Last();
            string contextToSearch = size > 2 ? string.Join(' ', ngramContext.Take(size - 2)) : "";
            double fT_discriminator = ngrams[size].NGrams.Count(n => n.Value.ContainsKey(nextToSearch) && n.Key.EndsWith(contextToSearch));
            firstTerm = fT_numerator / fT_discriminator;
        }

        // lambda
        int numberOfDifferentFinalWordsSuccedingContext = ngramsWithContext.Count;
        double lambda = discount / occurencesOfContextBeforeAnything * numberOfDifferentFinalWordsSuccedingContext;

        // Pcont
        double pcont = 0;
        if (lambda != 0)
        {
            int pcont_numerator = ngramTable.Count(following => following.Value.ContainsKey(next));
            int ngramTableLength = ngramTable.Sum(_ => _.Value.Count());
            pcont = (double)pcont_numerator / ngramTableLength;
        }

        // Sum
        double smoothedNGramProbability = firstTerm + lambda * pcont;
        return Math.Log10(smoothedNGramProbability);
    }
}