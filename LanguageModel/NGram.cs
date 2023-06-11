using System.Globalization;

namespace LanguageModel;

/// <summary>
/// Represents all ngrams of a common length, i.e. 2, and theri according probability.
/// </summary>
public class NGram
{
    /// <summary>
    /// The size of all ngrams that are stored inside this object
    /// </summary>
    public readonly uint Size;

    /// <summary>
    /// Allows access to all ngrams that are stored in this object and their according probabilities
    /// </summary>
    public readonly IDictionary<string, IDictionary<string, double>> NGrams;

    /// <summary>
    /// Creates a new instance of <see cref="NGram"/> for ngrams with the specified size.
    /// </summary>
    /// <param name="size">The size of ngrams that will be stored within the created object.</param>
    public NGram(uint size)
    {
        Size = size;
        NGrams = new Dictionary<string, IDictionary<string, double>>();
    }

    /// <summary>
    /// Adds a new ngram and it's probability into this container. This does not take care if the ngram already exists in this container, so be careful!
    /// </summary>
    /// <param name="context">The context of the ngram to add.</param>
    /// <param name="next">The next word of the ngram to add.</param>
    /// <param name="possibility">The possibility of the ngram to add</param>
    public void AddNGram(string context, string next, double possibility)
    {
        if (!NGrams.ContainsKey(context))
        {
            // Lazy initialization of objects
            NGrams.Add(context, new Dictionary<string, double>());
        }
        NGrams[context].Add(next, possibility);
    }

    /// <summary>
    /// Creates a representation of all the ngrams and their probabilities in the ARPA format and writes them to the specified stream.
    /// </summary>
    /// <param name="outputStreamWriter">The stream to write the ARPA representation of the ngrams to.</param>
    public void GetArpaRepresentation(StreamWriter outputStreamWriter)
    {
        outputStreamWriter.WriteLine($"\\{Size}-grams:");

        // format all ngrams
        foreach (var context in NGrams)
        {
            foreach (var next in context.Value)
            {
                // ARPA format. Uses InvariantCulture to always use a point as separator of floating point number
                string ngram = string.IsNullOrWhiteSpace(context.Key) ? $"{next.Key}" : $"{context.Key} {next.Key}";
                outputStreamWriter.WriteLine($"{next.Value.ToString(CultureInfo.InvariantCulture)} {ngram}");
            }
        }
    }
}
