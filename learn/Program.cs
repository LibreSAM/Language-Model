using learn;

Console.WriteLine("Bitte den Pfad zur Eingabedatei eingeben.");
string? inputFilePath = Console.ReadLine();
if (String.IsNullOrWhiteSpace(inputFilePath))
{
    Console.WriteLine("No filepath provided for input, exiting.");
    return;
}

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

var onegrams = new List<Onegram>();
var twograms = new List<Twogram>();
uint wordCount = 0;

foreach (string line in inputLines)
{
    string[] words = line.Split(' ');
    string previous = "<s>";

    foreach (string word in words)
    {
        wordCount++;

        // 1-grams
        Onegram? onegram = onegrams.Find(o => o.Word == word);
        if (onegram is null)
        {
            onegram = new Onegram(word);
            onegrams.Add(onegram);
        }
        onegram.IncrementOccurenceCountByOne();

        // 2-grams
        Twogram? twogram = twograms.Find(o => o.Before == previous && o.Word == word);
        if (twogram is null)
        {
            twogram = new Twogram(previous, word);
            twograms.Add(twogram);
        }
        twogram.IncrementOccurenceCountByOne();

        // update for next cycle
        previous = word;
    }

    string current = "</s>";
    Twogram? sentenceEnd = twograms.Find(o => o.Before == previous && o.Word == current);
    if (sentenceEnd is null)
    {
        sentenceEnd = new Twogram(previous, current);
        twograms.Add(sentenceEnd);
    }
    sentenceEnd.IncrementOccurenceCountByOne();
}

Console.WriteLine("\\data\\");
Console.WriteLine($"ngram 1 = {onegrams.Count}");
Console.WriteLine($"ngram 2 = {twograms.Count}");
Console.WriteLine($"ngram 3 = {onegrams.Count}");
Console.WriteLine();
Console.WriteLine("\\1-grams:");
foreach (Onegram item in onegrams)
{
    Console.WriteLine($"{item.OccuranceCount} {item.Word}");
}
Console.WriteLine();
Console.WriteLine("\\2-grams:");
foreach (Twogram item in twograms)
{
    Console.WriteLine($"{item.OccuranceCount} {item.Before} {item.Word}");
}
