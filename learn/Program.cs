using learn;
using Microsoft.Extensions.Logging;

Console.WriteLine("Bitte den Pfad zur Eingabedatei eingeben.");
string? inputFilePath = Console.ReadLine();
if (String.IsNullOrWhiteSpace(inputFilePath))
{
    Console.WriteLine("No filepath provided for input, exiting.");
    return;
}

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddSimpleConsole();
});
var lm = new LanguageModel(loggerFactory);
lm.Learn(inputFilePath);
Console.WriteLine(lm.GetArpaRepresentation());
