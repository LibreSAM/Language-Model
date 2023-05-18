namespace LanguageModel;
public class NGramLanguageModel
{
    public readonly IDictionary<uint,NGram> NGrams;

    public NGramLanguageModel()
    {
        NGrams = new Dictionary<uint,NGram>();
    }

    public void AddNGram(uint size, string context, string next, double possibility)
    {
        if (!NGrams.TryGetValue(size, out NGram? ngram))
        {
            ngram = new NGram(size);
            NGrams.Add(size, ngram);
        }
        ngram.AddNGram(context, next, possibility);
    }

    public void GetArpaRepresentation(StreamWriter outputStreamWriter)
    {
        outputStreamWriter.WriteLine("\\data\\");
        foreach (var item in NGrams.Values)
        {
            int count = 0;
            foreach (var ngram in item.NGrams.Values)
            {
                count += ngram.Count();
            }
            outputStreamWriter.WriteLine($"ngram {item.Size} = {count}");
        }
        outputStreamWriter.WriteLine();
        foreach (var item in NGrams.Values)
        {
            item.GetArpaRepresentation(outputStreamWriter);
            outputStreamWriter.WriteLine();
        }
        outputStreamWriter.WriteLine("\\end\\");
        outputStreamWriter.Flush();
    }

    public void GetArpaRepresentation(Stream outputStream) => GetArpaRepresentation(new StreamWriter(outputStream));
}
