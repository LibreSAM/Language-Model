using System.Globalization;

namespace LanguageModel;
public class NGram
{
    public readonly uint Size;
    public readonly IDictionary<string, IDictionary<string, double>> NGrams;

    public NGram(uint size)
    {
        Size = size;
        NGrams = new Dictionary<string, IDictionary<string, double>>();
    }

    public void AddNGram(string context, string next, double possibility)
    {
        if (!NGrams.ContainsKey(context))
        {
            NGrams.Add(context, new Dictionary<string, double>());
        }
        NGrams[context].Add(next, possibility);
    }

    public void GetArpaRepresentation(StreamWriter outputStreamWriter)
    {
        outputStreamWriter.WriteLine($"\\{Size}-grams:");
        foreach (var context in NGrams)
        {
            foreach (var next in context.Value)
            {
                string ngram = $"{context.Key} {next.Key}";
                outputStreamWriter.WriteLine($"{next.Value.ToString(CultureInfo.InvariantCulture)} {ngram}");
            }
        }
    }
}
