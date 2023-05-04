using System.Text;

namespace learn;
public class LanguageModel
{
    public IEnumerable<NGram> NGrams;

    public LanguageModel()
    {
        uint size = 3;

        NGrams = new List<NGram>();
        for (uint i = 1; i <= size; i++)
        {
            NGrams = NGrams.Append(new NGram(i));
        }
    }

    public void Learn(string inputFilePath)
    {
        IEnumerable<string> inputLines;
        try
        {
            inputLines = File.ReadAllLines(inputFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while reading the input file: {ex.ToString()}");
            return;
        }

        foreach (string line in inputLines)
        {
            string[] words = line.Split(' ');
            foreach (var item in NGrams)
            {
                item.Learn(words);
            }
        }
    }

    public string GetArpaRepresentation()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\\data\\");
        foreach (var item in NGrams)
        {
            stringBuilder.AppendLine($"ngram {item.Size} = {item.NGrams.Count}");
        }
        stringBuilder.AppendLine();
        foreach (var item in NGrams)
        {
            stringBuilder.AppendLine(item.GetArpaRepresentation());
        }
        stringBuilder.AppendLine("\\end\\");
        return stringBuilder.ToString();
    }
}
