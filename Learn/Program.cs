using Microsoft.Extensions.Logging;

namespace Learn;
public class Program
{
    public static void Main(string[] args)
    {
        if (!ParseArguments(args, out LM_LearnOptions options))
        {
            return;
        }

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddSimpleConsole();
        });

        var lm = new LanguageModel(loggerFactory);
        lm.Learn(options.InputFilePath);

        Console.WriteLine(lm.GetArpaRepresentation());
    }

    private static bool ParseArguments(string[] args, out LM_LearnOptions arguments)
    {
        arguments = new LM_LearnOptions();

        string? inputFilePath;
        if (TryGetIndexOfElement(args, "--inputfilepath", out int index))
        {
            inputFilePath = args.ElementAtOrDefault(index + 1);
        }
        else
        {
            Console.WriteLine("Bitte den Pfad zur Eingabedatei eingeben.");
            inputFilePath = Console.ReadLine();
        }

        if (String.IsNullOrWhiteSpace(inputFilePath))
        {
            Console.WriteLine("No filepath provided for input, exiting.");
            return false;
        }

        arguments.InputFilePath = inputFilePath;
        return true;
    }

    private static int? GetIndexOfElement(string[] elements, string searched)
    {
        for (int index = 0; index < elements.Count(); index++)
        {
            if (searched.Equals(elements[index]))
            {
                return index;
            }
        }

        return null;
    }

    private static bool TryGetIndexOfElement(string[] elements, string searched, out int index)
    {
        int? maybeIndex = GetIndexOfElement(elements, searched);
        if (maybeIndex is int i)
        {
            index = i;
            return true;
        }
        else
        {
            index = -1;
            return false;
        }
    }
}
