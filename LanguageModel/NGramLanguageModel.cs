namespace LanguageModel;
public class NGramLanguageModel
{
    public readonly IDictionary<uint,NGram> NGrams;

    public NGramLanguageModel()
    {
        NGrams = new Dictionary<uint,NGram>();
    }

    public void AddNGram(uint size, string context, string next, double possibility)
    {
        if (!NGrams.TryGetValue(size, out NGram? ngram))
        {
            ngram = new NGram(size);
            NGrams.Add(size, ngram);
        }
        ngram.AddNGram(context, next, possibility);
    }

    public void GetArpaRepresentation(StreamWriter outputStreamWriter)
    {
        outputStreamWriter.WriteLine("\\data\\");
        foreach (var item in NGrams.Values)
        {
            int count = 0;
            foreach (var ngram in item.NGrams.Values)
            {
                count += ngram.Count();
            }
            outputStreamWriter.WriteLine($"ngram {item.Size} = {count}");
        }
        outputStreamWriter.WriteLine();
        foreach (var item in NGrams.Values)
        {
            item.GetArpaRepresentation(outputStreamWriter);
            outputStreamWriter.WriteLine();
        }
        outputStreamWriter.WriteLine("\\end\\");
        outputStreamWriter.Flush();
    }

    public void GetArpaRepresentation(Stream outputStream) => GetArpaRepresentation(new StreamWriter(outputStream));

    public static NGramLanguageModel LoadFrom(StreamReader inputStream)
    {
        // First line of header
        string? currentLine = inputStream.ReadLine();
        if (!"\\data\\".Equals(currentLine))
        {
            throw new InvalidDataException($"Invalid ARPA data: Expected \"\\data\\\", but got {currentLine}");
        }

        // Remaining part of header
        var counts = new Dictionary<int, int>();
        while((currentLine = inputStream.ReadLine()) != "")
        {
            if (currentLine is null)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got NULL");
            }
            string[] splitted = currentLine.Split(' ');
            if (splitted.Length != 4)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got {currentLine}");
            }
            bool success = true;
            success &= splitted[0].Equals("ngram");
            success &= splitted[2].Equals("=");
            success &= Int32.TryParse(splitted[1], out int ngramSize);
            success &= Int32.TryParse(splitted[3], out int ngramCount);
            if (!success)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\" with <x> and <y> being positive integers, but got {currentLine}");
            }
            if (counts.ContainsKey(ngramSize))
            {
                throw new InvalidDataException($"Invalid ARPA data: Multiple count entries for ngram size {ngramSize}");
            }
            counts.Add(ngramSize, ngramCount);
        }

        // Empty line after header
        currentLine = inputStream.ReadLine();
        if (!"".Equals(currentLine))
        {
            throw new InvalidDataException($"Invalid ARPA data: Expected empty line, but got \"{currentLine}\"");
        }

        counts.OrderBy(x => x.Key);
        var lm = new NGramLanguageModel();

        // NGram probabilities
        for (int size = 1; size <= counts.Count; size++)
        {
            // NGram probabilities - header saying size of following ngrams
            currentLine = inputStream.ReadLine();
            if ($"\\{size}-grams:".Equals(currentLine))
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"{size}-grams:\", but got \"{currentLine}\"");
            }

            // NGram probabilities - the real values
            for (int count = 0; count < counts[size]; count++)
            {
                currentLine = inputStream.ReadLine();
                if (currentLine is null || currentLine.Equals(""))
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got NULL");
                }
                string[] splitted = currentLine.Split(' ');
                if (splitted.Length != size + 1)
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"<probability> <ngram>\", but got {currentLine} - wrong ngram format");
                }
                if (!Double.TryParse(splitted[0], out double p))
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"<probability> <ngram>\" with probability being a double, but got {currentLine}");
                }
                string next = splitted.Last();
                string context = String.Join(' ', splitted.Take(new Range(new Index(1), new Index(1, true))));

                lm.AddNGram((uint)size, context, next, p);
            }

            // NGram probabilities - blank line after all ngrams of one size
            currentLine = inputStream.ReadLine();
            if (!"".Equals(currentLine))
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected empty line, but got \"{currentLine}\"");
            }
        }

        // Footer
        currentLine = inputStream.ReadLine();
        if ("\\end\\".Equals(currentLine))
        {
            throw new InvalidDataException($"Invalid ARPA data: Expected \"\\end\\\", but got \"{currentLine}\"");
        }

        return lm;
    }

    public static NGramLanguageModel LoadFrom(string filePath) => LoadFrom(File.OpenText(filePath));

    public double GetPerplexity(string sentence)
    {
        List<string> tokens = sentence.Split(' ').ToList();
        tokens.Insert(0, "<s>");
        tokens.Add("</s>");
        double perplexity = 1;

        for (int index = 0; index < tokens.Count; index++)
        {
            // get probability of ngram
            double p = 0.5;

            // multiply
            perplexity *= p;
        }

        return perplexity;
    }
}
