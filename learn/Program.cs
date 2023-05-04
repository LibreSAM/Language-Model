using learn;

Console.WriteLine("Bitte den Pfad zur Eingabedatei eingeben.");
string? inputFilePath = Console.ReadLine();
if (String.IsNullOrWhiteSpace(inputFilePath))
{
    Console.WriteLine("No filepath provided for input, exiting.");
    return;
}

var lm = new LanguageModel();
lm.Learn(inputFilePath);
Console.WriteLine(lm.GetArpaRepresentation());
