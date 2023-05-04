using System.Linq;
using System.Text;

namespace learn;
public class NGram
{
    public readonly uint Size;
    public IDictionary<string, uint> NGrams;

    public NGram(uint size)
    {
        Size = size;
        NGrams = new Dictionary<string, uint>();
    }

    public string GetArpaRepresentation()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"\\{Size}-grams:");
        int ngramCount = (int)NGrams.Sum(i => i.Value); // Not working yet
        foreach (var item in NGrams)
        {
            stringBuilder.AppendLine($"{Math.Log10((double)item.Value / ngramCount)} {item.Key}");
        }
        return stringBuilder.ToString();
    }

    public void Learn(string[] line)
    {
        Queue<string> temp = new Queue<string>();
        for (int i = 0; i < Size; i++)
        {
            temp.Enqueue("<s>");
        }
        foreach (var item in line)
        {
            temp.Enqueue(item);
        }
        for (int i = 0; i < Size; i++)
        {
            temp.Enqueue("</s>");
        }

        while (temp.Count() >= Size)
        {
            string ngram = temp.Dequeue();
            for (int i = 0; i < Size - 1; i++)
            {
                ngram += $" {temp.ElementAt(i)}";
            }
            if (NGrams.ContainsKey(ngram))
            {
                NGrams[ngram]++;
            }
            else
            {
                NGrams.Add(ngram, 1);
            }
        }
    }
}
