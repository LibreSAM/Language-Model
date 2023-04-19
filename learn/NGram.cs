using System.Text;

namespace learn;
public class NGram
{
    public IEnumerable<Onegram> Onegrams;
    public IEnumerable<Twogram> Twograms;
    public IEnumerable<Threegram> Threegrams;

    public NGram()
    {
        Onegrams = new List<Onegram>();
        Twograms = new List<Twogram>();
        Threegrams = new List<Threegram>();
    }

    public string GetArpaRepresentation()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\\data\\");
        stringBuilder.AppendLine($"ngram 1 = {Onegrams.Count()}");
        stringBuilder.AppendLine($"ngram 2 = {Twograms.Count()}");
        stringBuilder.AppendLine($"ngram 3 = {Threegrams.Count()}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("\\1-grams:");
        foreach (Onegram item in Onegrams)
        {
            stringBuilder.AppendLine($"{item.OccuranceCount} {item.Word}");
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("\\2-grams:");
        foreach (Twogram item in Twograms)
        {
            stringBuilder.AppendLine($"{item.OccuranceCount} {item.Before} {item.Word}");
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("\\3-grams:");
        foreach (Threegram item in Threegrams)
        {
            stringBuilder.AppendLine($"{item.OccuranceCount} {item.BeforeBefore} {item.Before} {item.Word}");
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("\\end\\");
        return stringBuilder.ToString();
    }
}
