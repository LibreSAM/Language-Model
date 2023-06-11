namespace LanguageModel.Smoothing;

/// <summary>
/// Implements a Kneser-Ney smoothing algorithm based on the Kneser-ney probability equation.
/// </summary>
public class KneserNeySmoothing : ISmoothing
{
    private const double _discount = 0.75;

    /// <summary>
    /// Calculate the smoothed probability for a ngram using Kneser-Ney smoothing.
    /// This is implemented according to Jurafsky (3.7), formula 3.40
    /// </summary>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="ngrams">A list of all ngrams to consider in the language model.</param>
    /// <returns>The smoothed probability of the ngram.</returns>
    public double Smooth(string next, string context, IList<NGramCounter> ngrams) => Smooth(next, context, ngrams, true);

    /// <summary>
    /// Calculate the smoothed probability for a ngram using Kneser-Ney smoothing.
    /// This is implemented according to Jurafsky (3.7), formula 3.40
    /// </summary>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="ngrams">A list of all ngrams to consider in the language model.</param>
    /// <returns>The smoothed probability of the ngram.</returns>
    public double Smooth(string next, string context, IList<NGramCounter> ngrams, bool isHighestOrder)
    {
        // firstTerm
        double firstTerm = CalculateFirstTerm(next, context, ngrams, isHighestOrder);

        // Lambda
        double lambda = CalculateLambda(context, ngrams);

        // Pcont
        double pcont = CalculatePCont(next, context, ngrams);

        // Sum
        double smoothedNGramProbability = firstTerm + lambda * pcont;
        return smoothedNGramProbability;
    }

    // Algorithm is different for highest order and lower orders, see Jurafsky (3.7), formula 3.41
    private double CalculateFirstTerm(string next, string context, IList<NGramCounter> ngrams, bool isHighestOrder)
    {
        if (isHighestOrder)
        {
            return CalculateFirstTermHighestOrder(next, context, ngrams);
        }
        else
        {
            return CalculateFirstTermLowerOrders(next, context, ngrams);
        }
    }

    // Jurafsky (3.7), formula 3.41, first case
    private double CalculateFirstTermHighestOrder(string next, string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Length != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;
        IDictionary<string, uint> ngramsWithContext = ngrams[size - 1].NGrams[context];

        uint occurencesOfContextBeforeAnything = (uint)ngramsWithContext.Sum(next => next.Value);
        uint completeNGramOccurenceCount = ngramsWithContext[next];

        double fT_numerator = Math.Max(completeNGramOccurenceCount - _discount, 0);
        double firstTerm = fT_numerator / occurencesOfContextBeforeAnything;
        return firstTerm;
    }

    // Jurafsky (3.7), formula 3.41, second case
    private double CalculateFirstTermLowerOrders(string next, string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Length != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;

        double fT_numerator = ngrams[size].NGrams.Count(n => n.Key.EndsWith(context) && n.Value.ContainsKey(next));
        fT_numerator = Math.Max(fT_numerator - _discount, 0);
        double fT_discriminator;

        // Logic for higher orders, we handle lowest order (1-grams) separately as this logic won't work for them.
        // At 1-grams we must count how many words can come before "", that's just all 1-grams,
        // but ContainsKey("") as below matches none of them and so returns 0.
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

        double firstTerm = fT_numerator / fT_discriminator;
        return firstTerm;
    }

    // Jurafsky (3.7), formula 3.39
    private double CalculateLambda(string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Length != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;
        IDictionary<string, uint> ngramsWithContext = ngrams[size - 1].NGrams[context];
        uint occurencesOfContextBeforeAnything = (uint)ngramsWithContext.Sum(next => next.Value);

        int numberOfDifferentFinalWordsSuccedingContext = ngramsWithContext.Count;
        double lambda = _discount / occurencesOfContextBeforeAnything * numberOfDifferentFinalWordsSuccedingContext;
        return lambda;
    }

    private double CalculatePCont(string next, string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Length != 0 ? 1 : 0) + context.Count((c) => c == ' ') + 1;
        IDictionary<string, IDictionary<string, uint>> ngramTable = ngrams[size - 1].NGrams;

        if (size > 1) // Higher orders -> use continuation count method
        {
            // Jurafsky (3.7), formula 3.40
            IEnumerable<string> currentContext = context.Split(' ');
            string newContext = string.Join(' ', currentContext.TakeLast(size - 2));
            double pcont = Smooth(next, newContext, ngrams, false);

            // Jurafsky (3.7), formula 3.38
            // int pcont_numerator = ngramTable.Count(following => following.Value.ContainsKey(next));
            // int ngramTableLength = ngramTable.Sum(_ => _.Value.Count);
            // double pcont = (double)pcont_numerator / ngramTableLength;

            return pcont;
        }
        else // Lowest order -> uniform distribution
        {
            // Jurafsky (3.7), formula 3.42
            // For 1-grams, count length of 1-gram table as described above, size - 1 always equals 1 in this case.
            // 1-grams only can have the context "", so we don't need to perform the whole searching process like above for higher orders.
            int unigramCount = ngrams[0].NGrams[""].Count;
            double pcont = 1.0 / unigramCount;
            return pcont;
        }
    }
}