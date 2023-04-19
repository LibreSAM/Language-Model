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
