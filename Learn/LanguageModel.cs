using System.Text;
using Microsoft.Extensions.Logging;

namespace Learn;
public class LanguageModel
{
    public IEnumerable<NGram> NGrams;
    private readonly ILogger _logger;

    public LanguageModel(ILoggerFactory loggerFactory)
    {
        uint size = 3;

        _logger = loggerFactory.CreateLogger<LanguageModel>();
        NGrams = new List<NGram>();
        for (uint i = 1; i <= size; i++)
        {
            NGrams = NGrams.Append(new NGram(i, loggerFactory.CreateLogger<NGram>()));
        }
    }

    public void Learn(string inputFilePath)
    {
        _logger.LogInformation("Starting to learn the language model");
        _logger.LogDebug($"Input file for learning: \"{inputFilePath}\"");

        IEnumerable<string> inputLines;
        try
        {
            inputLines = File.ReadAllLines(inputFilePath);
            _logger.LogDebug("Finished read learning input file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read the contents of the input file for learning.");
            return;
        }

        foreach (string line in inputLines)
        {
            _logger.LogTrace($"Using input text \"{line}\"");
            string[] words = line.Split(' ');
            foreach (var item in NGrams)
            {
                _logger.LogDebug($"Learning {item.Size}-Grams");
                item.Learn(words);
            }
        }

        _logger.LogInformation("Finished learning language model");
    }

    public string GetArpaRepresentation()
    {
        _logger.LogInformation("Computing ARPA-Representation of language model...");

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\\data\\");
        foreach (var item in NGrams)
        {
            int count = 0;
            foreach (var ngram in item.NGrams.Values)
            {
                count += ngram.Count();
            }
            stringBuilder.AppendLine($"ngram {item.Size} = {count}");
        }
        stringBuilder.AppendLine();
        foreach (var item in NGrams)
        {
            stringBuilder.AppendLine(item.GetArpaRepresentation());
        }
        stringBuilder.AppendLine("\\end\\");

        _logger.LogInformation("Finished computing ARPA-Representation of language model");
        return stringBuilder.ToString();
    }
}
