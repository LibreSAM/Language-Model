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
uint wordCount = 0;

foreach (string line in inputLines)
{
    string[] words = line.Split(' ');

    foreach (string word in words)
    {
        wordCount++;
        var onegram = new Onegram(word);
        Onegram? existing = onegrams.Find(o => o.Word == onegram.Word);
        if (existing is not null)
        {
            existing.IncrementOccurenceCountByOne();
        }
        else
        {
            onegram.IncrementOccurenceCountByOne();
            onegrams.Add(onegram);
        }
    }
}

Console.WriteLine("\\1-grams");
foreach (Onegram item in onegrams)
{
    Console.WriteLine($"\"{item.Word}\" {item.OccuranceCount}");
}
