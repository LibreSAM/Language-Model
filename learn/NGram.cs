using System.Text;
using Microsoft.Extensions.Logging;

namespace learn;
public class NGram
{
    public readonly uint Size;
    public IDictionary<string, uint> NGrams;
    private readonly ILogger _logger;

    public NGram(uint size, ILogger logger)
    {
        _logger = logger;
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
        _logger.LogDebug($"Started learning {Size}-Gram");
        _logger.LogTrace($"Learning text: \"{String.Join(' ', line)}\"");

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
                _logger.LogTrace($"Found occurance of existing ngram: \"{ngram}\"");
                NGrams[ngram]++;
            }
            else
            {
                _logger.LogTrace($"Found new ngram: \"{ngram}\"");
                NGrams.Add(ngram, 1);
            }
        }
        _logger.LogDebug($"Finished learning {Size}-Gram");
    }
}
