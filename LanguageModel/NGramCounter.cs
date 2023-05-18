using Microsoft.Extensions.Logging;

namespace LanguageModel;
public class NGramCounter
{
    public readonly uint Size;
    public IDictionary<string, IDictionary<string, uint>> NGrams;
    private readonly ILogger _logger;

    public NGramCounter(uint size, ILogger logger)
    {
        _logger = logger;
        Size = size;
        NGrams = new Dictionary<string, IDictionary<string, uint>>();
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
            var ngram = new List<string>();
            ngram.Add(temp.Dequeue());
            for (int i = 0; i < Size - 1; i++)
            {
                ngram.Add(temp.ElementAt(i));
            }
            var context = String.Join(' ', ngram.Take(ngram.Count - 1));
            var next = ngram.Last();
            if (NGrams.ContainsKey(context))
            {
                if (NGrams[context].ContainsKey(next))
                {
                    _logger.LogTrace($"Found occurance of existing ngram: \"{String.Join(' ', context)} {next}\"");
                    NGrams[context][next]++;
                }
                else
                {
                    _logger.LogTrace($"Found new word that can follow on existing ngram context: \"{next}\" can follow on \"{String.Join(' ', context)}\"");
                    NGrams[context].Add(next, 1);
                }
            }
            else
            {
                _logger.LogTrace($"Found new ngram: \"{String.Join(' ', ngram)}\"");
                NGrams.Add(context, new Dictionary<string, uint>() { { next, 1 } });
            }
        }

        _logger.LogDebug($"Finished learning {Size}-Gram");
    }
}
