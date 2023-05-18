namespace LanguageModel.Smoothing;
public class Regular : ISmoothing
{
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