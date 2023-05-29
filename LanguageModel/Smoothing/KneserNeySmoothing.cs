namespace LanguageModel.Smoothing;
public class KneserNeySmoothing : ISmoothing
{
    // cf. https://medium.com/@dennyc/a-simple-numerical-example-for-kneser-ney-smoothing-nlp-4600addf38b8
    public double Smooth(string next, string context, IList<NGramCounter> ngrams)
    {
        int size = (context.Count() != 0 ? 1 : 0) + context.Count((c) => c == ' ');
        double discount = 0; // TODO: use proper discount value
        IDictionary<string, IDictionary<string, uint>> ngramTable = ngrams[size].NGrams;
        IDictionary<string, uint> ngramsWithContext = ngramTable[context];
        uint occurencesOfContextBeforeAnything = (uint)ngramsWithContext.Sum(next => next.Value);

        // firstTerm
        uint completeNGramOccurenceCount = ngramsWithContext[next];
        double fT_numerator = Math.Max(completeNGramOccurenceCount - discount, 0);
        double firstTerm = fT_numerator / occurencesOfContextBeforeAnything;

        // lambda
        int numberOfDifferentFinalWordsSuccedingContext = ngramsWithContext.Count;
        double lambda = discount / occurencesOfContextBeforeAnything * numberOfDifferentFinalWordsSuccedingContext;

        // Pcont
        int pcont_numerator = ngramTable.Count(following => following.Value.ContainsKey(next));
        int ngramTableLength = ngramTable.Sum(_ => _.Value.Count());
        double pcont = pcont_numerator / ngramTableLength;

        // Sum
        double smoothedNGramProbability = firstTerm + lambda * pcont;
        return smoothedNGramProbability;
    }
}