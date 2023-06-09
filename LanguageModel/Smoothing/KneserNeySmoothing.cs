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
        int size = (context.Length != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;
        IDictionary<string, IDictionary<string, uint>> ngramTable = ngrams[size - 1].NGrams;
        IDictionary<string, uint> ngramsWithContext = ngramTable[context];
        uint occurencesOfContextBeforeAnything = (uint)ngramsWithContext.Sum(next => next.Value);
        double discount = (size == ngrams.Count) ? 0 : 0.75;

        // firstTerm
        double firstTerm;
        if (size == ngrams.Count) // Highest-order NGram
        {
            uint completeNGramOccurenceCount = ngramsWithContext[next];
            double fT_numerator = Math.Max(completeNGramOccurenceCount - discount, 0);
            firstTerm = fT_numerator / occurencesOfContextBeforeAnything;
        }
        else // Lower-order NGram
        {
            double fT_numerator = ngrams[size].NGrams.Count(n => n.Key.EndsWith(context) && n.Value.ContainsKey(next));
            fT_numerator = Math.Max(fT_numerator - discount, 0);

            // Logic for higher orders, we handle lowest order (1-grams) separately as this logic won't work for them.
            // At 1-grams we must count how many words can come before "", that's just all 1-grams,
            // but ContainsKey("") as below matches none of them and so returns 0.
            double fT_discriminator;
            if (size > 1)
            {
                IEnumerable<string> ngramContext = context.Split(' ').ToList();
                string nextToSearch = ngramContext.Last();
                string contextToSearch = size > 2 ? string.Join(' ', ngramContext.Take(size - 2)) : "";
                fT_discriminator = ngrams[size - 1].NGrams.Count(n => n.Value.ContainsKey(nextToSearch) && n.Key.EndsWith(contextToSearch));
            }
            // For 1-grams, count length of 1-gram table as described above, size - 1 always equals 0 in this case.
            // 1-grams only can have the context "", so we don't need to perform the whole searching process like above for higher orders.
            else
            {
                fT_discriminator = ngrams[0].NGrams[""].Count;
            }
            firstTerm = fT_numerator / fT_discriminator;
        }

        // Lambda
        int numberOfDifferentFinalWordsSuccedingContext = ngramsWithContext.Count;
        double lambda = discount / occurencesOfContextBeforeAnything * numberOfDifferentFinalWordsSuccedingContext;

        // Pcont
        double pcont = 0;
        if (size > 1) // Higher orders -> use continuation count method
        {
            int pcont_numerator = ngramTable.Count(following => following.Value.ContainsKey(next));
            int ngramTableLength = ngramTable.Sum(_ => _.Value.Count);
            pcont = (double)pcont_numerator / ngramTableLength;
        }
        else // Lower orders -> uniform distribution
        {
            // For 1-grams, count length of 1-gram table as described above, size - 1 always equals 1 in this case.
            // 1-grams only can have the context "", so we don't need to perform the whole searching process like above for higher orders.
            int unigramCount = ngrams[0].NGrams[""].Count;
            pcont = 1.0 / unigramCount;
        }

        // Sum
        double smoothedNGramProbability = firstTerm + lambda * pcont;
        return Math.Log10(smoothedNGramProbability);
    }
}