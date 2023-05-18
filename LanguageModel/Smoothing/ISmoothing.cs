namespace LanguageModel.Smoothing;
public interface ISmoothing
{
    public double Smooth(string next, string context, IList<NGramCounter> ngrams);
}